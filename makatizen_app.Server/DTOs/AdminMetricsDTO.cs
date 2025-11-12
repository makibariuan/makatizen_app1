using System;
using System.ComponentModel.DataAnnotations;

namespace Makatizen.AdminPortal.Models
{
    // DTO for returning key metrics to the DashboardAdmin.vue component
    public class AdminMetricsResponse
    {
        /// <summary>
        /// Total count of System Users (e.g., Admins, staff).
        /// </summary>
        [Required]
        public int SystemUserCount { get; set; } = 0;

        /// <summary>
        /// Total count of Kit Users (e.g., personnel using the enrollment kits).
        /// </summary>
        [Required]
        public int KitUserCount { get; set; } = 0;

        /// <summary>
        /// Total count of enrolled citizens.
        /// </summary>
        [Required]
        public int CitizenCount { get; set; } = 0;

        /// <summary>
        /// Date of the most recent citizen enrollment.
        /// </summary>
        public DateTime? LastEnrollmentDate { get; set; }

        /// <summary>
        /// Count of critical system logs or errors needing review.
        /// </summary>
        [Required]
        public int UnresolvedLogs { get; set; } = 0;

        /// <summary>
        /// Description of the last critical error logged.
        /// </summary>
        public string LastLogError { get; set; }
    }
}