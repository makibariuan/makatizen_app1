using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace makatizen_app.Server.Models
{
    [Table("UsersSystem")]
    public class UsersSystem: IResettableUser, IStatusUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserType { get; set; } // 1 for Super Admin, 2 for System User

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

        //public DateTime BirthDate { get; set; }
        //[Required]
        //[MaxLength(500)]

        public string PasswordHash { get; set; } = string.Empty;

        // --- Core System Flags (Previously implemented) ---
        public bool MustResetPassword { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // JWT/Session Management
        [Column(TypeName = "nvarchar(max)")]
        public string? ActiveToken { get; set; }

        // One-Time PIN for General Verification (int, not null)
        public int OneTimePIN { get; set; }

        // Email Confirmation Fields
        public bool? IsEmailConfirmed { get; set; }

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
        public bool IsActive { get; set; }
    }
}