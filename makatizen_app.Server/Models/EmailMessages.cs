using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace makatizen_app.Server.Models
{
    // Corresponds to dbo.EmailMessages (For successful/pending emails)
    [Table("EmailMessages")]
    public class EmailMessages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string To { get; set; } = string.Empty;

        [Required]
        [MaxLength(512)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Body { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty; // e.g., "Sent", "Pending"

        [Required]
        public int RetryCount { get; set; } = 0;

        // Use DateTime? for nullable datetime2(7) in SQL
        public DateTime? LastTriedAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}