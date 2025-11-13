using System.ComponentModel.DataAnnotations;

namespace makatizen_app.Server.DTOs
{
    // DTO for toggling the active status of a System or Kit User
    public class ToggleUserStatusRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}