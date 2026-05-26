<template>
  <div class="home-view">
    <!-- Hero Section -->
    <div class="hero-banner">
      <div class="hero-content container">
        <h1>Giao Lưu Cầu Lông Biên Hòa</h1>
        <p>Tìm kiếm và tham gia các kèo cầu lông phù hợp với trình độ của bạn, hoặc tự tạo kèo mới để mời mọi người cùng chơi!</p>
        <router-link to="/create-match" class="btn btn-primary btn-lg create-btn">
          + Tạo Kèo Mới Ngay
        </router-link>
      </div>
    </div>

    <div class="container content-section">
      <div class="page-header">
        <h2>Danh sách kèo hiện có</h2>
      </div>

      <FilterBar @filter-changed="onFilterChange" />

      <div v-if="matchStore.loading" class="match-list">
        <MatchCardSkeleton v-for="i in 3" :key="i" />
      </div>

      <div v-else-if="matchStore.error" class="text-danger text-center p-4">
        {{ matchStore.error }}
      </div>

      <div v-else-if="matchStore.matches.length === 0" class="empty-state card text-center">
        <div class="empty-icon">🏸</div>
        <h3>Chưa có kèo nào đang mở</h3>
        <p class="text-secondary">Hiện tại không có kèo nào phù hợp với bộ lọc của bạn hoặc chưa ai tạo kèo mới.</p>
        <router-link to="/create-match" class="btn btn-primary mt-3">
          Hãy là người đầu tiên tạo kèo!
        </router-link>
      </div>

      <div v-else class="match-list">
        <MatchCard 
          v-for="match in matchStore.matches" 
          :key="match.id" 
          :match="match" 
          :isHost="authStore.user?.id === match.hostUserId"
          @mark-full="handleMarkFull"
          @join="handleJoin"
          @leave="handleLeave"
        />
      </div>
    </div>

    <router-link to="/create-match" class="fab" v-if="authStore.isAuthenticated">
      +
    </router-link>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useMatchStore } from '@/stores/matches'
import { useAuthStore } from '@/stores/auth'
import { useToast } from 'vue-toastification'
import MatchCard from '@/components/MatchCard.vue'
import MatchCardSkeleton from '@/components/MatchCardSkeleton.vue'
import FilterBar from '@/components/FilterBar.vue'

const matchStore = useMatchStore()
const authStore = useAuthStore()
const toast = useToast()

onMounted(() => {
  matchStore.fetchMatches({ status: 1 }) // Mặc định lấy kèo đang mở
})

const onFilterChange = (filters) => {
  matchStore.fetchMatches(filters)
}

const handleMarkFull = async (matchId) => {
  if(confirm('Bạn xác nhận kèo này đã đủ người?')) {
    await matchStore.updateStatus(matchId, 2) // 2 = Full
    toast.success('Đã cập nhật trạng thái!')
  }
}

const handleJoin = async (matchId) => {
  if(confirm('Bạn xác nhận muốn tham gia kèo này?')) {
    const success = await matchStore.joinMatch(matchId)
    if (success) {
      toast.success('Tham gia thành công! Bạn có thể xem link Zalo của Host.')
      // Refetch to update participantIds and slots
      matchStore.fetchMatches({ status: 1 }) 
    } else {
      toast.error(matchStore.error || 'Tham gia thất bại')
    }
  }
}

const handleLeave = async (matchId) => {
  if(confirm('Bạn chắc chắn muốn hủy tham gia kèo này?')) {
    const success = await matchStore.leaveMatch(matchId)
    if (success) {
      toast.success('Đã hủy tham gia.')
      matchStore.fetchMatches({ status: 1 }) 
    } else {
      toast.error(matchStore.error || 'Hủy tham gia thất bại')
    }
  }
}
</script>

<style scoped>
.hero-banner {
  background: linear-gradient(135deg, var(--primary-color) 0%, #1e40af 100%);
  color: white;
  padding: 60px 20px;
  text-align: center;
  margin-bottom: 32px;
}
.hero-content {
  max-width: 800px;
  margin: 0 auto;
}
.hero-content h1 {
  font-size: 36px;
  margin-bottom: 16px;
}
.hero-content p {
  font-size: 18px;
  opacity: 0.9;
  margin-bottom: 32px;
}
.create-btn {
  font-size: 16px;
  padding: 14px 32px;
  border-radius: 30px;
  background: white;
  color: var(--primary-color);
  font-weight: bold;
  box-shadow: 0 4px 15px rgba(0,0,0,0.1);
  transition: transform 0.2s;
}
.create-btn:hover {
  transform: translateY(-2px);
  background: #f8fafc;
}

.content-section {
  padding-bottom: 60px;
}

.empty-state {
  padding: 60px 20px;
  margin-top: 20px;
  background: white;
  border-radius: var(--border-radius);
  border: 1px dashed var(--border-color);
}
.empty-icon {
  font-size: 48px;
  margin-bottom: 16px;
}
.empty-state h3 {
  margin-bottom: 8px;
}
.mt-3 {
  margin-top: 24px;
}
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
  margin-top: var(--spacing-md);
}
.match-list {
  display: flex;
  flex-direction: column;
  gap: var(--spacing-md);
  margin-top: var(--spacing-md);
}
.text-center { text-align: center; }
.p-4 { padding: 24px; }
.text-danger { color: var(--danger-color); }
  border: 1px dashed var(--border-color);
}
.fab {
  position: fixed;
  bottom: 24px;
  right: 24px;
  width: 56px;
  height: 56px;
  border-radius: 50%;
  background: var(--primary-color);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 28px;
  text-decoration: none;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3);
  z-index: 90;
  transition: transform 0.2s;
}
.fab:hover {
  transform: scale(1.05);
  background: var(--primary-hover);
}
@media (min-width: 640px) {
  .fab { display: none; }
}
</style>
