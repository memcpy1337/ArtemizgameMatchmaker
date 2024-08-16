using Application.Common.DTOs.Data;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Wrappers;
using Contracts.Common.Models.Enums;
using Domain.Exceptions;
using Forbids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Data;

public record RedeemTicketCommand(string ticket) : IRequestWrapper<RedeemTicketResponse>, IAuthorizedServer
{
    public string? ServerId { get; set; }
    public string? MatchId { get; set; }
}

internal sealed class RedeemTicketCommandHandler : IHandlerWrapper<RedeemTicketCommand, RedeemTicketResponse>
{
    private readonly IForbid _forbid;
    private readonly IQueueService _queueService;
    private readonly IUserToMatchRepository _userToMatchRepository;

    public RedeemTicketCommandHandler(IForbid forbid, IQueueService queueService, IUserToMatchRepository userToMatch)
    {
        _queueService = queueService;
        _forbid = forbid;
        _userToMatchRepository = userToMatch;
    }

    public async Task<IResponse<RedeemTicketResponse>> Handle(RedeemTicketCommand request, CancellationToken cancellationToken)
    {
        var data = await _userToMatchRepository.GetUserPlayDataByTicket(request.ticket, request.MatchId!);

        _forbid.Null(data, TicketIncorrentException.Instance);

        return Response.Success(new RedeemTicketResponse() 
        { 
            NickName = data!.User.Elo.ToString(),
            UserId = data!.User.Id,
            PlayerType = data!.UserType
        });
    }
}
