using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.DTOs;
using Application.Common.DTOs.Match;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Validators.Match;
using Application.Common.Wrappers;
using Domain.Entities;
using Domain.Exceptions;
using FluentValidation;
using Forbids;
using Mapster;

namespace Application.Commands.Match;

public record PlayUserCommand(PlayUserRequest PlayUserRequest) : IRequestWrapper<PlayResponse>, IAuthorizedUser, IUserIp
{
    public User User { get; set; }
    public string UserIp { get; set; }
}

internal sealed class PlayUserCommandHandler : IHandlerWrapper<PlayUserCommand, PlayResponse>
{
    private readonly IForbid _forbid;
    private readonly IQueueService _queueService;
    private readonly IUserToMatchRepository _userToMatchRepository;

    public PlayUserCommandHandler(IForbid forbid, IQueueService queueService, IUserToMatchRepository userToMatch)
    {
        _queueService = queueService;
        _forbid = forbid;
        _userToMatchRepository = userToMatch;
    }

    public async Task<IResponse<PlayResponse>> Handle(PlayUserCommand request, CancellationToken cancellationToken)
    {
        var isInQueue = await _queueService.IsInQueue(request.User.UserId);
        var isInMatch = await _userToMatchRepository.IsUserInMatch(request.User);

        _forbid.True(isInQueue || isInMatch, UserAlreadyInMatchException.Instance);

        var ticket = Guid.NewGuid();

#if DEBUG
        request.UserIp = "8.8.8.8"; //fix cant fetch ip in docker desktop cause NAT
#endif

        await _queueService.AddUserToQueueAsync(new UserQueueRequest()
        {
            UserId = request.User.UserId,
            UserIp = request.UserIp,
            Elo = request.User.Elo,
            GameRegime = request.PlayUserRequest.GameRegime,
            PlayerType = request.PlayUserRequest.PlayerType,
            Ticket = ticket
        }, cancellationToken);

        return Response.Success(new PlayResponse() { Ticket = ticket });
    }
}