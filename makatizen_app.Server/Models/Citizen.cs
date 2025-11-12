using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace makatizen_app.Server.Models
{
    [Table("Citizen")]
    public class Citizen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CitizenType { get; set; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

    
        [Column(TypeName = "smalldatetime")]
        public DateTime BirthDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for biometrics
        public virtual ICollection<BiometricDataEnrollment> BiometricEnrollments { get; set; } = new List<BiometricDataEnrollment>();

        // Note: For 'Citizen can only read their biometrics' logic, 
        // if they need to log in, you must add Email/Username and PasswordHash columns here. 
        // This is not in the current DB schema image and is left as a future design decision.
    }
}