using System.ComponentModel.DataAnnotations;
using Domain;

namespace Api.Infrastructure.Models;

public class EntityBase: IIdentifier
{
    [Required]
    [Key]
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}