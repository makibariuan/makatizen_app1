using BCrypt.Net;
using makatizen_app.Server.Data;
using makatizen_app.Server.DTOs;
using makatizen_app.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace makatizen_app.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PasswordController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            //Find the user in UsersSystem (Super Admin or System User)
            var userSystem = await _context.UsersSystems
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

            if (userSystem != null)
            {
                return await ProcessPasswordReset(userSystem, request.CurrentPassword, request.NewPassword);
            }

            //Find the user in UsersKit (Kit User)
            var userKit = await _context.UsersKits
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

            if (userKit != null)
            {
                return await ProcessPasswordReset(userKit, request.CurrentPassword, request.NewPassword);
            }

            return NotFound(new { message = "User not found." });
        }

        // Handles the actual hashing and update for UsersSystem
        private async Task<IActionResult> ProcessPasswordReset(UsersSystem user, string currentPassword, string newPassword)
        {
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "Invalid current password." });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.MustResetPassword = false;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Password reset successfully. Please log in with your new password." });
        }

        // Handles the actual hashing and update for UsersKit
        private async Task<IActionResult> ProcessPasswordReset(UsersKit user, string currentPassword, string newPassword)
        {
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "Invalid current password." });
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.MustResetPassword = false;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Password reset successfully. Please log in with your new password." });
        }
    }
}