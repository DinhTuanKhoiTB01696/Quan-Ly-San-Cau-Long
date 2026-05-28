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

        // Clear old courts to seed the new ones cleanly
        if (await context.Courts.AnyAsync())
        {
            // Xóa các kèo liên quan trước để tránh lỗi khóa ngoại (Foreign Key)
            var matches = await context.Matches.ToListAsync();
            context.Matches.RemoveRange(matches);
            
            var courtsToDelete = await context.Courts.ToListAsync();
            context.Courts.RemoveRange(courtsToDelete);
            await context.SaveChangesAsync();
        }

        var courts = new List<Court>
        {
            new Court { Name = "Sân Cầu Lông An Bình", Area = Area.TanMai, Address = "684/12 Khu Phố 2, Phường An Bình, Biên Hòa", Price = 70000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.8, Phone = "0909123456", IsFeatured = true, ImageUrl = "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?w=800" },
            new Court { Name = "Sân Cầu Lông Happy", Area = Area.Khac, Address = "44/1 Đ. Đặng Văn Trơn, Phường Hiệp Hoà, Biên Hòa", Price = 60000, Ceiling = "04:00 - 23:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.9, Phone = "0819397742", IsFeatured = true, ImageUrl = "https://images.unsplash.com/photo-1611689342806-0863700ce1e4?w=800" },
            new Court { Name = "CLB Cầu Lông Đăng Khoa", Area = Area.Khac, Address = "Khu vực Phường Hiệp Hòa, Biên Hòa", Price = 60000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.5, Phone = "0988222333", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1521412644187-c49fa049e84d?w=800" },
            new Court { Name = "Sân Cầu Lông Vườn Mít", Area = Area.Khac, Address = "Ngã 4 Vườn Mít, Phường Trung Dũng, Biên Hòa", Price = 80000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.7, Phone = "0903111222", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1572621406839-44047c94b72f?w=800" },
            new Court { Name = "Sân Cầu Lông BH", Area = Area.Khac, Address = "A4/364 Khu phố 4, Phường Trung Dũng, Biên Hòa", Price = 70000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.4, Phone = "0909123456", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1582234372722-50d7ccc30ebd?w=800" },
            new Court { Name = "Sân Cầu Lông Đỉnh Cao", Area = Area.Khac, Address = "86/2 Lý Văn Sâm, KP.8, Phường Tam Hiệp, Biên Hòa", Price = 70000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.6, Phone = "0979353753", IsFeatured = true, ImageUrl = "https://images.unsplash.com/photo-1622279457486-69d73510d554?w=800" },
            new Court { Name = "Hội Quán Cầu Lông ChenZan", Area = Area.Khac, Address = "1186/9 Phạm Văn Thuận, Phường Tam Hiệp, Biên Hòa", Price = 75000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.5, Phone = "0933444555", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=800" },
            new Court { Name = "Sân Cầu Lông Tân Mai", Area = Area.TanMai, Address = "Khu vực bên trong Nghĩa trang Tân Mai, Tân Mai, Biên Hòa", Price = 50000, Ceiling = "05:00 - 21:00", Light = "Trung bình", Surface = "Thảm PVC", Rating = 4.2, Phone = "Liên hệ trực tiếp sân", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1611689225620-41f9e984920b?w=800" },
            new Court { Name = "Sân Cầu Lông Đồng Nai", Area = Area.Khac, Address = "Khu vực Phường Tân Tiến, Biên Hòa", Price = 80000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.7, Phone = "0918555666", IsFeatured = true, ImageUrl = "https://images.unsplash.com/photo-1626224583808-8316dfa9c402?w=800" },
            new Court { Name = "Sân Cầu Lông Hoàng Đạo", Area = Area.TrangDai, Address = "95/65 tổ 9, Khu Phố 5, Phường Trảng Dài, Biên Hòa", Price = 50000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.3, Phone = "0818880099", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1596766779836-e8892ccf25f2?w=800" },
            new Court { Name = "Sân Cầu Lông Bảo Thư", Area = Area.TrangDai, Address = "Tổ 33, 142, Khu phố 2A, Phường Trảng Dài, Biên Hòa", Price = 50000, Ceiling = "06:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.1, Phone = "0908777888", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1582234372722-50d7ccc30ebd?w=800" },
            new Court { Name = "CLB Cầu Lông Trảng Dài", Area = Area.TrangDai, Address = "Khu vực trung tâm Trảng Dài, Biên Hòa", Price = 60000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.4, Phone = "0977123123", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?w=800" },
            new Court { Name = "Sân Cầu Lông Phương My", Area = Area.TrangDai, Address = "Phường Trảng Dài, Biên Hòa", Price = 50000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.0, Phone = "0909123456", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1572621406839-44047c94b72f?w=800" },
            new Court { Name = "Sân Cầu Lông Tỉnh Đội", Area = Area.TrangDai, Address = "Đường Đồng Khởi, Phường Tân Phong, Biên Hòa", Price = 60000, Ceiling = "17:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.5, Phone = "Liên hệ bảo vệ", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1611689342806-0863700ce1e4?w=800" },
            new Court { Name = "Sân Cầu Lông Tiến Minh", Area = Area.TrangDai, Address = "722 Nguyễn Ái Quốc, Khu Phố 1, Biên Hòa", Price = 80000, Ceiling = "05:00 - 00:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.8, Phone = "0934999888", IsFeatured = true, ImageUrl = "https://images.unsplash.com/photo-1522778119026-d647f0596c20?w=800" },
            new Court { Name = "Sân Cầu Lông Hố Nai", Area = Area.HoNai, Address = "Khu vực trung tâm Hố Nai, Biên Hòa", Price = 60000, Ceiling = "05:00 - 21:30", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.2, Phone = "0909234567", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1596766779836-e8892ccf25f2?w=800" },
            new Court { Name = "Sân Cầu Lông Trấn Biên", Area = Area.Khac, Address = "Gần Văn Miếu Trấn Biên, Phường Bửu Long, Biên Hòa", Price = 70000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.7, Phone = "0987654321", IsFeatured = true, ImageUrl = "https://images.unsplash.com/photo-1521412644187-c49fa049e84d?w=800" },
            new Court { Name = "Sân Cầu Lông Bửu Long", Area = Area.Khac, Address = "Khu du lịch Bửu Long (khu vực cổng sau), Biên Hòa", Price = 70000, Ceiling = "06:00 - 21:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.3, Phone = "0909123456", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1626224583808-8316dfa9c402?w=800" },
            new Court { Name = "Sân Cầu Lông Hóa An", Area = Area.Khac, Address = "168/27/5 Hoàng Minh Chánh, Biên Hòa", Price = 60000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.1, Phone = "0933112233", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1572621406839-44047c94b72f?w=800" },
            new Court { Name = "Sân Cầu Lông & Pickleball Khánh Bảo", Area = Area.LongBinh, Address = "47/1 đường Châu Văn Lồng, Khu phố Long Điềm, Biên Hòa", Price = 80000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm Yonex", Rating = 4.9, Phone = "0966555444", IsFeatured = true, ImageUrl = "https://images.unsplash.com/photo-1622279457486-69d73510d554?w=800" },
            new Court { Name = "Sân Cầu Lông Phước Tân", Area = Area.Khac, Address = "Khu vực VWR3+6RW, Phường Phước Tân, Biên Hòa", Price = 60000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.0, Phone = "0909123456", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1611689225620-41f9e984920b?w=800" }
        };

        context.Courts.AddRange(courts);

        await context.SaveChangesAsync();
    }
}
