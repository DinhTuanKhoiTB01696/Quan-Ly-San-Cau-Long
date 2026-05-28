<template>
  <div class="courts-view-container container">
    <!-- Header Section -->
    <div class="header-section animate-fade">
      <div class="header-title">
        <span class="fire-emoji">🔥</span>
        <h2>Hệ Thống Sân Cầu Lông Biên Hòa</h2>
      </div>
      
      <!-- Filters -->
      <div class="filter-controls">
        <div class="search-box">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" class="search-icon"><circle cx="11" cy="11" r="8"></circle><line x1="21" y1="21" x2="16.65" y2="16.65"></line></svg>
          <input 
            type="text" 
            v-model="searchQuery" 
            placeholder="Tìm theo tên sân, địa chỉ..." 
            class="filter-input" 
          />
        </div>
        
        <select v-model="selectedArea" @change="fetchData" class="filter-select">
          <option :value="null">🌍 Tất cả khu vực</option>
          <option value="1">📍 Tân Mai</option>
          <option value="2">📍 Trảng Dài</option>
          <option value="3">📍 Long Bình</option>
          <option value="4">📍 Tân Hiệp</option>
          <option value="5">📍 Hố Nai</option>
        </select>
      </div>
    </div>

    <div v-if="courtStore.loading" class="text-center py-5 card">
      <div class="spinner"></div>
      <p class="mt-3 text-secondary">Đang tải dữ liệu sân đấu...</p>
    </div>

    <div v-else-if="filteredCourts.length === 0" class="empty-state card text-center py-5">
      <div class="empty-icon">🏸</div>
      <h3>Không tìm thấy sân đấu nào</h3>
      <p class="text-secondary">Không có sân nào phù hợp với bộ lọc tìm kiếm hiện tại.</p>
    </div>

    <!-- MAIN TWO-COLUMN LAYOUT -->
    <div v-else class="courts-layout-grid animate-fade">
      <!-- COLUMN 1: COURTS LIST -->
      <div class="courts-list-column">
        <div 
          v-for="court in filteredCourts" 
          :key="court.id" 
          class="card court-item-card"
          :class="{ 'active-selection': activeCourtId === court.id }"
          @click="selectCourt(court)"
        >
          <!-- NEW: Court Image Area -->
          <div class="court-card-banner">
            <img 
              :src="court.imageUrl || 'https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?w=800'" 
              alt="Hình ảnh sân cầu lông Biên Hòa" 
              class="court-img-pic"
              loading="lazy"
            />
            <div class="court-featured-banner" v-if="court.isFeatured">✨ Nổi Bật</div>
            <span class="rating-badge-banner">⭐ {{ court.rating.toFixed(1) }}</span>
          </div>

          <div class="court-card-body">
            <div class="court-badge-row">
              <span class="badge badge-area">{{ areaName(court.area) }}</span>
            </div>
            
            <h3 class="court-name-title">{{ court.name }}</h3>
            
            <p class="court-addr text-secondary">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" class="addr-icon"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
              {{ court.address }}
            </p>
            
            <div class="court-amenities">
              <span class="amenity-tag">Mặt {{ court.surface }}</span>
              <span class="amenity-tag">Đèn {{ court.light }}</span>
            </div>
            
            <div class="court-card-footer">
              <div class="court-price">
                <span class="price-val">{{ formatCurrency(court.price) }}</span>
                <span class="price-unit"> / giờ</span>
              </div>
              <div class="card-btn-group">
                <button class="btn btn-outline btn-sm map-btn" @click.stop="focusOnMap(court)">Bản đồ 🗺️</button>
                <button class="btn btn-primary btn-sm" @click.stop="goToDetail(court.id)">Chi tiết</button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- COLUMN 2: LARGE INTERACTIVE MAP -->
      <div class="courts-map-column">
        <div class="map-sticky-card card">
          <div class="map-header">
            <div class="map-title-info">
              <h4>Bản Đồ Chỉ Đường</h4>
              <p class="text-secondary small">{{ activeCourtName }}</p>
            </div>
            <a 
              :href="'https://www.google.com/maps/search/?api=1&query=' + encodeURIComponent(activeAddress)" 
              target="_blank" 
              class="btn btn-outline btn-sm external-map-link"
            >
              Mở trên Google Maps ↗
            </a>
          </div>
          
          <div class="map-iframe-container">
            <iframe 
              :src="`https://maps.google.com/maps?q=${encodeURIComponent(activeAddress)}&t=&z=16&ie=UTF8&iwloc=&output=embed`"
              frameborder="0" 
              scrolling="no" 
              marginheight="0" 
              marginwidth="0"
              class="google-map-iframe"
            ></iframe>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useCourtStore } from '@/stores/courts'

const router = useRouter()
const courtStore = useCourtStore()
const selectedArea = ref(null)
const searchQuery = ref('')

const activeCourtId = ref(null)
const activeCourtName = ref('Chọn một sân cầu lông để hiển thị bản đồ chỉ đường')
const activeAddress = ref('Biên Hòa, Đồng Nai')

const filteredCourts = computed(() => {
  let list = courtStore.courts;
  
  if (searchQuery.value) {
    const q = searchQuery.value.toLowerCase();
    list = list.filter(c => c.name.toLowerCase().includes(q) || c.address.toLowerCase().includes(q));
  }
  
  return list;
})

const fetchData = () => {
  courtStore.fetchCourts(selectedArea.value)
}

onMounted(async () => {
  if (courtStore.courts.length === 0) {
    await courtStore.fetchCourts()
  }
  
  // Set default active court to first one
  if (courtStore.courts.length > 0) {
    selectCourt(courtStore.courts[0])
  }
})

const selectCourt = (court) => {
  activeCourtId.value = court.id
  activeCourtName.value = court.name
  activeAddress.value = court.name + ', ' + court.address
}

const focusOnMap = (court) => {
  selectCourt(court)
  // Scroll map column into view on mobile
  const mapElement = document.querySelector('.courts-map-column')
  if (mapElement && window.innerWidth < 992) {
    mapElement.scrollIntoView({ behavior: 'smooth' })
  }
}

const goToDetail = (id) => {
  router.push(`/courts/${id}`)
}

const formatCurrency = (val) => {
  if (!val) return 'Liên hệ'
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}

const areaName = (areaVal) => {
  const map = { 1: 'Tân Mai', 2: 'Trảng Dài', 3: 'Long Bình', 4: 'Tân Hiệp', 5: 'Hố Nai' }
  return map[areaVal] || 'Khác'
}
</script>

<style scoped>
.courts-view-container {
  padding-bottom: 60px;
}

.header-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: var(--spacing-lg);
  flex-wrap: wrap;
  gap: 16px;
}

.header-title {
  display: flex;
  align-items: center;
  gap: 10px;
}

.header-title h2 {
  font-size: 26px;
  font-weight: 800;
  color: var(--text-primary);
  text-shadow: 0 0 15px rgba(163, 230, 53, 0.2);
}

.fire-emoji {
  font-size: 28px;
  filter: drop-shadow(0 0 8px rgba(163, 230, 53, 0.5));
}

.filter-controls {
  display: flex;
  gap: 12px;
  flex-wrap: wrap;
}

.search-box {
  position: relative;
  width: 280px;
}

.search-icon {
  position: absolute;
  left: 14px;
  top: 50%;
  transform: translateY(-50%);
  color: var(--text-secondary);
}

.filter-input {
  padding-left: 40px !important;
}

.filter-select {
  width: 200px;
}

/* TWO-COLUMN LAYOUT */
.courts-layout-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 30px;
  align-items: start;
}

.courts-list-column {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.court-item-card {
  margin-bottom: 0;
  padding: 0; /* Changed to 0 for image spacing */
  overflow: hidden;
  cursor: pointer;
  border-color: rgba(255, 255, 255, 0.05);
  display: flex;
  flex-direction: column;
  transition: all 0.3s ease;
}

.court-item-card:hover, .active-selection {
  border-color: var(--primary-color);
  box-shadow: var(--shadow-neon);
}

.active-selection {
  background: linear-gradient(135deg, rgba(163, 230, 53, 0.05) 0%, rgba(15, 23, 42, 0.85) 100%);
}

/* NEW: COURT CARD BANNER (IMAGE) */
.court-card-banner {
  width: 100%;
  height: 180px;
  position: relative;
  overflow: hidden;
}

.court-img-pic {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.5s ease;
}

.court-item-card:hover .court-img-pic {
  transform: scale(1.05);
}

.court-featured-banner {
  position: absolute;
  top: 12px;
  left: 12px;
  background: linear-gradient(135deg, #f43f5e 0%, #be123c 100%);
  color: white;
  padding: 4px 10px;
  border-radius: 6px;
  font-size: 11px;
  font-weight: 800;
  letter-spacing: 0.05em;
  box-shadow: 0 4px 10px rgba(244, 63, 94, 0.4);
}

.rating-badge-banner {
  position: absolute;
  top: 12px;
  right: 12px;
  background: rgba(15, 23, 42, 0.85);
  color: #f59e0b;
  padding: 4px 10px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: 800;
  backdrop-filter: blur(4px);
  border: 1px solid rgba(245, 158, 11, 0.2);
}

.court-card-body {
  padding: var(--spacing-lg);
  flex: 1;
  display: flex;
  flex-direction: column;
}

.court-badge-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.badge-area {
  background: rgba(163, 230, 53, 0.1);
  color: var(--primary-color);
  font-size: 11px;
  border: 1px solid rgba(163, 230, 53, 0.15);
}

.court-name-title {
  font-size: 22px;
  font-weight: 800;
  color: var(--text-primary);
  margin-bottom: 6px;
}

.court-addr {
  font-size: 14px;
  margin-bottom: 14px;
  display: flex;
  align-items: center;
  gap: 6px;
}

.addr-icon {
  color: var(--primary-color);
  flex-shrink: 0;
}

.court-amenities {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  margin-bottom: 18px;
}

.amenity-tag {
  background: rgba(255, 255, 255, 0.03);
  color: var(--text-secondary);
  border: 1px solid rgba(255, 255, 255, 0.05);
  padding: 3px 8px;
  border-radius: 6px;
  font-size: 11px;
  font-weight: 600;
}

.court-card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
  padding-top: 14px;
  margin-top: auto;
}

.court-price .price-val {
  font-size: 20px;
  font-weight: 800;
  color: var(--primary-color);
  text-shadow: 0 0 10px rgba(163, 230, 53, 0.3);
}

.court-price .price-unit {
  font-size: 12px;
  color: var(--text-secondary);
}

.card-btn-group {
  display: flex;
  gap: 8px;
}

.map-btn {
  background: rgba(255, 255, 255, 0.02);
}

/* STICKY MAP COLUMN */
.courts-map-column {
  position: sticky;
  top: 90px;
}

.map-sticky-card {
  padding: 20px;
  height: calc(100vh - 140px);
  min-height: 500px;
  display: flex;
  flex-direction: column;
}

.map-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  margin-bottom: 16px;
  gap: 16px;
}

.map-title-info h4 {
  font-size: 16px;
  font-weight: 800;
  color: var(--text-primary);
}

.map-title-info p {
  margin-top: 4px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 250px;
}

.external-map-link {
  font-size: 12px;
  padding: 6px 12px;
}

.map-iframe-container {
  flex: 1;
  width: 100%;
  border-radius: 12px;
  overflow: hidden;
  border: 1px solid rgba(255, 255, 255, 0.08);
  position: relative;
}

.google-map-iframe {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  border: 0;
  filter: invert(90%) hue-rotate(180deg) brightness(95%) contrast(90%); /* Cool Dark Mode Map filter */
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

.empty-icon {
  font-size: 50px;
  filter: drop-shadow(0 0 10px rgba(163, 230, 53, 0.5));
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.animate-fade {
  animation: fadeIn 0.4s ease forwards;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

/* RESPONSIVE BREAKPOINTS */
@media (max-width: 991px) {
  .courts-layout-grid {
    grid-template-columns: 1fr;
  }
  
  .map-sticky-card {
    height: 400px;
    min-height: 400px;
  }
  
  .header-section {
    flex-direction: column;
    align-items: flex-start;
  }
  
  .filter-controls {
    width: 100%;
  }
  
  .search-box, .filter-select {
    width: 100%;
  }
}
</style>
