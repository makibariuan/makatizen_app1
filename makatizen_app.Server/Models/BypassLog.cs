using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace makatizen_app.Server.Models
{
    // Note: Since your SQL didn't define a PK, I'm adding one (Id) 
    // and setting the original columns as they were defined.
    public class BypassLog
    {
        [Key] // Primary Key for EF Core
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PersonID { get; set; }

        [MaxLength(100)]
        public string StepName { get; set; }

        [MaxLength(100)]
        public string ReasonCode { get; set; }

        [MaxLength(500)]
        public string ReasonDetails { get; set; }

        public DateTime DateBypassed { get; set; }
    }
}
