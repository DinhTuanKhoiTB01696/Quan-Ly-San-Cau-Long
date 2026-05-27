using BadmintonApp.Application.DTOs.Users;

namespace BadmintonApp.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task ToggleLockUserAsync(int userId);
}
