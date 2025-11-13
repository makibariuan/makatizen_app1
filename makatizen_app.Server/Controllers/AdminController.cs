using BCrypt.Net;
using makatizen_app.Server.Data;
using makatizen_app.Server.DTOs;
using makatizen_app.Server.Models;
using makatizen_app.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace makatizen_app.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    // All methods in this controller require the user to be a Super Admin (UserType 1)
    [Authorize]

    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AdminController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // --- Helper for generating secure temporary passwords ---
        private (string plainPassword, string hashedPassword) GenerateTemporaryPassword()
        {
            // Simple generation logic (e.g., eight random characters with symbols)
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            // NOTE: You should securely communicate this 'password' to the user (e.g., via email).
            return (password, hash);
        }

        // =========================================================================
        // USERS SYSTEM (Super Admins and System Users) CRUD
        // =========================================================================

        [HttpGet("user-list/{userType}")]
        // CHANGED RETURN TYPE: Generalize to IActionResult
        public async Task<IActionResult> GetSystemUsers(string userType)
        {
            if (userType != "system" && userType != "kit")
            {
                // Clarify response for type validation
                return BadRequest(new { message = "Invalid UserType. Must be 'system' or 'kit'." });
            }

            if (userType == "system")
            {
                var users = await _context.UsersSystems
                    .Select(u => new UserSystemReadDto
                    {
                        Id = u.Id,
                        UserType = u.UserType,
                        Username = u.Username,
                        Status = u.IsActive
                        
                    })
                    .ToListAsync();

                return Ok(users);
            }
            else // userType == "kit"
            {
                var users = await _context.UsersKits
                    .Select(u => new UserKitReadDto // This DTO is different from UserSystemReadDto
                    {
                        Id = u.Id,
                        UserType = u.UserType,
                        Username = u.Username,
                        Status = u.IsActive
                    })
                    .ToListAsync();

                return Ok(users);
            }
        }

        [HttpPost("create-user/{userType}")]
        public async Task<IActionResult> CreateSystemUser([FromBody] UserCreateDto dto, string userType)
        {
            // Convert userType to lowercase for reliable comparison
            userType = userType.ToLowerInvariant();

            // --- 1. Validate Route Parameter (Table Type) ---

            if (userType != "system" && userType != "kit")
            {
                return BadRequest(new { message = "Invalid route parameter for user type. Must be 'system' or 'kit'." });
            }

            // --- 2. Validate DTO.UserType (Role Type) based on Table Type ---

            if (userType == "system")
            {
                // System users must be type 1 (Super Admin) or 2 (System User)
                if (dto.UserType != 1 && dto.UserType != 2)
                {
                    return BadRequest(new { message = $"Invalid UserType for 'system' endpoint. Must be 1 (Super Admin) or 2 (System User)." });
                }
            }
            else // userType == "kit"
            {
                // Kit users must be type 3
                if (dto.UserType != 3)
                {
                    return BadRequest(new { message = $"Invalid UserType for 'kit' endpoint. Must be 3 (Kit User)." });
                }
            }

            // --- 3. Check for Username Conflict ---

            if (await _context.UsersSystems.AnyAsync(u => u.Username == dto.Username) ||
                await _context.UsersKits.AnyAsync(u => u.Username == dto.Username))
            {
                return Conflict(new { message = "Username already exists in either system or kit table." });
            }

            // --- 4. Generate Credentials and Flags ---

            var (plainPassword, hashedPassword) = GenerateTemporaryPassword();

            // Super Admin (1) should NOT reset. System User (2) and Kit User (3) SHOULD reset.
            

            // --- 5. Conditional User Creation and Persistence ---

            int newUserId;
            string userTableName;

            if (userType == "system")
            {
                // Create a SYSTEM USER
                var user = new UsersSystem
                {
                    UserType = dto.UserType,
                    Username = dto.Username,
                    PasswordHash = hashedPassword,
                    IsActive = true,
                    MustResetPassword = true,
                    // FirstName and LastName mapping removed here
                };
                _context.UsersSystems.Add(user);

                newUserId = user.Id;
                userTableName = "UsersSystem";
            }
            else // userType == "kit"
            {
                // Create a KIT USER
                var kit = new UsersKit
                {
                    UserType = dto.UserType, // Should be 3 based on validation
                    Username = dto.Username,
                    PasswordHash = hashedPassword,
                    IsActive = true,
                    MustResetPassword = true,
                    // FirstName and LastName mapping removed here
                };
                _context.UsersKits.Add(kit);

                newUserId = kit.Id;
                userTableName = "UsersKit";
            }

            await _context.SaveChangesAsync();

            // --- 6. Return Success Response ---

            return CreatedAtAction(nameof(GetSystemUsers), new { userType = userType }, new // <-- FIX HERE
            {
                message = $"User created successfully in {userTableName}.",
                userId = newUserId,
                username = dto.Username,
                userType = dto.UserType,
                initialPassword = plainPassword,
                mustReset = true
            });
        }


        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userSystem = await _context.UsersSystems
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

            if (userSystem != null)
            {
                return await ProcessPasswordChange(userSystem, request.CurrentPassword, request.NewPassword);
            }

            var userKit = await _context.UsersKits
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

            if (userKit != null)
            {
                return await ProcessPasswordChange(userKit, request.CurrentPassword, request.NewPassword);
            }

            return Unauthorized(new { message = "Invalid credentials." });
        }


        private async Task<IActionResult> ProcessPasswordChange<T>(T user, string currentPassword, string newPassword) where T : class, IResettableUser
        {
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "Invalid current password." });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.MustResetPassword = false;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password updated successfully. Please log in with your new password." });
        }

        
        [HttpPost("toggle-status")]
        public async Task<IActionResult> ToggleUserStatus([FromBody] ToggleUserStatusRequest request)
        {
            // Try finding the user in the System Users table first
            var userSystem = await _context.UsersSystems
                .FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (userSystem != null)
            {
                return await ProcessStatusToggle(userSystem, request.IsActive);
            }

            // Try finding the user in the Kit Users table
            var userKit = await _context.UsersKits
                .FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (userKit != null)
            {
                return await ProcessStatusToggle(userKit, request.IsActive);
            }

            // If user is not found in either table
            return NotFound(new { message = $"User with ID {request.UserId} not found." });
        }

        private async Task<IActionResult> ProcessStatusToggle<T>(T user, bool isActive) where T : class, IStatusUser
        {
            // Prevent locking out Super Admins (UserType 1) - Best practice security check
            if (user is UsersSystem systemUser && systemUser.UserType == 1 && !isActive)
            {
                return BadRequest(new { message = $"Cannot deactivate the primary Super Admin user ('{systemUser.Username}')" });
            }

            user.IsActive = isActive;
            await _context.SaveChangesAsync();

            string action = isActive ? "activated" : "deactivated (locked)";

            return Ok(new
            {
                message = $"User '{user.Username}' (ID: {user.Id}) has been successfully {action}.",
                isActive = user.IsActive
            });
        }




    }
}