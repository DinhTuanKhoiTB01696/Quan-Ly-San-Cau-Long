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
          <p>✔️ Thông báo email trực tiếp tới Admin duyệt ngay</p>
        </div>
        <button class="btn btn-primary w-100 pkg-btn">Chọn gói này</button>
      </div>
    </div>

    <!-- QR PAYMENT SECTION -->
    <div class="card payment-section animate-fade" v-else>
      <button class="btn btn-outline btn-sm back-package-btn" @click="cancelPayment">
        ← Chọn lại gói nạp
      </button>
      
      <div class="payment-details mt-3 text-center">
        <p class="warning-text">Lưu ý: <strong>GIỮ NGUYÊN</strong> nội dung chuyển khoản.</p>
        
        <div class="timer-countdown mb-3">
          <span class="timer-icon">⏳</span>
          <span class="timer-val">{{ formattedTime }}</span>
        </div>

        <div class="qr-wrapper mb-4">
          <div class="qr-container">
            <img :src="qrCodeUrl" alt="Mã VietQR tự động" class="qr-image" v-if="qrCodeUrl" />
            <div class="spinner" v-else></div>
          </div>
        </div>

        <!-- Banking Details Box -->
        <div class="banking-info-box mb-4">
          <p class="bank-name">MB BANK — 7911235813</p>
          <p class="account-name">HO KINH DOANH LUYEN NOI</p>
          <p class="amount-val">Số tiền: <strong>{{ formatCurrency(selectedPackage.price) }}</strong></p>
          <p class="transfer-code">
            Nội dung CK: <strong>{{ transactionCode }}</strong>
            <button class="btn-copy" @click="copyToClipboard(transactionCode)" title="Sao chép nội dung">📋</button>
          </p>
        </div>

        <div class="owner-email mb-3">
          <span>khoidttb01696@gmail.com</span>
        </div>

        <div class="help-link mb-4">
          <router-link :to="{ name: 'feedback', query: { type: 'Nạp tiền', code: transactionCode } }" class="btn-help">
            Chuyển rồi mà chưa được duyệt → nhắn gửi ảnh chuyển khoản và email
          </router-link>
        </div>
        
        <button 
          class="btn btn-primary w-100 confirm-payment-btn" 
          @click="confirmPayment" 
          :disabled="submitting || scanning"
        >
          <span v-if="scanning" class="btn-spinner"></span>
          {{ scanning ? 'Đang gửi yêu cầu nạp tiền...' : submitting ? 'Đang xử lý...' : 'Tôi đã chuyển khoản thành công' }}
        </button>
      </div>
    </div>

    <!-- AUTO-APPROVAL OVERLAY SCANNING SIMULATION -->
    <div class="bank-scan-overlay" v-if="scanning">
      <div class="scan-card card animate-fade">
        <div class="pulse-scan-ring">
          <span class="scan-emoji">🏦</span>
        </div>
        <h3>Đang gửi yêu cầu nạp lượt...</h3>
        <p class="text-secondary">Hệ thống đang kết nối và gửi thông báo trực tiếp tới Email Admin để kiểm tra và phê duyệt...</p>
        
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
import { ref, computed, onUnmounted } from 'vue'
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
const transactionCode = ref('')
const countdown = ref(1800) // 30 phút (1800s)
let timerInterval = null

const formattedTime = computed(() => {
  const mins = Math.floor(countdown.value / 60)
  const secs = countdown.value % 60
  return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`
})

const qrCodeUrl = computed(() => {
  if (!selectedPackage.value) return ''
  const bank = 'MB'
  const account = '7911235813'
  const amount = selectedPackage.value.price
  const info = encodeURIComponent(transactionCode.value)
  const accountName = encodeURIComponent('HO KINH DOANH LUYEN NOI')
  return `https://img.vietqr.io/image/${bank}-${account}-print.png?amount=${amount}&addInfo=${info}&accountName=${accountName}`
})

const selectPackage = (pkg) => {
  selectedPackage.value = pkg
  
  // Tạo mã giao dịch chuyên nghiệp (LN + yymmddhhMM + 4 kí tự ngẫu nhiên)
  const randStr = Math.random().toString(36).substring(2, 6).toUpperCase();
  const dateObj = new Date();
  const timeStr = `${dateObj.getFullYear().toString().slice(2)}${(dateObj.getMonth() + 1).toString().padStart(2, '0')}${dateObj.getDate().toString().padStart(2, '0')}${dateObj.getHours().toString().padStart(2, '0')}${dateObj.getMinutes().toString().padStart(2, '0')}`;
  transactionCode.value = `LN${timeStr}${randStr}`;

  // Bắt đầu đếm ngược 30 phút
  countdown.value = 1800;
  if (timerInterval) clearInterval(timerInterval);
  timerInterval = setInterval(() => {
    if (countdown.value > 0) {
      countdown.value--;
    } else {
      clearInterval(timerInterval);
    }
  }, 1000);
}

const cancelPayment = () => {
  selectedPackage.value = null
  if (timerInterval) {
    clearInterval(timerInterval)
    timerInterval = null
  }
}

const formatCurrency = (val) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}

const copyToClipboard = (text) => {
  navigator.clipboard.writeText(text)
  toast.success('📋 Đã sao chép nội dung chuyển khoản!')
}

const confirmPayment = async () => {
  if (!selectedPackage.value) return
  scanning.value = true
  
  try {
    // 1. Tạo giao dịch Pending lưu vào DB và kích hoạt gửi Email ở Backend
    await api.post('/Transactions', {
      amount: selectedPackage.value.price,
      creditsAdded: selectedPackage.value.credits
    })
    
    // 2. Chờ giả lập 2.4 giây để hiện thanh loading chuyên nghiệp
    setTimeout(() => {
      scanning.value = false
      toast.success('🎉 Yêu cầu nạp tiền đã được gửi! Hệ thống vừa gửi Email thông báo tự động cho Admin. Admin sẽ kiểm tra tài khoản và phê duyệt số lượt cho bạn ngay lập tức!')
      router.push('/')
    }, 2400)
  } catch (err) {
    toast.error('Có lỗi xảy ra khi gửi yêu cầu nạp tiền.')
    console.error(err)
    scanning.value = false
  }
}

onUnmounted(() => {
  if (timerInterval) clearInterval(timerInterval)
})
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
  max-width: 500px;
  margin: 0 auto;
  padding: 30px 24px;
  background: rgba(15, 23, 42, 0.7);
  border: 1px solid var(--border-color);
  border-radius: 16px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
}

.back-package-btn {
  background: rgba(255, 255, 255, 0.02);
}

.warning-text {
  font-size: 14px;
  color: var(--text-secondary);
  margin-bottom: 12px;
}

.warning-text strong {
  color: var(--primary-color);
}

.timer-countdown {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  font-size: 24px;
  font-weight: 700;
  color: #ef4444;
  text-shadow: 0 0 8px rgba(239, 68, 68, 0.3);
}

.qr-wrapper {
  background: white;
  padding: 16px;
  border-radius: 16px;
  display: inline-block;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
}

.qr-container {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 250px;
  height: 250px;
  margin: 0 auto;
}

.qr-image {
  max-width: 100%;
  max-height: 100%;
  object-fit: contain;
}

/* Banking info box */
.banking-info-box {
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: 12px;
  padding: 18px;
  text-align: center;
}

.bank-name {
  font-size: 16px;
  font-weight: 700;
  color: var(--text-primary);
  margin-bottom: 4px;
}

.account-name {
  font-size: 14px;
  color: var(--text-secondary);
  margin-bottom: 8px;
  letter-spacing: 0.5px;
}

.amount-val {
  font-size: 18px;
  font-weight: 600;
  color: var(--text-primary);
  margin-bottom: 6px;
}

.amount-val strong {
  color: var(--primary-color);
}

.transfer-code {
  font-size: 14px;
  color: var(--text-secondary);
}

.transfer-code strong {
  font-size: 16px;
  color: var(--primary-color);
  font-family: monospace;
  background: rgba(163, 230, 53, 0.1);
  padding: 2px 8px;
  border-radius: 4px;
  border: 1px solid rgba(163, 230, 53, 0.2);
}

.btn-copy {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 16px;
  margin-left: 6px;
  padding: 0;
  transition: transform 0.2s;
}

.btn-copy:hover {
  transform: scale(1.2);
}

.owner-email {
  font-size: 13px;
  color: rgba(255, 255, 255, 0.4);
  font-style: italic;
}

.help-link .btn-help {
  font-size: 13px;
  color: var(--text-secondary);
  text-decoration: underline;
  cursor: pointer;
  transition: color 0.2s;
}

.help-link .btn-help:hover {
  color: var(--primary-color);
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
