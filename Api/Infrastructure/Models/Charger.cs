using Api.Infrastructure.Enums;
using Api.Infrastructure.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Infrastructure.Models;

[EntityTypeConfiguration(typeof(ChargerConfiguration))]
public class Charger : EntityBase
{
    [Required]
    [StringLength(50)]
    public string ChargerCode { get; set; }

    [Required]
    public Guid ChargerGroupId { get; set; }

    [ForeignKey("ChargerGroupId")]
    public virtual ChargerGroup ChargerGroup { get; set; }

    [Required]
    public ChargerStatus CurrentStatus { get; set; }

    // One charger can have many events (status changes and charging sessions)
    public virtual ICollection<ChargerEvent> Events { get; set; }
}