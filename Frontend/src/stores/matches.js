import { defineStore } from 'pinia'
import api from '@/api/axios'

export const useMatchStore = defineStore('matches', {
  state: () => ({
    matches: [],
    myMatches: [],
    loading: false,
    error: null
  }),

  actions: {
    async fetchMatches(filters = {}) {
      this.loading = true
      this.error = null
      try {
        const params = new URLSearchParams()
        if (filters.area !== undefined && filters.area !== null) params.append('area', filters.area)
        if (filters.level !== undefined && filters.level !== null) params.append('level', filters.level)
        if (filters.status !== undefined && filters.status !== null) params.append('status', filters.status)

        const response = await api.get(`/matches?${params.toString()}`)
        this.matches = response.data
      } catch (err) {
        this.error = err.response?.data?.message || 'Lỗi khi lấy danh sách kèo'
      } finally {
        this.loading = false
      }
    },

    async fetchMyMatches() {
      this.loading = true
      this.error = null
      try {
        const response = await api.get('/matches/my-matches')
        this.myMatches = response.data
      } catch (err) {
        this.error = err.response?.data?.message || 'Lỗi khi lấy danh sách kèo của bạn'
      } finally {
        this.loading = false
      }
    },

    async createMatch(matchData) {
      this.loading = true
      this.error = null
      try {
        const response = await api.post('/matches', matchData)
        // Cập nhật state cục bộ thay vì fetch lại nếu muốn nhanh
        this.myMatches.unshift(response.data)
        this.matches.unshift(response.data)
        return true
      } catch (err) {
        this.error = err.response?.data?.message || 'Tạo kèo thất bại'
        return false
      } finally {
        this.loading = false
      }
    },

    async updateStatus(id, status) {
      try {
        await api.put(`/matches/${id}/status`, status, {
          headers: { 'Content-Type': 'application/json' }
        })
        
        // Cập nhật trực tiếp trên state thay vì fetch lại
        const myMatch = this.myMatches.find(m => m.id === id)
        if (myMatch) myMatch.status = status
        
        const match = this.matches.find(m => m.id === id)
        if (match) match.status = status
        
        return true
      } catch (err) {
        this.error = err.response?.data?.message || 'Cập nhật trạng thái thất bại'
        return false
      }
    },

    async joinMatch(id) {
      try {
        await api.post(`/matches/${id}/join`)
        return true
      } catch (err) {
        this.error = err.response?.data?.message || 'Tham gia thất bại'
        return false
      }
    },

    async leaveMatch(id) {
      try {
        await api.post(`/matches/${id}/leave`)
        return true
      } catch (err) {
        this.error = err.response?.data?.message || 'Hủy tham gia thất bại'
        return false
      }
    }
  }
})
