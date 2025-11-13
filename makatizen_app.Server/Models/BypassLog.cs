using makatizen_app.Server.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("BypassLog")]
public class BypassLog
{
    // Assuming an auto-incrementing key for EF Core, even if not shown in DB screenshot
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // Foreign Key to Citizen
    public int? PersonId { get; set; }
    [ForeignKey(nameof(PersonId))]
    public Citizen? Citizen { get; set; }

    // StepName (nvarchar(100), null)
    [MaxLength(100)]
    public string? StepName { get; set; }

    // ReasonCode (nvarchar(100), null) - e.g., "NO_RIGHT_FINGERS"
    [MaxLength(100)]
    public string? ReasonCode { get; set; }

    // ReasonDetails (nvarchar(500), null)
    [MaxLength(500)]
    public string? ReasonDetails { get; set; }

    // DateBypassed (datetime, null)
    public DateTime? DateBypassed { get; set; }
}