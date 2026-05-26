<template>
  <NavBar />
  <main class="main-content">
    <router-view v-slot="{ Component }">
      <transition name="fade" mode="out-in">
        <component :is="Component" />
      </transition>
    </router-view>
  </main>
</template>

<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import NavBar from '@/components/NavBar.vue'

const authStore = useAuthStore()

// If there's an existing token/user in localStorage, it's already loaded by Pinia state function.
// We could optionally verify the token with the backend here.
onMounted(() => {
  // console.log('App loaded. Auth:', authStore.isAuthenticated)
})
</script>

<style>
.main-content {
  min-height: calc(100vh - var(--nav-height));
  padding-bottom: var(--spacing-lg);
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
