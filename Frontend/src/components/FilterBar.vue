<template>
  <div class="filter-bar card">
    <div class="filter-group">
      <label>Khu vực</label>
      <select v-model="filters.area" @change="emitFilter">
        <option :value="null">Tất cả khu vực</option>
        <option value="1">Tân Mai</option>
        <option value="2">Trảng Dài</option>
        <option value="3">Long Bình</option>
        <option value="4">Tân Hiệp</option>
        <option value="5">Hố Nai</option>
      </select>
    </div>

    <div class="filter-group">
      <label>Trình độ</label>
      <select v-model="filters.level" @change="emitFilter">
        <option :value="null">Tất cả trình độ</option>
        <option value="1">Yếu (Mới chơi)</option>
        <option value="2">Trung bình (Đánh rally 10+)</option>
        <option value="3">Khá (Biết smash)</option>
        <option value="4">Giỏi (Thi đấu phong trào)</option>
      </select>
    </div>

    <div class="filter-group">
      <label>Trạng thái</label>
      <select v-model="filters.status" @change="emitFilter">
        <option :value="null">Tất cả</option>
        <option value="1">Đang tuyển</option>
        <option value="2">Đã đủ người</option>
      </select>
    </div>
  </div>
</template>

<script setup>
import { reactive } from 'vue'

const emit = defineEmits(['filter-changed'])

const filters = reactive({
  area: null,
  level: null,
  status: 1 // Mặc định hiển thị đang tuyển (Open = 1)
})

const emitFilter = () => {
  emit('filter-changed', { ...filters })
}
</script>

<style scoped>
.filter-bar {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(120px, 1fr));
  gap: 12px;
  padding: 12px;
}
.filter-group {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.filter-group label {
  font-size: 12px;
  font-weight: 500;
  color: var(--text-secondary);
}
select {
  padding: 8px 10px;
}
</style>
