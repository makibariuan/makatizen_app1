using System.ComponentModel.DataAnnotations;

namespace makatizen_app.Server.DTOs
{
    public class BypassLogDto
    {
        /// True to set the BiometricBypass flag on the Citizen, False to remove it.
        [Required]
        public bool ShouldBypass { get; set; }

        /// The specific step being bypassed (e.g., "Fingerprint Scan", "Photo Capture").
        [MaxLength(100)]
        public string? StepName { get; set; }

        /// A required, short, standardized code for the reason (e.g., "NO_HANDS", "BLIND").
        [MaxLength(100)]
        [Required]
        public string ReasonCode { get; set; } = string.Empty;

        /// Detailed explanation by the Kit User for the bypass.
        [MaxLength(500)]
        public string? ReasonDetails { get; set; }
    }
}
