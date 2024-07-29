using Contracts.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class UserToMatch
{
    public int UserId { get; set; }
    public User User { get; set; }
    public int MatchId { get; set; }
    public Match Match { get; set; }
    public PlayerTypeEnum UserType { get; set; }
    public required string UserIp { get; set; }
    public bool IsActive { get; set; }
    public bool IsConnected { get; set; } = false;
}
