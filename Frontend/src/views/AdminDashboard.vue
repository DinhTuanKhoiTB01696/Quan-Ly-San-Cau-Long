<template>
  <div class="admin-dashboard container">
    <div class="page-header">
      <h2>Quản Trị Hệ Thống</h2>
    </div>

    <div class="admin-tabs">
      <button :class="{ active: currentTab === 'dashboard' }" @click="currentTab = 'dashboard'">Thống Kê</button>
      <button :class="{ active: currentTab === 'users' }" @click="currentTab = 'users'">Người Dùng</button>
      <button :class="{ active: currentTab === 'matches' }" @click="currentTab = 'matches'">Quản lý Kèo</button>
      <button :class="{ active: currentTab === 'courts' }" @click="currentTab = 'courts'">Quản lý Sân</button>
      <button :class="{ active: currentTab === 'reports' }" @click="currentTab = 'reports'">Báo Cáo</button>
      <button :class="{ active: currentTab === 'feedback' }" @click="currentTab = 'feedback'">Góp Ý</button>
      <button :class="{ active: currentTab === 'transactions' }" @click="currentTab = 'transactions'">Duyệt Nạp Tiền</button>
    </div>

    <div v-if="loading" class="text-center p-4">Đang tải dữ liệu...</div>
    <div v-else-if="error" class="text-danger text-center p-4">{{ error }}</div>
    
    <!-- TAB: DASHBOARD -->
    <div v-else-if="currentTab === 'dashboard'" class="tab-content">
      <div v-if="stats" class="dashboard-grid">
        <!-- Tổng quan -->
        <div class="stat-card">
          <h3>Người Dùng</h3>
          <div class="stat-value">{{ stats.totalUsers }}</div>
          <div class="stat-desc">+{{ stats.newUsersThisMonth }} trong tháng này</div>
        </div>
        <div class="stat-card">
          <h3>Tổng Kèo</h3>
          <div class="stat-value">{{ stats.totalMatches }}</div>
          <div class="stat-desc">Đã tạo trên hệ thống</div>
        </div>
        <div class="stat-card">
          <h3>Kèo Đang Mở</h3>
          <div class="stat-value text-primary">{{ stats.openMatches }}</div>
          <div class="stat-desc">Cần tuyển thêm người</div>
        </div>
        <div class="stat-card">
          <h3>Báo Cáo Tồn Đọng</h3>
          <div class="stat-value text-danger">{{ stats.pendingReports }}</div>
          <div class="stat-desc">Cần xử lý ngay</div>
        </div>

        <!-- Chi tiết -->
        <div class="stat-panel full-width">
          <h3>Top Sân Được Yêu Thích Nhất</h3>
          <div class="table-responsive">
            <table class="admin-table">
              <thead>
                <tr>
                  <th>Top</th>
                  <th>Tên Sân</th>
                  <th>Số lượng Kèo đã tạo</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="(court, index) in stats.topCourts" :key="court.courtId">
                  <td>#{{ index + 1 }}</td>
                  <td>{{ court.courtName }}</td>
                  <td><strong>{{ court.matchCount }}</strong> kèo</td>
                </tr>
                <tr v-if="!stats.topCourts || stats.topCourts.length === 0">
                  <td colspan="3" class="text-center">Chưa có dữ liệu</td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
        
        <!-- Biểu đồ -->
        <div class="stat-panel full-width" style="display: grid; grid-template-columns: 2fr 1fr; gap: 24px;">
          <div>
            <h3>Số lượng kèo 7 ngày qua</h3>
            <div style="height: 300px;">
              <Bar v-if="barChartData" :data="barChartData" :options="chartOptions" />
            </div>
          </div>
          <div>
            <h3>Tỷ lệ trạng thái kèo</h3>
            <div style="height: 300px;">
              <Pie v-if="pieChartData" :data="pieChartData" :options="chartOptions" />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Quản Lý Sân -->
    <div v-else-if="currentTab === 'courts'" class="tab-content">
      <div class="actions-row">
        <h3>Danh sách Sân</h3>
        <button class="btn btn-primary btn-sm" @click="openCreateModal">+ Thêm Sân</button>
      </div>
      
      <div class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Tên Sân</th>
              <th>Khu Vực</th>
              <th>Giá/Giờ</th>
              <th>Đánh giá</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="court in courts" :key="court.id">
              <td>{{ court.id }}</td>
              <td>{{ court.name }}</td>
              <td>{{ areaName(court.area) }}</td>
              <td>{{ formatCurrency(court.price) }}</td>
              <td>⭐ {{ court.rating }}</td>
              <td>
                <button class="btn btn-outline btn-sm" @click="openEditModal(court)">Sửa</button>
                <button class="btn btn-danger btn-sm" style="margin-left:4px" @click="handleDeleteCourt(court.id)">Xóa</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Quản Lý Người Dùng -->
    <div v-else-if="currentTab === 'users'" class="tab-content">
      <h3>Danh sách Người Dùng</h3>
      <div class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Tên đăng nhập</th>
              <th>Họ và Tên</th>
              <th>SĐT</th>
              <th>Quyền</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="user in users" :key="user.id">
              <td>{{ user.id }}</td>
              <td>{{ user.username }}</td>
              <td>{{ user.fullName }}</td>
              <td>{{ user.phone || 'N/A' }}</td>
              <td>{{ user.role }}</td>
              <td>
                <span :class="user.isLocked ? 'text-danger' : 'text-success'">
                  {{ user.isLocked ? 'Bị Khóa' : 'Hoạt động' }}
                </span>
              </td>
              <td>
                <button 
                  v-if="user.role !== 'Admin'"
                  class="btn btn-sm" 
                  :class="user.isLocked ? 'btn-primary' : 'btn-danger'" 
                  @click="handleToggleLock(user.id)"
                >
                  {{ user.isLocked ? 'Mở Khóa' : 'Khóa' }}
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Quản Lý Kèo -->
    <div v-else-if="currentTab === 'matches'" class="tab-content">
      <h3>Tất Cả Các Kèo</h3>
      <div class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Host</th>
              <th>Sân</th>
              <th>Ngày & Giờ</th>
              <th>Trạng thái</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="match in matches" :key="match.id">
              <td>{{ match.id }}</td>
              <td>{{ match.hostName }}</td>
              <td>{{ match.courtName }}</td>
              <td>{{ new Date(match.date).toLocaleDateString('vi-VN') }} {{ match.time }}</td>
              <td>{{ getMatchStatusText(match.status) }}</td>
              <td>
                <button class="btn btn-danger btn-sm" @click="handleDeleteMatch(match.id)">Xóa</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Quản Lý Báo Cáo -->
    <div v-else-if="currentTab === 'reports'" class="tab-content">
      <h3>Danh sách Báo Cáo Chờ Xử Lý</h3>
      <div class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Match ID</th>
              <th>Lý do</th>
              <th>Mô tả</th>
              <th>Ngày gửi</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="reports.length === 0">
              <td colspan="6" class="text-center">Không có báo cáo nào.</td>
            </tr>
            <tr v-for="report in reports" :key="report.id">
              <td>{{ report.id }}</td>
              <td>{{ report.matchId }}</td>
              <td>{{ reasonText(report.reason) }}</td>
              <td>{{ report.description }}</td>
              <td>{{ new Date(report.createdAt).toLocaleDateString('vi-VN') }}</td>
              <td>
                <button class="btn btn-primary btn-sm" @click="handleResolveReport(report.id)">Đã Xử Lý</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Xem Góp Ý -->
    <div v-else-if="currentTab === 'feedback'" class="tab-content">
      <h3>Danh sách Góp Ý</h3>
      <div class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Hữu ích?</th>
              <th>Tính năng mong muốn</th>
              <th>Sân mong muốn</th>
              <th>Ngày gửi</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="feedbacks.length === 0">
              <td colspan="5" class="text-center">Chưa có góp ý nào.</td>
            </tr>
            <tr v-for="fb in feedbacks" :key="fb.id">
              <td>{{ fb.id }}</td>
              <td>{{ fb.isHelpful ? 'Có 👍' : 'Không 👎' }}</td>
              <td>{{ fb.missingFeature || '-' }}</td>
              <td>{{ fb.wantedCourt || '-' }}</td>
              <td>{{ new Date(fb.createdAt).toLocaleDateString('vi-VN') }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Duyệt Nạp Tiền -->
    <div v-else-if="currentTab === 'transactions'" class="tab-content">
      <h3>Danh sách Nạp Tiền (Chờ duyệt)</h3>
      <div class="table-responsive">
        <table class="admin-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Tên đăng nhập</th>
              <th>Số tiền</th>
              <th>Lượt cộng thêm</th>
              <th>Ngày gửi</th>
              <th>Thao tác</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="transactions.length === 0">
              <td colspan="6" class="text-center">Không có yêu cầu nạp tiền nào.</td>
            </tr>
            <tr v-for="trx in transactions" :key="trx.id">
              <td>{{ trx.id }}</td>
              <td><strong>{{ trx.username }}</strong></td>
              <td class="text-primary fw-bold">{{ formatCurrency(trx.amount) }}</td>
              <td>+{{ trx.creditsAdded }} lượt</td>
              <td>{{ new Date(trx.createdAt).toLocaleString('vi-VN') }}</td>
              <td>
                <button class="btn btn-success btn-sm" @click="handleUpdateTransaction(trx.id, 1)">Duyệt</button>
                <button class="btn btn-danger btn-sm" style="margin-left:4px" @click="handleUpdateTransaction(trx.id, 2)">Từ chối</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal Form Sân -->
    <CourtModal 
      :show="showCourtModal"
      :court-data="selectedCourt"
      @close="showCourtModal = false"
      @save="handleSaveCourt"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, watch } from 'vue'
import api from '@/api/axios'
import CourtModal from '@/components/CourtModal.vue'
import { useToast } from 'vue-toastification'
import { Bar, Pie } from 'vue-chartjs'
import { Chart as ChartJS, Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, ArcElement } from 'chart.js'

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale, ArcElement)

const currentTab = ref('dashboard')
const users = ref([])
const matches = ref([])
const courts = ref([])
const reports = ref([])
const feedbacks = ref([])
const transactions = ref([])
const stats = ref(null)
const loading = ref(false)
const error = ref(null)

const barChartData = ref(null)
const pieChartData = ref(null)
const chartOptions = { responsive: true, maintainAspectRatio: false }

const toast = useToast()

const showCourtModal = ref(false)
const selectedCourt = ref(null)

const fetchStats = async () => {
  loading.value = true
  try {
    const res = await api.get('/stats/dashboard')
    stats.value = res.data
    
    if (stats.value.matchCountsByDate) {
      barChartData.value = {
        labels: stats.value.matchCountsByDate.map(m => m.date),
        datasets: [{
          label: 'Số kèo tạo mới',
          backgroundColor: '#3b82f6',
          data: stats.value.matchCountsByDate.map(m => m.count)
        }]
      }
    }

    pieChartData.value = {
      labels: ['Đang mở', 'Đã full', 'Hết hạn'],
      datasets: [{
        backgroundColor: ['#3b82f6', '#64748b', '#ef4444'],
        data: [stats.value.openMatches, stats.value.fullMatches, stats.value.expiredMatches]
      }]
    }
  } catch (err) {
    error.value = 'Lỗi tải thống kê'
  } finally {
    loading.value = false
  }
}

const fetchCourts = async () => {
  loading.value = true
  try {
    const res = await api.get('/courts')
    courts.value = res.data
  } catch (err) {
    error.value = 'Lỗi tải sân'
  } finally {
    loading.value = false
  }
}

const fetchReports = async () => {
  loading.value = true
  try {
    const res = await api.get('/reports')
    reports.value = res.data
  } catch (err) {
    error.value = 'Lỗi tải báo cáo'
  } finally {
    loading.value = false
  }
}

const fetchUsers = async () => {
  loading.value = true
  try {
    const res = await api.get('/users')
    users.value = res.data
  } catch (err) {
    error.value = 'Lỗi tải người dùng'
  } finally {
    loading.value = false
  }
}

const fetchMatches = async () => {
  loading.value = true
  try {
    const res = await api.get('/matches')
    matches.value = res.data
  } catch (err) {
    error.value = 'Lỗi tải danh sách kèo'
  } finally {
    loading.value = false
  }
}

const fetchFeedbacks = async () => {
  loading.value = true
  try {
    const res = await api.get('/feedback')
    feedbacks.value = res.data
  } catch (err) {
    error.value = 'Lỗi tải danh sách góp ý'
  } finally {
    loading.value = false
  }
}

const fetchTransactions = async () => {
  loading.value = true
  try {
    const res = await api.get('/Transactions/pending')
    transactions.value = res.data
  } catch (err) {
    error.value = 'Lỗi tải danh sách nạp tiền'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  fetchStats()
  fetchCourts()
  fetchReports()
})

watch(currentTab, (newTab) => {
  if (newTab === 'users' && users.value.length === 0) fetchUsers()
  if (newTab === 'matches' && matches.value.length === 0) fetchMatches()
  if (newTab === 'courts' && courts.value.length === 0) fetchCourts()
  if (newTab === 'reports' && reports.value.length === 0) fetchReports()
  if (newTab === 'feedback' && feedbacks.value.length === 0) fetchFeedbacks()
  if (newTab === 'transactions' && transactions.value.length === 0) fetchTransactions()
  if (newTab === 'dashboard' && !stats.value) fetchStats()
})

const handleToggleLock = async (id) => {
  try {
    await api.put(`/users/${id}/lock`)
    const user = users.value.find(u => u.id === id)
    if (user) user.isLocked = !user.isLocked
    toast.success('Đã thay đổi trạng thái khóa')
  } catch (err) {
    toast.error(err.response?.data?.message || 'Có lỗi xảy ra')
  }
}

const handleDeleteMatch = async (id) => {
  if (confirm('Bạn chắc chắn muốn xóa kèo này?')) {
    try {
      await api.delete(`/matches/${id}`)
      matches.value = matches.value.filter(m => m.id !== id)
      toast.success('Đã xóa kèo')
    } catch {
      toast.error('Xóa thất bại')
    }
  }
}

const handleDeleteCourt = async (id) => {
  if (confirm('Bạn chắc chắn muốn xóa sân này?')) {
    try {
      await api.delete(`/courts/${id}`)
      courts.value = courts.value.filter(c => c.id !== id)
      toast.success('Đã xóa sân')
    } catch {
      toast.error('Xóa thất bại')
    }
  }
}

const openCreateModal = () => {
  selectedCourt.value = null
  showCourtModal.value = true
}

const openEditModal = (court) => {
  selectedCourt.value = { ...court }
  showCourtModal.value = true
}

const handleSaveCourt = async (courtData) => {
  try {
    if (selectedCourt.value) {
      // Edit
      await api.put(`/courts/${selectedCourt.value.id}`, courtData)
      toast.success('Cập nhật thành công')
    } else {
      // Create
      await api.post('/courts', courtData)
      toast.success('Tạo mới thành công')
    }
    showCourtModal.value = false
    fetchCourts() // reload
  } catch (err) {
    toast.error(err.response?.data?.message || 'Lưu thất bại')
  }
}

const handleResolveReport = async (id) => {
  try {
    await api.put(`/reports/${id}/resolve`)
    reports.value = reports.value.filter(r => r.id !== id)
    toast.success('Đã đánh dấu xử lý')
  } catch {
    toast.error('Lỗi xử lý')
  }
}

const handleUpdateTransaction = async (id, status) => {
  if (!confirm(status === 1 ? 'Bạn xác nhận duyệt nạp tiền này?' : 'Bạn muốn từ chối yêu cầu này?')) return
  try {
    await api.put(`/Transactions/${id}/status`, status, {
      headers: { 'Content-Type': 'application/json' }
    })
    transactions.value = transactions.value.filter(t => t.id !== id)
    toast.success('Đã cập nhật trạng thái')
  } catch (err) {
    toast.error('Cập nhật thất bại')
  }
}

// Utils
const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)

const areaName = (areaVal) => {
  const map = { 1: 'Tân Mai', 2: 'Trảng Dài', 3: 'Long Bình', 4: 'Tân Hiệp', 5: 'Hố Nai' }
  return map[areaVal] || 'Khác'
}

const reasonText = (reasonVal) => {
  const map = { 1: 'Kèo Ảo', 2: 'Sai Giá', 3: 'Sai Giờ', 4: 'Khác' }
  return map[reasonVal] || 'Khác'
}

const getMatchStatusText = (status) => {
  const map = { 1: 'Đang mở', 2: 'Đã full', 3: 'Hết hạn' }
  return map[status] || 'Không xác định'
}
</script>

<style scoped>
.admin-dashboard {
  margin-top: 24px;
}
.admin-tabs {
  display: flex;
  gap: 8px;
  margin-bottom: 24px;
  border-bottom: 1px solid var(--border-color);
}
.admin-tabs button {
  background: none;
  border: none;
  padding: 12px 24px;
  cursor: pointer;
  font-size: 15px;
  font-weight: 500;
  color: var(--text-secondary);
  border-bottom: 2px solid transparent;
}
.admin-tabs button.active {
  color: var(--primary-color);
  border-bottom-color: var(--primary-color);
}
.admin-tabs button:hover {
  background: #f1f5f9;
}
.tab-content {
  background: white;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 4px 6px -1px rgba(0,0,0,0.1);
}

/* Dashboard Styles */
.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}
.stat-card {
  background: #f8fafc;
  padding: 20px;
  border-radius: 12px;
  border: 1px solid var(--border-color);
  text-align: center;
}
.stat-card h3 {
  font-size: 14px;
  color: var(--text-secondary);
  margin: 0 0 8px 0;
}
.stat-value {
  font-size: 32px;
  font-weight: bold;
  margin-bottom: 4px;
}
.stat-desc {
  font-size: 13px;
  color: #64748b;
}
.text-primary { color: var(--primary-color); }
.text-danger { color: var(--danger-color); }

.stat-panel {
  background: #f8fafc;
  padding: 24px;
  border-radius: 12px;
  border: 1px solid var(--border-color);
  margin-top: 20px;
}
.full-width {
  grid-column: 1 / -1;
}
.stat-panel h3 {
  margin-top: 0;
  margin-bottom: 16px;
  font-size: 16px;
}
.ratio-bar {
  display: flex;
  height: 40px;
  border-radius: 20px;
  overflow: hidden;
  color: white;
  font-weight: bold;
  font-size: 13px;
  text-align: center;
  line-height: 40px;
}
.ratio-item.open { background: var(--primary-color); }
.ratio-item.full { background: #64748b; }
.ratio-item.expired { background: var(--danger-color); }

.actions-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}
.admin-table {
  width: 100%;
  border-collapse: collapse;
  background: white;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
}
.admin-table th, .admin-table td {
  padding: 12px 16px;
  text-align: left;
  border-bottom: 1px solid var(--border-color);
  font-size: 14px;
}
.admin-table th {
  background: var(--secondary-color);
  font-weight: 600;
  color: var(--text-secondary);
}
.text-center { text-align: center; }
.btn-danger {
  background: var(--danger-color);
  color: white;
  border: none;
}
.text-success { color: #10b981; font-weight: 600; }
.table-responsive {
  width: 100%;
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
}
@media (max-width: 768px) {
  .admin-tabs {
    flex-wrap: wrap;
    justify-content: center;
  }
  .stat-panel[style] {
    grid-template-columns: 1fr !important;
  }
}
</style>
