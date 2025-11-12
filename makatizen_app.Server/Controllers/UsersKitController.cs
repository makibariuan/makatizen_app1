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
    [Route("api/admin/kitusers")]
    
    //[Authorize(Policy = "RequireSuperAdminOrSystemUser")]
    [Authorize]
    public class KitUsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public KitUsersController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // --- Helper for generating secure temporary passwords ---
        private (string plainPassword, string hashedPassword) GenerateTemporaryPassword()
        {
            // Simple generation logic (e.g., ten random characters with symbols)
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var hash = BCrypt.Net.BCrypt.HashPassword(password);

            return (password, hash);
        }

        [HttpGet] // Route: GET api/admin/kitusers
        public async Task<ActionResult<IEnumerable<UserKitReadDto>>> GetKitUsers()
        {
            var users = await _context.UsersKits
                .Select(u => new UserKitReadDto
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
        [HttpPost] // Route: POST api/admin/kitusers
        public async Task<IActionResult> CreateKitUser([FromBody] UserKitCreateDto dto)
        {
            // 1. Check for unique username across both user types (System and Kit)
            if (await _context.UsersSystems.AnyAsync(u => u.Username == dto.Username) ||
                await _context.UsersKits.AnyAsync(u => u.Username == dto.Username))
            {
                return Conflict(new { message = "Username already exists." });
            }

            var (plainPassword, hashedPassword) = GenerateTemporaryPassword();

            var user = new UsersKit
            {
                UserType = 3, // Kit User type is fixed
                Username = dto.Username,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PasswordHash = hashedPassword,
                MustResetPassword = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.UsersKits.Add(user);
            await _context.SaveChangesAsync();

            try
            {
                // Send temporary password via email
                string subject = "Welcome to Makatizen: Your New Kit User Credentials";
                string body = $@"
                    <h1>Welcome, {user.FirstName}!</h1>
                    <p>Your new <strong>Kit User</strong> account has been created.</p>
                    <p><strong>Username:</strong> {user.Username}</p>
                    <p><strong>Temporary Password:</strong> <code>{plainPassword}</code></p>
                    <p><strong>IMPORTANT:</strong> You will be required to change this password immediately upon your first login for security purposes.</p>
                    <p>Thank you.</p>";

                await _emailService.SendEmailAsync(user.Email, subject, body);

                return CreatedAtAction(nameof(GetKitUsers), new { id = user.Id }, new
                {
                    message = "Kit User created successfully. Temporary password sent via email.",
                    userId = user.Id
                });
            }
            catch (Exception)
            {
                // If the email fails, return a 500 error but confirm user creation
                return StatusCode(500, new { message = "Kit User created, but failed to send temporary password email." });
            }
        }


        [HttpPut("{id}")] // Route: PUT api/admin/kitusers/{id}
        public async Task<IActionResult> UpdateKitUser(int id, [FromBody] UserKitUpdateDto dto)
        {
            var user = await _context.UsersKits.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"Kit User with ID {id} not found." });
            }

            // Update fields
            user.Email = dto.Email;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            _context.UsersKits.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Kit User '{user.Username}' updated successfully." });
        }

        [HttpDelete("{id}")] // Route: DELETE api/admin/kitusers/{id}
        public async Task<IActionResult> DeleteKitUser(int id)
        {
            var user = await _context.UsersKits.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"Kit User with ID {id} not found." });
            }

            _context.UsersKits.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Kit User '{user.Username}' (ID: {id}) deleted successfully." });
        }
    }
}