using Application.Common.DTOs;
using Contracts.Common.Models.Enums;
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
    Task<Match> CreateAsync(GameTypeEnum gameType, User owner);
    Task<Match?> GetMatchWithParams(int eloMin, int eloMax, PlayerTypeEnum playerType, GameTypeEnum gameType);
    Task<Match?> Get(string matchId);
    Task UpdateStatus(MatchStatusEnum status, string matchId);
}
