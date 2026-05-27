<template>
  <div class="topup-view container">
    <div class="page-header">
      <h2>Mua Lượt Đăng Kèo</h2>
      <p>Bạn hiện có: <strong>{{ authStore.credits }} lượt</strong></p>
    </div>

    <div class="card pricing-cards" v-if="!selectedPackage">
      <div class="pricing-card" v-for="pkg in packages" :key="pkg.credits" @click="selectPackage(pkg)">
        <h3>{{ pkg.credits }} Lượt</h3>
        <p class="price">{{ formatCurrency(pkg.price) }}</p>
        <button class="btn btn-outline w-100">Chọn gói này</button>
      </div>
    </div>

    <div class="card payment-section" v-else>
      <button class="btn btn-outline mb-3" @click="selectedPackage = null">← Chọn lại gói</button>
      
      <div class="payment-details">
        <h3>Thanh toán qua mã QR</h3>
        <p>Gói đã chọn: <strong>{{ selectedPackage.credits }} lượt</strong></p>
        <p>Số tiền cần chuyển: <strong class="price">{{ formatCurrency(selectedPackage.price) }}</strong></p>
        <p>Nội dung chuyển khoản: <strong class="text-primary">{{ transactionCode }}</strong></p>
        
        <div class="qr-container mt-4 mb-4">
          <img :src="qrCodeUrl" alt="Mã VietQR" class="qr-image" v-if="qrCodeUrl" />
          <div class="loader" v-else>Đang tạo mã QR...</div>
        </div>

        <p class="text-muted small">Sau khi chuyển khoản thành công, vui lòng nhấn nút bên dưới. Admin sẽ duyệt và cộng lượt cho bạn trong vòng 5-10 phút.</p>
        
        <button class="btn btn-primary w-100 mt-3" @click="confirmPayment" :disabled="submitting">
          {{ submitting ? 'Đang gửi...' : 'Tôi đã chuyển khoản thành công' }}
        </button>
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
  { credits: 1, price: 10000 },
  { credits: 5, price: 40000 },
  { credits: 10, price: 70000 }
]

const selectedPackage = ref(null)
const submitting = ref(false)

const transactionCode = computed(() => {
  return `CAULONG ${authStore.user?.username?.toUpperCase() || ''}`
})

// Demo using VietQR API with a dummy account (MBBank - 0123456789)
const qrCodeUrl = computed(() => {
  if (!selectedPackage.value) return ''
  const bank = 'MB'
  const account = '0123456789'
  const amount = selectedPackage.value.price
  const info = encodeURIComponent(transactionCode.value)
  return `https://img.vietqr.io/image/${bank}-${account}-compact.png?amount=${amount}&addInfo=${info}&accountName=QUAN%20LY%20SAN`
})

const selectPackage = (pkg) => {
  selectedPackage.value = pkg
}

const formatCurrency = (val) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
}

const confirmPayment = async () => {
  if (!selectedPackage.value) return
  submitting.value = true
  
  try {
    await api.post('/api/Transactions', {
      amount: selectedPackage.value.price,
      creditsAdded: selectedPackage.value.credits
    })
    toast.success('Đã gửi yêu cầu nạp tiền! Vui lòng chờ Admin duyệt.')
    router.push('/')
  } catch (err) {
    toast.error('Có lỗi xảy ra khi gửi yêu cầu')
    console.error(err)
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.page-header {
  text-align: center;
  margin-bottom: 2rem;
  margin-top: 1rem;
}
.pricing-cards {
  display: flex;
  gap: 20px;
  justify-content: center;
  background: transparent;
  box-shadow: none;
  padding: 0;
}
.pricing-card {
  background: var(--bg-card);
  border-radius: var(--radius-md);
  padding: 24px;
  flex: 1;
  max-width: 300px;
  text-align: center;
  border: 1px solid var(--border-color);
  cursor: pointer;
  transition: all 0.2s ease;
}
.pricing-card:hover {
  transform: translateY(-5px);
  border-color: var(--primary-color);
  box-shadow: 0 10px 20px rgba(0,0,0,0.1);
}
.pricing-card h3 {
  font-size: 24px;
  margin-bottom: 12px;
}
.pricing-card .price {
  font-size: 28px;
  font-weight: 700;
  color: var(--primary-color);
  margin-bottom: 20px;
}
.payment-section {
  max-width: 500px;
  margin: 0 auto;
}
.payment-details {
  text-align: center;
}
.payment-details h3 {
  margin-bottom: 20px;
}
.qr-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 250px;
  background: #f8f9fa;
  border-radius: 8px;
  padding: 16px;
}
.qr-image {
  max-width: 100%;
  height: auto;
  border-radius: 8px;
}
@media (max-width: 768px) {
  .pricing-cards {
    flex-direction: column;
    align-items: center;
  }
  .pricing-card {
    width: 100%;
  }
}
</style>
