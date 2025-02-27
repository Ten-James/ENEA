using System.ComponentModel.DataAnnotations;
using Api.Infrastructure.Models.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Api.Infrastructure.Models;

[EntityTypeConfiguration(typeof(UserConfiguration))]
public class User: EntityBase
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}