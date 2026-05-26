using BadmintonApp.Domain.Entities;
using BadmintonApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BadmintonApp.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Seed Admin User
        if (!await context.Users.AnyAsync(u => u.Username == "admin"))
        {
            context.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FullName = "Administrator",
                Phone = "0999999999",
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            });
        }

        // Seed Courts
        if (!await context.Courts.AnyAsync())
        {
            var courts = new List<Court>
            {
                new Court { Name = "Sân Thành Công", Area = (Area)1, Address = "Tân Mai, Biên Hòa", Price = 40000, Ceiling = "Cao", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.8, Phone = "0909123456", IsFeatured = true },
                new Court { Name = "Sân Phú Cường", Area = (Area)1, Address = "Tân Mai, Biên Hòa", Price = 45000, Ceiling = "Trung", Light = "Trung bình", Surface = "Thảm PVC", Rating = 4.5, Phone = "0909123457", IsFeatured = false },
                new Court { Name = "Sân Hùng Vương", Area = (Area)2, Address = "Trảng Dài, Biên Hòa", Price = 35000, Ceiling = "Thấp", Light = "Chói", Surface = "Xi măng", Rating = 3.8, Phone = "0909123458", IsFeatured = false },
                new Court { Name = "Sân Thái Bình", Area = (Area)2, Address = "Trảng Dài, Biên Hòa", Price = 40000, Ceiling = "Cao", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.7, Phone = "0909123459", IsFeatured = true },
                new Court { Name = "Sân Đồng Nai", Area = (Area)3, Address = "Long Bình, Biên Hòa", Price = 50000, Ceiling = "Cao", Light = "Tốt", Surface = "Thảm Yonex", Rating = 4.9, Phone = "0909123460", IsFeatured = true },
                new Court { Name = "Sân Hòa Bình", Area = (Area)3, Address = "Long Bình, Biên Hòa", Price = 45000, Ceiling = "Trung", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.4, Phone = "0909123461", IsFeatured = false },
                new Court { Name = "Sân Kim Long", Area = (Area)4, Address = "Tân Hiệp, Biên Hòa", Price = 40000, Ceiling = "Cao", Light = "Trung bình", Surface = "Thảm PVC", Rating = 4.2, Phone = "0909123462", IsFeatured = false },
                new Court { Name = "Sân Phước Tân", Area = (Area)4, Address = "Tân Hiệp, Biên Hòa", Price = 35000, Ceiling = "Trung", Light = "Trung bình", Surface = "Gỗ", Rating = 4.0, Phone = "0909123463", IsFeatured = false },
                new Court { Name = "Sân Phú Mỹ", Area = (Area)5, Address = "Hố Nai, Biên Hòa", Price = 40000, Ceiling = "Cao", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.6, Phone = "0909123464", IsFeatured = true },
                new Court { Name = "Sân Đại Lợi", Area = (Area)5, Address = "Hố Nai, Biên Hòa", Price = 45000, Ceiling = "Trung", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.5, Phone = "0909123465", IsFeatured = false },
                new Court { Name = "Sân Nguyễn Ái Quốc", Area = (Area)1, Address = "Tân Mai, Biên Hòa", Price = 50000, Ceiling = "Cao", Light = "Tốt", Surface = "Thảm Yonex", Rating = 4.9, Phone = "0909123466", IsFeatured = true },
                new Court { Name = "Sân Bình Đa", Area = (Area)1, Address = "Tân Mai, Biên Hòa", Price = 38000, Ceiling = "Trung", Light = "Trung bình", Surface = "Thảm PVC", Rating = 4.1, Phone = "0909123467", IsFeatured = false },
                new Court { Name = "Sân Tân Phong", Area = (Area)2, Address = "Trảng Dài, Biên Hòa", Price = 42000, Ceiling = "Cao", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.5, Phone = "0909123468", IsFeatured = false },
                new Court { Name = "Sân Bửu Long", Area = (Area)3, Address = "Long Bình, Biên Hòa", Price = 48000, Ceiling = "Cao", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.7, Phone = "0909123469", IsFeatured = true },
                new Court { Name = "Sân An Bình", Area = (Area)4, Address = "Tân Hiệp, Biên Hòa", Price = 36000, Ceiling = "Thấp", Light = "Chói", Surface = "Xi măng", Rating = 3.5, Phone = "0909123470", IsFeatured = false },
                new Court { Name = "Sân Quyết Thắng", Area = (Area)5, Address = "Hố Nai, Biên Hòa", Price = 40000, Ceiling = "Trung", Light = "Trung bình", Surface = "Thảm PVC", Rating = 4.2, Phone = "0909123471", IsFeatured = false },
                new Court { Name = "Sân Thanh Bình", Area = (Area)1, Address = "Tân Mai, Biên Hòa", Price = 44000, Ceiling = "Cao", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.6, Phone = "0909123472", IsFeatured = false },
                new Court { Name = "Sân Long Bình Tân", Area = (Area)3, Address = "Long Bình, Biên Hòa", Price = 38000, Ceiling = "Trung", Light = "Trung bình", Surface = "Thảm PVC", Rating = 4.0, Phone = "0909123473", IsFeatured = false },
                new Court { Name = "Sân Tân Tiến", Area = (Area)2, Address = "Trảng Dài, Biên Hòa", Price = 40000, Ceiling = "Cao", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.5, Phone = "0909123474", IsFeatured = false },
                new Court { Name = "Sân Trung Dũng", Area = (Area)4, Address = "Tân Hiệp, Biên Hòa", Price = 42000, Ceiling = "Cao", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.4, Phone = "0909123475", IsFeatured = false }
            };

            context.Courts.AddRange(courts);
        }

        await context.SaveChangesAsync();
    }
}
