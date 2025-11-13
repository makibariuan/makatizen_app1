using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace makatizen_app.Server.Models
{
    [Table("UsersKit")]
    public class UsersKit :  IResettableUser, IStatusUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserType { get; set; } // e.g., 3 for Kit User

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        public bool MustResetPassword { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; }

        // JWT/Session Management
        [Column(TypeName = "nvarchar(max)")]
        public string? ActiveToken { get; set; }

        // One-Time PIN for General Verification
        public int? OneTimePIN { get; set; } // Added '?' as many related fields are nullable

        // Email Confirmation Fields
        public bool? IsEmailConfirmed { get; set; } // bit, null

        [Column(TypeName = "nvarchar(max)")]
        public string? EmailConfirmationToken { get; set; }

        public DateTime? EmailTokenExpiry { get; set; }

        // Password Reset Fields
        [Column(TypeName = "nvarchar(max)")]
        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiry { get; set; }

        // Login OTP Fields (for 2FA/MFA)
        [MaxLength(10)]
        public string? LoginOtp { get; set; }

        public DateTime? LoginOtpExpiry { get; set; }
    }
}