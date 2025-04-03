using System.ComponentModel.DataAnnotations;
using TenJames.DtoGenerator;

namespace Api.Infrastructure.Models;

public class EntityBase
{
    /// <summary>
    ///     Identifier of the entity
    /// </summary>
    [Required]
    [Key]
    [DtoVisibility(DtoType.AllRead)]
    public Guid Id { get; set; }

    /// <summary>
    ///     Date and time of entity creation
    /// </summary>
    [DtoVisibility(DtoType.ReadDetail)]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Date and time of entity update
    /// </summary>
    [DtoVisibility(DtoType.ReadDetail)]
    public DateTime UpdatedAt { get; set; }
}