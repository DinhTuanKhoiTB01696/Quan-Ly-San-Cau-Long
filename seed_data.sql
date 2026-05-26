USE [BadmintonDB];
GO

-- Seed Admin Account (Password: Admin@123)
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, PasswordHash, FullName, Phone, Role, CreatedAt)
    VALUES ('admin', '$2a$11$N5h.gC/Z1jM4i4R9JvO41uw0G4Y1Wc9O1f3eJ3O8wD3k1.v3/4FzW', 'Administrator', '0999999999', 'Admin', GETUTCDATE());
END

-- Seed Courts
IF NOT EXISTS (SELECT * FROM Courts)
BEGIN
    SET IDENTITY_INSERT Courts ON;
    INSERT INTO Courts (Id, Name, Area, Address, Price, Ceiling, Light, Surface, Rating, Phone, IsFeatured, CreatedAt) VALUES
    (1, N'Sân Thành Công', 1, N'Tân Mai, Biên Hòa', 40000, N'Cao', N'Tốt', N'Thảm PVC', 4.8, '0909123456', 1, GETUTCDATE()),
    (2, N'Sân Phú Cường', 1, N'Tân Mai, Biên Hòa', 45000, N'Trung', N'Trung bình', N'Thảm PVC', 4.5, '0909123457', 0, GETUTCDATE()),
    (3, N'Sân Hùng Vương', 2, N'Trảng Dài, Biên Hòa', 35000, N'Thấp', N'Chói', N'Xi măng', 3.8, '0909123458', 0, GETUTCDATE()),
    (4, N'Sân Thái Bình', 2, N'Trảng Dài, Biên Hòa', 40000, N'Cao', N'Tốt', N'Thảm PVC', 4.7, '0909123459', 1, GETUTCDATE()),
    (5, N'Sân Đồng Nai', 3, N'Long Bình, Biên Hòa', 50000, N'Cao', N'Tốt', N'Thảm Yonex', 4.9, '0909123460', 1, GETUTCDATE()),
    (6, N'Sân Hòa Bình', 3, N'Long Bình, Biên Hòa', 45000, N'Trung', N'Tốt', N'Thảm PVC', 4.4, '0909123461', 0, GETUTCDATE()),
    (7, N'Sân Kim Long', 4, N'Tân Hiệp, Biên Hòa', 40000, N'Cao', N'Trung bình', N'Thảm PVC', 4.2, '0909123462', 0, GETUTCDATE()),
    (8, N'Sân Phước Tân', 4, N'Tân Hiệp, Biên Hòa', 35000, N'Trung', N'Trung bình', N'Gỗ', 4.0, '0909123463', 0, GETUTCDATE()),
    (9, N'Sân Phú Mỹ', 5, N'Hố Nai, Biên Hòa', 40000, N'Cao', N'Tốt', N'Thảm PVC', 4.6, '0909123464', 1, GETUTCDATE()),
    (10, N'Sân Đại Lợi', 5, N'Hố Nai, Biên Hòa', 45000, N'Trung', N'Tốt', N'Thảm PVC', 4.5, '0909123465', 0, GETUTCDATE()),
    (11, N'Sân Nguyễn Ái Quốc', 1, N'Tân Mai, Biên Hòa', 50000, N'Cao', N'Tốt', N'Thảm Yonex', 4.9, '0909123466', 1, GETUTCDATE()),
    (12, N'Sân Bình Đa', 1, N'Tân Mai, Biên Hòa', 38000, N'Trung', N'Trung bình', N'Thảm PVC', 4.1, '0909123467', 0, GETUTCDATE()),
    (13, N'Sân Tân Phong', 2, N'Trảng Dài, Biên Hòa', 42000, N'Cao', N'Tốt', N'Thảm PVC', 4.5, '0909123468', 0, GETUTCDATE()),
    (14, N'Sân Bửu Long', 3, N'Long Bình, Biên Hòa', 48000, N'Cao', N'Tốt', N'Thảm PVC', 4.7, '0909123469', 1, GETUTCDATE()),
    (15, N'Sân An Bình', 4, N'Tân Hiệp, Biên Hòa', 36000, N'Thấp', N'Chói', N'Xi măng', 3.5, '0909123470', 0, GETUTCDATE()),
    (16, N'Sân Quyết Thắng', 5, N'Hố Nai, Biên Hòa', 40000, N'Trung', N'Trung bình', N'Thảm PVC', 4.2, '0909123471', 0, GETUTCDATE()),
    (17, N'Sân Thanh Bình', 1, N'Tân Mai, Biên Hòa', 44000, N'Cao', N'Tốt', N'Thảm PVC', 4.6, '0909123472', 0, GETUTCDATE()),
    (18, N'Sân Long Bình Tân', 3, N'Long Bình, Biên Hòa', 38000, N'Trung', N'Trung bình', N'Thảm PVC', 4.0, '0909123473', 0, GETUTCDATE()),
    (19, N'Sân Tân Tiến', 2, N'Trảng Dài, Biên Hòa', 40000, N'Cao', N'Tốt', N'Thảm PVC', 4.5, '0909123474', 0, GETUTCDATE()),
    (20, N'Sân Trung Dũng', 4, N'Tân Hiệp, Biên Hòa', 42000, N'Cao', N'Tốt', N'Thảm PVC', 4.4, '0909123475', 0, GETUTCDATE());
    SET IDENTITY_INSERT Courts OFF;
END
GO
