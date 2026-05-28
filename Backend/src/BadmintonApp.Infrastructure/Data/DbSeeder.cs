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
            new Court { Name = "Sân Cầu Lông Happy", Area = Area.TanMai, Address = "44/1 Đ. Đặng Văn Trơn, Hiệp Hòa, Biên Hòa", Price = 45000, Ceiling = "00:00 - 23:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.8, Phone = "0909123456", IsFeatured = true, ImageUrl = "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?w=800" },
            new Court { Name = "Sân Cầu Lông An Bình", Area = Area.Khac, Address = "684/12 Khu phố 2, An Bình, Biên Hòa", Price = 40000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.5, Phone = "0909123457", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1517649763962-0c623066013b?w=800" },
            new Court { Name = "CLB Cầu Lông Đăng Khoa", Area = Area.Khac, Address = "Khu vực Hiệp Hòa, Biên Hòa", Price = 42000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.6, Phone = "0909123458", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1521537634199-673cb821b7ed?w=800" },
            new Court { Name = "Sân Cầu Đỉnh Cao", Area = Area.TanHiep, Address = "21/2 Lý Văn Sâm, KP.8, Tam Hiệp, Biên Hòa", Price = 45000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.7, Phone = "0909123459", IsFeatured = true, ImageUrl = "https://images.unsplash.com/photo-1554068865-24bccd4e34b8?w=800" },
            new Court { Name = "Hội Quán Cầu Lông ChenZan", Area = Area.TanHiep, Address = "1186/9 Phạm Văn Thuận, Tam Hiệp, Biên Hòa", Price = 45000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.4, Phone = "0909123460", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1599447421416-3414500d18a5?w=800" },
            new Court { Name = "Sân Cầu Lông Tân Mai", Area = Area.TanMai, Address = "Khu vực bên trong Nghĩa trang Tân Mai, Tân Mai, Biên Hòa", Price = 40000, Ceiling = "05:00 - 21:00", Light = "Trung bình", Surface = "Thảm PVC", Rating = 4.2, Phone = "0909123461", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1613918431208-675077bdc581?w=800" },
            new Court { Name = "Sân Cầu Lông Hoàng Đạo", Area = Area.TrangDai, Address = "95/65 tổ 9, KP 5, Trảng Dài, Biên Hòa", Price = 40000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.3, Phone = "0909123462", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1608245449230-4ac19066d2d0?w=800" },
            new Court { Name = "CLB Cầu Lông Trảng Dài", Area = Area.TrangDai, Address = "Khu vực XVPF+M8J, Trảng Dài, Biên Hòa", Price = 40000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.5, Phone = "0909123463", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=800" },
            new Court { Name = "Sân Cầu Lông Phương My", Area = Area.TrangDai, Address = "Phường Trảng Dài, Biên Hòa", Price = 40000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.1, Phone = "0909123464", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1544698310-74ea9d1c8258?w=800" },
            new Court { Name = "Sân Cầu Lông & Pickleball Khánh Bảo", Area = Area.LongBinh, Address = "47/1 đường Châu Văn Lồng, KP Long Điềm, Long Bình Tân, Biên Hòa", Price = 50000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm Yonex", Rating = 4.9, Phone = "0909123465", IsFeatured = true, ImageUrl = "https://images.unsplash.com/photo-1526676082484-915f01dbb361?w=800" },
            new Court { Name = "Sân Cầu Lông Hóa An", Area = Area.Khac, Address = "168/27/5 Hoàng Minh Chánh, Hóa An, Biên Hòa", Price = 38000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.0, Phone = "0909123466", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1626224583764-f87db24ac4ea?w=800" },
            new Court { Name = "Sân Cầu Lông Phước Tân", Area = Area.Khac, Address = "Khu vực VWR3+6RW, Phước Tân, Biên Hòa", Price = 40000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.2, Phone = "0909123467", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1517649763962-0c623066013b?w=800" },
            new Court { Name = "Sân Cầu Lông Vườn Mít", Area = Area.Khac, Address = "Khu vực ngã 4 Vườn Mít, Trung Dũng, Biên Hòa", Price = 45000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.4, Phone = "0909123468", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1521537634199-673cb821b7ed?w=800" },
            new Court { Name = "Sân Cầu Lông Tỉnh Đội", Area = Area.TrangDai, Address = "Đường Đồng Khởi, Tân Phong, Biên Hòa", Price = 45000, Ceiling = "17:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.6, Phone = "0909123469", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1554068865-24bccd4e34b8?w=800" },
            new Court { Name = "Sân Cầu Lông Trấn Biên", Area = Area.Khac, Address = "Gần Văn Miếu Trấn Biên, Bửu Long, Biên Hòa", Price = 48000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.7, Phone = "0909123470", IsFeatured = true, ImageUrl = "https://images.unsplash.com/photo-1599447421416-3414500d18a5?w=800" },
            new Court { Name = "Sân Cầu Lông Đồng Nai", Area = Area.Khac, Address = "Đường Phạm Văn Thuận, Tân Tiến, Biên Hòa", Price = 45000, Ceiling = "05:00 - 22:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.5, Phone = "0909123471", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1613918431208-675077bdc581?w=800" },
            new Court { Name = "Sân Cầu Lông Hố Nai", Area = Area.HoNai, Address = "Khu vực Hố Nai, Biên Hòa", Price = 40000, Ceiling = "05:00 - 21:30", Light = "Trung bình", Surface = "Thảm PVC", Rating = 4.1, Phone = "0909123472", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1608245449230-4ac19066d2d0?w=800" },
            new Court { Name = "Sân Cầu Lông Bửu Long", Area = Area.Khac, Address = "Khu du lịch Bửu Long (cổng sau), Bửu Long, Biên Hòa", Price = 45000, Ceiling = "06:00 - 21:00", Light = "Tốt", Surface = "Thảm PVC", Rating = 4.3, Phone = "0909123473", IsFeatured = false, ImageUrl = "https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=800" }
        };

        context.Courts.AddRange(courts);

        await context.SaveChangesAsync();
    }
}
