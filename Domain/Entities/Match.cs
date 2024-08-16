
using Contracts.Common.Models.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Entities;
public class Match
{
    public required string Id { get; set; }
    public GameTypeEnum Regime { get; set; }
    public required string OwnerUserId { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateCreated { get; set; }
    public DateTime DateFinish { get; set; }
    public ICollection<UserToMatch> Users { get; } = new List<UserToMatch>();
    public MatchStatusEnum Status { get; set; } = MatchStatusEnum.Init;
}
