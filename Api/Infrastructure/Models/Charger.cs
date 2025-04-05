using Api.Infrastructure.Enums;
using Api.Infrastructure.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TenJames.DtoGenerator;

namespace Api.Infrastructure.Models;

[GenerateDto(DtoType.All)]
[EntityTypeConfiguration(typeof(ChargerConfiguration))]
public class Charger : EntityBase
{
    [Required]
    [StringLength(50)]
    public string ChargerCode { get; set; }

    [Required]
    public Guid ChargerGroupId { get; set; }

    [ForeignKey("ChargerGroupId")]

    //[DtoVisibility(DtoType.AllRead)]
    //[MapTo(typeof(ChargerGroupReadDto), "src.ChargerGroup")]
    [DtoIgnore]
    public virtual ChargerGroup ChargerGroup { get; set; }

    [Required]
    public ChargerStatus CurrentStatus { get; set; }

    // One charger can have many events (status changes and charging sessions)
    [DtoVisibility(DtoType.ReadDetail)]
    public virtual ICollection<ChargerEvent> Events { get; set; }
}