using System.ComponentModel.DataAnnotations;

public class ResetPasswordRequest
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string CurrentPassword { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "New password must be at least 8 characters.")]
    public string NewPassword { get; set; }
}