import { defineStore } from 'pinia'
import api from '@/api/axios'

export const useCourtStore = defineStore('courts', {
  state: () => ({
    courts: [],
    loading: false,
    error: null
  }),

  actions: {
    async fetchCourts(area = null) {
      this.loading = true
      this.error = null
      try {
        let url = '/courts'
        if (area !== null) {
          url += `?area=${area}`
        }
        const response = await api.get(url)
        this.courts = response.data
      } catch (err) {
        this.error = err.response?.data?.message || 'Lỗi khi lấy danh sách sân'
      } finally {
        this.loading = false
      }
    }
  }
})
