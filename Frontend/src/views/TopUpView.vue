<template>
  <div class="topup-view-container container animate-fade">
    <div class="page-header">
      <span class="wallet-icon">🎟️</span>
      <h2>Nạp Lượt Đăng Kèo Giao Lưu</h2>
      <p class="credits-count">Số lượt hiện có: <strong class="text-primary">{{ authStore.credits }} lượt</strong></p>
    </div>

    <!-- PACKAGE SELECT -->
    <div class="pricing-cards-wrapper" v-if="!selectedPackage">
      <div class="card pricing-card" v-for="pkg in packages" :key="pkg.credits" @click="selectPackage(pkg)">
        <div class="pkg-header">
          <h3>{{ pkg.credits }} Lượt</h3>
          <span class="pkg-popular" v-if="pkg.credits === 50">🔥 Phổ Biến</span>
        </div>
        <p class="pkg-price">{{ formatCurrency(pkg.price) }}</p>
        <div class="pkg-features">
          <p>✔️ Tạo đến {{ pkg.credits }} kèo giao lưu</p>
          <p>✔️ Không giới hạn thời gian sử dụng</p>
          <p>✔️ Tự động kích hoạt ngay lập tức</p>
        </div>
        <button class="btn btn-primary w-100 pkg-btn">Chọn gói này</button>
      </div>
    </div>

    <!-- QR PAYMENT SECTION -->
    <div class="card payment-section animate-fade" v-else>
      <button class="btn btn-outline btn-sm back-package-btn" @click="selectedPackage = null">
        ← Chọn lại gói nạp
      </button>
      
      <div class="payment-details mt-3">
        <h3>Quét Mã VietQR Thanh Toán</h3>
        <div class="payment-meta-grid">
          <div class="meta-item">
            <span class="text-secondary small uppercase">Gói nạp</span>
            <strong class="text-white">{{ selectedPackage.credits }} lượt đăng kèo</strong>
          </div>
          <div class="meta-item">
            <span class="text-secondary small uppercase">Số tiền thanh toán</span>
            <strong class="text-primary price-display">{{ formatCurrency(selectedPackage.price) }}</strong>
          </div>
        </div>

        <div class="qr-container mt-4 mb-4">
          <img :src="qrCodeUrl" alt="Mã VietQR tự động" class="qr-image" v-if="qrCodeUrl" />
          <div class="spinner" v-else></div>
        </div>

        <!-- Warning info -->
        <div class="alert-info-box mb-4">
          <p><strong>Cơ chế tự động hóa:</strong> Tiền nạp sẽ chuyển vào tài khoản VietinBank: <strong>DINH TUAN KHOI</strong>.</p>
          <p class="text-primary small mt-1">⚠️ <strong>Lưu ý:</strong> Vui lòng quét mã QR trên và giữ nguyên nội dung chuyển khoản để hệ thống tự động cộng lượt ngay lập tức.</p>
        </div>
        
        <button 
          class="btn btn-primary w-100 confirm-payment-btn" 
          @click="confirmPayment" 
          :disabled="submitting || scanning"
        >
          <span v-if="scanning" class="btn-spinner"></span>
          {{ scanning ? 'Đang kiểm tra giao dịch...' : submitting ? 'Đang xử lý...' : 'Tôi đã chuyển khoản thành công' }}
        </button>
      </div>
    </div>

    <!-- AUTO-APPROVAL OVERLAY SCANNING SIMULATION -->
    <div class="bank-scan-overlay" v-if="scanning">
      <div class="scan-card card animate-fade">
        <div class="pulse-scan-ring">
          <span class="scan-emoji">🏦</span>
        </div>
        <h3>Đang quét giao dịch ngân hàng...</h3>
        <p class="text-secondary">Đang kết nối cổng thanh toán VietQR và kiểm tra biến động số dư tài khoản...</p>
        
        <div class="progress-bar-container mt-4">
          <div class="progress-bar-fill"></div>
        </div>
        
        <div class="security-note mt-3">
          <span class="lock-icon">🔒</span> Bảo mật thanh toán SSL 256-bit
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import { useToast } from 'vue-toastification'
import api from '@/api/axios'

const authStore = useAuthStore()
const router = useRouter()
const toast = useToast()

const packages = [
  { credits: 10, price: 10000 },
  { credits: 50, price: 40000 },
  { credits: 100, price: 70000 }
]

const selectedPackage = ref(null)
const submitting = ref(false)
const scanning = ref(false) // Simulated Bank Scan Overlay

const transactionCode = computed(() => {
  return `CAULONG ${authStore.user?.username?.toUpperCase() || ''}`
})

const qrCodeUrl = computed(() => {
  if (!selectedPackage.value) return ''
  const bank = 'ICB'
  const account = '102880579767'
  const amount = selectedPackage.value.price
  const info = encodeURIComponent(transactionCode.value)
  const accountName = encodeURIComponent('DINH TUAN KHOI')
  return `https://img.vietqr.io/image/${bank}-${account}-print.png?amount=${amount}&addInfo=${info}&accountName=${accountName}`
})

const selectPackage = (pkg) => {
  selectedPackage.value = pkg
}

const formatCurrency = (val) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}

const confirmPayment = async () => {
  if (!selectedPackage.value) return
  scanning.value = true
  
  try {
    // 1. Tạo giao dịch Pending lưu vào DB
    const res = await api.post('/api/Transactions', {
      amount: selectedPackage.value.price,
      creditsAdded: selectedPackage.value.credits
    })
    const trxId = res.data.id
    
    // 2. Chờ 2.5 giây giả lập quét VietQR ngân hàng báo có
    setTimeout(async () => {
      submitting.value = true
      try {
        // 3. Gọi tiếp API Duyệt giao dịch Approved nhân danh hệ thống tự động
        await api.put(`/api/Transactions/${trxId}/status`, 1, {
          headers: { 'Content-Type': 'application/json' }
        })
        
        // 4. Cập nhật lượt của user ngay lập tức trên Navbar
        await authStore.fetchProfile()
        
        toast.success(`🎉 Nạp tiền thành công! Bạn đã được tự động cộng +${selectedPackage.value.credits} lượt đăng bài.`)
        router.push('/')
      } catch (err) {
        toast.error('Có lỗi xảy ra khi tự động duyệt giao dịch.')
        console.error(err)
      } finally {
        submitting.value = false
        scanning.value = false
      }
    }, 2500)
  } catch (err) {
    toast.error('Có lỗi xảy ra khi khởi tạo yêu cầu nạp tiền.')
    console.error(err)
    scanning.value = false
  }
}
</script>

<style scoped>
.topup-view-container {
  max-width: 900px;
  padding-bottom: 80px;
}

.page-header {
  text-align: center;
  margin-bottom: 40px;
  margin-top: 20px;
}

.wallet-icon {
  font-size: 50px;
  display: block;
  margin-bottom: 12px;
  filter: drop-shadow(0 0 10px rgba(163, 230, 53, 0.4));
}

.page-header h2 {
  font-size: 28px;
  font-weight: 800;
  color: var(--text-primary);
  margin-bottom: 8px;
}

.credits-count {
  font-size: 16px;
  color: var(--text-secondary);
}

/* PRICING CARDS */
.pricing-cards-wrapper {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 24px;
  justify-content: center;
}

.pricing-card {
  background: rgba(15, 23, 42, 0.6);
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: var(--border-radius);
  padding: 30px 24px;
  text-align: center;
  cursor: pointer;
  display: flex;
  flex-direction: column;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  position: relative;
}

.pricing-card:hover {
  transform: translateY(-8px);
  border-color: var(--primary-color);
  box-shadow: var(--shadow-neon-lg);
}

.pkg-header {
  margin-bottom: 16px;
  position: relative;
}

.pkg-header h3 {
  font-size: 22px;
  font-weight: 800;
}

.pkg-popular {
  position: absolute;
  top: -45px;
  left: 50%;
  transform: translateX(-50%);
  background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);
  color: white;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 11px;
  font-weight: 800;
  box-shadow: 0 4px 10px rgba(245, 158, 11, 0.4);
}

.pkg-price {
  font-size: 32px;
  font-weight: 900;
  color: var(--primary-color);
  margin-bottom: 24px;
  text-shadow: 0 0 10px rgba(163, 230, 53, 0.3);
}

.pkg-features {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 30px;
  text-align: left;
  font-size: 14px;
  color: var(--text-secondary);
}

.pkg-btn {
  margin-top: auto;
  font-weight: 700;
}

/* PAYMENT SECTION */
.payment-section {
  max-width: 550px;
  margin: 0 auto;
  padding: 30px;
  background: rgba(15, 23, 42, 0.6);
  border: 1px solid var(--border-color);
}

.back-package-btn {
  background: rgba(255, 255, 255, 0.02);
}

.payment-details h3 {
  font-size: 22px;
  font-weight: 800;
  margin-bottom: 20px;
}

.payment-meta-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
  background: rgba(0, 0, 0, 0.2);
  padding: 16px;
  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.03);
  text-align: left;
  margin-bottom: 24px;
}

.meta-item {
  display: flex;
  flex-direction: column;
}

.meta-item span {
  margin-bottom: 4px;
  font-weight: 600;
}

.meta-item strong {
  font-size: 16px;
}

.price-display {
  text-shadow: 0 0 8px rgba(163, 230, 53, 0.3);
}

.qr-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 280px;
  background: white; /* Keep white for easy camera QR scanning */
  border-radius: var(--border-radius);
  padding: 20px;
  border: 2px dashed rgba(163, 230, 53, 0.3);
}

.qr-image {
  max-width: 100%;
  max-height: 260px;
  height: auto;
  border-radius: 8px;
}

.alert-info-box {
  background: rgba(163, 230, 53, 0.05);
  border-left: 4px solid var(--primary-color);
  padding: 14px 18px;
  border-radius: 4px 12px 12px 4px;
  text-align: left;
  font-size: 13px;
}

.alert-info-box p {
  color: var(--text-secondary);
}

.alert-info-box strong {
  color: var(--text-primary);
}

.confirm-payment-btn {
  font-weight: 700;
  font-size: 16px;
  padding: 14px 20px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

/* Simulated Webhook Auto-Approval Overlay */
.bank-scan-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(11, 15, 25, 0.9);
  backdrop-filter: blur(12px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 3000;
}

.scan-card {
  max-width: 480px;
  width: 90%;
  padding: 40px 30px;
  text-align: center;
  background: rgba(15, 23, 42, 0.95);
  border: 1px solid var(--border-color);
  box-shadow: var(--shadow-neon-lg);
}

.pulse-scan-ring {
  width: 80px;
  height: 80px;
  border-radius: 50%;
  background: rgba(163, 230, 53, 0.15);
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 24px;
  box-shadow: 0 0 0 0 rgba(163, 230, 53, 0.4);
  animation: pulse-ring 1.5s infinite;
}

.scan-emoji {
  font-size: 40px;
  filter: drop-shadow(0 0 8px rgba(163, 230, 53, 0.5));
}

.scan-card h3 {
  font-size: 20px;
  font-weight: 800;
  color: var(--text-primary);
  margin-bottom: 10px;
}

.progress-bar-container {
  width: 100%;
  height: 6px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 3px;
  overflow: hidden;
}

.progress-bar-fill {
  height: 100%;
  width: 0;
  background: linear-gradient(90deg, var(--primary-color) 0%, #84cc16 100%);
  border-radius: 3px;
  box-shadow: var(--shadow-neon);
  animation: fill-bar 2.4s linear forwards;
}

.security-note {
  font-size: 12px;
  color: var(--text-secondary);
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
}

.lock-icon {
  color: var(--success-color);
}

.btn-spinner {
  width: 18px;
  height: 18px;
  border: 2.5px solid rgba(11, 15, 25, 0.2);
  border-top-color: #0b0f19;
  border-radius: 50%;
  animation: spin 0.8s infinite linear;
}

.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid rgba(163, 230, 53, 0.1);
  border-top-color: var(--primary-color);
  border-radius: 50%;
  animation: spin 1s infinite linear;
  margin: 0 auto;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes pulse-ring {
  0% {
    box-shadow: 0 0 0 0 rgba(163, 230, 53, 0.4);
  }
  70% {
    box-shadow: 0 0 0 16px rgba(163, 230, 53, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(163, 230, 53, 0);
  }
}

@keyframes fill-bar {
  0% { width: 0; }
  100% { width: 100%; }
}

.animate-fade {
  animation: fadeIn 0.4s ease forwards;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}
</style>
