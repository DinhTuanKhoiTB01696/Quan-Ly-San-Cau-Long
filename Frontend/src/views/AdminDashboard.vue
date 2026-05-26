<template>
  <div class="admin-dashboard container">
    <div class="page-header">
      <h2>Quản Trị Hệ Thống</h2>
    </div>

    <div class="admin-tabs">
      <button :class="{ active: currentTab === 'courts' }" @click="currentTab = 'courts'">Quản lý Sân</button>
      <button :class="{ active: currentTab === 'reports' }" @click="currentTab = 'reports'">Báo Cáo</button>
    </div>

    <div v-if="loading" class="text-center p-4">Đang tải dữ liệu...</div>
    <div v-else-if="error" class="text-danger text-center p-4">{{ error }}</div>
    
    <!-- Quản Lý Sân -->
    <div v-else-if="currentTab === 'courts'" class="tab-content">
      <div class="actions-row">
        <h3>Danh sách Sân</h3>
        <button class="btn btn-primary btn-sm" @click="openCreateModal">+ Thêm Sân</button>
      </div>
      
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

    <!-- Quản Lý Báo Cáo -->
    <div v-else-if="currentTab === 'reports'" class="tab-content">
      <h3>Danh sách Báo Cáo Chờ Xử Lý</h3>
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

const currentTab = ref('courts')
const courts = ref([])
const reports = ref([])
const loading = ref(false)
const error = ref(null)

const toast = useToast()

const showCourtModal = ref(false)
const selectedCourt = ref(null)

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

onMounted(() => {
  fetchCourts()
})

watch(currentTab, (newTab) => {
  if (newTab === 'courts' && courts.value.length === 0) fetchCourts()
  if (newTab === 'reports' && reports.value.length === 0) fetchReports()
})

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
</style>
