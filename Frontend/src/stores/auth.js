import { defineStore } from 'pinia'
import api from '@/api/axios'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: JSON.parse(localStorage.getItem('user')) || null,
    token: localStorage.getItem('token') || null,
    loading: false,
    error: null,
    credits: 0
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    isAdmin: (state) => state.user?.role === 'Admin'
  },

  actions: {
    async login(username, password) {
      this.loading = true
      this.error = null
      try {
        const response = await api.post('/auth/login', { username, password })
        const { token, ...userData } = response.data
        
        this.token = token
        this.user = userData
        
        localStorage.setItem('token', token)
        localStorage.setItem('user', JSON.stringify(userData))
        
        return true
      } catch (err) {
        this.error = err.response?.data?.message || 'Đăng nhập thất bại'
        return false
      } finally {
        this.loading = false
      }
    },

    async register(userData) {
      this.loading = true
      this.error = null
      try {
        const response = await api.post('/auth/register', userData)
        const { token, ...user } = response.data
        
        this.token = token
        this.user = user
        
        localStorage.setItem('token', token)
        localStorage.setItem('user', JSON.stringify(user))
        
        return true
      } catch (err) {
        this.error = err.response?.data?.message || 'Đăng ký thất bại'
        return false
      } finally {
        this.loading = false
      }
    },

    logout() {
      this.user = null
      this.token = null
      localStorage.removeItem('token')
      localStorage.removeItem('user')
    },
    async fetchMe() {
      if (!this.token) return
      try {
        const { data } = await api.get('/api/Auth/me')
        this.token = data.token
        this.user = {
          id: data.userId,
          username: data.username,
          fullName: data.fullName,
          phone: data.phone,
          role: data.role
        }
        this.credits = data.credits || 0
        localStorage.setItem('token', data.token)
        localStorage.setItem('user', JSON.stringify(this.user))
      } catch (err) {
        console.error('Failed to fetch me', err)
      }
    }
  }
})
