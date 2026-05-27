<template>
  <nav class="navbar">
    <div class="container nav-content">
      <router-link to="/" class="brand">🏸 Biên Hòa Cầu Lông</router-link>
      
      <div class="nav-links">
        <router-link to="/courts" class="nav-link">Danh sách sân</router-link>
        <router-link to="/feedback" class="nav-link">Góp ý</router-link>
        <template v-if="authStore.isAuthenticated">
          <div class="user-info">
            <span class="nav-link user-greeting" style="font-weight:bold; color:var(--text-primary)">
              Chào, {{ authStore.user?.fullName }}
            </span>
            <router-link to="/topup" class="credits-badge" title="Lượt tạo kèo còn lại">
              🎟️ {{ authStore.credits }} lượt
            </router-link>
          </div>
          <router-link v-if="authStore.user?.role === 'Admin'" to="/admin" class="nav-link" style="color:var(--primary-color)">Admin</router-link>
          <router-link to="/my-matches" class="nav-link">Kèo của tôi</router-link>
          <button @click="handleLogout" class="btn btn-outline btn-sm">Đăng xuất</button>
        </template>
        <template v-else>
          <router-link to="/login" class="nav-link">Đăng nhập</router-link>
          <router-link to="/register" class="btn btn-primary btn-sm">Đăng ký</router-link>
        </template>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()

const handleLogout = () => {
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.navbar {
  background: var(--glass-bg, rgba(255, 255, 255, 0.85));
  backdrop-filter: blur(12px);
  -webkit-backdrop-filter: blur(12px);
  border-bottom: 1px solid var(--border-color);
  position: sticky;
  top: 0;
  z-index: 100;
  height: var(--nav-height);
  display: flex;
  align-items: center;
  box-shadow: var(--shadow-sm, 0 1px 2px rgba(0,0,0,0.05));
}
.nav-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
}
.brand {
  font-weight: 700;
  font-size: 18px;
  color: var(--primary-color);
  text-decoration: none;
}
.nav-links {
  display: flex;
  align-items: center;
  gap: 12px;
}
.nav-link {
  color: var(--text-secondary);
  text-decoration: none;
  font-size: 14px;
  font-weight: 500;
}
.nav-link:hover {
  color: var(--primary-color);
}
.user-greeting {
  font-size: 14px;
  color: var(--text-primary);
  display: none;
}
.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
}
.credits-badge {
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  color: white;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: bold;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 4px;
  box-shadow: 0 2px 4px rgba(245, 158, 11, 0.3);
  transition: transform 0.2s;
}
.credits-badge:hover {
  transform: scale(1.05);
}
@media (min-width: 480px) {
  .user-greeting {
    display: inline;
  }
}
.btn-sm {
  padding: 6px 12px;
  font-size: 13px;
}
</style>
