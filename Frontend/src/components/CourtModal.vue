<template>
  <div v-if="show" class="modal-backdrop">
    <div class="modal-content">
      <div class="modal-header">
        <h3>{{ isEdit ? 'Cập nhật Sân' : 'Thêm Sân Mới' }}</h3>
        <button class="btn-close" @click="$emit('close')">&times;</button>
      </div>
      
      <form @submit.prevent="handleSubmit" class="modal-body">
        <div class="form-group">
          <label>Tên sân</label>
          <input v-model="form.name" type="text" required />
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
            <label>Giá/Giờ (VNĐ)</label>
            <input v-model.number="form.price" type="number" required />
          </div>
        </div>

        <div class="form-group">
          <label>Địa chỉ</label>
          <input v-model="form.address" type="text" required />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>Trần nhà (Cao/Thấp)</label>
            <input v-model="form.ceiling" type="text" required />
          </div>
          <div class="form-group">
            <label>Ánh sáng</label>
            <input v-model="form.light" type="text" required />
          </div>
          <div class="form-group">
            <label>Thảm sân</label>
            <input v-model="form.surface" type="text" required />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label>Đánh giá (1-5)</label>
            <input v-model.number="form.rating" type="number" step="0.1" min="1" max="5" required />
          </div>
          <div class="form-group">
            <label>Số điện thoại</label>
            <input v-model="form.phone" type="text" required />
          </div>
        </div>

        <div class="form-group checkbox-group">
          <label>
            <input type="checkbox" v-model="form.isFeatured" />
            Đánh dấu là sân nổi bật
          </label>
        </div>

        <div class="modal-footer">
          <button type="button" class="btn btn-outline" @click="$emit('close')">Hủy</button>
          <button type="submit" class="btn btn-primary" :disabled="loading">
            {{ loading ? 'Đang lưu...' : 'Lưu' }}
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
  isFeatured: false
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
        phone: '', isFeatured: false
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
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.modal-content {
  background: white;
  border-radius: 8px;
  width: 90%;
  max-width: 500px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 10px 25px rgba(0,0,0,0.2);
}
.modal-header {
  padding: 16px 24px;
  border-bottom: 1px solid var(--border-color);
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.modal-header h3 { margin: 0; font-size: 18px; }
.btn-close {
  background: none; border: none; font-size: 24px; cursor: pointer;
}
.modal-body {
  padding: 24px;
}
.form-group {
  margin-bottom: 16px;
  display: flex;
  flex-direction: column;
}
.form-group label {
  font-size: 14px; font-weight: 500; margin-bottom: 6px;
}
.form-group input, .form-group select {
  padding: 8px 12px;
  border: 1px solid var(--border-color);
  border-radius: 6px;
  font-size: 14px;
}
.form-row {
  display: flex; gap: 12px;
}
.form-row .form-group {
  flex: 1;
}
.checkbox-group {
  flex-direction: row; align-items: center; gap: 8px;
}
.modal-footer {
  display: flex; justify-content: flex-end; gap: 12px;
  margin-top: 24px;
}
</style>
