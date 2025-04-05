using Api.Infrastructure.Enums;
using Api.Infrastructure.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Infrastructure.Models;

[EntityTypeConfiguration(typeof(ChargerEventConfiguration))]
public class ChargerEvent : EntityBase
{
    [Required]
    public Guid ChargerId { get; set; }

    [ForeignKey("ChargerId")]
    public virtual Charger Charger { get; set; }

    [Required]
    public EventType EventType { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    [NotMapped]
    public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;

    // Status change specific fields
    public ChargerStatus? OldStatus { get; set; }
    public ChargerStatus? NewStatus { get; set; }
    public string? ChangedBy { get; set; }
    public string? Notes { get; set; }


    [Required]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }

    public ChargingSessionStatus? SessionStatus { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public double? TotalPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public double? EnergyConsumed { get; set; } // in kWh

    public bool IsCompleted { get; set; }
}