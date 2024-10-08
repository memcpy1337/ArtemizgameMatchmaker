﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<UserToMatch> UserToMatches { get; set; }
    public DatabaseFacade DatabaseFescade { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
