<template>
  <div class="profile-view container">
    <div class="page-header" style="margin-bottom: 32px;">
      <h2>Hồ Sơ Của Tôi</h2>
    </div>

    <div class="profile-layout" style="display: grid; grid-template-columns: 250px 1fr; gap: 32px;">
      
      <!-- Sidebar Navigation -->
      <div class="profile-sidebar">
        <div class="user-summary card" style="text-align: center; margin-bottom: 16px; padding: 24px 16px;">
          <div class="avatar mx-auto" style="width: 80px; height: 80px; border-radius: 50%; background: var(--primary-color); color: white; display: flex; align-items: center; justify-content: center; font-size: 32px; font-weight: bold; margin: 0 auto 16px;">
            {{ authStore.user?.fullName?.charAt(0).toUpperCase() }}
          </div>
          <h3 style="margin: 0 0 4px; font-size: 18px;">{{ authStore.user?.fullName }}</h3>
          <p class="text-secondary" style="margin: 0 0 12px; font-size: 14px;">@{{ authStore.user?.username }}</p>
          <span class="badge" style="background: #f1f5f9; color: #475569; padding: 4px 12px; border-radius: 12px; font-size: 12px;">{{ authStore.user?.role }}</span>
        </div>

        <div class="card" style="padding: 8px;">
          <ul class="nav-menu" style="list-style: none; padding: 0; margin: 0;">
            <li>
              <button :class="['nav-btn', { active: currentTab === 'info' }]" @click="currentTab = 'info'">
                Thông tin cá nhân
              </button>
            </li>
            <li>
              <button :class="['nav-btn', { active: currentTab === 'hosted' }]" @click="currentTab = 'hosted'">
                Kèo đã tạo
              </button>
            </li>
            <li>
              <button :class="['nav-btn', { active: currentTab === 'joined' }]" @click="currentTab = 'joined'">
                Kèo tham gia
              </button>
            </li>
            <li>
              <button :class="['nav-btn', { active: currentTab === 'password' }]" @click="currentTab = 'password'">
                Đổi mật khẩu
              </button>
            </li>
            <li style="margin-top: 16px; padding-top: 16px; border-top: 1px solid var(--border-color);">
              <button class="nav-btn text-danger" @click="handleLogout" style="color: #ef4444;">
                Đăng xuất
              </button>
            </li>
          </ul>
        </div>
      </div>

      <!-- Main Content Area -->
      <div class="profile-content">
        
        <!-- Tab: Thông tin cá nhân -->
        <div v-if="currentTab === 'info'" class="card">
          <h3 style="margin: 0 0 24px; font-size: 20px;">Thông Tin Cá Nhân</h3>
          <form @submit.prevent="handleUpdateProfile">
            <div class="form-group mb-3">
              <label>Họ và Tên</label>
              <input type="text" v-model="profileForm.fullName" required class="form-control" />
            </div>
            <div class="form-group mb-3">
              <label>Số điện thoại / Zalo</label>
              <input type="text" v-model="profileForm.phone" class="form-control" placeholder="Nhập số điện thoại liên lạc" />
            </div>
            <div class="form-group mb-4">
              <label>Trình độ chơi</label>
              <select v-model="profileForm.skillLevel" class="form-select">
                <option value="Mới chơi">Mới chơi</option>
                <option value="Trung bình">Trung bình</option>
                <option value="Khá">Khá</option>
                <option value="Tốt">Tốt</option>
              </select>
            </div>
            <button type="submit" class="btn btn-primary" :disabled="loading.profile">
              {{ loading.profile ? 'Đang lưu...' : 'Lưu Thay Đổi' }}
            </button>
          </form>
        </div>

        <!-- Tab: Đổi mật khẩu -->
        <div v-if="currentTab === 'password'" class="card">
          <h3 style="margin: 0 0 24px; font-size: 20px;">Đổi Mật Khẩu</h3>
          <form @submit.prevent="handleChangePassword">
            <div class="form-group mb-3">
              <label>Mật khẩu hiện tại</label>
              <input type="password" v-model="passwordForm.oldPassword" required class="form-control" />
            </div>
            <div class="form-group mb-3">
              <label>Mật khẩu mới</label>
              <input type="password" v-model="passwordForm.newPassword" required class="form-control" />
            </div>
            <div class="form-group mb-4">
              <label>Xác nhận mật khẩu mới</label>
              <input type="password" v-model="passwordForm.confirmPassword" required class="form-control" />
            </div>
            <button type="submit" class="btn btn-primary" :disabled="loading.password">
              {{ loading.password ? 'Đang xử lý...' : 'Cập Nhật Mật Khẩu' }}
            </button>
          </form>
        </div>

        <!-- Tab: Kèo đã tạo -->
        <div v-if="currentTab === 'hosted'">
          <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px;">
            <h3 style="margin: 0; font-size: 20px;">Kèo Của Tôi (Đã tạo)</h3>
            <router-link to="/create-match" class="btn btn-primary btn-sm">+ Tạo kèo mới</router-link>
          </div>
          
          <div v-if="loading.matches" class="text-center py-4">
            <div class="spinner" style="width: 30px; height: 30px; border: 3px solid #e2e8f0; border-top-color: var(--primary-color); border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto;"></div>
          </div>
          <div v-else-if="hostedMatches.length === 0" class="card text-center py-5 text-secondary">
            Bạn chưa tạo kèo nào.
          </div>
          <div v-else class="matches-grid" style="display: grid; gap: 16px;">
            <MatchCard 
              v-for="match in hostedMatches" 
              :key="match.id" 
              :match="match" 
              :isHost="true" 
              @mark-full="markMatchFull"
            />
          </div>
        </div>

        <!-- Tab: Kèo tham gia -->
        <div v-if="currentTab === 'joined'">
          <h3 style="margin: 0 0 24px; font-size: 20px;">Lịch Sử Tham Gia</h3>
          
          <div v-if="loading.matches" class="text-center py-4">
            <div class="spinner" style="width: 30px; height: 30px; border: 3px solid #e2e8f0; border-top-color: var(--primary-color); border-radius: 50%; animation: spin 1s linear infinite; margin: 0 auto;"></div>
          </div>
          <div v-else-if="joinedMatches.length === 0" class="card text-center py-5 text-secondary">
            Bạn chưa tham gia kèo nào. Hãy ra trang chủ để tìm kèo nhé!
          </div>
          <div v-else class="matches-grid" style="display: grid; gap: 16px;">
            <MatchCard 
              v-for="match in joinedMatches" 
              :key="match.id" 
              :match="match" 
              :isHost="false" 
              @leave="leaveMatch"
            />
          </div>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useToast } from 'vue-toastification'
import api from '@/api/axios'
import MatchCard from '@/components/MatchCard.vue'

const router = useRouter()
const authStore = useAuthStore()
const toast = useToast()

const currentTab = ref('info') // info, password, hosted, joined

const loading = ref({
  profile: false,
  password: false,
  matches: false
})

const profileForm = ref({
  fullName: authStore.user?.fullName || '',
  phone: authStore.user?.phone || '',
  skillLevel: authStore.user?.skillLevel || 'Trung bình'
})

const passwordForm = ref({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const hostedMatches = ref([])
const joinedMatches = ref([])

// Lấy dữ liệu mỗi khi tab thay đổi sang tab matches
watch(currentTab, (newTab) => {
  if (newTab === 'hosted' && hostedMatches.value.length === 0) {
    fetchHostedMatches()
  } else if (newTab === 'joined' && joinedMatches.value.length === 0) {
    fetchJoinedMatches()
  }
})

const handleUpdateProfile = async () => {
  loading.value.profile = true
  try {
    const res = await api.put('/auth/profile', {
      fullName: profileForm.value.fullName,
      phone: profileForm.value.phone,
      skillLevel: profileForm.value.skillLevel
    })
    // Cập nhật lại token và user store
    authStore.setAuth(res.data.token, {
      id: res.data.userId,
      username: res.data.username,
      fullName: res.data.fullName,
      phone: res.data.phone,
      role: res.data.role,
      skillLevel: res.data.skillLevel
    })
    toast.success('Cập nhật hồ sơ thành công')
  } catch (err) {
    toast.error(err.response?.data?.message || 'Có lỗi xảy ra')
  } finally {
    loading.value.profile = false
  }
}

const handleChangePassword = async () => {
  if (passwordForm.value.newPassword !== passwordForm.value.confirmPassword) {
    toast.error('Mật khẩu xác nhận không khớp!')
    return
  }
  if (passwordForm.value.newPassword.length < 6) {
    toast.error('Mật khẩu mới phải từ 6 ký tự trở lên.')
    return
  }
  loading.value.password = true
  try {
    await api.put('/auth/change-password', {
      oldPassword: passwordForm.value.oldPassword,
      newPassword: passwordForm.value.newPassword
    })
    toast.success('Đổi mật khẩu thành công!')
    passwordForm.value = { oldPassword: '', newPassword: '', confirmPassword: '' }
  } catch (err) {
    toast.error(err.response?.data?.message || 'Đổi mật khẩu thất bại')
  } finally {
    loading.value.password = false
  }
}

const fetchHostedMatches = async () => {
  loading.value.matches = true
  try {
    const res = await api.get('/matches/my-matches')
    hostedMatches.value = res.data
  } catch (err) {
    toast.error('Không thể tải lịch sử tạo kèo')
  } finally {
    loading.value.matches = false
  }
}

const fetchJoinedMatches = async () => {
  loading.value.matches = true
  try {
    const res = await api.get('/matches/joined-matches')
    joinedMatches.value = res.data
  } catch (err) {
    toast.error('Không thể tải lịch sử tham gia kèo')
  } finally {
    loading.value.matches = false
  }
}

const markMatchFull = async (id) => {
  try {
    await api.put(`/matches/${id}/status`, 2) // 2 = Full
    toast.success('Đã cập nhật trạng thái đủ kèo')
    fetchHostedMatches()
  } catch (err) {
    toast.error('Có lỗi xảy ra')
  }
}

const leaveMatch = async (id) => {
  if (confirm('Bạn có chắc chắn muốn hủy tham gia kèo này?')) {
    try {
      await api.post(`/matches/${id}/leave`)
      toast.info('Đã hủy tham gia kèo')
      fetchJoinedMatches() // Refresh danh sách
    } catch (err) {
      toast.error(err.response?.data?.message || 'Có lỗi xảy ra')
    }
  }
}

const handleLogout = () => {
  if (confirm('Bạn có muốn đăng xuất?')) {
    authStore.logout()
    router.push('/login')
  }
}
</script>

<style scoped>
.nav-menu {
  display: flex;
  flex-direction: column;
}
.nav-btn {
  width: 100%;
  text-align: left;
  padding: 12px 16px;
  background: transparent;
  border: none;
  border-radius: 8px;
  font-size: 15px;
  color: #475569;
  cursor: pointer;
  transition: all 0.2s;
  margin-bottom: 4px;
}
.nav-btn:hover {
  background: #f1f5f9;
}
.nav-btn.active {
  background: #eff6ff;
  color: var(--primary-color);
  font-weight: 600;
}

@media (max-width: 768px) {
  .profile-layout {
    grid-template-columns: 1fr !important;
  }
  .nav-menu {
    flex-direction: row;
    overflow-x: auto;
    padding-bottom: 8px;
  }
  .nav-menu li {
    flex-shrink: 0;
  }
  .nav-btn {
    white-space: nowrap;
    padding: 8px 12px;
  }
  .nav-menu li:last-child {
    margin-top: 0 !important;
    padding-top: 0 !important;
    border-top: none !important;
    border-left: 1px solid var(--border-color);
    margin-left: 8px;
    padding-left: 8px;
  }
}

@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
