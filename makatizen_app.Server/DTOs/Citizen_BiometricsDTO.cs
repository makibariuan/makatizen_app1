using System.ComponentModel.DataAnnotations;

namespace makatizen_app.Server.DTOs
{
    // --- Biometric Data DTO (Input) ---
    public class BiometricEnrollmentDto
    {
        public string? Photo { get; set; }
        public string? Signature { get; set; }
        public string? LeftThumb { get; set; }
        public string? LeftIndex { get; set; }
        public string? LeftMiddle { get; set; }
        public string? LeftRing { get; set; }
        public string? LeftSmall { get; set; }
        public string? RightThumb { get; set; }
        public string? RightIndex { get; set; }
        public string? RightMiddle { get; set; }
        public string? RightRing { get; set; }
        public string? RightSmall { get; set; }
        public string? EyeLeft { get; set; }
        public string? EyeRight { get; set; }
        public string? BiometricLeft { get; set; }
        public string? BiometricRight { get; set; }

        // Status can be set by the Kit User during enrollment
        public int? Status { get; set; }
    }

    // --- Combined Citizen + Biometric DTO (Create/Update) ---
    public class CitizenCreateDto
    {
        [Required]
        public int CitizenType { get; set; }

        [Required, MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public BiometricEnrollmentDto Biometrics { get; set; } = new BiometricEnrollmentDto();
    }

    // --- Combined Citizen + Biometric DTO (Read) ---
    public class CitizenReadDto
    {
        public int Id { get; set; }
        public int CitizenType { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? BiometricId { get; set; }
        public DateTime? DateCapture { get; set; }
        public int? Status { get; set; }
    }
}