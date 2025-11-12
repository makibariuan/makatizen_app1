using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace makatizen_app.Server.DTOs
{
    // =========================================================
    //                    SYSTEM USER DTOs (UserType 1 & 2)
    // =========================================================

    // DTO for creating a new System User (Super Admin or System User)
    public class UserSystemCreateDto
    {
        [Required]
        public int UserType { get; set; } // Should be 1 (Super Admin) or 2 (System User)

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        // CRITICAL: Password must be included for creation
        [Required, MinLength(8), MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }
    }

    // DTO for updating System User data (excludes Username, includes UserType)
    public class UserSystemUpdateDto
    {
        [Required]
        public int UserType { get; set; } // Must be 1 or 2

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        // Optional: Include a property to manually force a password reset
        public bool MustResetPassword { get; set; } = false;

        // Optional: Allow password update without requiring it
        [MinLength(8), MaxLength(255)]
        public string? NewPassword { get; set; }
    }

    // DTO for returning System User data (omits the PasswordHash)
    public class UserSystemReadDto
    {
        public int Id { get; set; }
        public int UserType { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // This will be null/skipped during JSON serialization if using System.Text.Json
        // Use a DTO specific for returning data if using computed properties
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public bool MustResetPassword { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // =========================================================
    //                    KIT USER DTOs (UserType 3)
    // =========================================================

    // DTO for creating a new Kit User
    public class UserKitCreateDto
    {
        public int UserType { get; set; } = 3;

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        // CRITICAL: Password must be included for creation
        [Required, MinLength(8), MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        // Note: Kit users typically don't have BirthDate, but including for consistency if your database requires it.
        public DateTime? BirthDate { get; set; }
    }

    // DTO for updating Kit User data
    public class UserKitUpdateDto
    {
        // Username is generally not updated. UserType is implicitly 3.

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        // Optional: Include a property to manually force a password reset
        public bool MustResetPassword { get; set; } = false;

        // Optional: Allow password update without requiring it
        [MinLength(8), MaxLength(255)]
        public string? NewPassword { get; set; }
    }

    // DTO for returning Kit User data
    public class UserKitReadDto
    {
        public int Id { get; set; }
        public int UserType { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public bool MustResetPassword { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}