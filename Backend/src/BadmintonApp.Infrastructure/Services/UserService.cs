using BadmintonApp.Application.DTOs.Users;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BadmintonApp.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _context.Users.OrderByDescending(u => u.CreatedAt).ToListAsync();
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            FullName = u.FullName,
            Phone = u.Phone,
            Role = u.Role,
            IsLocked = u.IsLocked,
            Credits = u.Credits,
            CreatedAt = u.CreatedAt
        });
    }

    public async Task ToggleLockUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        if (user.Role == "Admin") throw new InvalidOperationException("Cannot lock an Admin account");

        user.IsLocked = !user.IsLocked;
        await _context.SaveChangesAsync();
    }
}
