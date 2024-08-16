using Contracts.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class UserToMatch
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public User User { get; set; }
    public required string MatchId { get; set; }
    public Match Match { get; set; }
    public required string Ticket { get; set; }
    public PlayerTypeEnum UserType { get; set; }
    public required string UserIp { get; set; }
    public bool IsActive { get; set; }
    public bool IsConnected { get; set; } = false;
}
