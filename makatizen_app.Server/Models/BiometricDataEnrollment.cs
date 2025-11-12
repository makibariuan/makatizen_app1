using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace makatizen_app.Server.Models
{
    [Table("BiometricDataEnrollment")]
    public class BiometricDataEnrollment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign Key to Citizen
        public int PersonId { get; set; }
        public Citizen? Citizen { get; set; }

        // --- Date Fields ---
        public DateTime? DateCapture { get; set; }
        public DateTime? DateUpload { get; set; }
        public DateTime? DateActivate { get; set; }

        // --- Biometric Data Fields ---
        public string? Photo { get; set; }
        public string? Signature { get; set; }

        // --- Left Hand Fingerprints ---
        public string? LeftThumb { get; set; }
        public string? LeftIndex { get; set; }
        public string? LeftMiddle { get; set; }
        public string? LeftRing { get; set; }
        public string? LeftSmall { get; set; }

        // --- Right Hand Fingerprints ---
        public string? RightThumb { get; set; }
        public string? RightIndex { get; set; }
        public string? RightMiddle { get; set; }
        public string? RightRing { get; set; }
        public string? RightSmall { get; set; }

        // --- Eye/Iris Data ---
        public string? EyeLeft { get; set; }
        public string? EyeRight { get; set; }

        // --- General/Other Biometric Data ---
        public string? BiometricLeft { get; set; }
        public string? BiometricRight { get; set; }

        // --- Status Fields ---
        public int Hit { get; set; }
        public int Status { get; set; }
    }
}