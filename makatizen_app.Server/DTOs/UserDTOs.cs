using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace makatizen_app.Server.DTOs
{
    // =========================================================
    //               USER CREATION/UPDATE DTOs
    // =========================================================

    // DTO for creating a new user (System or Kit), used by AdminController.
    // FirstName and LastName are explicitly removed as requested.


    // DTO for creating a new user (System or Kit), used by AdminController.
    public class UserCreateDto
    {
        [Required]
        public int UserType { get; set; } // Must be 1 (Super Admin), 2 (System User), or 3 (Kit User)

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;
    }

    // DTO for returning System User data
    public class UserSystemReadDto
    {
        public int Id { get; set; }
        public int UserType { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool MustResetPassword { get; set; }
        public bool Status { get; set; }
    }

    // DTO for returning Kit User data
    public class UserKitReadDto
    {
        public int Id { get; set; }
        public int UserType { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool MustResetPassword { get; set; }
        public bool Status { get; set; }
    }
}