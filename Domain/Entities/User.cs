

using Contracts.Common.Models.Enums;
using System;

namespace Domain.Entities;

public class User
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public int Elo { get; set; }
    public int? MatchId { get; set; }
    public Match? Match { get; set; }
    public PlayerTypeEnum PlayerType { get; set; }
    public GameTypeEnum Regime { get; set; }
    public required string Ticket { get; set; }
    public bool IsActive { get; set; } = true;
}