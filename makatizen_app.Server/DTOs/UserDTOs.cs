using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace makatizen_app.Server.DTOs
{
    // =========================================================
    //               USER CREATION/UPDATE DTOs
    // =========================================================

    // DTO for creating a new user (System or Kit), used by AdminController.
    // FirstName and LastName are explicitly removed as requested.
    public class UserCreateDto
    {
        [Required]
        // Must be 1 (Super Admin), 2 (System User), or 3 (Kit User)
        public int UserType { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        // FirstName and LastName properties removed.
    }

    // DTO for returning System User data
    public class UserSystemReadDto
    {
        public int Id { get; set; }
        public int UserType { get; set; }
        public string Username { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public bool MustResetPassword { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool Status { get; set; }
    }

    // =========================================================
    //                 KIT USER DTOs (UserType 3)
    // =========================================================

    // DTO for returning Kit User data
    public class UserKitReadDto
    {
        public int Id { get; set; }
        public int UserType { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        // public string FullName => $"{FirstName} {LastName}"; // Uncomment if needed
        public bool MustResetPassword { get; set; }
        //public DateTime CreatedAt { get; set; } // Uncomment if Kit model includes CreatedAt
        public bool Status { get; set; }
    }
}