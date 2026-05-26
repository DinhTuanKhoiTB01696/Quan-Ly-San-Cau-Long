<template>
  <div class="card match-card" :class="{'is-full': match.status === 1, 'is-expired': match.status === 2}" @click="goToDetail" style="cursor: pointer;">
    <div class="match-header">
      <div class="court-info">
        <h3 class="court-name">{{ match.court?.name || 'Sân không xác định' }}</h3>
        <p class="court-address">📍 {{ match.court?.address || '...' }}</p>
      </div>
      <div class="status-badge" :class="statusClass">
        {{ statusText }}
      </div>
    </div>

    <div class="match-details">
      <div class="detail-item">
        <span class="icon">📅</span>
        <span>{{ formatDate(match.date) }}</span>
      </div>
      <div class="detail-item">
        <span class="icon">⏰</span>
        <span class="time-highlight">{{ match.timeStart }} - {{ match.timeEnd }}</span>
      </div>
      <div class="detail-item">
        <span class="icon">👥</span>
        <span>Slots: <strong>{{ match.slotsFilled }} / {{ match.slotsTotal }}</strong></span>
      </div>
      <div class="detail-item">
        <span class="icon">⭐</span>
        <span>Trình độ: <strong>{{ levelText }}</strong></span>
      </div>
      <div class="detail-item">
        <span class="icon">💰</span>
        <span>Chi phí: <strong class="cost">{{ formatCurrency(match.cost) }}</strong></span>
      </div>
      <div class="detail-item full-width" v-if="match.note">
        <span class="icon">📝</span>
        <span class="note-text">"{{ match.note }}"</span>
      </div>
    </div>

    <div class="match-footer">
      <div class="host-info">
        <span class="host-avatar">{{ match.hostName.charAt(0).toUpperCase() }}</span>
        <span>{{ match.hostName }}</span>
      </div>
      <div class="actions">
        <!-- Host Logic -->
        <template v-if="isHost && match.status === 1">
          <button @click="$emit('mark-full', match.id)" class="btn btn-outline btn-sm">Đã đủ kèo</button>
        </template>
        
        <!-- Participant Logic -->
        <template v-if="!isHost && auth.isAuthenticated && match.status === 1">
          <button v-if="!hasJoined" @click.stop="$emit('join', match.id)" class="btn btn-primary btn-sm">Tham Gia</button>
          <template v-else>
            <button @click.stop="$emit('leave', match.id)" class="btn btn-outline btn-sm" style="color: var(--danger-color); border-color: var(--danger-color);">Hủy</button>
            <a :href="'https://zalo.me/' + match.zalo" target="_blank" @click.stop class="btn btn-primary btn-sm zalo-btn">
              💬 Liên hệ Host (Zalo)
            </a>
          </template>
        </template>
        
        <!-- Unauthenticated Logic -->
        <template v-if="!isHost && !auth.isAuthenticated && match.status === 1">
          <router-link to="/login" class="btn btn-primary btn-sm">Đăng nhập để tham gia</router-link>
        </template>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const auth = useAuthStore()

const props = defineProps({
  match: {
    type: Object,
    required: true
  },
  isHost: {
    type: Boolean,
    default: false
  }
})

defineEmits(['mark-full', 'join', 'leave'])

const goToDetail = () => {
  router.push(`/matches/${props.match.id}`)
}

const hasJoined = computed(() => {
  if (!auth.isAuthenticated || !auth.user) return false;
  return props.match.participantIds?.includes(auth.user.id);
})

// Status: 1=Open, 2=Full, 3=Expired
const statusClass = computed(() => {
  if (props.match.status === 2) return 'bg-gray'
  if (props.match.status === 3) return 'bg-red'
  return 'bg-green'
})

const statusText = computed(() => {
  if (props.match.status === 2) return 'Đã đủ người'
  if (props.match.status === 3) return 'Đã kết thúc'
  return 'Đang tuyển'
})

// Level: 1=Yeu, 2=Trung, 3=Kha, 4=Gioi
const levelText = computed(() => {
  const levels = {
    1: 'Yếu (Mới chơi)',
    2: 'Trung bình (Đánh rally 10+)',
    3: 'Khá (Biết smash)',
    4: 'Giỏi (Thi đấu phong trào)'
  }
  return levels[props.match.level] || 'Tất cả'
})

const formatDate = (dateStr) => {
  const date = new Date(dateStr)
  return new Intl.DateTimeFormat('vi-VN', { weekday: 'short', day: '2-digit', month: '2-digit' }).format(date)
}

const formatCurrency = (val) => {
  if (!val) return 'Miễn phí'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}
</script>

<style scoped>
.match-card {
  transition: all 0.2s;
  padding: 16px;
}
.match-card:hover {
  border-color: var(--primary-color);
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.05);
}
.is-full, .is-expired {
  opacity: 0.75;
}
.match-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 16px;
}
.court-name {
  font-size: 16px;
  font-weight: 600;
  margin-bottom: 4px;
}
.court-address {
  font-size: 13px;
  color: var(--text-secondary);
}
.status-badge {
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 12px;
  font-weight: 600;
}
.bg-green { background: #dcfce7; color: #166534; }
.bg-gray { background: #f1f5f9; color: #475569; }
.bg-red { background: #fee2e2; color: #991b1b; }

.match-details {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
  margin-bottom: 16px;
  background: var(--secondary-color);
  padding: 12px;
  border-radius: 8px;
}
.detail-item {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
}
.full-width {
  grid-column: 1 / -1;
  color: var(--text-secondary);
  font-style: italic;
}
.time-highlight {
  color: var(--primary-color);
  font-weight: 600;
}
.cost {
  color: var(--warning-color);
}
.match-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-top: 1px solid var(--border-color);
  padding-top: 12px;
}
.host-info {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 14px;
  font-weight: 500;
}
.host-avatar {
  width: 28px;
  height: 28px;
  background: var(--primary-color);
  color: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
}
.actions {
  display: flex;
  gap: 8px;
}
.zalo-btn {
  background-color: #0068FF;
}
.zalo-btn:hover {
  background-color: #005ce6;
}
.btn-sm {
  padding: 6px 12px;
  font-size: 13px;
}
</style>
