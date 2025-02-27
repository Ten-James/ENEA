using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Infrastructure.Models.Configuration;

public class UserConfiguration: EntityBaseConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(x => x.Email)
            .IsUnique();

        base.Configure(builder);
    }
}