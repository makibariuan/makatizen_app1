using System.ComponentModel.DataAnnotations;

namespace makatizen_app.Server.DTOs
{
    public class BypassLogCreateDto
    {
        [Required]
        public int PersonID { get; set; }

        [Required]
        [StringLength(100)]
        public string StepName { get; set; }

        [Required]
        [StringLength(100)]
        public string ReasonCode { get; set; }

        [StringLength(500)]
        public string ReasonDetails { get; set; }
    }
}