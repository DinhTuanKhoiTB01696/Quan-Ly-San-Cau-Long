@echo off
chcp 65001 >nul
title 🏸 Ghép Kèo Cầu Lông Biên Hòa

echo.
echo =====================================================
echo    GHEP KEO CAU LONG BIEN HOA - HE THONG KHOI DONG
echo =====================================================
echo.

:: ---- Hỏi reset DB ----
set /p resetDB="Ban co muon reset Database va seed data khong? (Y/N): "
if /I "%resetDB%"=="Y" (
    echo.
    echo [1/5] Drop Database cu...
    cd Backend\src\BadmintonApp.API
    dotnet ef database drop -f --project ../BadmintonApp.Infrastructure --startup-project .
    if errorlevel 1 (
        echo [SKIP] Khong co Database cu de xoa, tiep tuc...
    )

    echo.
    echo [2/5] Xoa cac migration cu...
    if exist "..\BadmintonApp.Infrastructure\Migrations" (
        rd /s /q "..\BadmintonApp.Infrastructure\Migrations"
        echo        Da xoa folder Migrations.
    ) else (
        echo        Khong co migration cu.
    )

    echo.
    echo [3/5] Tao migration moi 'InitialCreate'...
    dotnet ef migrations add InitialCreate --project ../BadmintonApp.Infrastructure --startup-project .
    if errorlevel 1 (
        echo [LOI] Tao migration that bai! Kiem tra lai code.
        pause
        exit /b 1
    )

    echo.
    echo [4/5] Cap nhat Database...
    dotnet ef database update --project ../BadmintonApp.Infrastructure --startup-project .
    if errorlevel 1 (
        echo [LOI] Cap nhat database that bai!
        pause
        exit /b 1
    )

    echo.
    echo [5/5] Seed Data - 20 san cau long + tai khoan admin...
    sqlcmd -S KHOI\SQLEXPRESS -d BadmintonDB -E -i "..\..\..\seed_data.sql"
    if errorlevel 1 (
        echo [CANH BAO] Seed data that bai. Ban co the chay thu cong: sqlcmd -S KHOI\SQLEXPRESS -d BadmintonDB -E -i seed_data.sql
    ) else (
        echo        Seed data thanh cong!
    )

    cd ..\..\..
    echo.
    echo =====================================================
    echo    RESET DATABASE HOAN TAT
    echo =====================================================
)

:: ---- Kiểm tra và cài dependencies Frontend ----
echo.
echo --- KIEM TRA FRONTEND DEPENDENCIES ---
if not exist "Frontend\node_modules" (
    echo    Chua co node_modules, dang cai dat...
    cd Frontend
    call npm install
    cd ..
    echo    Cai dat xong!
) else (
    echo    node_modules da ton tai, bo qua.
)

:: ---- Khởi động Backend ----
echo.
echo --- KHOI DONG BACKEND ---
echo    URL: http://localhost:5219
echo    Swagger: http://localhost:5219/swagger
start "Backend API - Cau Long" cmd /k "cd Backend\src\BadmintonApp.API && dotnet watch run"

:: ---- Khởi động Frontend ----
echo.
echo --- KHOI DONG FRONTEND ---
echo    URL: http://localhost:5173
start "Frontend Vue - Cau Long" cmd /k "cd Frontend && npm run dev"

:: ---- Thông tin ----
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
