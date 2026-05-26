<template>
  <div class="card match-card-skeleton">
    <div class="skeleton-header">
      <div class="skeleton-court-info">
        <div class="skeleton-line title"></div>
        <div class="skeleton-line address"></div>
      </div>
      <div class="skeleton-badge"></div>
    </div>
    <div class="skeleton-details">
      <div class="skeleton-line detail-item" v-for="i in 4" :key="i"></div>
      <div class="skeleton-line detail-item full-width"></div>
    </div>
    <div class="skeleton-footer">
      <div class="skeleton-host">
        <div class="skeleton-avatar"></div>
        <div class="skeleton-line name"></div>
      </div>
      <div class="skeleton-button"></div>
    </div>
  </div>
</template>

<style scoped>
.match-card-skeleton {
  padding: 16px;
  animation: pulse 1.5s infinite ease-in-out;
}

@keyframes pulse {
  0% { opacity: 0.6; }
  50% { opacity: 1; }
  100% { opacity: 0.6; }
}

.skeleton-line {
  background: var(--border-color);
  border-radius: 4px;
  height: 16px;
}

.skeleton-header {
  display: flex;
  justify-content: space-between;
  margin-bottom: 16px;
}
.skeleton-court-info { flex: 1; }
.skeleton-court-info .title { width: 60%; height: 20px; margin-bottom: 8px; }
.skeleton-court-info .address { width: 40%; height: 14px; }
.skeleton-badge {
  width: 80px; height: 24px; border-radius: 12px;
  background: var(--border-color);
}

.skeleton-details {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 12px;
  margin-bottom: 16px;
  background: var(--secondary-color);
  padding: 12px;
  border-radius: 8px;
}
.skeleton-details .detail-item {
  width: 80%;
}
.skeleton-details .full-width {
  grid-column: 1 / -1;
  width: 95%;
}

.skeleton-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-top: 1px solid var(--border-color);
  padding-top: 12px;
}
.skeleton-host {
  display: flex; align-items: center; gap: 8px;
}
.skeleton-avatar {
  width: 28px; height: 28px; border-radius: 50%;
  background: var(--border-color);
}
.skeleton-host .name { width: 80px; }
.skeleton-button {
  width: 100px; height: 32px; border-radius: 6px;
  background: var(--border-color);
}
</style>
