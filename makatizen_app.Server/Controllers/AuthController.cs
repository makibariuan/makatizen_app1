using BCrypt.Net;
using makatizen_app.Server.Data;
using makatizen_app.Server.DTOs;
using makatizen_app.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace makatizen_app.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Try to find the user in UsersSystem (Super Admin, System User)
            var userSystem = await _context.UsersSystems
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

            if (userSystem != null)
            {
                return AuthenticateUser(userSystem.Id, userSystem.PasswordHash, userSystem.UserType, userSystem.MustResetPassword, request.Password, "system");
            }

            // 2. Try to find the user in UsersKit (Kit User)
            var userKit = await _context.UsersKits
                .FirstOrDefaultAsync(u => u.Username.ToLower() == request.Username.ToLower());

            if (userKit != null)
            {
                return AuthenticateUser(userKit.Id, userKit.PasswordHash, userKit.UserType, userKit.MustResetPassword, request.Password, "kit");
            }

            return Unauthorized(new { message = "Invalid credentials." });
        }

        private IActionResult AuthenticateUser(int userId, string storedHash, int userType, bool mustResetPassword, string providedPassword, string userSource)
        { 
            if (!BCrypt.Net.BCrypt.Verify(providedPassword, storedHash))
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            if (mustResetPassword)
            {
              
                return Ok(new LoginResponse
                {
                    Token = "",
                    Role = GetRoleName(userType),
                    UserId = userId.ToString(),
                    MustResetPassword = true
                });
            }

            // Generate the token
            var token = GenerateJwtToken(userId.ToString(), userType.ToString(), GetRoleName(userType));

            return Ok(new LoginResponse
            {
                Token = token,
                Role = GetRoleName(userType),
                UserId = userId.ToString(),
                MustResetPassword = false
            });
        }

        private string GenerateJwtToken(string userId, string userType, string roleName)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);
            var expiryMinutes = int.Parse(jwtSection["ExpireMinutes"]!);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, roleName),
                new Claim("UserType", userType)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                Issuer = jwtSection["Issuer"],
                Audience = jwtSection["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GetRoleName(int userType) => userType switch
        {
            1 => "Super Admin",
            2 => "System User",
            3 => "Kit User",
            _ => "Unknown"
        };
    }
}