using AutoMapper;
using Timesheet.Application.DTOs.User;
using Timesheet.Application.Interfaces.Repositories;
using Timesheet.Application.Interfaces.Services;
using Timesheet.Domain.Entities;

namespace Timesheet.Application.Services
{
    /// <summary>
    /// User Service Implementation.
    /// 
    /// DESIGN PATTERN: Service Layer
    /// - Contains business logic
    /// - Uses repositories for data access
    /// - Uses AutoMapper for DTO conversion
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<IEnumerable<UserDto>> GetActiveEmployeesAsync()
        {
            var employees = await _unitOfWork.Users.GetActiveEmployeesAsync();
            return _mapper.Map<IEnumerable<UserDto>>(employees);
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            // Check if email already exists
            if (await _unitOfWork.Users.EmailExistsAsync(dto.Email))
            {
                throw new InvalidOperationException($"User with email '{dto.Email}' already exists.");
            }

            var user = _mapper.Map<User>(dto);
            
            // Hash password (simple hash for demo - use proper hashing in production!)
            user.PasswordHash = HashPassword(dto.Password);
            user.IsActive = true;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return null;

            // Check if email is being changed and new email already exists
            if (user.Email.ToLower() != dto.Email.ToLower() && 
                await _unitOfWork.Users.EmailExistsAsync(dto.Email))
            {
                throw new InvalidOperationException($"User with email '{dto.Email}' already exists.");
            }

            _mapper.Map(dto, user);
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) return false;

            // Soft delete - just deactivate
            user.IsActive = false;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (user == null || !user.IsActive) return null;

            // Verify password
            if (!VerifyPassword(dto.Password, user.PasswordHash))
                return null;

            // In a real application, generate JWT token here
            return new LoginResponseDto
            {
                Token = GenerateToken(user), // Placeholder - implement JWT
                User = _mapper.Map<UserDto>(user)
            };
        }

        // Simple password hashing - USE PROPER HASHING (BCrypt/Argon2) IN PRODUCTION!
        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        private string GenerateToken(User user)
        {
            // Placeholder - implement JWT token generation
            return $"token-{user.Id}-{DateTime.UtcNow.Ticks}";
        }
    }
}
