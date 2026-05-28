<template>
  <div class="create-match-view container">
    <div class="page-header">
      <router-link to="/" class="back-link">← Quay lại</router-link>
      <h2>Tạo Kèo Mới</h2>
    </div>

    <div class="card">
      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label>Sân cầu lông</label>
          <select v-model="form.courtId" required>
            <option value="" disabled>-- Chọn sân --</option>
            <option v-for="court in courtStore.courts" :key="court.id" :value="court.id">
              {{ court.name }} - {{ formatArea(court.area) }} ({{ formatCurrency(court.price) }}/h)
            </option>
          </select>
        </div>

        <div class="form-row">
          <div class="form-group flex-1">
            <label>Ngày đánh</label>
            <input type="date" v-model="form.date" :min="today" required />
          </div>
          <div class="form-group flex-1">
            <label>Giờ bắt đầu</label>
            <input type="time" v-model="form.timeStart" required />
          </div>
          <div class="form-group flex-1">
            <label>Giờ kết thúc</label>
            <input type="time" v-model="form.timeEnd" required />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group flex-1">
            <label>Tổng số slot cần tuyển</label>
            <input type="number" v-model.number="form.slotsTotal" min="1" max="10" required />
          </div>
          <div class="form-group flex-1">
            <label>Chi phí dự kiến (VNĐ)</label>
            <input type="number" v-model.number="form.cost" min="0" step="1000" required />
          </div>
        </div>

        <div class="form-group">
          <label>Trình độ yêu cầu</label>
          <select v-model.number="form.level" required>
            <option value="1">Yếu (Mới chơi)</option>
            <option value="2">Trung bình (Đánh rally 10+)</option>
            <option value="3">Khá (Biết smash)</option>
            <option value="4">Giỏi (Thi đấu phong trào)</option>
          </select>
        </div>

        <div class="form-group">
          <label>Số Zalo để liên hệ</label>
          <input type="tel" v-model="form.zalo" required placeholder="Ví dụ: 0912345678" />
        </div>

        <div class="form-group">
          <label>Ghi chú thêm (Tùy chọn)</label>
          <textarea v-model="form.note" rows="3" placeholder="Ví dụ: Cần 2 nam 1 nữ đánh cố định..."></textarea>
        </div>

        <div v-if="matchStore.error" class="error-msg">
          {{ matchStore.error }}
        </div>

        <div v-if="!authStore.isAdmin && authStore.availablePosts <= 0" class="error-msg alert-warning" style="margin-top: 10px;">
          <p>Bạn đã hết lượt đăng kèo. Vui lòng nạp thêm để tiếp tục sử dụng dịch vụ.</p>
          <router-link to="/topup" class="btn btn-outline w-100" style="margin-top: 10px;">Nạp Lượt Đăng</router-link>
        </div>

        <button type="submit" class="btn btn-primary w-100" :disabled="matchStore.loading || (!authStore.isAdmin && authStore.availablePosts <= 0)" style="margin-top: 16px;">
          {{ matchStore.loading ? 'Đang tạo...' : 'Xác nhận Đăng Kèo' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { reactive, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useCourtStore } from '@/stores/courts'
import { useMatchStore } from '@/stores/matches'
import { useAuthStore } from '@/stores/auth'
import { useToast } from 'vue-toastification'

const router = useRouter()
const courtStore = useCourtStore()
const matchStore = useMatchStore()
const authStore = useAuthStore()
const toast = useToast()

const today = computed(() => {
  const d = new Date()
  return d.toISOString().split('T')[0]
})

const form = reactive({
  courtId: '',
  date: today.value,
  timeStart: '18:00',
  timeEnd: '20:00',
  slotsTotal: 4,
  level: 2,
  cost: 0,
  zalo: '',
  note: ''
})

onMounted(() => {
  if (courtStore.courts.length === 0) {
    courtStore.fetchCourts()
  }
})

const handleSubmit = async () => {
  if (form.timeStart >= form.timeEnd) {
    toast.error('Giờ bắt đầu phải nhỏ hơn giờ kết thúc!')
    return
  }

  // format times to "HH:mm:ss"
  const payload = {
    ...form,
    timeStart: form.timeStart.length === 5 ? `${form.timeStart}:00` : form.timeStart,
    timeEnd: form.timeEnd.length === 5 ? `${form.timeEnd}:00` : form.timeEnd
  }

  const success = await matchStore.createMatch(payload)
  if (success) {
    toast.success('Tạo kèo thành công!')
    await authStore.fetchMe() // Cập nhật lại số lượt đăng
    router.push('/my-matches')
  } else {
    toast.error(matchStore.error || 'Tạo kèo thất bại')
  }
}

const formatArea = (areaVal) => {
  const areas = { 1: 'Tân Mai', 2: 'Trảng Dài', 3: 'Long Bình', 4: 'Tân Hiệp', 5: 'Hố Nai' }
  return areas[areaVal] || ''
}

const formatCurrency = (val) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)
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
.form-group {
  margin-bottom: 16px;
}
.form-group label {
  display: block;
  margin-bottom: 6px;
  font-weight: 500;
  font-size: 14px;
}
.form-row {
  display: flex;
  gap: 12px;
}
.flex-1 {
  flex: 1;
}
.w-100 { width: 100%; }
.error-msg {
  color: var(--danger-color);
  font-size: 13px;
  margin-bottom: 16px;
  text-align: center;
}
.alert-warning {
  background: #fffbeb;
  border: 1px solid #fcd34d;
  color: #b45309;
  padding: 12px;
  border-radius: 8px;
}
@media (max-width: 480px) {
  .form-row {
    flex-direction: column;
    gap: 0;
  }
}
</style>
