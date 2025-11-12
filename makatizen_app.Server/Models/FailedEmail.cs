using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace makatizen_app.Server.Models
{
    // Corresponds to dbo.FailedEmails (For logging email delivery errors)
    [Table("FailedEmails")]
    public class FailedEmails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string ToEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(512)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Body { get; set; } = string.Empty;

        // ErrorMessage is nullable in schema
        [Column(TypeName = "nvarchar(max)")]
        public string? ErrorMessage { get; set; }

        [Required]
        public int RetryCount { get; set; } = 0;

        // Use DateTime? for nullable datetime2(7) in SQL
        public DateTime? LastTriedAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // e.g., "Failed"
    }
}