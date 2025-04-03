using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Models.Configuration;

public class ChargerGroupConfiguration : EntityBaseConfiguration<ChargerGroup>
{
    public void Configure(EntityTypeBuilder<ChargerGroup> builder)
    {
        base.Configure(builder);
    }
}