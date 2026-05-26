import axios from 'axios'

const apiClient = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor: attach token
apiClient.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
}, error => {
  return Promise.reject(error)
})

// Response interceptor: handle 401
apiClient.interceptors.response.use(response => {
  return response
}, error => {
  if (error.response && error.response.status === 401) {
    // Tự động logout nếu token hết hạn (tuỳ chọn)
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    // Nếu dùng vue-router ở ngoài store, có thể cần đẩy về login: window.location.href = '/login'
  }
  return Promise.reject(error)
})

export default apiClient
