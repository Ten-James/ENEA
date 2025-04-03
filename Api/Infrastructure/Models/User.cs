using Api.Infrastructure.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TenJames.DtoGenerator;

namespace Api.Infrastructure.Models;

[GenerateDto(DtoType.All)]
[EntityTypeConfiguration(typeof(UserConfiguration))]
public class User : EntityBase
{
    /// <summary>
    ///     Login name
    /// </summary>
    /// <remarks>Specification by project assigment</remarks>
    /// <example>Log004</example>
    [Required]
    public string Name { get; set; }

    public string? Email { get; set; }

    [DtoIgnore][Required] public string Password { get; set; } = string.Empty;
}