using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Wrappers;
using Domain.Entities;
using Forbids;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Match;

public record CancelSearchCommand() : IRequestWrapper<Unit>, IAuthorizedUser
{
    public User User { get; set; }
}

internal sealed class CancelSearchCommandHandler : IHandlerWrapper<CancelSearchCommand, Unit>
{
    private readonly IQueueService _queueService;

    public CancelSearchCommandHandler(IForbid forbid, IQueueService queueService, IUserToMatchRepository userToMatch, IMatchService matchService)
    {
        _queueService = queueService;
    }

    public async Task<IResponse<Unit>> Handle(CancelSearchCommand request, CancellationToken cancellationToken)
    {
        await _queueService.RemoveUserFromQueueAsync(request.User.Id, CancellationToken.None);

        return Response.Success(Unit.Value);
    }
}