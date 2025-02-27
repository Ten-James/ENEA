using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Models.Configuration;

public class EntityBaseConfiguration<T>: IEntityTypeConfiguration<T> where T : EntityBase
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValue(DateTime.Now);

        builder.Property(e => e.UpdatedAt)
            .HasDefaultValue(DateTime.Now);
    }
}