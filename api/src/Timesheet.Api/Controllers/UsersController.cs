using Microsoft.AspNetCore.Mvc;
using Timesheet.Application.DTOs.Common;
using Timesheet.Application.DTOs.User;
using Timesheet.Application.Interfaces.Services;

namespace Timesheet.Api.Controllers
{
    /// <summary>
    /// User Management Controller.
    /// Handles user CRUD operations and authentication.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(users));
        }

        /// <summary>
        /// Get user by ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found."));

            return Ok(ApiResponse<UserDto>.SuccessResponse(user));
        }

        /// <summary>
        /// Get all active employees.
        /// </summary>
        [HttpGet("employees")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetActiveEmployees()
        {
            var employees = await _userService.GetActiveEmployeesAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(employees));
        }

        /// <summary>
        /// Create a new user.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserDto>>> Create([FromBody] CreateUserDto dto)
        {
            try
            {
                var user = await _userService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, 
                    ApiResponse<UserDto>.SuccessResponse(user, "User created successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Update an existing user.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> Update(int id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var user = await _userService.UpdateAsync(id, dto);
                if (user == null)
                    return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found."));

                return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User updated successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<UserDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Delete (deactivate) a user.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _userService.DeleteAsync(id);
            if (!result)
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found."));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "User deactivated successfully."));
        }

        /// <summary>
        /// User login.
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto dto)
        {
            var response = await _userService.LoginAsync(dto);
            if (response == null)
                return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse("Invalid credentials."));

            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful."));
        }
    }
}
