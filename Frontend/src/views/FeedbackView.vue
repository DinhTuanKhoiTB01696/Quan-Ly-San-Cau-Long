<template>
  <div class="feedback-view container">
    <div class="page-header" style="text-align: center; margin-bottom: 32px;">
      <h2>Góp Ý Xây Dựng Ứng Dụng</h2>
      <p class="text-secondary mt-2">Chúng tôi luôn lắng nghe để cải thiện trải nghiệm của bạn</p>
    </div>

    <div class="card feedback-card mx-auto" style="max-width: 600px; padding: 32px;">
      <div v-if="submitted" class="success-message text-center">
        <div style="font-size: 48px; color: var(--success-color); margin-bottom: 16px;">🎉</div>
        <h3 style="color: var(--success-color); margin-bottom: 8px;">Cảm ơn bạn đã góp ý!</h3>
        <p class="text-secondary mb-4">Những đóng góp của bạn sẽ giúp cộng đồng cầu lông Biên Hòa ngày càng phát triển.</p>
        <button class="btn btn-primary" @click="resetForm">Gửi thêm góp ý</button>
      </div>

      <form v-else @submit.prevent="submitFeedback">
        <div class="form-group mb-4">
          <label style="font-weight: 600; display: block; margin-bottom: 12px;">Bạn có thấy ứng dụng này hữu ích không?</label>
          <div style="display: flex; gap: 24px;">
            <label class="radio-label">
              <input type="radio" v-model="form.isHelpful" :value="true" name="isHelpful" /> 
              <span>Có, rất hữu ích 👍</span>
            </label>
            <label class="radio-label">
              <input type="radio" v-model="form.isHelpful" :value="false" name="isHelpful" /> 
              <span>Chưa tốt lắm 👎</span>
            </label>
          </div>
        </div>

        <div class="form-group mb-4">
          <label style="font-weight: 600; display: block; margin-bottom: 8px;">Tính năng nào bạn muốn có thêm?</label>
          <textarea 
            v-model="form.missingFeature" 
            class="form-control" 
            rows="4" 
            placeholder="Ví dụ: Tính năng chat nhóm, xếp hạng người chơi..."
          ></textarea>
        </div>

        <div class="form-group mb-4">
          <label style="font-weight: 600; display: block; margin-bottom: 8px;">Sân cầu lông nào ở Biên Hòa bạn muốn ứng dụng cập nhật thêm?</label>
          <input 
            type="text" 
            v-model="form.wantedCourt" 
            class="form-control" 
            placeholder="Nhập tên sân (Ví dụ: Sân A7, Sân Bửu Long...)"
          />
        </div>

        <button type="submit" class="btn btn-primary w-100" style="padding: 12px; font-size: 16px;" :disabled="loading">
          {{ loading ? 'Đang gửi...' : 'Gửi Góp Ý' }}
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
  padding-top: 24px;
  padding-bottom: 48px;
}
.radio-label {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  font-size: 15px;
}
.radio-label input[type="radio"] {
  width: 18px;
  height: 18px;
  cursor: pointer;
}
</style>
