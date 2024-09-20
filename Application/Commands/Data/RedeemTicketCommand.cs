using Application.Common.DTOs.Data;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Wrappers;
using Contracts.Common.Models.Enums;
using Contracts.Events.UserEvents;
using Domain.Exceptions;
using Forbids;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Response = Application.Common.Models.Response;

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
    private readonly IRequestClient<UserInfoRequest> _clientUserInfo;

    public RedeemTicketCommandHandler(IForbid forbid, IQueueService queueService, IUserToMatchRepository userToMatch, IRequestClient<UserInfoRequest> clientUserInfo)
    {
        _queueService = queueService;
        _forbid = forbid;
        _userToMatchRepository = userToMatch;
        _clientUserInfo = clientUserInfo;
    }

    public async Task<IResponse<RedeemTicketResponse>> Handle(RedeemTicketCommand request, CancellationToken cancellationToken)
    {
        var data = await _userToMatchRepository.GetUserPlayDataByTicket(request.ticket, request.MatchId!);

        _forbid.Null(data, TicketIncorrentException.Instance);

        var response = await _clientUserInfo.GetResponse<UserInfoResponse>(new UserInfoRequest() { UserId = data!.UserId }, cancellationToken);

        _forbid.Null(response, TicketIncorrentException.Instance);

        return Response.Success(new RedeemTicketResponse() 
        { 
            NickName = response.Message.NickName,
            UserId = data!.User.Id,
            PlayerType = data!.UserType
        });
    }
}
