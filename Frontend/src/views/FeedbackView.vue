<template>
  <div class="feedback-view container animate-fade">
    <div class="page-header text-center">
      <h2 class="neon-text">Góp Ý Xây Dựng</h2>
      <p class="text-secondary mt-2">Chúng tôi luôn lắng nghe để cải thiện trải nghiệm của bạn</p>
    </div>
 
    <div class="card feedback-card mx-auto">
      <div v-if="submitted" class="success-message text-center animate-scale">
        <div class="success-icon">🎉</div>
        <h3 class="text-success-neon">Cảm ơn bạn đã góp ý!</h3>
        <p class="text-secondary mb-4">Những đóng góp quý giá của bạn sẽ giúp cộng đồng cầu lông Biên Hòa ngày càng phát triển hơn.</p>
        <button class="btn btn-outline" @click="resetForm">Gửi thêm góp ý</button>
      </div>
 
      <form v-else @submit.prevent="submitFeedback" class="feedback-form">
        <!-- Radio Option Group: Helpful -->
        <div class="form-group mb-4">
          <label class="form-label text-white">Bạn có thấy ứng dụng này hữu ích không?</label>
          <div class="helpful-options">
            <label class="option-pill" :class="{ active: form.isHelpful === true }">
              <input type="radio" v-model="form.isHelpful" :value="true" name="isHelpful" class="hidden-radio" /> 
              <span class="pill-text">Có, rất hữu ích 👍</span>
            </label>
            <label class="option-pill" :class="{ active: form.isHelpful === false }">
              <input type="radio" v-model="form.isHelpful" :value="false" name="isHelpful" class="hidden-radio" /> 
              <span class="pill-text">Chưa tốt lắm 👎</span>
            </label>
          </div>
        </div>
 
        <!-- Input Textarea: Missing features -->
        <div class="form-group mb-4">
          <label class="form-label text-white">Tính năng nào bạn muốn ứng dụng cập nhật thêm?</label>
          <textarea 
            v-model="form.missingFeature" 
            class="form-control feedback-input" 
            rows="4" 
            placeholder="Ví dụ: Tính năng chat nhóm, xếp hạng người chơi, bản đồ định vị..."
          ></textarea>
        </div>
 
        <!-- Input Text: Wanted court -->
        <div class="form-group mb-4">
          <label class="form-label text-white">Sân cầu lông nào ở Biên Hòa bạn muốn ứng dụng thêm vô?</label>
          <input 
            type="text" 
            v-model="form.wantedCourt" 
            class="form-control feedback-input" 
            placeholder="Nhập tên sân (Ví dụ: Sân A7, Sân Bửu Long...)"
          />
        </div>
 
        <button type="submit" class="btn btn-primary w-100 submit-btn" :disabled="loading">
          {{ loading ? 'Đang gửi...' : 'Gửi Ý Kiến Ngay' }}
        </button>
      </form>
    </div>
  </div>
</template>
 
<script setup>
import { ref } from 'vue'
import { useToast } from 'vue-toastification'
import api from '@/api/axios'
 
const toast = useToast()
const loading = ref(false)
const submitted = ref(false)
 
const form = ref({
  isHelpful: true,
  missingFeature: '',
  wantedCourt: ''
})
 
const submitFeedback = async () => {
  loading.value = true
  try {
    await api.post('/feedback', {
      isHelpful: form.value.isHelpful,
      missingFeature: form.value.missingFeature,
      wantedCourt: form.value.wantedCourt
    })
    submitted.value = true
    toast.success('Gửi góp ý thành công!')
  } catch (error) {
    toast.error('Có lỗi xảy ra khi gửi góp ý. Vui lòng thử lại sau.')
  } finally {
    loading.value = false
  }
}
 
const resetForm = () => {
  form.value = {
    isHelpful: true,
    missingFeature: '',
    wantedCourt: ''
  }
  submitted.value = false
}
</script>
 
<style scoped>
.feedback-view {
  padding-top: 40px;
  padding-bottom: 80px;
  max-width: 800px;
}
.neon-text {
  font-size: 28px;
  font-weight: 800;
  text-transform: uppercase;
  letter-spacing: 1px;
  background: linear-gradient(135deg, #a3e635 0%, #10b981 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  margin-bottom: 8px;
}
.feedback-card {
  max-width: 600px;
  padding: 32px 24px;
  background: #111111;
  border: 1px solid rgba(255, 255, 255, 0.05);
  border-radius: 16px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4);
}
.form-label {
  font-size: 15px;
  font-weight: 600;
  display: block;
  margin-bottom: 10px;
  letter-spacing: 0.5px;
}
.helpful-options {
  display: flex;
  gap: 16px;
}
.hidden-radio {
  display: none;
}
.option-pill {
  flex: 1;
  text-align: center;
  padding: 14px 8px;
  background: #181818;
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  display: flex;
  align-items: center;
  justify-content: center;
}
.option-pill:hover {
  background: #222222;
  border-color: rgba(163, 230, 53, 0.3);
}
.option-pill.active {
  background: rgba(163, 230, 53, 0.1);
  border-color: #a3e635;
  box-shadow: 0 0 12px rgba(163, 230, 53, 0.15);
}
.pill-text {
  font-size: 14px;
  font-weight: 600;
  color: #cccccc;
  transition: color 0.3s;
}
.option-pill.active .pill-text {
  color: #a3e635;
}
.feedback-input {
  background: #181818;
  border: 1px solid rgba(255, 255, 255, 0.08);
  color: #ffffff;
  border-radius: 10px;
  padding: 12px 16px;
  font-size: 14px;
  transition: all 0.3s;
}
.feedback-input:focus {
  border-color: #a3e635;
  box-shadow: 0 0 10px rgba(163, 230, 53, 0.1);
  background: #1c1c1c;
}
.submit-btn {
  padding: 14px;
  font-size: 15px;
  font-weight: 700;
  border-radius: 10px;
  letter-spacing: 0.5px;
  transition: transform 0.2s, box-shadow 0.2s;
}
.submit-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 20px rgba(163, 230, 53, 0.3);
}
.success-icon {
  font-size: 56px;
  margin-bottom: 16px;
}
.text-success-neon {
  color: #a3e635;
  font-size: 20px;
  font-weight: 700;
  margin-bottom: 8px;
}
 
@media (max-width: 576px) {
  .helpful-options {
    flex-direction: column;
    gap: 12px;
  }
  .feedback-card {
    padding: 24px 16px;
  }
}
</style>
