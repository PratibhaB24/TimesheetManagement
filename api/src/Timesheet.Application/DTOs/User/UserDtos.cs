using Timesheet.Domain.Enums;

namespace Timesheet.Application.DTOs.User
{
    /// <summary>
    /// DTO for returning User data to the client.
    /// We don't expose PasswordHash for security reasons.
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string RoleName => Role.ToString();
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    /// <summary>
    /// DTO for creating a new User.
    /// </summary>
    public class CreateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing User.
    /// </summary>
    public class UpdateUserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for user login request.
    /// </summary>
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for login response with token.
    /// </summary>
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }
}
