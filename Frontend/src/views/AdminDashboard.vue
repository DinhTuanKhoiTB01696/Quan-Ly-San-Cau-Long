<template>
  <div class="admin-dashboard-container container">
    <div class="admin-layout">
      <!-- SIDEBAR -->
      <aside class="admin-sidebar">
        <div class="sidebar-header">
          <div class="pulse-dot"></div>
          <h2>Quản Trị Hệ Thống</h2>
        </div>
        
        <nav class="sidebar-nav">
          <button :class="{ active: currentTab === 'dashboard' }" @click="currentTab = 'dashboard'">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="3" y="3" width="7" height="9"></rect><rect x="14" y="3" width="7" height="5"></rect><rect x="14" y="12" width="7" height="9"></rect><rect x="3" y="16" width="7" height="5"></rect></svg>
            Thống Kê
          </button>
          <button :class="{ active: currentTab === 'users' }" @click="currentTab = 'users'">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path><circle cx="9" cy="7" r="4"></circle><path d="M23 21v-2a4 4 0 0 0-3-3.87"></path><path d="M16 3.13a4 4 0 0 1 0 7.75"></path></svg>
            Người Dùng
          </button>
          <button :class="{ active: currentTab === 'matches' }" @click="currentTab = 'matches'">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><path d="m4.93 4.93 4.24 4.24"></path><path d="m14.83 9.17 4.24-4.24"></path><path d="m14.83 14.83 4.24 4.24"></path><path d="m9.17 14.83-4.24 4.24"></path></svg>
            Quản lý Kèo
          </button>
          <button :class="{ active: currentTab === 'courts' }" @click="currentTab = 'courts'">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="2" y="3" width="20" height="18" rx="2" ry="2"></rect><line x1="12" y1="3" x2="12" y2="21"></line><line x1="2" y1="12" x2="22" y2="12"></line></svg>
            Quản lý Sân
          </button>
          <button :class="{ active: currentTab === 'reports' }" @click="currentTab = 'reports'">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3Z"></path><line x1="12" y1="9" x2="12" y2="13"></line><line x1="12" y1="17" x2="12.01" y2="17"></line></svg>
            Báo Cáo
          </button>
          <button :class="{ active: currentTab === 'feedback' }" @click="currentTab = 'feedback'">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path></svg>
            Góp Ý
          </button>
          <button :class="{ active: currentTab === 'transactions' }" @click="currentTab = 'transactions'">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="1" x2="12" y2="23"></line><path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"></path></svg>
            Duyệt Nạp Tiền
          </button>
          <button :class="{ active: currentTab === 'joinRequests' }" @click="currentTab = 'joinRequests'">
            <svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path><circle cx="9" cy="7" r="4"></circle><path d="M23 21v-2a4 4 0 0 0-3-3.87"></path><path d="M16 3.13a4 4 0 0 1 0 7.75"></path></svg>
            Duyệt Tham Gia Kèo
          </button>
        </nav>
      </aside>

      <!-- MAIN CONTENT -->
      <main class="admin-main">
        <div v-if="loading" class="text-center p-5 card">
          <div class="spinner"></div>
          <p class="mt-3">Đang tải dữ liệu hệ thống...</p>
        </div>
        <div v-else-if="error" class="text-danger text-center p-5 card">{{ error }}</div>
        
        <div v-else class="admin-view-content">
          <!-- TAB: DASHBOARD -->
          <div v-if="currentTab === 'dashboard'" class="tab-content animate-fade">
            <h2 class="section-title">Tổng quan Hệ Thống</h2>
            
            <div v-if="stats" class="dashboard-grid">
              <div class="stat-card dashboard-glass">
                <div class="stat-card-header">
                  <h3>Người Dùng</h3>
                  <div class="stat-icon u-icon">👥</div>
                </div>
                <div class="stat-value">{{ stats.totalUsers }}</div>
                <div class="stat-desc">+{{ stats.newUsersThisMonth }} thành viên mới tháng này</div>
              </div>
              
              <div class="stat-card dashboard-glass">
                <div class="stat-card-header">
                  <h3>Tổng Kèo Đấu</h3>
                  <div class="stat-icon m-icon">🏸</div>
                </div>
                <div class="stat-value">{{ stats.totalMatches }}</div>
                <div class="stat-desc">Tổng số kèo đã tạo trên web</div>
              </div>
              
              <div class="stat-card dashboard-glass glow-lime">
                <div class="stat-card-header">
                  <h3>Kèo Đang Mở</h3>
                  <div class="stat-icon active-icon">⚡</div>
                </div>
                <div class="stat-value text-primary">{{ stats.openMatches }}</div>
                <div class="stat-desc">Kèo đang mở tuyển người chơi</div>
              </div>
              
              <div class="stat-card dashboard-glass glow-rose">
                <div class="stat-card-header">
                  <h3>Báo Cáo Tồn Đọng</h3>
                  <div class="stat-icon danger-icon">⚠️</div>
                </div>
                <div class="stat-value text-danger">{{ stats.pendingReports }}</div>
                <div class="stat-desc">Khiếu nại cần xử lý ngay</div>
              </div>

              <!-- Top Sân -->
              <div class="stat-panel full-width card">
                <h3 class="panel-title">⭐ Top Sân Được Yêu Thích Nhất</h3>
                <div class="table-responsive">
                  <table class="admin-table">
                    <thead>
                      <tr>
                        <th style="width: 80px">Hạng</th>
                        <th>Tên Sân Cầu Lông</th>
                        <th>Số lượng Kèo đã tổ chức</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="(court, index) in stats.topCourts" :key="court.courtId">
                        <td><span class="rank-badge" :class="'rank-' + (index + 1)">#{{ index + 1 }}</span></td>
                        <td><strong class="text-white">{{ court.courtName }}</strong></td>
                        <td><span class="badge badge-primary">{{ court.matchCount }} kèo đấu</span></td>
                      </tr>
                      <tr v-if="!stats.topCourts || stats.topCourts.length === 0">
                        <td colspan="3" class="text-center p-4">Chưa có dữ liệu thống kê sân</td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
              
              <!-- Biểu đồ -->
              <div class="stat-panel full-width card chart-panel">
                <div class="chart-container">
                  <h3 class="panel-title">📊 Số lượng kèo 7 ngày qua</h3>
                  <div class="chart-wrapper">
                    <Bar v-if="barChartData" :data="barChartData" :options="chartOptions" />
                  </div>
                </div>
                <div class="chart-container">
                  <h3 class="panel-title">🍩 Tỷ lệ trạng thái kèo đấu</h3>
                  <div class="chart-wrapper">
                    <Pie v-if="pieChartData" :data="pieChartData" :options="chartOptions" />
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- TAB: QUẢN LÝ SÂN -->
          <div v-else-if="currentTab === 'courts'" class="tab-content animate-fade card">
            <div class="actions-row">
              <h2 class="section-title">Danh Sách Sân Hoạt Động</h2>
              <button class="btn btn-primary" @click="openCreateModal">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
                Thêm Sân Mới
              </button>
            </div>
            
            <div class="table-responsive mt-3">
              <table class="admin-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Tên Sân</th>
                    <th>Khu Vực</th>
                    <th>Giá Thuê / Giờ</th>
                    <th>Đánh Giá</th>
                    <th style="width: 160px; text-align: center">Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="court in courts" :key="court.id">
                    <td>#{{ court.id }}</td>
                    <td><strong class="text-white">{{ court.name }}</strong></td>
                    <td><span class="badge badge-secondary">{{ areaName(court.area) }}</span></td>
                    <td class="text-primary fw-bold">{{ formatCurrency(court.price) }}</td>
                    <td><span class="text-warning">⭐ {{ court.rating.toFixed(1) }}</span></td>
                    <td class="text-center">
                      <div class="btn-group">
                        <button class="btn btn-outline btn-sm" @click="openEditModal(court)">Sửa</button>
                        <button class="btn btn-danger btn-sm" @click="handleDeleteCourt(court.id)">Xóa</button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- TAB: QUẢN LÝ NGƯỜI DÙNG -->
          <div v-else-if="currentTab === 'users'" class="tab-content animate-fade card">
            <h2 class="section-title">Quản Lý Danh Sách Thành Viên</h2>
            
            <div class="table-responsive">
              <table class="admin-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Tên tài khoản</th>
                    <th>Họ và Tên</th>
                    <th>Số Điện Thoại</th>
                    <th>Quyền Hạn</th>
                    <th>Trạng Thái</th>
                    <th style="width: 140px; text-align: center">Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="user in users" :key="user.id">
                    <td>#{{ user.id }}</td>
                    <td class="text-white"><strong>{{ user.username }}</strong></td>
                    <td>{{ user.fullName }}</td>
                    <td>{{ user.phone || 'Chưa cung cấp' }}</td>
                    <td>
                      <span class="badge" :class="user.role === 'Admin' ? 'badge-danger' : 'badge-primary'">
                        {{ user.role }}
                      </span>
                    </td>
                    <td>
                      <span class="status-indicator" :class="user.isLocked ? 'locked' : 'active'">
                        {{ user.isLocked ? 'Bị Khóa' : 'Đang Hoạt Động' }}
                      </span>
                    </td>
                    <td class="text-center">
                      <button 
                        v-if="user.role !== 'Admin'"
                        class="btn btn-sm" 
                        :class="user.isLocked ? 'btn-primary' : 'btn-danger'" 
                        @click="handleToggleLock(user.id)"
                      >
                        {{ user.isLocked ? 'Mở Khóa' : 'Khóa Acc' }}
                      </button>
                      <span v-else class="text-secondary small">Không thể khóa</span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- TAB: QUẢN LÝ KÈO -->
          <div v-else-if="currentTab === 'matches'" class="tab-content animate-fade card">
            <h2 class="section-title">Danh Sách Tất Cả Kèo Đấu</h2>
            
            <div class="table-responsive">
              <table class="admin-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Trưởng Kèo (Host)</th>
                    <th>Sân Tổ Chức</th>
                    <th>Thời Gian Kèo Đấu</th>
                    <th>Trạng Thái</th>
                    <th style="width: 100px; text-align: center">Hủy/Xóa</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="match in matches" :key="match.id">
                    <td>#{{ match.id }}</td>
                    <td class="text-white"><strong>{{ match.hostName }}</strong></td>
                    <td>{{ match.courtName }}</td>
                    <td>
                      <span class="text-primary fw-bold">
                        {{ new Date(match.date).toLocaleDateString('vi-VN') }}
                      </span> 
                      <span class="text-secondary"> @ {{ match.time }}</span>
                    </td>
                    <td>
                      <span class="badge" :class="'status-' + match.status">
                        {{ getMatchStatusText(match.status) }}
                      </span>
                    </td>
                    <td class="text-center">
                      <button class="btn btn-danger btn-sm" @click="handleDeleteMatch(match.id)">Hủy Kèo</button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- TAB: QUẢN LÝ BÁO CÁO -->
          <div v-else-if="currentTab === 'reports'" class="tab-content animate-fade card">
            <h2 class="section-title">Khiếu Nại & Báo Cáo Từ Người Dùng</h2>
            
            <div class="table-responsive">
              <table class="admin-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Kèo Bị Báo Cáo</th>
                    <th>Lý Do Vi Phạm</th>
                    <th>Mô Tả Chi Tiết</th>
                    <th>Ngày Gửi</th>
                    <th style="width: 120px; text-align: center">Xử lý</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-if="reports.length === 0">
                    <td colspan="6" class="text-center p-4 text-secondary">Tất cả báo cáo vi phạm đã được giải quyết sạch sẽ! 👍</td>
                  </tr>
                  <tr v-for="report in reports" :key="report.id">
                    <td>#{{ report.id }}</td>
                    <td><router-link :to="'/courts/' + report.matchId" class="text-primary">Xem kèo #{{ report.matchId }}</router-link></td>
                    <td><span class="badge badge-danger">{{ reasonText(report.reason) }}</span></td>
                    <td>{{ report.description }}</td>
                    <td>{{ new Date(report.createdAt).toLocaleDateString('vi-VN') }}</td>
                    <td class="text-center">
                      <button class="btn btn-primary btn-sm" @click="handleResolveReport(report.id)">Đã Xử Lý</button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- TAB: XEM GÓP Ý -->
          <div v-else-if="currentTab === 'feedback'" class="tab-content animate-fade card">
            <h2 class="section-title">Danh Sách Ý Kiến Góp Ý Phát Triển Web</h2>
            
            <div class="table-responsive">
              <table class="admin-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Ứng dụng hữu ích?</th>
                    <th>Tính Năng Mong Muốn Thêm</th>
                    <th>Sân Muốn Tích Hợp Thêm</th>
                    <th>Ngày Gửi</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-if="feedbacks.length === 0">
                    <td colspan="5" class="text-center p-4 text-secondary">Chưa có ý kiến góp ý nào được gửi.</td>
                  </tr>
                  <tr v-for="fb in feedbacks" :key="fb.id">
                    <td>#{{ fb.id }}</td>
                    <td>
                      <span class="badge" :class="fb.isHelpful ? 'badge-primary' : 'badge-danger'">
                        {{ fb.isHelpful ? 'Có 👍' : 'Không 👎' }}
                      </span>
                    </td>
                    <td class="text-white">{{ fb.missingFeature || '-' }}</td>
                    <td>{{ fb.wantedCourt || '-' }}</td>
                    <td>{{ new Date(fb.createdAt).toLocaleDateString('vi-VN') }}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- TAB: DUYỆT & LỊCH SỬ NẠP TIỀN -->
          <div v-else-if="currentTab === 'transactions'" class="tab-content animate-fade card">
            <h2 class="section-title">Yêu Cầu Nạp Tiền Chờ Duyệt (Pending)</h2>
            
            <div class="table-responsive mb-5">
              <table class="admin-table">
                <thead>
                  <tr>
                    <th>ID Giao dịch</th>
                    <th>Tên Tài Khoản</th>
                    <th>Số Tiền Nạp</th>
                    <th>Lượt Cộng Thêm</th>
                    <th>Ngày Gửi Yêu Cầu</th>
                    <th style="width: 180px; text-align: center">Hành Động Duyệt</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-if="transactions.length === 0">
                    <td colspan="6" class="text-center p-4 text-secondary">Không có yêu cầu nạp tiền nào đang chờ duyệt. (Đã tự động duyệt xong) 🎉</td>
                  </tr>
                  <tr v-for="trx in transactions" :key="trx.id">
                    <td>#{{ trx.id }}</td>
                    <td><strong class="text-white">{{ trx.username }}</strong></td>
                    <td class="text-primary fw-bold">{{ formatCurrency(trx.amount) }}</td>
                    <td><span class="badge badge-primary">+{{ trx.creditsAdded }} lượt</span></td>
                    <td>{{ new Date(trx.createdAt).toLocaleString('vi-VN') }}</td>
                    <td class="text-center">
                      <div class="btn-group">
                        <button class="btn btn-primary btn-sm btn-success-neon" @click="handleUpdateTransaction(trx.id, 1)">Duyệt</button>
                        <button class="btn btn-danger btn-sm" @click="handleUpdateTransaction(trx.id, 2)">Từ chối</button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Lịch sử giao dịch nạp tiền -->
            <h2 class="section-title mt-5">Lịch Sử Toàn Bộ Giao Dịch Nạp Tiền (History)</h2>
            <div class="table-responsive">
              <table class="admin-table">
                <thead>
                  <tr>
                    <th>ID Giao dịch</th>
                    <th>Tên Tài Khoản</th>
                    <th>Số Tiền Nạp</th>
                    <th>Lượt Cộng Thêm</th>
                    <th>Ngày Gửi Yêu Cầu</th>
                    <th style="width: 150px; text-align: center">Trạng Thái</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-if="transactionHistory.length === 0">
                    <td colspan="6" class="text-center p-4 text-secondary">Chưa có giao dịch lịch sử nào trong hệ thống.</td>
                  </tr>
                  <tr v-for="trx in transactionHistory" :key="trx.id">
                    <td>#{{ trx.id }}</td>
                    <td><strong class="text-white">{{ trx.username }}</strong></td>
                    <td class="text-primary fw-bold">{{ formatCurrency(trx.amount) }}</td>
                    <td><span class="badge badge-primary">+{{ trx.creditsAdded }} lượt</span></td>
                    <td>{{ new Date(trx.createdAt).toLocaleString('vi-VN') }}</td>
                    <td class="text-center">
                      <span class="badge" :class="trx.status === 1 ? 'badge-success-neon' : trx.status === 2 ? 'badge-danger' : 'badge-warning'">
                        {{ trx.status === 1 ? 'Đã duyệt' : trx.status === 2 ? 'Từ chối' : 'Chờ duyệt' }}
                      </span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          <!-- TAB: DUYỆT THAM GIA KÈO -->
          <div v-else-if="currentTab === 'joinRequests'" class="tab-content animate-fade card">
            <h2 class="section-title">Duyệt Yêu Cầu Ký Quỹ Tham Gia Kèo (Chống Bùng)</h2>
            <p class="text-secondary mb-4" style="font-size: 14px;">
              Danh sách các tài khoản chuyển khoản cọc cho Admin để tham gia kèo giao lưu. Vui lòng xác thực số tiền chuyển khoản thực tế khớp với số tiền yêu cầu rồi click Duyệt.
            </p>
            
            <div class="table-responsive">
              <table class="admin-table">
                <thead>
                  <tr>
                    <th>Thông Tin Kèo</th>
                    <th>Người Tham Gia</th>
                    <th>Thông Tin Liên Hệ</th>
                    <th>Số Tiền Cọc</th>
                    <th>Ngày Yêu Cầu</th>
                    <th style="width: 200px; text-align: center">Hành Động</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-if="pendingJoins.length === 0">
                    <td colspan="6" class="text-center p-4 text-secondary">Không có yêu cầu tham gia nào đang chờ duyệt. 🎉</td>
                  </tr>
                  <tr v-for="req in pendingJoins" :key="req.matchId + '-' + req.userId">
                    <td>
                      <strong class="text-white">Kèo #{{ req.matchId }}</strong><br />
                      <span class="text-secondary" style="font-size: 12px;">{{ req.courtName }}</span><br />
                      <span class="badge" style="background: rgba(255,255,255,0.05); color: #ccc; font-size: 10px; padding: 2px 6px; border-radius: 4px; display: inline-block; margin-top: 4px;">{{ formatDate(req.date) }} | {{ formatTime(req.timeStart) }}</span>
                    </td>
                    <td>
                      <strong class="text-white">{{ req.fullName }}</strong><br />
                      <span class="text-secondary" style="font-size: 12px;">@{{ req.username }}</span><br />
                      <span class="badge" style="background: rgba(163, 230, 53, 0.15); color: #a3e635; font-size: 11px; padding: 2px 6px; border-radius: 4px; display: inline-block; margin-top: 4px;">{{ req.skillLevel || 'Trung bình' }}</span>
                    </td>
                    <td>
                      <span class="text-white font-bold">{{ req.phone }}</span><br />
                      <a :href="'https://zalo.me/' + req.phone" target="_blank" style="color: #0068ff; font-size: 12px; text-decoration: underline;">Nhắn Zalo</a>
                    </td>
                    <td class="text-primary fw-bold">{{ formatCurrency(req.cost) }}</td>
                    <td style="font-size: 13px;">{{ new Date(req.joinedAt).toLocaleString('vi-VN') }}</td>
                    <td class="text-center">
                      <div style="display: flex; gap: 8px; justify-content: center;">
                        <button @click="handleApproveJoin(req.matchId, req.userId)" class="btn btn-sm btn-success-neon" style="font-size: 12px; padding: 6px 12px;">Duyệt</button>
                        <button @click="handleRejectJoin(req.matchId, req.userId)" class="btn btn-sm btn-danger" style="font-size: 12px; padding: 6px 12px;">Từ chối</button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </main>
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
const transactionHistory = ref([])
const stats = ref(null)
const loading = ref(false)
const error = ref(null)

const barChartData = ref(null)
const pieChartData = ref(null)

// Modern Sporty Chart Options
const chartOptions = { 
  responsive: true, 
  maintainAspectRatio: false,
  plugins: {
    legend: {
      labels: {
        color: '#94a3b8', // text secondary
        font: {
          family: "'Outfit', sans-serif",
          size: 12
        }
      }
    }
  },
  scales: {
    x: {
      grid: {
        color: 'rgba(255, 255, 255, 0.05)'
      },
      ticks: {
        color: '#94a3b8'
      }
    },
    y: {
      grid: {
        color: 'rgba(255, 255, 255, 0.05)'
      },
      ticks: {
        color: '#94a3b8'
      }
    }
  }
}

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
          backgroundColor: '#a3e635', // Lime Neon color
          borderColor: '#84cc16',
          borderWidth: 1,
          borderRadius: 6,
          data: stats.value.matchCountsByDate.map(m => m.count)
        }]
      }
    }

    pieChartData.value = {
      labels: ['Đang mở', 'Đã full', 'Hết hạn'],
      datasets: [{
        backgroundColor: ['#a3e635', '#475569', '#f43f5e'], // Lime Neon, Cool Grey, Rose Red
        borderWidth: 1,
        borderColor: '#1e293b',
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
    const [pendingRes, historyRes] = await Promise.all([
      api.get('/Transactions/pending'),
      api.get('/Transactions/history')
    ])
    transactions.value = pendingRes.data
    transactionHistory.value = historyRes.data
  } catch (err) {
    error.value = 'Lỗi tải danh sách nạp tiền'
  } finally {
    loading.value = false
  }
}

const pendingJoins = ref([])
const fetchPendingJoins = async () => {
  loading.value = true
  try {
    const res = await api.get('/matches/pending-joins')
    pendingJoins.value = res.data
  } catch (err) {
    error.value = 'Lỗi tải yêu cầu tham gia kèo'
  } finally {
    loading.value = false
  }
}

const handleApproveJoin = async (matchId, userId) => {
  try {
    await api.post(`/matches/${matchId}/approve-join/${userId}`)
    toast.success('Duyệt người tham gia kèo thành công! Lượt đã được xác nhận.')
    fetchPendingJoins()
  } catch (err) {
    toast.error(err.response?.data?.message || 'Duyệt tham gia thất bại')
  }
}

const handleRejectJoin = async (matchId, userId) => {
  if (confirm('Bạn có chắc muốn từ chối và xóa yêu cầu tham gia này?')) {
    try {
      await api.delete(`/matches/${matchId}/reject-join/${userId}`)
      toast.info('Đã từ chối yêu cầu tham gia.')
      fetchPendingJoins()
    } catch (err) {
      toast.error(err.response?.data?.message || 'Từ chối thất bại')
    }
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
  if (newTab === 'joinRequests') fetchPendingJoins()
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
  if (confirm('Bạn chắc chắn muốn hủy kèo đấu này?')) {
    try {
      await api.delete(`/matches/${id}`)
      matches.value = matches.value.filter(m => m.id !== id)
      toast.success('Đã hủy kèo đấu thành công')
    } catch {
      toast.error('Hủy thất bại')
    }
  }
}

const handleDeleteCourt = async (id) => {
  if (confirm('Bạn chắc chắn muốn xóa sân này? Dữ liệu liên quan cũng sẽ bị ảnh hưởng.')) {
    try {
      await api.delete(`/courts/${id}`)
      courts.value = courts.value.filter(c => c.id !== id)
      toast.success('Đã xóa sân thành công')
    } catch {
      toast.error('Xóa sân thất bại')
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
      await api.put(`/courts/${selectedCourt.value.id}`, courtData)
      toast.success('Cập nhật thông tin sân thành công')
    } else {
      await api.post('/courts', courtData)
      toast.success('Thêm sân mới thành công')
    }
    showCourtModal.value = false
    fetchCourts()
  } catch (err) {
    toast.error(err.response?.data?.message || 'Lưu thất bại')
  }
}

const handleResolveReport = async (id) => {
  try {
    await api.put(`/reports/${id}/resolve`)
    reports.value = reports.value.filter(r => r.id !== id)
    toast.success('Đã xác nhận xử lý khiếu nại')
  } catch {
    toast.error('Lỗi xử lý')
  }
}

const handleUpdateTransaction = async (id, status) => {
  if (!confirm(status === 1 ? 'Bạn xác nhận duyệt nạp tiền này?' : 'Bạn muốn từ chối yêu cầu nạp tiền này?')) return
  try {
    await api.put(`/Transactions/${id}/status`, status, {
      headers: { 'Content-Type': 'application/json' }
    })
    toast.success('Cập nhật giao dịch thành công')
    fetchTransactions() // Tự động load lại cả pending và history
  } catch (err) {
    toast.error('Duyệt giao dịch thất bại')
  }
}

const formatCurrency = (val) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val)

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  const date = new Date(dateStr)
  return date.toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

const formatTime = (timeStr) => {
  if (!timeStr) return ''
  const parts = timeStr.split(':')
  return `${parts[0]}:${parts[1]}`
}

const areaName = (areaVal) => {
  const map = { 1: 'Tân Mai', 2: 'Trảng Dài', 3: 'Long Bình', 4: 'Tân Hiệp', 5: 'Hố Nai' }
  return map[areaVal] || 'Khác'
}

const reasonText = (reasonVal) => {
  const map = { 1: 'Kèo Ảo', 2: 'Sai Giá', 3: 'Sai Giờ', 4: 'Khác' }
  return map[reasonVal] || 'Khác'
}

const getMatchStatusText = (status) => {
  const map = { 1: 'Đang tuyển', 2: 'Đã Full', 3: 'Đã hết hạn' }
  return map[status] || 'Không rõ'
}
</script>

<style scoped>
.admin-dashboard-container {
  max-width: 100% !important;
  padding: var(--spacing-lg) !important;
}

.admin-layout {
  display: grid;
  grid-template-columns: 280px 1fr;
  gap: 30px;
  min-height: 80vh;
}

/* SIDEBAR STYLES */
.admin-sidebar {
  background: rgba(15, 23, 42, 0.6);
  border: 1px solid var(--border-color);
  border-radius: var(--border-radius);
  padding: var(--spacing-lg) var(--spacing-md);
  height: fit-content;
  backdrop-filter: blur(12px);
  position: sticky;
  top: 90px;
}

.sidebar-header {
  display: flex;
  align-items: center;
  gap: 10px;
  padding-bottom: var(--spacing-lg);
  border-bottom: 1px solid var(--border-color);
  margin-bottom: var(--spacing-lg);
}

.sidebar-header h2 {
  font-size: 18px;
  font-weight: 800;
  color: var(--text-primary);
  text-shadow: 0 0 10px rgba(163, 230, 53, 0.2);
}

.pulse-dot {
  width: 10px;
  height: 10px;
  background-color: var(--primary-color);
  border-radius: 50%;
  box-shadow: 0 0 10px var(--primary-color);
  animation: pulse-green 2s infinite;
}

.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.sidebar-nav button {
  display: flex;
  align-items: center;
  gap: 12px;
  background: transparent;
  border: 1px solid transparent;
  padding: 14px 18px;
  border-radius: 12px;
  cursor: pointer;
  font-size: 15px;
  font-weight: 600;
  color: var(--text-secondary);
  text-align: left;
  transition: all 0.3s ease;
  font-family: inherit;
}

.sidebar-nav button svg {
  transition: transform 0.3s ease;
}

.sidebar-nav button:hover {
  background: rgba(163, 230, 53, 0.05);
  color: var(--primary-color);
  border-color: rgba(163, 230, 53, 0.1);
  padding-left: 22px;
}

.sidebar-nav button:hover svg {
  transform: scale(1.1);
}

.sidebar-nav button.active {
  background: linear-gradient(135deg, rgba(163, 230, 53, 0.15) 0%, rgba(132, 204, 22, 0.05) 100%);
  border-color: var(--primary-color);
  color: var(--primary-color);
  box-shadow: var(--shadow-neon);
}

/* MAIN CONTENT */
.admin-main {
  width: 100%;
}

.tab-content {
  width: 100%;
}

.section-title {
  font-size: 24px;
  font-weight: 800;
  margin-bottom: var(--spacing-lg);
  color: var(--text-primary);
  border-left: 4px solid var(--primary-color);
  padding-left: 12px;
}

/* DASHBOARD STATS */
.dashboard-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 20px;
}

.stat-card {
  padding: 24px;
  border-radius: var(--border-radius);
  text-align: left;
  position: relative;
  overflow: hidden;
}

.dashboard-glass {
  background: rgba(15, 23, 42, 0.6);
  border: 1px solid rgba(255, 255, 255, 0.05);
  backdrop-filter: blur(12px);
  box-shadow: var(--shadow-sm);
  transition: all 0.3s ease;
}

.dashboard-glass:hover {
  transform: translateY(-4px);
  border-color: rgba(255, 255, 255, 0.15);
  box-shadow: var(--shadow-md);
}

.glow-lime {
  border-color: rgba(163, 230, 53, 0.2);
}
.glow-lime:hover {
  border-color: var(--primary-color);
  box-shadow: var(--shadow-neon);
}

.glow-rose {
  border-color: rgba(244, 63, 94, 0.2);
}
.glow-rose:hover {
  border-color: var(--danger-color);
  box-shadow: 0 0 15px rgba(244, 63, 94, 0.2);
}

.stat-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.stat-card h3 {
  font-size: 14px;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: var(--text-secondary);
  margin: 0;
}

.stat-icon {
  font-size: 20px;
  opacity: 0.8;
}

.stat-value {
  font-size: 38px;
  font-weight: 800;
  margin-bottom: 6px;
  letter-spacing: -0.03em;
}

.stat-desc {
  font-size: 13px;
  color: var(--text-secondary);
}

.stat-panel {
  margin-top: 24px;
}

.panel-title {
  font-size: 18px;
  font-weight: 700;
  margin-bottom: 20px;
}

.full-width {
  grid-column: 1 / -1;
}

/* CHART PANEL */
.chart-panel {
  display: grid;
  grid-template-columns: 3fr 2fr;
  gap: 24px;
  background: rgba(15, 23, 42, 0.5) !important;
}

.chart-container {
  display: flex;
  flex-direction: column;
}

.chart-wrapper {
  height: 320px;
  position: relative;
  width: 100%;
}

/* TABLES STYLING */
.admin-table {
  width: 100%;
  border-collapse: collapse;
  background: transparent;
}

.admin-table th, .admin-table td {
  padding: 16px 20px;
  text-align: left;
  border-bottom: 1px solid rgba(255, 255, 255, 0.05);
  font-size: 14px;
}

.admin-table th {
  background: rgba(30, 41, 59, 0.4);
  font-weight: 700;
  color: var(--text-secondary);
  text-transform: uppercase;
  font-size: 12px;
  letter-spacing: 0.05em;
}

.admin-table tr {
  transition: background-color 0.2s ease;
}

.admin-table tr:hover {
  background: rgba(255, 255, 255, 0.02);
}

/* BADGES */
.badge {
  display: inline-flex;
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 11px;
  font-weight: 700;
  text-transform: uppercase;
}

.badge-primary {
  background: rgba(163, 230, 53, 0.15);
  color: var(--primary-color);
  border: 1px solid rgba(163, 230, 53, 0.25);
}

.badge-secondary {
  background: rgba(148, 163, 184, 0.15);
  color: var(--text-secondary);
}

.badge-success-neon {
  background: rgba(16, 185, 129, 0.15);
  color: #10b981;
  border: 1px solid rgba(16, 185, 129, 0.25);
}

.badge-warning {
  background: rgba(245, 158, 11, 0.15);
  color: #f59e0b;
  border: 1px solid rgba(245, 158, 11, 0.25);
}

.badge-danger {
  background: rgba(244, 63, 94, 0.15);
  color: var(--danger-color);
  border: 1px solid rgba(244, 63, 94, 0.25);
}

.rank-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  font-weight: 800;
  font-size: 12px;
}

.rank-1 { background: #f59e0b; color: #0b0f19; box-shadow: 0 0 10px rgba(245, 158, 11, 0.5); }
.rank-2 { background: #94a3b8; color: #0b0f19; }
.rank-3 { background: #b45309; color: white; }

/* STATUS INDICATORS */
.status-indicator {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  font-weight: 600;
}
.status-indicator::before {
  content: '';
  width: 8px;
  height: 8px;
  border-radius: 50%;
}
.status-indicator.active { color: var(--success-color); }
.status-indicator.active::before { background-color: var(--success-color); box-shadow: 0 0 8px var(--success-color); }

.status-indicator.locked { color: var(--danger-color); }
.status-indicator.locked::before { background-color: var(--danger-color); }

/* MATCH STATUS BADGES */
.status-1 {
  background: rgba(163, 230, 53, 0.15);
  color: var(--primary-color);
}
.status-2 {
  background: rgba(148, 163, 184, 0.15);
  color: var(--text-secondary);
}
.status-3 {
  background: rgba(244, 63, 94, 0.15);
  color: var(--danger-color);
}

/* DUYỆT NẠP TIỀN CUSTOM BUTTON */
.btn-success-neon {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  color: white;
  box-shadow: 0 4px 10px rgba(16, 185, 129, 0.3);
}
.btn-success-neon:hover {
  transform: translateY(-1px);
  box-shadow: 0 0 15px rgba(16, 185, 129, 0.5);
}

/* SPINNER & ANIMATIONS */
.spinner {
  width: 40px;
  height: 40px;
  border: 4px solid rgba(163, 230, 53, 0.1);
  border-top-color: var(--primary-color);
  border-radius: 50%;
  animation: spin 1s infinite linear;
  margin: 0 auto;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes pulse-green {
  0% { box-shadow: 0 0 0 0 rgba(163, 230, 53, 0.6); }
  70% { box-shadow: 0 0 0 8px rgba(163, 230, 53, 0); }
  100% { box-shadow: 0 0 0 0 rgba(163, 230, 53, 0); }
}

.animate-fade {
  animation: fadeIn 0.4s ease forwards;
}

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(10px); }
  to { opacity: 1; transform: translateY(0); }
}

/* ACTIONS ROW */
.actions-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.btn-group {
  display: inline-flex;
  gap: 6px;
}

.btn-sm {
  padding: 6px 12px;
  font-size: 13px;
  border-radius: 6px;
}

/* RESPONSIVE STYLES */
@media (max-width: 991px) {
  .admin-layout {
    grid-template-columns: 1fr;
  }

  .admin-sidebar {
    position: relative;
    top: 0;
    overflow-x: auto;
    white-space: nowrap;
    -webkit-overflow-scrolling: touch;
    padding: 16px;
  }

  .sidebar-header {
    display: none;
  }

  .sidebar-nav {
    flex-direction: row;
    overflow-x: auto;
    padding-bottom: 4px;
  }

  .sidebar-nav button {
    padding: 10px 16px;
    font-size: 14px;
  }
}

@media (max-width: 768px) {
  .chart-panel {
    grid-template-columns: 1fr !important;
  }
  
  .actions-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 12px;
  }
}
</style>
