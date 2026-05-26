<template>
  <nav class="navbar">
    <div class="container nav-content">
      <router-link to="/" class="brand">🏸 Biên Hòa Cầu Lông</router-link>
      
      <div class="nav-links">
        <template v-if="authStore.isAuthenticated">
          <span class="user-greeting">Chào, {{ authStore.user?.fullName }}</span>
          <router-link v-if="authStore.user?.role === 'Admin'" to="/admin" class="nav-link" style="color:var(--primary-color)">Admin Panel</router-link>
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
  background: var(--card-bg);
  border-bottom: 1px solid var(--border-color);
  position: sticky;
  top: 0;
  z-index: 100;
  height: var(--nav-height);
  display: flex;
  align-items: center;
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
