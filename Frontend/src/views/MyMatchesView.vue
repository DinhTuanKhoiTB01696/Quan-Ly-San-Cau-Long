<template>
  <div class="my-matches-view container">
    <div class="page-header">
      <router-link to="/" class="back-link">← Về trang chủ</router-link>
      <h2>Kèo Của Tôi</h2>
    </div>

    <div v-if="matchStore.loading" class="match-list p-4">
      <MatchCardSkeleton v-for="i in 2" :key="i" />
    </div>

    <div v-else-if="matchStore.myMatches.length === 0" class="empty-state p-4 text-center">
      Bạn chưa tạo kèo nào.
    </div>

    <div v-else class="match-list">
      <MatchCard 
        v-for="match in matchStore.myMatches" 
        :key="match.id" 
        :match="match" 
        :isHost="true"
        @mark-full="handleMarkFull"
      />
    </div>
  </div>
</template>

<script setup>
import { onMounted } from 'vue'
import { useMatchStore } from '@/stores/matches'
import MatchCard from '@/components/MatchCard.vue'
import MatchCardSkeleton from '@/components/MatchCardSkeleton.vue'

const matchStore = useMatchStore()

onMounted(() => {
  matchStore.fetchMyMatches()
})

const handleMarkFull = async (matchId) => {
  if(confirm('Bạn xác nhận kèo này đã đủ người?')) {
    await matchStore.updateStatus(matchId, 1) // 1 = Full
  }
}
</script>

<style scoped>
.page-header {
  margin-bottom: var(--spacing-md);
  margin-top: var(--spacing-md);
}
.back-link {
  display: inline-block;
  color: var(--text-secondary);
  text-decoration: none;
  font-size: 14px;
  margin-bottom: 8px;
}
.match-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.empty-state {
  color: var(--text-secondary);
  background: white;
  border-radius: var(--border-radius);
  border: 1px dashed var(--border-color);
}
.text-center { text-align: center; }
.p-4 { padding: 24px; }
</style>
