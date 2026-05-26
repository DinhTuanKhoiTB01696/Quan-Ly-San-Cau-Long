# 🏸 Ghép Kèo Cầu Lông Biên Hòa

Dự án ứng dụng web hỗ trợ tìm sân, tạo kèo và ghép kèo cầu lông tại Biên Hòa.
Ứng dụng được thiết kế theo mô hình **Clean Architecture** (ASP.NET Core 10) và **Vue 3 + Vite** (Frontend).

## 🌟 Tính Năng Nổi Bật
- **Quản lý kèo**: Đăng kèo, lọc kèo theo khu vực/trình độ, tự động ẩn kèo sau khi kết thúc 2 giờ.
- **Xin slot nhanh**: Nút xin slot chuyển hướng trực tiếp sang Zalo của Host.
- **Đánh giá & Báo cáo**: Báo cáo sân sai giá, kèo ảo (tự động khóa kèo nếu bị report 3 lần).
- **Thiết kế Mobile-First**: Tối ưu UI cho thiết bị di động.

## 🏗️ Kiến Trúc Hệ Thống

Dự án chia làm 2 phần độc lập:

1. **Backend** (ASP.NET Core 10 LTS - Clean Architecture)
   - `BadmintonApp.Domain`: Chứa Entities (User, Court, Match, Report, Feedback) và Enums.
   - `BadmintonApp.Application`: Chứa Interfaces và DTOs.
   - `BadmintonApp.Infrastructure`: Triển khai DbContext (EF Core SQL Server) và Services.
   - `BadmintonApp.API`: Chứa Controllers, Setup DI, JWT Auth và Swagger.

2. **Frontend** (Vue 3 + Vite)
   - `components`: Các component dùng chung (NavBar, MatchCard, FilterBar).
   - `views`: Các trang giao diện chính (Home, Login, Register, CreateMatch, MyMatches).
   - `stores`: Quản lý state bằng Pinia (`auth.js`, `matches.js`, `courts.js`).
   - `api`: Cấu hình Axios với JWT interceptors.

## 🚀 Hướng Dẫn Chạy Cục Bộ (Local)

### Yêu Cầu Cài Đặt
- **.NET 10 SDK**
- **Node.js 18+**
- **SQL Server** (Mặc định cấu hình dùng `KHOI\SQLEXPRESS`)

### Cách Khởi Động Nhanh
Dự án có sẵn script `run.bat` ở thư mục gốc để tự động hóa mọi thứ.

1. Clone repository về máy.
2. Click đúp vào file `run.bat`.
3. Khi được hỏi `Ban co muon reset Database va chay Db Migrations + Seed Data khong? (Y/N):`, nhập **Y** trong lần chạy đầu tiên.
   - Script sẽ tự động xóa DB cũ, chạy EF migrations mới, cập nhật database, và chạy file `seed_data.sql` để tạo 20 sân Biên Hòa + tài khoản admin (`admin` / `Admin@123`).
4. Script sẽ tự động bật 2 cửa sổ cmd mới chạy Backend (`http://localhost:5219`) và Frontend (`http://localhost:5173`).
   - Swagger UI: `http://localhost:5219/swagger`

### Tài khoản thử nghiệm
- **Admin**: `admin` / `Admin@123`
- Hoặc bạn có thể tự đăng ký tài khoản mới trên giao diện.

## 🤖 CI/CD (GitHub Actions)
Dự án sử dụng **1 workflow duy nhất** `.github/workflows/ci.yml` để tự động kiểm tra cả Backend và Frontend:
- Chạy khi **push** hoặc **pull request** vào `main` / `develop`
- **2 jobs song song**: Backend (.NET 10 build) + Frontend (Vue 3 build)
- Cả 2 jobs phải **PASS** thì mới được merge PR

### 🔒 Cách bật Branch Protection (bắt buộc CI pass trước khi merge)
1. Vào **GitHub Repo → Settings → Branches**
2. Nhấn **Add branch protection rule**
3. Branch name pattern: `main`
4. Tick ✅ **Require status checks to pass before merging**
5. Search và chọn: `🔧 Backend (.NET 10)` và `🎨 Frontend (Vue 3)`
6. Nhấn **Create** → Xong!
