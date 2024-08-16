using Contracts.Common.Models.Enums;

namespace Application.Common.DTOs.Data;

public class RedeemTicketResponse
{
    public PlayerTypeEnum PlayerType { get; set; }
    public required string UserId { get; set; }
    public required string NickName { get; set; }
}
