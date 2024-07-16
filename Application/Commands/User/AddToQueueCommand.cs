using Application.Common.DTOs;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Wrappers;
using FluentValidation;
using System;

using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.User;

public record AddToQueueCommand(UserQueueRequest UserQueueRequest) : IRequestWrapper<AddUserQueueResponse>;

internal sealed class AddToQueueCommandHandler : IHandlerWrapper<AddToQueueCommand, AddUserQueueResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UserQueueRequest> _validator;
    private readonly IQueueService _queueService;

    public AddToQueueCommandHandler(IUserRepository userRepository, IValidator<UserQueueRequest> validator, IQueueService queueService)
    {
        _userRepository = userRepository;
        _validator = validator;
        _queueService = queueService;
    }

    public async Task<IResponse<AddUserQueueResponse>> Handle(AddToQueueCommand request, CancellationToken cancellationToken)
    {
        var validation = await _validator.ValidateAsync(request.UserQueueRequest, cancellationToken);

        if (!validation.IsValid)
        {
            throw new System.Exception(validation.Errors[0].ErrorMessage);
        }

        var ticket = Guid.NewGuid().ToString();

        await _userRepository.AddAsync(new Domain.Entities.User()
        {
            UserId = request.UserQueueRequest.UserId,
            Elo = request.UserQueueRequest.Elo,
            PlayerType = request.UserQueueRequest.PlayerType,
            Regime = request.UserQueueRequest.GameRegime,
            Ticket = ticket
        }, cancellationToken);

        await _queueService.AddedUserToQueueAsync(request.UserQueueRequest.UserId, ticket, cancellationToken);

        return Response.Success(new AddUserQueueResponse()
        {
            Token = ticket
        });
    }
}
