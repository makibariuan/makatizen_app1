using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace makatizen_app.Server.DTOs
{
    // --- Biometric Data DTO (Input) ---
    // Used inside CitizenCreateDto for enrolling new biometric data.
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

    // --- Biometric Data DTO (Read/Output) ---
    // Used to list historical biometric records inside CitizenDetailDto.
    // Crucially, it excludes the Citizen navigation property to break serialization cycles.
    public class BiometricReadDto
    {
        public int Id { get; set; }
        public DateTime DateCapture { get; set; }
        public DateTime DateUpload { get; set; }
        public DateTime? DateActivate { get; set; }
        public int Status { get; set; } // Note: Status is non-nullable in this output DTO

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

        
    }

    // --- Combined Citizen + Biometric DTO (Create/Update Input) ---
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

        // Biometrics is required for creation, optional for simple citizen updates
        public BiometricEnrollmentDto? Biometrics { get; set; } = new BiometricEnrollmentDto();
    }

    // --- Simple Citizen DTO (List Output) ---
    public class CitizenReadDto
    {
        public int Id { get; set; }
        public int CitizenType { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; }

        // Metadata for the *latest* biometric enrollment
        public int? BiometricId { get; set; }
        public DateTime? DateCapture { get; set; }
        public int? Status { get; set; }
        public bool BiometricBypass { get; set; }
    }

    // --- Detailed Citizen DTO (Single View Output) ---
    // This is used by GET /api/Citizen/{id} to return the full history, breaking the cycle.
    public class CitizenDetailDto
    {
        public int Id { get; set; }
        public int CitizenType { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool BiometricBypass { get; set; }

        // The collection is now a list of BiometricReadDto (no circular reference)
        public List<BiometricReadDto> BiometricHistory { get; set; } = new List<BiometricReadDto>();
    }
}