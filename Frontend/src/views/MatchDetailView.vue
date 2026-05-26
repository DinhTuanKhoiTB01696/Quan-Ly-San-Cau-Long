<template>
  <div class="match-detail-view container" v-if="match">
    <!-- Nút Back -->
    <div class="mb-4">
      <router-link to="/" class="btn btn-outline" style="display: inline-flex; align-items: center; gap: 8px;">
        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
          <line x1="19" y1="12" x2="5" y2="12"></line>
          <polyline points="12 19 5 12 12 5"></polyline>
        </svg>
        Quay lại
      </router-link>
    </div>

    <div class="grid" style="display: grid; grid-template-columns: 2fr 1fr; gap: 24px;">
      <!-- Main Column -->
      <div class="main-column">
        <!-- Match Header -->
        <div class="card mb-4">
          <div class="match-header" style="display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 20px;">
            <div>
              <h1 style="margin: 0 0 8px 0; font-size: 24px;">Kèo sân {{ match.court?.name || 'Chưa rõ sân' }}</h1>
              <p class="text-secondary" style="margin: 0; display: flex; align-items: center; gap: 6px;">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z"></path><circle cx="12" cy="10" r="3"></circle></svg>
                {{ match.court?.address || 'Chưa có địa chỉ' }}
              </p>
            </div>
            <span :class="['status-badge', getStatusClass(match.status)]">
              {{ getStatusText(match.status) }}
            </span>
          </div>

          <div class="match-info-grid" style="display: grid; grid-template-columns: 1fr 1fr; gap: 16px; margin-bottom: 24px; padding: 16px; background: #f8fafc; border-radius: 8px;">
            <div class="info-item">
              <span class="label text-secondary">Thời gian:</span>
              <div class="value font-bold">{{ formatDate(match.date) }}</div>
              <div class="value">{{ formatTime(match.timeStart) }} - {{ formatTime(match.timeEnd) }}</div>
            </div>
            <div class="info-item">
              <span class="label text-secondary">Trình độ yêu cầu:</span>
              <div class="value font-bold" style="color: var(--primary-color);">{{ getLevelText(match.level) }}</div>
            </div>
            <div class="info-item">
              <span class="label text-secondary">Chi phí dự kiến:</span>
              <div class="value font-bold" style="color: #10b981;">{{ match.cost ? match.cost.toLocaleString('vi-VN') + ' đ' : 'Miễn phí' }} / người</div>
            </div>
            <div class="info-item">
              <span class="label text-secondary">Sĩ số:</span>
              <div class="value font-bold">{{ match.slotsFilled }} / {{ match.slotsTotal }} người</div>
            </div>
          </div>

          <div class="match-note" v-if="match.note">
            <h3 style="font-size: 16px; margin-bottom: 8px;">Ghi chú từ Host:</h3>
            <p style="background: #fffbeb; padding: 12px; border-radius: 6px; border-left: 4px solid #fbbf24; margin: 0;">{{ match.note }}</p>
          </div>
        </div>

        <!-- Participants List -->
        <div class="card">
          <h3 style="font-size: 18px; margin: 0 0 16px 0; display: flex; justify-content: space-between; align-items: center;">
            Danh sách tham gia
            <span class="badge" style="background: #e2e8f0; color: #475569; padding: 4px 10px; border-radius: 20px; font-size: 14px;">
              {{ match.slotsFilled }} / {{ match.slotsTotal }}
            </span>
          </h3>
          
          <div v-if="!match.participants || match.participants.length === 0" class="empty-state text-center text-secondary py-4">
            Chưa có ai tham gia kèo này.
          </div>
          
          <div class="participants-list" style="display: flex; flex-direction: column; gap: 12px;">
            <!-- Host is always first, though not in participants array, we should render them manually -->
            <div class="participant-item host-item" style="display: flex; align-items: center; gap: 12px; padding: 12px; border-radius: 8px; border: 1px solid var(--border-color); background: #f8fafc;">
              <div class="avatar" style="width: 40px; height: 40px; border-radius: 50%; background: var(--primary-color); color: white; display: flex; align-items: center; justify-content: center; font-weight: bold; font-size: 16px;">
                {{ getInitials(match.hostName) }}
              </div>
              <div class="info" style="flex: 1;">
                <div class="name font-bold" style="display: flex; align-items: center; gap: 8px;">
                  {{ match.hostName }}
                  <span class="badge" style="background: #ef4444; color: white; padding: 2px 6px; border-radius: 4px; font-size: 10px; text-transform: uppercase;">Host</span>
                </div>
              </div>
            </div>

            <!-- Other Participants -->
            <div v-for="p in match.participants" :key="p.userId" class="participant-item" style="display: flex; align-items: center; gap: 12px; padding: 12px; border-radius: 8px; border: 1px solid var(--border-color);">
              <div class="avatar" style="width: 40px; height: 40px; border-radius: 50%; background: #e2e8f0; color: #475569; display: flex; align-items: center; justify-content: center; font-weight: bold; font-size: 16px;">
                {{ getInitials(p.fullName) }}
              </div>
              <div class="info">
                <div class="name font-bold">{{ p.fullName }}</div>
                <div class="username text-secondary" style="font-size: 12px;">@{{ p.username }}</div>
              </div>
            </div>
            
            <!-- Empty Slots placeholders -->
            <div v-for="i in emptySlots" :key="'empty-'+i" class="participant-item empty" style="display: flex; align-items: center; gap: 12px; padding: 12px; border-radius: 8px; border: 1px dashed #cbd5e1; opacity: 0.6;">
              <div class="avatar" style="width: 40px; height: 40px; border-radius: 50%; border: 2px dashed #cbd5e1; display: flex; align-items: center; justify-content: center; color: #94a3b8;">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path><circle cx="12" cy="7" r="4"></circle></svg>
              </div>
              <div class="info text-secondary">
                Slot còn trống
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Sidebar -->
      <div class="sidebar">
        <div class="card" style="position: sticky; top: 80px;">
          <h3 style="font-size: 18px; margin: 0 0 16px 0;">Thao tác</h3>
          
          <div class="actions-wrapper" style="display: flex; flex-direction: column; gap: 12px;">
            <template v-if="isHost">
              <button class="btn btn-outline w-100" style="margin-bottom: 8px;" disabled>
                Bạn là chủ kèo này
              </button>
            </template>
            
            <template v-else-if="authStore.isAuthenticated">
              <button 
                v-if="!hasJoined && match.status === 1" 
                @click="handleJoin" 
                class="btn btn-primary w-100"
                :disabled="isProcessing"
              >
                {{ isProcessing ? 'Đang xử lý...' : 'Tham Gia Kèo' }}
              </button>
              
              <template v-else-if="hasJoined">
                <div class="alert success text-center mb-2" style="background: #dcfce7; color: #166534; padding: 10px; border-radius: 6px; font-weight: bold;">
                  ✓ Bạn đã tham gia kèo này
                </div>
                
                <a :href="'https://zalo.me/' + match.zalo" target="_blank" class="btn btn-primary w-100 mb-2" style="background: #0068ff; border-color: #0068ff;">
                  Nhắn Zalo cho Host
                </a>
                
                <button 
                  v-if="match.status !== 3"
                  @click="handleLeave" 
                  class="btn btn-outline w-100 text-danger" 
                  style="border-color: #ef4444; color: #ef4444;"
                  :disabled="isProcessing"
                >
                  {{ isProcessing ? 'Đang xử lý...' : 'Hủy Tham Gia' }}
                </button>
              </template>
              
              <button v-else-if="match.status === 2" class="btn btn-outline w-100" disabled>Đã Đủ Người</button>
              <button v-else-if="match.status === 3" class="btn btn-outline w-100" disabled>Đã Kết Thúc</button>
            </template>
            
            <template v-else>
              <router-link :to="{ name: 'login', query: { redirect: $route.fullPath }}" class="btn btn-primary w-100 text-center">
                Đăng Nhập Để Tham Gia
              </router-link>
            </template>
            
            <hr style="border: 0; border-top: 1px solid var(--border-color); margin: 8px 0;" />
            
            <!-- Nút Báo cáo (Hiển thị cho mọi người trừ host) -->
            <button v-if="!isHost" @click="showReportModal = true" class="btn w-100" style="background: transparent; color: #64748b; font-size: 14px;">
              <svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="margin-right: 4px; vertical-align: middle;"><path d="M4 15s1-1 4-1 5 2 8 2 4-1 4-1V3s-1 1-4 1-5-2-8-2-4 1-4 1z"></path><line x1="4" y1="22" x2="4" y2="15"></line></svg>
              Báo cáo kèo xấu
            </button>
          </div>
        </div>
      </div>
    </div>
    
    <!-- Report Modal -->
    <div v-if="showReportModal" class="modal-overlay" @click.self="showReportModal = false">
      <div class="modal-content card" style="max-width: 400px; width: 100%;">
        <h3>Báo cáo Kèo này</h3>
        <p class="text-secondary mb-3">Vui lòng chọn lý do báo cáo. Hệ thống sẽ tự động hủy kèo nếu có nhiều người cùng báo cáo.</p>
        
        <div class="form-group mb-3">
          <label>Lý do</label>
          <select v-model="reportReason" class="form-control">
            <option value="1">Kèo ảo / Số điện thoại (Zalo) giả</option>
            <option value="2">Spam / Quảng cáo</option>
            <option value="3">Sai thông tin (Giá, Giờ, Sân)</option>
          </select>
        </div>
        
        <div style="display: flex; gap: 10px; justify-content: flex-end; margin-top: 20px;">
          <button @click="showReportModal = false" class="btn btn-outline">Hủy</button>
          <button @click="submitReport" class="btn btn-primary" style="background: #ef4444; border-color: #ef4444;" :disabled="isReporting">
            {{ isReporting ? 'Đang gửi...' : 'Gửi Báo Cáo' }}
          </button>
        </div>
      </div>
    </div>
  </div>
  
  <div v-else-if="loading" class="container text-center" style="padding: 100px 0;">
    <div class="spinner" style="width: 40px; height: 40px; border: 4px solid #e2e8f0; border-top-color: var(--primary-color); border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto 20px;"></div>
    <p>Đang tải thông tin kèo...</p>
  </div>
  
  <div v-else class="container text-center" style="padding: 100px 0;">
    <h2>Không tìm thấy kèo</h2>
    <p class="text-secondary mb-4">Kèo này có thể đã bị xóa hoặc không tồn tại.</p>
    <router-link to="/" class="btn btn-primary">Quay lại Trang Chủ</router-link>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useToast } from 'vue-toastification'
import api from '@/api/axios'

const route = useRoute()
const router = useRouter()
const authStore = useAuthStore()
const toast = useToast()

const match = ref(null)
const loading = ref(true)
const isProcessing = ref(false)

// Report modal state
const showReportModal = ref(false)
const reportReason = ref('1')
const isReporting = ref(false)

const isHost = computed(() => {
  return authStore.user && match.value && authStore.user.id === match.value.hostUserId
})

const hasJoined = computed(() => {
  return authStore.user && match.value && match.value.participantIds?.includes(authStore.user.id)
})

const emptySlots = computed(() => {
  if (!match.value) return 0
  const slots = match.value.slotsTotal - match.value.slotsFilled
  return slots > 0 ? slots : 0
})

const fetchMatch = async () => {
  loading.value = true
  try {
    const res = await api.get(`/matches/${route.params.id}`)
    match.value = res.data
  } catch (err) {
    console.error(err)
    if (err.response?.status === 404) {
      toast.error('Kèo không tồn tại')
    } else {
      toast.error('Lỗi khi tải thông tin kèo')
    }
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchMatch()
})

const handleJoin = async () => {
  if (!authStore.isAuthenticated) {
    router.push({ name: 'login', query: { redirect: route.fullPath } })
    return
  }
  
  isProcessing.value = true
  try {
    await api.post(`/matches/${match.value.id}/join`)
    toast.success('Tham gia kèo thành công!')
    await fetchMatch() // Reload match to get new participant list
  } catch (err) {
    toast.error(err.response?.data || 'Lỗi khi tham gia kèo')
  } finally {
    isProcessing.value = false
  }
}

const handleLeave = async () => {
  if (confirm('Bạn có chắc chắn muốn hủy tham gia kèo này?')) {
    isProcessing.value = true
    try {
      await api.post(`/matches/${match.value.id}/leave`)
      toast.info('Đã hủy tham gia')
      await fetchMatch()
    } catch (err) {
      toast.error(err.response?.data || 'Lỗi khi hủy tham gia')
    } finally {
      isProcessing.value = false
    }
  }
}

const submitReport = async () => {
  isReporting.value = true
  try {
    await api.post('/reports', {
      matchId: match.value.id,
      reason: parseInt(reportReason.value)
    })
    toast.success('Báo cáo đã được ghi nhận. Cảm ơn bạn!')
    showReportModal.value = false
  } catch (err) {
    toast.error(err.response?.data || 'Lỗi khi gửi báo cáo')
  } finally {
    isReporting.value = false
  }
}

// Helpers
const getStatusText = (status) => {
  switch(status) {
    case 1: return 'Đang tuyển'
    case 2: return 'Đã đủ người'
    case 3: return 'Đã kết thúc'
    default: return 'Không xác định'
  }
}

const getStatusClass = (status) => {
  switch(status) {
    case 1: return 'open'
    case 2: return 'full'
    case 3: return 'expired'
    default: return ''
  }
}

const getLevelText = (level) => {
  switch(level) {
    case 1: return 'Yếu (Mới chơi)'
    case 2: return 'Trung bình'
    case 3: return 'Khá'
    case 4: return 'Giỏi'
    default: return 'Mọi trình độ'
  }
}

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return date.toLocaleDateString('vi-VN', { weekday: 'long', day: '2-digit', month: '2-digit', year: 'numeric' })
}

const formatTime = (timeStr) => {
  if (!timeStr) return ''
  // timeStr từ C# TimeSpan có dạng "HH:mm:ss"
  const parts = timeStr.split(':')
  return `${parts[0]}:${parts[1]}`
}

const getInitials = (name) => {
  if (!name) return '?'
  return name.charAt(0).toUpperCase()
}
</script>

<style scoped>
.status-badge {
  display: inline-block;
  padding: 6px 12px;
  border-radius: 20px;
  font-size: 14px;
  font-weight: 600;
}
.status-badge.open {
  background: #dcfce7;
  color: #166534;
}
.status-badge.full {
  background: #e2e8f0;
  color: #475569;
}
.status-badge.expired {
  background: #fee2e2;
  color: #991b1b;
}

@media (max-width: 768px) {
  .grid {
    grid-template-columns: 1fr !important;
  }
  .sidebar {
    order: -1; /* Hiển thị các thao tác (Tham gia) lên trên cùng ở mobile */
  }
}

/* Modal styles */
.modal-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.modal-content {
  padding: 24px;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
