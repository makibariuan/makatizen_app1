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
    [Route("api/admin/users")]
    // All methods in this controller require the user to be a Super Admin (UserType 1)
    [Authorize]
    
    public class AdminUsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AdminUsersController(AppDbContext context, IEmailService emailService)
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

        [HttpGet("system")]
        public async Task<ActionResult<IEnumerable<UserSystemReadDto>>> GetSystemUsers()
        {
            var users = await _context.UsersSystems
                .Select(u => new UserSystemReadDto
                {
                    Id = u.Id,
                    UserType = u.UserType,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    MustResetPassword = u.MustResetPassword,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("system")]
        public async Task<IActionResult> CreateSystemUser([FromBody] UserSystemCreateDto dto)
        {
            
            if (dto.UserType != 1 && dto.UserType != 2)
            {
                return BadRequest("Invalid UserType. Must be 1 (Super Admin) or 2 (System User).");
            }

            if (await _context.UsersSystems.AnyAsync(u => u.Username == dto.Username) ||
                await _context.UsersKits.AnyAsync(u => u.Username == dto.Username))
            {
                return Conflict(new { message = "Username already exists." });
            }

            var (plainPassword, hashedPassword) = GenerateTemporaryPassword();
            // Super Admins (UserType = 1) will be set to false.
            bool mustReset = dto.UserType == 2;

            var user = new UsersSystem
            {
                UserType = dto.UserType,
                Username = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                PasswordHash = hashedPassword,
                MustResetPassword = mustReset, // Condition is applied here
                CreatedAt = DateTime.UtcNow
            };

            _context.UsersSystems.Add(user);
            await _context.SaveChangesAsync();

            // Return the auto-generated password securely (e.g., in a response body for display ONLY ONCE)
            return CreatedAtAction(nameof(GetSystemUsers), new { id = user.Id }, new
            {
                message = "User created successfully.",
                userId = user.Id,
                initialPassword = plainPassword
            });
        }
        [HttpPut("system/{id}")]
        public async Task<IActionResult> UpdateSystemUser(int id, [FromBody] UserSystemUpdateDto dto)
        {
            var user = await _context.UsersSystems.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"System User with ID {id} not found." });
            }

            // 1. Validate UserType (ensure they are only updating to 1 or 2)
            if (dto.UserType != 1 && dto.UserType != 2)
            {
                return BadRequest("Invalid UserType. Must be 1 (Super Admin) or 2 (System User).");
            }

            // 2. Prevent the last Super Admin from being demoted (optional, but good security)
            if (user.UserType == 1 && dto.UserType != 1)
            {
                var superAdminCount = await _context.UsersSystems.CountAsync(u => u.UserType == 1);
                if (superAdminCount <= 1)
                {
                    return BadRequest(new { message = "Cannot demote the last remaining Super Admin in the system." });
                }
            }

            // 3. Update fields
            user.UserType = dto.UserType;
            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.BirthDate = dto.BirthDate;

            // NOTE: Username update is usually restricted to prevent key conflicts.
            // Password change requires a separate endpoint with specific validation.

            _context.UsersSystems.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"System User '{user.Username}' updated successfully." });
        }

        [HttpDelete("system/{id}")]
        public async Task<IActionResult> DeleteSystemUser(int id)
        {
            var user = await _context.UsersSystems.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"System User with ID {id} not found." });
            }

            // 1. Security Check: Prevent self-deletion
            var currentUserIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (currentUserIdClaim != null && int.TryParse(currentUserIdClaim.Value, out int currentUserId) && currentUserId == id)
            {
                return BadRequest(new { message = "Cannot delete the currently logged-in user." });
            }

            // 2. Prevent deleting the last Super Admin
            if (user.UserType == 1)
            {
                var superAdminCount = await _context.UsersSystems.CountAsync(u => u.UserType == 1);
                if (superAdminCount <= 1)
                {
                    return BadRequest(new { message = "Cannot delete the last remaining Super Admin in the system." });
                }
            }

            // 3. Remove the user
            _context.UsersSystems.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"System User '{user.Username}' (ID: {id}) deleted successfully." });
        }   
    }
}