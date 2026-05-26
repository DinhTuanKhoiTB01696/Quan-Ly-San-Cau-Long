<template>
  <div class="auth-view container">
    <div class="card auth-card">
      <h2 class="text-center mb-4">Đăng Ký Tài Khoản</h2>
      
      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label>Tên đăng nhập</label>
          <input type="text" v-model="form.username" required placeholder="Nhập username" />
        </div>
        
        <div class="form-group">
          <label>Mật khẩu</label>
          <input type="password" v-model="form.password" required placeholder="Nhập mật khẩu" />
        </div>
        
        <div class="form-group">
          <label>Họ và tên</label>
          <input type="text" v-model="form.fullName" required placeholder="Ví dụ: Nguyễn Văn A" />
        </div>
        
        <div class="form-group">
          <label>Số điện thoại</label>
          <input type="tel" v-model="form.phone" required placeholder="Số ĐT của bạn" />
        </div>
        
        <div v-if="authStore.error" class="error-msg">
          {{ authStore.error }}
        </div>
        
        <button type="submit" class="btn btn-primary w-100" :disabled="authStore.loading">
          {{ authStore.loading ? 'Đang xử lý...' : 'Đăng Ký' }}
        </button>
      </form>
      
      <div class="auth-links text-center mt-3">
        Đã có tài khoản? <router-link to="/login">Đăng nhập</router-link>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()

const form = reactive({
  username: '',
  password: '',
  fullName: '',
  phone: ''
})

const handleSubmit = async () => {
  const success = await authStore.register(form)
  if (success) {
    router.push('/')
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
</style>
