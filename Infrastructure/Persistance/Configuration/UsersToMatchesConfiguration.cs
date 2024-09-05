using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Configuration;

public class UsersToMatchesConfiguration : IEntityTypeConfiguration<UserToMatch>
{
    public void Configure(EntityTypeBuilder<UserToMatch> builder)
    {
        builder.HasKey(um => new { um.UserId, um.MatchId });

        builder.HasOne(um => um.User)
        .WithMany()
        .HasForeignKey(um => um.UserId);

        builder
            .HasOne(ut => ut.Match)
            .WithMany(m => m.Users)
            .HasForeignKey(ut => ut.MatchId);

        builder.HasIndex(x => x.MatchId).IncludeProperties(b => b.IsActive).IncludeProperties(b => b.IsConnected);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Ticket);
    }
}
