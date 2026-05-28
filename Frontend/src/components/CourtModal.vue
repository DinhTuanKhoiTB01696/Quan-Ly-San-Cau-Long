<template>
  <div v-if="show" class="modal-backdrop animate-fade">
    <div class="modal-content card">
      <div class="modal-header">
        <h3>{{ isEdit ? '⚡ Cập nhật Sân Cầu Lông' : '🏟️ Thêm Sân Mới' }}</h3>
        <button class="btn-close" @click="$emit('close')">&times;</button>
      </div>
      
      <form @submit.prevent="handleSubmit" class="modal-body">
        <div class="form-group">
          <label>Tên sân cầu lông</label>
          <input v-model="form.name" type="text" placeholder="Ví dụ: Sân Nguyễn Ái Quốc" required />
        </div>
        
        <div class="form-row">
          <div class="form-group">
            <label>Khu vực</label>
            <select v-model.number="form.area" required>
              <option value="1">Tân Mai</option>
              <option value="2">Trảng Dài</option>
              <option value="3">Long Bình</option>
              <option value="4">Tân Hiệp</option>
              <option value="5">Hố Nai</option>
            </select>
          </div>
          <div class="form-group">
            <label>Giá thuê / Giờ (VNĐ)</label>
            <input v-model.number="form.price" type="number" min="0" required />
          </div>
        </div>

        <div class="form-group">
          <label>Địa chỉ cụ thể</label>
          <input v-model="form.address" type="text" placeholder="Ví dụ: 12 Nguyễn Ái Quốc, Tân Mai" required />
        </div>

        <!-- NEW: Image URL field -->
        <div class="form-group">
          <label>Đường dẫn hình ảnh sân (URL)</label>
          <input v-model="form.imageUrl" type="url" placeholder="https://unsplash.com/... hoặc link ảnh bất kỳ" />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>Trần nhà (Cao/Trung/Thấp)</label>
            <input v-model="form.ceiling" type="text" placeholder="Cao" required />
          </div>
          <div class="form-group">
            <label>Ánh sáng (Tốt/Chói...)</label>
            <input v-model="form.light" type="text" placeholder="Tốt" required />
          </div>
          <div class="form-group">
            <label>Thảm sân (PVC/Gỗ...)</label>
            <input v-model="form.surface" type="text" placeholder="Thảm PVC" required />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>Đánh giá (1.0 - 5.0)</label>
            <input v-model.number="form.rating" type="number" step="0.1" min="1" max="5" required />
          </div>
          <div class="form-group">
            <label>Số điện thoại đặt sân</label>
            <input v-model="form.phone" type="text" placeholder="0909xxxxxx" required />
          </div>
        </div>

        <div class="form-group checkbox-group">
          <label class="checkbox-label">
            <input type="checkbox" v-model="form.isFeatured" class="custom-checkbox" />
            <span>Đánh dấu là sân nổi bật (Featured)</span>
          </label>
        </div>

        <div class="modal-footer">
          <button type="button" class="btn btn-outline" @click="$emit('close')">Hủy</button>
          <button type="submit" class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Đang lưu...' : 'Lưu Sân' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'

const props = defineProps({
  show: Boolean,
  courtData: {
    type: Object,
    default: null
  }
})

const emit = defineEmits(['close', 'save'])

const loading = ref(false)
const isEdit = ref(false)

const form = ref({
  name: '',
  area: 1,
  address: '',
  price: 0,
  ceiling: '',
  light: '',
  surface: '',
  rating: 5,
  phone: '',
  isFeatured: false,
  imageUrl: ''
})

watch(() => props.show, (newVal) => {
  if (newVal) {
    if (props.courtData) {
      isEdit.value = true
      form.value = { ...props.courtData }
    } else {
      isEdit.value = false
      form.value = {
        name: '', area: 1, address: '', price: 40000,
        ceiling: 'Cao', light: 'Tốt', surface: 'Thảm PVC', rating: 5,
        phone: '', isFeatured: false, imageUrl: ''
      }
    }
  }
})

const handleSubmit = () => {
  emit('save', form.value)
}
</script>

<style scoped>
.modal-backdrop {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(11, 15, 25, 0.8);
  backdrop-filter: blur(8px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
}

.modal-content {
  background: rgba(15, 23, 42, 0.95) !important;
  border: 1px solid var(--border-color) !important;
  box-shadow: var(--shadow-neon-lg) !important;
  border-radius: var(--border-radius);
  width: 95%;
  max-width: 550px;
  max-height: 90vh;
  overflow-y: auto;
  padding: 0;
}

.modal-header {
  padding: 20px 24px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.modal-header h3 {
  margin: 0;
  font-size: 20px;
  font-weight: 800;
  color: var(--text-primary);
  text-shadow: 0 0 10px rgba(163, 230, 53, 0.2);
}

.btn-close {
  background: none;
  border: none;
  font-size: 28px;
  color: var(--text-secondary);
  cursor: pointer;
  line-height: 1;
  transition: color 0.2s;
}

.btn-close:hover {
  color: var(--primary-color);
}

.modal-body {
  padding: 24px;
}

.form-group {
  margin-bottom: 18px;
  display: flex;
  flex-direction: column;
}

.form-group label {
  font-size: 13px;
  font-weight: 600;
  color: var(--text-secondary);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-bottom: 8px;
}

.form-row {
  display: flex;
  gap: 16px;
}

.form-row .form-group {
  flex: 1;
}

/* Checkbox group custom styling */
.checkbox-group {
  flex-direction: row;
  align-items: center;
  margin-top: 8px;
}

.checkbox-label {
  display: flex;
  align-items: center;
  gap: 10px;
  cursor: pointer;
  user-select: none;
}

.checkbox-label span {
  font-size: 14px;
  color: var(--text-primary);
  text-transform: none;
  letter-spacing: 0;
  font-weight: 500;
}

.custom-checkbox {
  width: 18px;
  height: 18px;
  accent-color: var(--primary-color);
  cursor: pointer;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 28px;
  border-top: 1px solid rgba(255, 255, 255, 0.05);
  padding-top: 20px;
}

/* Spinner & animations */
.animate-fade {
  animation: fadeIn 0.3s ease forwards;
}

@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}
</style>
