using Makatizen.AdminPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Makatizen.AdminPortal.Controllers
{
    // Sets the base route for the controller: /api/dashboard
    [Route("api/[controller]")]
    [ApiController]
    // Ensures only users with the "SuperAdmin" or "Admin" role can access any endpoint in this controller
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class DashboardController : ControllerBase
    {
        // NOTE: In a real application, you would inject services here 
        // (e.g., IUserService, ICitizenService, ILogService) to fetch real data.

        /// <summary>
        /// Retrieves key metrics for the administrative dashboard.
        /// GET /api/dashboard/admin-metrics
        /// </summary>
        /// <returns>A response object containing various system metrics.</returns>
        [HttpGet("admin-metrics")]
        [ProducesResponseType(typeof(AdminMetricsResponse), 200)]
        [ProducesResponseType(401)] // Unauthorized
        [ProducesResponseType(403)] // Forbidden (if token is valid but role is wrong)
        public async Task<IActionResult> GetAdminMetrics()
        {
            // --- Placeholder/Mock Data Implementation ---
            // Replace this section with actual asynchronous database calls (e.g., Entity Framework) 
            // to retrieve real counts and dates.

            // Simulate loading data from the database
            await Task.Delay(500);

            var metrics = new AdminMetricsResponse
            {
                SystemUserCount = 5,
                KitUserCount = 42,
                CitizenCount = 15480,
                LastEnrollmentDate = DateTime.UtcNow.AddHours(-3), // 3 hours ago
                UnresolvedLogs = 12,
                LastLogError = "DB Connection timeout (Severity Critical)"
            };

            // --- End Placeholder/Mock Data Implementation ---

            return Ok(metrics);
        }
    }
}