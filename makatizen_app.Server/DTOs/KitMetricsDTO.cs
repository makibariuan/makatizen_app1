using System;
using System.ComponentModel.DataAnnotations;

namespace Makatizen.AdminPortal.Models
{
    // DTO for returning metrics specific to a single Kit User
    public class KitMetricsResponse
    {
        /// <summary>
        /// The unique ID of the Kit User's device/unit.
        /// </summary>
        [Required]
        public string KitUnitId { get; set; } = "N/A";

        /// <summary>
        /// Total number of citizens enrolled by this specific kit user.
        /// </summary>
        [Required]
        public int TotalCitizensEnrolled { get; set; } = 0;

        /// <summary>
        /// The number of successful enrollments completed today.
        /// </summary>
        [Required]
        public int EnrollmentsToday { get; set; } = 0;

        /// <summary>
        /// The total number of pending biometric uploads (enrollments waiting to sync).
        /// </summary>
        [Required]
        public int PendingUploads { get; set; } = 0;

        /// <summary>
        /// Date and time of the last successful data synchronization/upload.
        /// </summary>
        public DateTime? LastSyncTime { get; set; }

        /// <summary>
        /// Status of the kit unit (e.g., Active, Offline, Maintenance).
        /// </summary>
        [Required]
        public string UnitStatus { get; set; } = "Offline";
    }
}