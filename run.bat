@echo off
chcp 65001 >nul
title Ghep Keo Cau Long Bien Hoa

:: Luu duong dan root cua project
set "ROOT=%~dp0"

echo.
echo =====================================================
echo    GHEP KEO CAU LONG BIEN HOA - HE THONG KHOI DONG
echo =====================================================
echo.

:: ---- Hoi reset DB ----
set /p resetDB="Ban co muon reset Database va seed data khong? (Y/N): "
if /I NOT "%resetDB%"=="Y" goto SKIP_DB

echo.
echo [1/5] Drop Database cu...
dotnet ef database drop -f --project "%ROOT%Backend\src\BadmintonApp.Infrastructure" --startup-project "%ROOT%Backend\src\BadmintonApp.API"
echo.

echo [2/5] Xoa cac migration cu...
if exist "%ROOT%Backend\src\BadmintonApp.Infrastructure\Migrations" (
    rd /s /q "%ROOT%Backend\src\BadmintonApp.Infrastructure\Migrations"
    echo        Da xoa folder Migrations.
) else (
    echo        Khong co migration cu.
)
echo.

echo [3/5] Tao migration moi 'InitialCreate'...
dotnet ef migrations add InitialCreate --project "%ROOT%Backend\src\BadmintonApp.Infrastructure" --startup-project "%ROOT%Backend\src\BadmintonApp.API"
if errorlevel 1 (
    echo [LOI] Tao migration that bai!
    pause
    exit /b 1
)
echo.

echo [4/5] Cap nhat Database...
dotnet ef database update --project "%ROOT%Backend\src\BadmintonApp.Infrastructure" --startup-project "%ROOT%Backend\src\BadmintonApp.API"
if errorlevel 1 (
    echo [LOI] Cap nhat database that bai!
    pause
    exit /b 1
)
echo.

echo [5/5] Seed Data - 20 san cau long + tai khoan admin se duoc tu dong them boi EF Core khi khoi dong...

echo.
echo =====================================================
echo    RESET DATABASE HOAN TAT
echo =====================================================

:SKIP_DB

:: ---- Kiem tra Frontend dependencies ----
echo.
echo --- KIEM TRA FRONTEND DEPENDENCIES ---
if not exist "%ROOT%Frontend\node_modules" (
    echo    Chua co node_modules, dang cai dat...
    pushd "%ROOT%Frontend"
    call npm install
    popd
    echo    Cai dat xong!
) else (
    echo    node_modules da ton tai, bo qua.
)

:: ---- Khoi dong Backend ----
echo.
echo --- KHOI DONG BACKEND ---
echo    URL: http://localhost:5219
echo    Swagger: http://localhost:5219/swagger
start "Backend API" cmd /k "cd /d "%ROOT%Backend\src\BadmintonApp.API" && dotnet watch run"

:: ---- Khoi dong Frontend ----
echo.
echo --- KHOI DONG FRONTEND ---
echo    URL: http://localhost:5173
start "Frontend Vue" cmd /k "cd /d "%ROOT%Frontend" && npm run dev"

:: ---- Thong tin ----
echo.
echo =====================================================
echo    HE THONG DANG CHAY!
echo =====================================================
echo.
echo    Backend API : http://localhost:5219
echo    Swagger     : http://localhost:5219/swagger
echo    Frontend    : http://localhost:5173
echo.
echo    Admin login : admin / Admin@123
echo.
echo    Nhan phim bat ky de dong cua so nay...
echo    (Backend va Frontend van chay trong cac cua so rieng)
echo =====================================================
pause
