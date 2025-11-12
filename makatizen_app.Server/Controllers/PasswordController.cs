using BCrypt.Net;
using makatizen_app.Server.Data;
using makatizen_app.Server.DTOs;
using makatizen_app.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace makatizen_app.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    //use for auth  
    [Authorize]
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

        // --- CONSOLIDATED GENERIC HELPER METHOD ---
        // This single method handles both UsersSystem and UsersKit models using the interface constraint.
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
    }
}