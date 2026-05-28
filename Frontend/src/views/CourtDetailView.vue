<template>
  <div class="court-detail-container container animate-fade" v-if="court">
    <!-- Nút Quay lại -->
    <div class="back-navigation">
      <router-link to="/courts" class="btn btn-outline back-btn">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
          <line x1="19" y1="12" x2="5" y2="12"></line>
          <polyline points="12 19 5 12 12 5"></polyline>
        </svg>
        Quay lại danh sách sân
      </router-link>
    </div>

    <!-- Hero Image Area -->
    <div class="court-hero-banner">
      <div class="court-hero-overlay"></div>
      <div class="court-hero-icon">🏸</div>
      
      <div v-if="court.isFeatured" class="hero-featured-badge">
        ✨ SÂN NỔI BẬT
      </div>
      <div v-if="court.rating" class="hero-rating-badge">
        ⭐ {{ court.rating.toFixed(1) }} / 5.0
      </div>
    </div>

    <div class="court-detail-grid">
      <!-- Main Content Column -->
      <div class="main-column">
        <h1 class="court-detail-title text-white">{{ court.name }}</h1>
        <p class="court-detail-address text-secondary">
          <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="text-primary"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
          {{ court.address }}
        </p>

        <!-- Features Grid -->
        <div class="features-grid">
          <div class="feature-detail-card card">
            <div class="feat-desc text-secondary">Khu vực</div>
            <div class="feat-val text-white">{{ getAreaName(court.area) }}</div>
          </div>
          <div class="feature-detail-card card">
            <div class="feat-desc text-secondary">Mặt sân</div>
            <div class="feat-val text-white">{{ court.surface }}</div>
          </div>
          <div class="feature-detail-card card">
            <div class="feat-desc text-secondary">Ánh sáng</div>
            <div class="feat-val text-white">{{ court.light }}</div>
          </div>
          <div class="feature-detail-card card">
            <div class="feat-desc text-secondary">Chiều cao trần</div>
            <div class="feat-val text-white">{{ court.ceiling }}</div>
          </div>
        </div>

        <!-- Amenities Section -->
        <div class="detail-section card">
          <h3 class="section-sub-title">🏟️ Tiện ích tại sân</h3>
          <div class="amenities-list-grid">
            <div class="amenity-item">
              <span class="amenity-icon">📶</span>
              <div class="amenity-info">
                <strong>Wifi Miễn Phí</strong>
                <span class="text-secondary small">Tốc độ cao</span>
              </div>
            </div>
            <div class="amenity-item">
              <span class="amenity-icon">🚗</span>
              <div class="amenity-info">
                <strong>Bãi Đỗ Xe Rộng</strong>
                <span class="text-secondary small">Có chỗ đỗ ô tô</span>
              </div>
            </div>
            <div class="amenity-item">
              <span class="amenity-icon">🥤</span>
              <div class="amenity-info">
                <strong>Căn Tin Ăn Uống</strong>
                <span class="text-secondary small">Nước uống & đồ ăn nhẹ</span>
              </div>
            </div>
            <div class="amenity-item">
              <span class="amenity-icon">👟</span>
              <div class="amenity-info">
                <strong>Thuê Vợt & Giày</strong>
                <span class="text-secondary small">Dành cho khách lẻ</span>
              </div>
            </div>
            <div class="amenity-item">
              <span class="amenity-icon">Shower 🚿</span>
              <div class="amenity-info">
                <strong>Phòng Tắm Sạch Sẽ</strong>
                <span class="text-secondary small">Có nước tắm nóng lạnh</span>
              </div>
            </div>
          </div>
        </div>

        <!-- Description -->
        <div class="detail-section card">
          <h3 class="section-sub-title">📝 Giới thiệu chung</h3>
          <p class="court-intro-text text-secondary">
            Sân cầu lông <span class="text-white fw-bold">{{ court.name }}</span> là địa điểm tập luyện và thi đấu phong trào lý tưởng tại khu vực <span class="text-primary fw-bold">{{ getAreaName(court.area) }}</span>. 
            Sân được đầu tư thảm cao su <span class="text-white fw-bold">{{ court.surface }}</span> chống trơn trượt cực tốt, hệ thống dàn đèn chiếu sáng <span class="text-white fw-bold">{{ court.light }}</span> chống chói hiện đại, 
            kèm theo trần nhà cao <span class="text-white fw-bold">{{ court.ceiling }}</span> thông thoáng giúp những pha đập cầu, phông cầu của bạn diễn ra hoàn hảo nhất.
          </p>
        </div>

        <!-- Google Maps Integration -->
        <div class="detail-section card">
          <h3 class="section-sub-title">🗺️ Vị trí & Chỉ đường</h3>
          <div class="map-iframe-wrapper">
            <iframe 
              width="100%" 
              height="320" 
              style="border:0;" 
              loading="lazy" 
              allowfullscreen 
              referrerpolicy="no-referrer-when-downgrade" 
              class="detail-google-map"
              :src="`https://maps.google.com/maps?q=${encodeURIComponent(court.name + ' ' + court.address)}&t=&z=16&ie=UTF8&iwloc=&output=embed`">
            </iframe>
          </div>
        </div>

        <!-- Match List at this Court -->
        <div class="detail-section card">
          <h3 class="section-sub-title">🏸 Kèo đang tuyển tại sân này</h3>
          <div v-if="courtMatches.length === 0" class="empty-matches-card text-center text-secondary py-4">
            Hiện tại chưa có kèo giao lưu nào đang mở tại sân này. Hãy là người đầu tiên tạo kèo!
          </div>
          <div v-else class="match-list-stack">
            <div 
              v-for="match in courtMatches" 
              :key="match.id" 
              class="match-card-item card" 
              @click="router.push(`/matches/${match.id}`)"
            >
              <div class="match-card-info">
                <div class="match-time-row">
                  <span class="text-primary fw-bold">{{ formatDate(match.date) }}</span>
                  <span class="text-secondary ml-2">@ {{ formatTime(match.timeStart) }} - {{ formatTime(match.timeEnd) }}</span>
                </div>
                <div class="match-meta-row text-secondary">
                  Trình độ: <span class="badge badge-secondary">{{ getLevelText(match.level) }}</span>
                  <span class="mx-2">•</span>
                  Thành viên: <span class="badge badge-primary">{{ match.slotsFilled }} / {{ match.slotsTotal }} slots</span>
                </div>
              </div>
              <button class="btn btn-primary btn-sm">Tham Gia</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Sidebar Column -->
      <div class="sidebar-column">
        <div class="sticky-sidebar card">
          <div class="price-showcase">
            <span class="price-amount text-primary">{{ formatCurrency(court.price) }}</span>
            <span class="price-label text-secondary"> / giờ thuê</span>
          </div>
          
          <hr class="divider-line" />

          <!-- Phone contact -->
          <div class="contact-box">
            <span class="text-secondary small fw-bold uppercase">Hotline Đặt Sân</span>
            <a :href="'tel:' + court.phone" class="btn btn-outline contact-phone-btn">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5"><path d="M22 16.92v3a2 2 0 0 1-2.18 2 19.79 19.79 0 0 1-8.63-3.07 19.5 19.5 0 0 1-6-6 19.79 19.79 0 0 1-3.07-8.67A2 2 0 0 1 4.11 2h3a2 2 0 0 1 2 1.72 12.84 12.84 0 0 0 .7 2.81 2 2 0 0 1-.45 2.11L8.09 9.91a16 16 0 0 0 6 6l1.27-1.27a2 2 0 0 1 2.11-.45 12.84 12.84 0 0 0 2.81.7A2 2 0 0 1 22 16.92z"></path></svg>
              {{ court.phone || 'Chưa cập nhật SĐT' }}
            </a>
            
            <a 
              v-if="court.phone" 
              :href="`https://zalo.me/${court.phone.replace(/[^0-9]/g, '')}`" 
              target="_blank" 
              class="btn btn-primary zalo-direct-btn"
            >
              💬 Nhắn Zalo chủ sân
            </a>
          </div>
          
          <router-link :to="{ path: '/create-match', query: { courtId: court.id } }" class="btn btn-primary w-100 create-match-button mt-3">
            Tạo kèo giao lưu tại đây
          </router-link>
        </div>
      </div>
    </div>

    <!-- FLOATING CONTACT ZALO BUTTON (Mobile/Desktop) -->
    <div class="floating-zalo-wrapper" v-if="court.phone">
      <a 
        :href="`https://zalo.me/${court.phone.replace(/[^0-9]/g, '')}`" 
        target="_blank" 
        class="floating-zalo-btn"
        title="Nhắn tin Zalo đặt sân ngay"
      >
        <span class="zalo-floating-icon">💬</span>
        <span class="zalo-floating-text">Đặt sân Zalo</span>
      </a>
    </div>
  </div>

  <!-- Loading State -->
  <div v-else-if="loading" class="container text-center py-5 card" style="margin-top: 100px;">
    <div class="spinner"></div>
    <p class="mt-3 text-secondary">Đang tải chi tiết sân cầu lông...</p>
  </div>

  <!-- Error State -->
  <div v-else class="container text-center py-5 card" style="margin-top: 100px;">
    <h2 class="text-danger">Không tìm thấy sân đấu</h2>
    <p class="text-secondary mb-4">Sân đấu này có thể đã bị xóa khỏi hệ thống hoặc không tồn tại.</p>
    <router-link to="/courts" class="btn btn-primary">Quay lại danh sách sân</router-link>
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
    const courtRes = await api.get(`/courts/${route.params.id}`)
    court.value = courtRes.data

    const matchesRes = await api.get(`/matches?area=${court.value.area}&status=1`)
    courtMatches.value = matchesRes.data.filter(m => m.courtId === court.value.id)
    
  } catch (err) {
    console.error(err)
    if (err.response?.status === 404) {
      toast.error('Sân không tồn tại trên hệ thống')
    } else {
      toast.error('Lỗi khi tải thông tin chi tiết sân')
    }
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchCourtAndMatches()
})

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
.court-detail-container {
  padding-bottom: 80px;
}

.back-navigation {
  margin-bottom: 24px;
}

.back-btn {
  background: rgba(255, 255, 255, 0.02);
}

/* HERO BANNER */
.court-hero-banner {
  height: 320px;
  background: linear-gradient(135deg, #1e293b 0%, #0b0f19 100%);
  border-radius: var(--border-radius);
  margin-bottom: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: hidden;
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-md);
}

.court-hero-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle, rgba(163, 230, 53, 0.08) 0%, transparent 80%);
}

.court-hero-icon {
  font-size: 90px;
  z-index: 1;
  filter: drop-shadow(0 0 15px rgba(163, 230, 53, 0.4));
  animation: float 4s ease-in-out infinite;
}

.hero-featured-badge {
  position: absolute;
  top: 20px;
  left: 20px;
  background: linear-gradient(135deg, #f43f5e 0%, #e11d48 100%);
  color: white;
  padding: 6px 14px;
  border-radius: 8px;
  font-weight: 800;
  font-size: 12px;
  letter-spacing: 0.05em;
  box-shadow: 0 4px 10px rgba(244, 63, 94, 0.4);
}

.hero-rating-badge {
  position: absolute;
  top: 20px;
  right: 20px;
  background: rgba(15, 23, 42, 0.8);
  border: 1px solid rgba(245, 158, 11, 0.3);
  color: #f59e0b;
  padding: 6px 16px;
  border-radius: 20px;
  font-weight: 800;
  font-size: 15px;
  box-shadow: var(--shadow-sm);
  backdrop-filter: blur(8px);
}

/* LAYOUT GRID */
.court-detail-grid {
  display: grid;
  grid-template-columns: 2fr 1fr;
  gap: 32px;
}

.court-detail-title {
  font-size: 34px;
  font-weight: 800;
  margin-bottom: 8px;
  letter-spacing: -0.02em;
  text-shadow: 0 0 15px rgba(255,255,255,0.1);
}

.court-detail-address {
  font-size: 16px;
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 24px;
}

/* FEATURES GRID */
.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(130px, 1fr));
  gap: 16px;
  margin-bottom: 30px;
}

.feature-detail-card {
  padding: 16px;
  margin-bottom: 0;
  border-color: rgba(255, 255, 255, 0.03);
  background: rgba(30, 41, 59, 0.3);
}

.feat-desc {
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 6px;
}

.feat-val {
  font-size: 18px;
  font-weight: 800;
}

.detail-section {
  margin-bottom: 24px;
  padding: 24px;
}

.section-sub-title {
  font-size: 18px;
  font-weight: 800;
  margin-bottom: 20px;
  color: var(--text-primary);
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  padding-bottom: 10px;
}

/* AMENITIES */
.amenities-list-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.amenity-item {
  display: flex;
  align-items: center;
  gap: 14px;
  padding: 12px;
  background: rgba(255, 255, 255, 0.02);
  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.03);
}

.amenity-icon {
  font-size: 24px;
  filter: drop-shadow(0 0 5px rgba(163, 230, 53, 0.3));
}

.amenity-info {
  display: flex;
  flex-direction: column;
}

.amenity-info strong {
  font-size: 14px;
  color: var(--text-primary);
}

.court-intro-text {
  line-height: 1.7;
  font-size: 15px;
}

/* GOOGLE MAPS */
.map-iframe-wrapper {
  border-radius: 12px;
  overflow: hidden;
  border: 1px solid rgba(255, 255, 255, 0.08);
}

.detail-google-map {
  border: 0;
  display: block;
  filter: invert(90%) hue-rotate(180deg) brightness(95%) contrast(90%);
}

/* MATCHES AT COURT */
.empty-matches-card {
  background: rgba(255, 255, 255, 0.01);
  border-radius: 10px;
  font-size: 14px;
}

.match-list-stack {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.match-card-item {
  margin-bottom: 0;
  padding: 16px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
  border-color: rgba(255, 255, 255, 0.03);
}

.match-card-item:hover {
  border-color: var(--primary-color);
  box-shadow: var(--shadow-neon);
}

.match-time-row {
  margin-bottom: 6px;
}

.match-time-row span {
  font-size: 15px;
}

.match-meta-row {
  font-size: 13px;
  display: flex;
  align-items: center;
}

.badge {
  display: inline-flex;
  padding: 2px 8px;
  border-radius: 10px;
  font-size: 10px;
  font-weight: 700;
  text-transform: uppercase;
  margin-left: 6px;
}

.badge-secondary {
  background: rgba(148, 163, 184, 0.15);
  color: var(--text-secondary);
}

.badge-primary {
  background: rgba(163, 230, 53, 0.15);
  color: var(--primary-color);
  border: 1px solid rgba(163, 230, 53, 0.25);
}

/* STICKY SIDEBAR */
.sticky-sidebar {
  position: sticky;
  top: 90px;
  padding: 24px;
  background: rgba(15, 23, 42, 0.6);
}

.price-showcase {
  text-align: center;
  margin-bottom: 20px;
}

.price-amount {
  font-size: 28px;
  font-weight: 900;
  text-shadow: 0 0 15px rgba(163, 230, 53, 0.4);
}

.price-label {
  font-size: 14px;
}

.divider-line {
  border: 0;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
  margin: 20px 0;
}

.contact-box {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.contact-box span {
  letter-spacing: 0.05em;
}

.contact-phone-btn {
  border-color: #0ea5e9;
  color: #0ea5e9;
  width: 100%;
}

.contact-phone-btn:hover {
  background: rgba(14, 165, 233, 0.1);
  border-color: #0ea5e9;
  box-shadow: 0 0 10px rgba(14, 165, 233, 0.3);
}

.zalo-direct-btn {
  background: linear-gradient(135deg, #2563eb 0%, #1d4ed8 100%);
  color: white;
  width: 100%;
  box-shadow: 0 4px 12px rgba(37, 99, 235, 0.3);
}

.zalo-direct-btn:hover {
  box-shadow: 0 0 15px rgba(37, 99, 235, 0.5);
  transform: translateY(-1px);
}

.create-match-button {
  box-shadow: var(--shadow-neon);
}

/* FLOATING ZALO BUTTON */
.floating-zalo-wrapper {
  position: fixed;
  bottom: 30px;
  right: 30px;
  z-index: 999;
}

.floating-zalo-btn {
  display: flex;
  align-items: center;
  gap: 8px;
  background: linear-gradient(135deg, #0ea5e9 0%, #2563eb 100%);
  color: white;
  padding: 12px 20px;
  border-radius: 30px;
  text-decoration: none;
  font-weight: 700;
  font-size: 14px;
  box-shadow: 0 8px 24px rgba(14, 165, 233, 0.4), 0 0 15px rgba(37, 99, 235, 0.3);
  transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  animation: pulse-blue 2s infinite;
}

.floating-zalo-btn:hover {
  transform: translateY(-4px) scale(1.05);
  box-shadow: 0 12px 30px rgba(14, 165, 233, 0.6), 0 0 25px rgba(37, 99, 235, 0.5);
}

.zalo-floating-icon {
  font-size: 18px;
}

/* SPINNER & ANIMATIONS */
.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid rgba(163, 230, 53, 0.1);
  border-top-color: var(--primary-color);
  border-radius: 50%;
  animation: spin 1s infinite linear;
  margin: 0 auto;
}

.ml-2 { margin-left: 8px; }
.mx-2 { margin-left: 8px; margin-right: 8px; }
.mt-3 { margin-top: 16px; }

@keyframes float {
  0% { transform: translateY(0px); }
  50% { transform: translateY(-10px); }
  100% { transform: translateY(0px); }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes pulse-blue {
  0% { box-shadow: 0 0 0 0 rgba(14, 165, 233, 0.5); }
  70% { box-shadow: 0 0 0 12px rgba(14, 165, 233, 0); }
  100% { box-shadow: 0 0 0 0 rgba(14, 165, 233, 0); }
}

/* RESPONSIVE STYLES */
@media (max-width: 991px) {
  .court-detail-grid {
    grid-template-columns: 1fr;
  }
  
  .sticky-sidebar {
    position: relative;
    top: 0;
  }
  
  .sidebar-column {
    order: -1;
  }
}
</style>
