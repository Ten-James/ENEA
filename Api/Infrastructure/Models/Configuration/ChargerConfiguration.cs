using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Models.Configuration;

public class ChargerConfiguration : EntityBaseConfiguration<Charger>
{
    public void Configure(EntityTypeBuilder<Charger> builder)
    {
        base.Configure(builder);
    }
}