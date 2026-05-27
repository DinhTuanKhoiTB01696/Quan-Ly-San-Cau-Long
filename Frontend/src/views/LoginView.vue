<template>
  <div class="auth-view container">
    <div class="card auth-card">
      <h2 class="text-center mb-4">Đăng Nhập</h2>
      
      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label>Tên đăng nhập</label>
          <input type="text" v-model="username" required placeholder="Nhập username" />
        </div>
        
        <div class="form-group">
          <label>Mật khẩu</label>
          <input type="password" v-model="password" required placeholder="Nhập mật khẩu" />
        </div>
        
        <div v-if="authStore.error" class="error-msg">
          {{ authStore.error }}
        </div>
        
        <button type="submit" class="btn btn-primary w-100 mb-3" :disabled="authStore.loading">
          {{ authStore.loading ? 'Đang xử lý...' : 'Đăng Nhập' }}
        </button>

        <div class="divider"><span>Hoặc</span></div>

        <div class="google-btn-wrapper">
          <GoogleLogin :callback="handleGoogleCallback" prompt auto-login />
        </div>
      </form>
      
      <div class="auth-links text-center mt-3">
        Chưa có tài khoản? <router-link to="/register">Đăng ký ngay</router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useToast } from 'vue-toastification'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()
const toast = useToast()

const username = ref('')
const password = ref('')

const handleSubmit = async () => {
  try {
    const success = await authStore.login(username.value, password.value)
    if (success) {
      toast.success('Đăng nhập thành công')
      const redirect = route.query.redirect || '/'
      router.push(redirect)
    } else {
      toast.error(authStore.error || 'Đăng nhập thất bại')
    }
  } catch (err) {
    toast.error('Đã xảy ra lỗi hệ thống')
  }
}

const handleGoogleCallback = async (response) => {
  try {
    if (response.credential) {
      await authStore.googleLogin(response.credential)
      toast.success('Đăng nhập Google thành công')
      const redirect = route.query.redirect || '/'
      router.push(redirect)
    }
  } catch (err) {
    toast.error(err.response?.data?.message || 'Đăng nhập Google thất bại')
  }
}
</script>

<style scoped>
.auth-view {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: calc(100vh - 100px);
}
.auth-card {
  width: 100%;
  max-width: 400px;
  padding: 32px 24px;
}
.text-center { text-align: center; }
.mb-4 { margin-bottom: 24px; }
.mt-3 { margin-top: 16px; }
.form-group {
  margin-bottom: 16px;
}
.form-group label {
  display: block;
  margin-bottom: 6px;
  font-weight: 500;
  font-size: 14px;
}
.w-100 { width: 100%; }
.error-msg {
  color: var(--danger-color);
  font-size: 13px;
  margin-bottom: 16px;
  text-align: center;
}
.auth-links {
  font-size: 14px;
}
.divider {
  display: flex;
  align-items: center;
  text-align: center;
  margin: 16px 0;
  color: var(--text-secondary);
  font-size: 13px;
}
.divider::before,
.divider::after {
  content: '';
  flex: 1;
  border-bottom: 1px solid var(--border-color);
}
.divider span {
  padding: 0 10px;
}
.google-btn-wrapper {
  display: flex;
  justify-content: center;
  margin-bottom: 8px;
}
.mb-3 { margin-bottom: 16px; }
</style>
