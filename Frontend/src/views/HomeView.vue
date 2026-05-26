<template>
  <div class="home-view">
    <div class="page-header">
      <h2>Tìm Kèo Cầu Lông Biên Hòa</h2>
      <router-link to="/create-match" class="btn btn-primary" v-if="authStore.isAuthenticated">
        + Đăng Kèo Mới
      </router-link>
    </div>

    <FilterBar @filter-changed="onFilterChange" />

    <div v-if="matchStore.loading" class="match-list">
      <MatchCardSkeleton v-for="i in 3" :key="i" />
    </div>

    <div v-else-if="matchStore.error" class="text-danger text-center p-4">
      {{ matchStore.error }}
    </div>

    <div v-else-if="matchStore.matches.length === 0" class="text-center p-4 empty-state">
      Chưa có kèo nào phù hợp với bộ lọc hiện tại.
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
.page-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-md);
  margin-top: var(--spacing-md);
}
.page-header h2 {
  font-size: 20px;
}
.match-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.text-center { text-align: center; }
.p-4 { padding: 24px; }
.text-danger { color: var(--danger-color); }
.empty-state {
  color: var(--text-secondary);
  background: white;
  border-radius: var(--border-radius);
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
