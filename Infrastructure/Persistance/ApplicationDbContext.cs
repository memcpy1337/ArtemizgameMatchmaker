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
    public DbSet<UserToMatch> UserToMatches { get; set; }

    public DatabaseFacade DatabaseFescade { get { return base.Database; } }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


        base.OnModelCreating(builder);
    }

}
