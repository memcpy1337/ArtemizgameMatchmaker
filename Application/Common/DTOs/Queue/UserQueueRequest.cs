using Contracts.Common.Models.Enums;
using System;

namespace Application.Common.DTOs;

public class UserQueueRequest
{
    public required string UserId { get; set; }
    public required string UserIp { get; set; }
    public int Elo { get; set; }
    public PlayerTypeEnum PlayerType { get; set; }
    public GameTypeEnum GameRegime { get; set; }
    public Guid Ticket { get; set; }

}
