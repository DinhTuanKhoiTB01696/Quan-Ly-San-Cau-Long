<template>
  <div class="not-found container text-center p-4">
    <div class="empty-icon" style="font-size: 80px; margin-bottom: 20px;">🏸</div>
    <h2>404 - Không tìm thấy trang</h2>
    <p class="text-secondary" style="margin-bottom: 24px;">Trang bạn đang tìm kiếm không tồn tại hoặc đã bị xóa.</p>
    <router-link to="/" class="btn btn-primary">Quay lại Trang Chủ</router-link>
  </div>
</template>

<style scoped>
.not-found {
  margin-top: 60px;
  background: white;
  border-radius: var(--border-radius);
  border: 1px dashed var(--border-color);
  padding: 60px 20px;
}
</style>
