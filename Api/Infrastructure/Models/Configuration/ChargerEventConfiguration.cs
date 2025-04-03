using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Models.Configuration;

public class ChargerEventConfiguration : EntityBaseConfiguration<ChargerEvent>
{
    public void Configure(EntityTypeBuilder<ChargerEvent> builder)
    {
        base.Configure(builder);
    }
}