using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistance.Configuration;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(i => i.Id);

        builder.HasIndex(x => x.Id).IncludeProperties(b => b.IsActive);

    }
}
