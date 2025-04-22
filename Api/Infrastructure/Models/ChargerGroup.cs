using Api.Infrastructure.Models.Configuration;
using GeneratedDtos;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TenJames.DtoGenerator;

namespace Api.Infrastructure.Models;

[GenerateDto(DtoType.AllRead)]
[EntityTypeConfiguration(typeof(ChargerGroupConfiguration))]
public class ChargerGroup : EntityBase
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    [StringLength(500)]
    [Required]
    public string Address { get; set; }

    // One station can have many chargers
    [DtoVisibility(DtoType.AllRead)]
    [MapTo(typeof(ICollection<ChargerReadDto>), "src.Chargers")]
    public virtual ICollection<Charger> Chargers { get; set; }
}