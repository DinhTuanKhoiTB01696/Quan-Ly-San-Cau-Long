<template>
  <div class="courts-view container">
    <div class="header-section" style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px;">
      <h2>Khám phá Sân Cầu Lông</h2>
      
      <!-- Lọc khu vực -->
      <div class="filter-controls">
        <select v-model="selectedArea" @change="fetchData" class="form-control" style="width: 200px;">
          <option :value="null">Tất cả khu vực</option>
          <option value="1">Tân Mai</option>
          <option value="2">Trảng Dài</option>
          <option value="3">Long Bình</option>
          <option value="4">Tân Hiệp</option>
          <option value="5">Hố Nai</option>
        </select>
      </div>
    </div>

    <div v-if="courtStore.loading" class="text-center py-4">
      <div class="spinner" style="width: 40px; height: 40px; border: 4px solid #e2e8f0; border-top-color: var(--primary-color); border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto;"></div>
    </div>

    <div v-else-if="courtStore.courts.length === 0" class="empty-state card text-center py-5">
      <div style="font-size: 48px; margin-bottom: 16px;">🏸</div>
      <h3>Không tìm thấy sân</h3>
      <p class="text-secondary">Chưa có sân nào ở khu vực này được cập nhật trên hệ thống.</p>
    </div>

    <div v-else class="courts-grid">
      <div 
        v-for="court in courtStore.courts" 
        :key="court.id" 
        class="card court-card"
        @click="goToDetail(court.id)"
      >
        <div class="court-image">
          <!-- Placeholder image since we don't have real images yet -->
          <div class="image-placeholder">
            <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1" style="opacity: 0.3;">
              <path d="M14.5 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V7.5L14.5 2z"></path>
              <polyline points="14 2 14 8 20 8"></polyline>
            </svg>
          </div>
          <div class="rating-badge" v-if="court.rating">
            ⭐ {{ court.rating }}
          </div>
          <div class="featured-badge" v-if="court.isFeatured">
            Nổi bật
          </div>
        </div>
        
        <div class="court-content">
          <h3 class="court-title">{{ court.name }}</h3>
          <p class="court-address text-secondary">
            <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="margin-right: 4px; vertical-align: middle;"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
            {{ court.address }}
          </p>
          
          <div class="court-features">
            <span class="feature-tag">Mặt sân {{ court.surface }}</span>
            <span class="feature-tag">Đèn {{ court.light }}</span>
          </div>
          
          <div class="court-footer">
            <div class="price font-bold" style="color: #10b981;">
              {{ formatCurrency(court.price) }}<span style="font-size: 12px; font-weight: normal; color: #64748b;"> / giờ</span>
            </div>
            <button class="btn btn-primary btn-sm">Xem chi tiết</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCourtStore } from '@/stores/courts'

const router = useRouter()
const courtStore = useCourtStore()
const selectedArea = ref(null)

const fetchData = () => {
  courtStore.fetchCourts(selectedArea.value)
}

onMounted(() => {
  if (courtStore.courts.length === 0) {
    fetchData()
  }
})

const goToDetail = (id) => {
  router.push(`/courts/${id}`)
}

const formatCurrency = (val) => {
  if (!val) return 'Liên hệ'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}
</script>

<style scoped>
.courts-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 24px;
}

.court-card {
  padding: 0;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  flex-direction: column;
}

.court-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 24px -8px rgba(0, 0, 0, 0.15);
  border-color: var(--primary-color);
}

.court-image {
  height: 160px;
  background: #f1f5f9;
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
}

.rating-badge {
  position: absolute;
  top: 12px;
  right: 12px;
  background: white;
  padding: 4px 8px;
  border-radius: 12px;
  font-size: 13px;
  font-weight: 600;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  color: #f59e0b;
}

.featured-badge {
  position: absolute;
  top: 12px;
  left: 12px;
  background: #ef4444;
  color: white;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 11px;
  font-weight: bold;
  text-transform: uppercase;
}

.court-content {
  padding: 16px;
  flex: 1;
  display: flex;
  flex-direction: column;
}

.court-title {
  margin: 0 0 8px 0;
  font-size: 18px;
}

.court-address {
  margin: 0 0 16px 0;
  font-size: 14px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.court-features {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-bottom: 16px;
}

.feature-tag {
  background: #f1f5f9;
  color: #475569;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
}

.court-footer {
  margin-top: auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-top: 1px solid var(--border-color);
  padding-top: 16px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
