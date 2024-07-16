using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


namespace Infrastructure.Persistance;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        return await base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Server> Servers { get; set; }
    public DatabaseFacade DatabaseFescade { get { return base.Database; } }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<User>()
            .HasKey(i => i.Id);

        builder.Entity<Match>().HasKey(i => i.Id);

        builder.Entity<Match>().HasMany(e => e.Users).WithOne(e => e.Match).HasForeignKey(e => e.MatchId).IsRequired(false);

        builder.Entity<User>().HasIndex(x => x.UserId).IncludeProperties(b => b.IsActive);

        builder.Entity<Match>().HasIndex(x => x.MatchId).IncludeProperties(b => b.IsActive);

        builder.Entity<Server>().HasOne(e => e.Match).WithOne(e => e.Server).HasForeignKey<Server>(e => e.MatchId).IsRequired();

        base.OnModelCreating(builder);
    }

}
