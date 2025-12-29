using Timesheet.Application.DTOs.User;

namespace Timesheet.Application.Interfaces.Services
{
    /// <summary>
    /// User Service Interface.
    /// Defines business operations for User management.
    /// </summary>
    public interface IUserService
    {
        Task<UserDto?> GetByIdAsync(int id);
        Task<UserDto?> GetByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task<IEnumerable<UserDto>> GetActiveEmployeesAsync();
        Task<UserDto> CreateAsync(CreateUserDto dto);
        Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteAsync(int id);
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);
    }
}
