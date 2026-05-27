<template>
  <div class="court-detail-view container" v-if="court">
    <!-- Nút Back -->
    <div class="mb-4">
      <router-link to="/courts" class="btn btn-outline" style="display: inline-flex; align-items: center; gap: 8px;">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <line x1="19" y1="12" x2="5" y2="12"></line>
          <polyline points="12 19 5 12 12 5"></polyline>
        </svg>
        Quay lại danh sách sân
      </router-link>
    </div>

    <!-- Hero Image Area -->
    <div class="court-hero" style="height: 300px; background: #e2e8f0; border-radius: 12px; margin-bottom: 32px; display: flex; align-items: center; justify-content: center; position: relative; overflow: hidden;">
      <svg xmlns="http://www.w3.org/2000/svg" width="80" height="80" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" style="opacity: 0.2; color: #475569;">
        <path d="M14.5 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7.5L14.5 2z"></path>
        <polyline points="14 2 14 8 20 8"></polyline>
      </svg>
      <div v-if="court.isFeatured" style="position: absolute; top: 16px; left: 16px; background: #ef4444; color: white; padding: 6px 12px; border-radius: 6px; font-weight: bold; text-transform: uppercase; font-size: 14px;">
        Sân Nổi Bật
      </div>
      <div v-if="court.rating" style="position: absolute; top: 16px; right: 16px; background: white; color: #f59e0b; padding: 6px 12px; border-radius: 20px; font-weight: bold; font-size: 16px; box-shadow: 0 4px 6px rgba(0,0,0,0.1);">
        ⭐ {{ court.rating }} / 5.0
      </div>
    </div>

    <div class="grid" style="display: grid; grid-template-columns: 2fr 1fr; gap: 32px;">
      <!-- Main Content -->
      <div class="main-column">
        <h1 style="font-size: 32px; margin: 0 0 12px 0;">{{ court.name }}</h1>
        <p class="text-secondary" style="font-size: 16px; display: flex; align-items: center; gap: 8px; margin-bottom: 24px;">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
          {{ court.address }}
        </p>

        <div class="features-grid" style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 32px;">
          <div class="feature-card" style="padding: 16px; background: #f8fafc; border-radius: 8px; border: 1px solid var(--border-color);">
            <div class="text-secondary mb-1" style="font-size: 13px;">Khu vực</div>
            <div class="font-bold" style="font-size: 16px;">{{ getAreaName(court.area) }}</div>
          </div>
          <div class="feature-card" style="padding: 16px; background: #f8fafc; border-radius: 8px; border: 1px solid var(--border-color);">
            <div class="text-secondary mb-1" style="font-size: 13px;">Mặt sân</div>
            <div class="font-bold" style="font-size: 16px;">{{ court.surface }}</div>
          </div>
          <div class="feature-card" style="padding: 16px; background: #f8fafc; border-radius: 8px; border: 1px solid var(--border-color);">
            <div class="text-secondary mb-1" style="font-size: 13px;">Ánh sáng</div>
            <div class="font-bold" style="font-size: 16px;">{{ court.light }}</div>
          </div>
          <div class="feature-card" style="padding: 16px; background: #f8fafc; border-radius: 8px; border: 1px solid var(--border-color);">
            <div class="text-secondary mb-1" style="font-size: 13px;">Chiều cao trần</div>
            <div class="font-bold" style="font-size: 16px;">{{ court.ceiling }}</div>
          </div>
        </div>

        <h3>Giới thiệu chung</h3>
        <p style="line-height: 1.6; color: #475569; margin-bottom: 32px;">
          Sân cầu lông {{ court.name }} là một trong những sân tập và thi đấu chất lượng tại khu vực {{ getAreaName(court.area) }}. 
          Với hệ thống thảm {{ court.surface }} đạt chuẩn, đèn chiếu sáng {{ court.light }} chống chói, 
          và không gian thoáng đãng (trần {{ court.ceiling }}), sân luôn là sự lựa chọn hàng đầu của các lông thủ.
        </p>

        <!-- Google Maps Integration -->
        <h3 style="margin-bottom: 16px;">Vị trí trên Bản đồ</h3>
        <div class="map-container" style="margin-bottom: 32px; border-radius: 12px; overflow: hidden; border: 1px solid var(--border-color); box-shadow: var(--shadow-sm);">
          <iframe 
            width="100%" 
            height="300" 
            style="border:0;" 
            loading="lazy" 
            allowfullscreen 
            referrerpolicy="no-referrer-when-downgrade" 
            :src="`https://maps.google.com/maps?q=${encodeURIComponent(court.name + ' ' + court.address)}&t=&z=15&ie=UTF8&iwloc=&output=embed`">
          </iframe>
        </div>

        <!-- Kèo đang mở tại sân này -->
        <h3 style="margin-bottom: 16px;">Kèo đang mở tại đây</h3>
        <div v-if="courtMatches.length === 0" class="empty-state text-center text-secondary py-4" style="background: #f8fafc; border-radius: 8px;">
          Hiện chưa có kèo nào đang tuyển tại sân này.
        </div>
        <div v-else style="display: flex; flex-direction: column; gap: 16px;">
          <!-- Custom small Match items -->
          <div v-for="match in courtMatches" :key="match.id" class="small-match-card" style="border: 1px solid var(--border-color); border-radius: 8px; padding: 16px; display: flex; justify-content: space-between; align-items: center; cursor: pointer; transition: all 0.2s;" @click="router.push(`/matches/${match.id}`)">
            <div>
              <div style="font-weight: bold; margin-bottom: 4px;">{{ formatDate(match.date) }} | {{ formatTime(match.timeStart) }} - {{ formatTime(match.timeEnd) }}</div>
              <div style="font-size: 13px; color: #64748b;">Trình độ: {{ getLevelText(match.level) }} • Slots: {{ match.slotsFilled }}/{{ match.slotsTotal }}</div>
            </div>
            <button class="btn btn-primary btn-sm">Xem kèo</button>
          </div>
        </div>
      </div>

      <!-- Sidebar -->
      <div class="sidebar">
        <div class="card" style="position: sticky; top: 80px;">
          <div style="font-size: 24px; font-weight: bold; color: #10b981; margin-bottom: 16px; text-align: center;">
            {{ formatCurrency(court.price) }}<span style="font-size: 14px; color: #64748b; font-weight: normal;"> / giờ</span>
          </div>
          
          <hr style="border: 0; border-top: 1px solid var(--border-color); margin: 16px 0;" />

          <div style="margin-bottom: 24px;">
            <div style="font-weight: 600; margin-bottom: 8px;">Liên hệ đặt sân</div>
            <a :href="'tel:' + court.phone" class="btn btn-outline w-100" style="display: flex; align-items: center; justify-content: center; gap: 8px; border-color: #0ea5e9; color: #0ea5e9;">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"></path></svg>
              {{ court.phone || 'Chưa cập nhật' }}
            </a>
          </div>
          
          <router-link :to="{ path: '/create-match', query: { courtId: court.id } }" class="btn btn-primary w-100 text-center">
            Tạo kèo tại sân này
          </router-link>
        </div>
      </div>
    </div>
  </div>

  <div v-else-if="loading" class="container text-center" style="padding: 100px 0;">
    <div class="spinner" style="width: 40px; height: 40px; border: 4px solid #e2e8f0; border-top-color: var(--primary-color); border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 20px;"></div>
    <p>Đang tải thông tin sân...</p>
  </div>

  <div v-else class="container text-center" style="padding: 100px 0;">
    <h2>Không tìm thấy sân</h2>
    <p class="text-secondary mb-4">Sân này có thể đã bị xóa hoặc không tồn tại.</p>
    <router-link to="/courts" class="btn btn-primary">Quay lại danh sách</router-link>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useToast } from 'vue-toastification'
import api from '@/api/axios'

const route = useRoute()
const router = useRouter()
const toast = useToast()

const court = ref(null)
const courtMatches = ref([])
const loading = ref(true)

const fetchCourtAndMatches = async () => {
  loading.value = true
  try {
    // Fetch Court info
    const courtRes = await api.get(`/courts/${route.params.id}`)
    court.value = courtRes.data

    // Fetch matches for this court (We use the public matches API)
    // Unfortunately backend doesn't have a specific `courtId` filter in `GetAllAsync`,
    // so we fetch all open matches in that Area, then filter by courtId locally.
    // This is a workaround since we didn't add CourtId to IMatchService.GetAllAsync.
    const matchesRes = await api.get(`/matches?area=${court.value.area}&status=1`)
    courtMatches.value = matchesRes.data.filter(m => m.courtId === court.value.id)
    
  } catch (err) {
    console.error(err)
    if (err.response?.status === 404) {
      toast.error('Sân không tồn tại')
    } else {
      toast.error('Lỗi khi tải thông tin sân')
    }
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchCourtAndMatches()
})

// Helpers
const getAreaName = (areaId) => {
  const areas = { 1: 'Tân Mai', 2: 'Trảng Dài', 3: 'Long Bình', 4: 'Tân Hiệp', 5: 'Hố Nai', 99: 'Khác' }
  return areas[areaId] || 'Không rõ'
}

const formatCurrency = (val) => {
  if (!val) return 'Liên hệ'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}

const getLevelText = (level) => {
  const levels = { 1: 'Yếu', 2: 'Trung bình', 3: 'Khá', 4: 'Giỏi' }
  return levels[level] || 'Tất cả'
}

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return new Intl.DateTimeFormat('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' }).format(date)
}

const formatTime = (timeStr) => {
  if (!timeStr) return ''
  const parts = timeStr.split(':')
  return `${parts[0]}:${parts[1]}`
}
</script>

<style scoped>
@media (max-width: 768px) {
  .grid {
    grid-template-columns: 1fr !important;
  }
  .sidebar {
    order: -1;
  }
}

.small-match-card:hover {
  border-color: var(--primary-color);
  background-color: #f8fafc;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
