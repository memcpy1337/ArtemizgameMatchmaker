using Domain.Entities;
using Netjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

[InjectAsScoped]
public interface IMatchRepository
{
    Task<Match> CreateAndAdd(User player);
    Task<Match?> GetMatchForPlayer(User player, int eloMin, int eloMax);
}
