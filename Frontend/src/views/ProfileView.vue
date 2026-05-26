<template>
  <div class="profile-view container">
    <div class="page-header">
      <h2>Hồ Sơ Của Tôi</h2>
    </div>

    <div class="profile-card card">
      <div class="profile-info">
        <div class="avatar">{{ authStore.user?.fullName?.charAt(0).toUpperCase() }}</div>
        <div class="details">
          <h3>{{ authStore.user?.fullName }}</h3>
          <p class="text-secondary">@{{ authStore.user?.username }}</p>
          <span class="role-badge">{{ authStore.user?.role }}</span>
        </div>
      </div>

      <hr class="divider" />

      <div class="change-password-section">
        <h3>Đổi Mật Khẩu</h3>
        <form @submit.prevent="handleChangePassword">
          <div class="form-group">
            <label>Mật khẩu cũ</label>
            <input type="password" v-model="form.oldPassword" required class="input-field" />
          </div>
          <div class="form-group">
            <label>Mật khẩu mới</label>
            <input type="password" v-model="form.newPassword" required class="input-field" />
          </div>
          <div class="form-group">
            <label>Xác nhận mật khẩu mới</label>
            <input type="password" v-model="form.confirmPassword" required class="input-field" />
          </div>
          
          <button type="submit" class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Đang xử lý...' : 'Lưu Thay Đổi' }}
          </button>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useToast } from 'vue-toastification'
import api from '@/api/axios'

const authStore = useAuthStore()
const toast = useToast()

const loading = ref(false)
const form = ref({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const handleChangePassword = async () => {
  if (form.value.newPassword !== form.value.confirmPassword) {
    toast.error('Mật khẩu xác nhận không khớp!')
    return
  }
  
  if (form.value.newPassword.length < 6) {
    toast.error('Mật khẩu mới phải từ 6 ký tự trở lên.')
    return
  }

  loading.value = true
  try {
    await api.put('/auth/change-password', {
      oldPassword: form.value.oldPassword,
      newPassword: form.value.newPassword
    })
    toast.success('Đổi mật khẩu thành công!')
    form.value = { oldPassword: '', newPassword: '', confirmPassword: '' }
  } catch (err) {
    toast.error(err.response?.data?.message || 'Đổi mật khẩu thất bại')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.profile-view {
  margin-top: 24px;
  max-width: 600px;
}
.page-header {
  margin-bottom: 24px;
}
.profile-card {
  padding: 32px;
}
.profile-info {
  display: flex;
  align-items: center;
  gap: 24px;
}
.avatar {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: var(--primary-color);
  color: white;
  font-size: 32px;
  font-weight: bold;
  display: flex;
  align-items: center;
  justify-content: center;
}
.details h3 {
  margin: 0 0 4px 0;
  font-size: 24px;
}
.text-secondary {
  color: var(--text-secondary);
  margin: 0 0 8px 0;
}
.role-badge {
  display: inline-block;
  padding: 4px 12px;
  background: #f1f5f9;
  color: #475569;
  border-radius: 20px;
  font-size: 13px;
  font-weight: 500;
}
.divider {
  border: none;
  border-top: 1px solid var(--border-color);
  margin: 32px 0;
}
.change-password-section h3 {
  margin-bottom: 20px;
}
.form-group {
  margin-bottom: 16px;
}
.form-group label {
  display: block;
  font-size: 14px;
  font-weight: 500;
  margin-bottom: 8px;
}
.input-field {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  font-size: 15px;
}
.btn {
  margin-top: 8px;
  width: 100%;
  padding: 12px;
}
</style>
