<template>
  <nav class="navbar">
    <div class="container nav-content">
      <router-link to="/" class="brand" @click="closeMenu">
        <span class="brand-icon">🏸</span> Biên Hòa Cầu Lông
      </router-link>
      
      <!-- Hamburger Button for Mobile -->
      <button class="hamburger" :class="{ 'is-active': isMenuOpen }" @click="toggleMenu" aria-label="Toggle navigation">
        <span class="line"></span>
        <span class="line"></span>
        <span class="line"></span>
      </button>

      <!-- Nav Links -->
      <div class="nav-links" :class="{ 'is-open': isMenuOpen }">
        <router-link to="/courts" class="nav-link" @click="closeMenu">Danh sách sân</router-link>
        <router-link to="/feedback" class="nav-link" @click="closeMenu">Góp ý</router-link>
        
        <template v-if="authStore.isAuthenticated">
          <div class="user-info">
            <div class="user-meta">
              <span class="user-greeting">
                Chào, <span class="user-name">{{ authStore.user?.fullName }}</span>
              </span>
              <!-- Show Admin Badge instead of Credits for Admin -->
              <span v-if="authStore.user?.role === 'Admin'" class="admin-badge">
                ⚡ QUẢN TRỊ VIÊN
              </span>
              <!-- Show Credits Badge for Normal Users -->
              <router-link v-else to="/topup" class="credits-badge" title="Lượt tạo kèo còn lại" @click="closeMenu">
                🎟️ {{ authStore.credits }} lượt
              </router-link>
            </div>
          </div>
          
          <router-link v-if="authStore.user?.role === 'Admin'" to="/admin" class="nav-link admin-link" @click="closeMenu">
            Quản trị
          </router-link>
          <router-link to="/my-matches" class="nav-link" @click="closeMenu">Kèo của tôi</router-link>
          <button @click="handleLogout" class="btn btn-outline btn-sm logout-btn">Đăng xuất</button>
        </template>
        
        <template v-else>
          <router-link to="/login" class="nav-link" @click="closeMenu">Đăng nhập</router-link>
          <router-link to="/register" class="btn btn-primary btn-sm reg-btn" @click="closeMenu">Đăng ký</router-link>
        </template>
      </div>
    </div>
  </nav>
</template>

<script setup>
import { ref } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const router = useRouter()
const isMenuOpen = ref(false)

const toggleMenu = () => {
  isMenuOpen.value = !isMenuOpen.value
}

const closeMenu = () => {
  isMenuOpen.value = false
}

const handleLogout = () => {
  closeMenu()
  authStore.logout()
  router.push('/login')
}
</script>

<style scoped>
.navbar {
  background: rgba(11, 15, 25, 0.85); /* Deep dark semi-transparent */
  backdrop-filter: blur(16px);
  -webkit-backdrop-filter: blur(16px);
  border-bottom: 1px solid var(--border-color);
  position: sticky;
  top: 0;
  z-index: 1000;
  height: var(--nav-height);
  display: flex;
  align-items: center;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.4);
}

.nav-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  padding: 0 var(--spacing-md);
}

.brand {
  font-weight: 800;
  font-size: 20px;
  color: var(--text-primary);
  text-decoration: none;
  display: flex;
  align-items: center;
  gap: 8px;
  letter-spacing: -0.02em;
  transition: all 0.3s ease;
}

.brand-icon {
  font-size: 24px;
  filter: drop-shadow(0 0 8px rgba(163, 230, 53, 0.6));
}

.brand:hover {
  color: var(--primary-color);
  text-shadow: 0 0 10px rgba(163, 230, 53, 0.4);
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 20px;
  transition: all 0.3s ease;
}

.nav-link {
  color: var(--text-secondary);
  text-decoration: none;
  font-size: 15px;
  font-weight: 600;
  transition: all 0.3s ease;
  position: relative;
  padding: 6px 0;
}

.nav-link::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 0;
  width: 0;
  height: 2px;
  background-color: var(--primary-color);
  transition: width 0.3s ease;
  box-shadow: var(--shadow-neon);
}

.nav-link:hover {
  color: var(--primary-color);
}

.nav-link:hover::after {
  width: 100%;
}

.nav-link.router-link-active {
  color: var(--primary-color);
}

.nav-link.router-link-active::after {
  width: 100%;
}

.admin-link {
  color: #ff9f43 !important; /* Standout color for Admin page */
}
.admin-link::after {
  background-color: #ff9f43 !important;
}

/* User Info Styling */
.user-info {
  display: flex;
  align-items: center;
}

.user-meta {
  display: flex;
  align-items: center;
  gap: 12px;
}

.user-greeting {
  font-size: 14px;
  color: var(--text-secondary);
  font-weight: 500;
}

.user-name {
  color: var(--text-primary);
  font-weight: 700;
}

.admin-badge {
  background: linear-gradient(135deg, #f43f5e 0%, #be123c 100%);
  color: white;
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 11px;
  font-weight: 800;
  letter-spacing: 0.05em;
  box-shadow: 0 0 10px rgba(244, 63, 94, 0.4);
  animation: pulse 2s infinite;
}

.credits-badge {
  background: linear-gradient(135deg, #a3e635 0%, #65a30d 100%);
  color: #0b0f19;
  padding: 4px 12px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 800;
  text-decoration: none;
  display: inline-flex;
  align-items: center;
  gap: 4px;
  box-shadow: 0 4px 10px rgba(163, 230, 53, 0.3);
  transition: all 0.3s ease;
}

.credits-badge:hover {
  transform: translateY(-1px);
  box-shadow: var(--shadow-neon);
}

/* Hamburger Menu styles */
.hamburger {
  display: none;
  flex-direction: column;
  justify-content: space-between;
  width: 24px;
  height: 18px;
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 0;
  z-index: 1010;
}

.hamburger .line {
  width: 24px;
  height: 2px;
  background-color: var(--text-primary);
  transition: all 0.3s ease;
}

.hamburger.is-active .line:nth-child(1) {
  transform: translateY(8px) rotate(45deg);
  background-color: var(--primary-color);
}

.hamburger.is-active .line:nth-child(2) {
  opacity: 0;
}

.hamburger.is-active .line:nth-child(3) {
  transform: translateY(-8px) rotate(-45deg);
  background-color: var(--primary-color);
}

.btn-sm {
  padding: 6px 14px;
  font-size: 13px;
  border-radius: 8px;
}

@keyframes pulse {
  0% {
    box-shadow: 0 0 0 0 rgba(244, 63, 94, 0.4);
  }
  70% {
    box-shadow: 0 0 0 8px rgba(244, 63, 94, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(244, 63, 94, 0);
  }
}

/* Responsive breakpoint for Tablets and Mobile */
@media (max-width: 991px) {
  .hamburger {
    display: flex;
  }

  .nav-links {
    position: fixed;
    top: 0;
    right: -100%;
    width: 280px;
    height: 100vh;
    background: rgba(11, 15, 25, 0.98);
    backdrop-filter: blur(24px);
    -webkit-backdrop-filter: blur(24px);
    border-left: 1px solid var(--border-color);
    flex-direction: column;
    align-items: flex-start;
    padding: 100px var(--spacing-lg) var(--spacing-lg) var(--spacing-lg);
    gap: 24px;
    box-shadow: -10px 0 30px rgba(0, 0, 0, 0.7);
  }

  .nav-links.is-open {
    right: 0;
  }

  .nav-link {
    font-size: 18px;
    width: 100%;
    padding: 10px 0;
  }

  .user-info {
    width: 100%;
    border-top: 1px solid var(--border-color);
    border-bottom: 1px solid var(--border-color);
    padding: 16px 0;
    margin: 8px 0;
  }

  .user-meta {
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
  }

  .logout-btn, .reg-btn {
    width: 100%;
    margin-top: 8px;
  }
}
</style>
