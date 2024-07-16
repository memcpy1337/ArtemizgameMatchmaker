using Application.Commands.User;
using Application.Common.DTOs;
using Application.Common.Models;
using Contracts.Events.MatchMakingEvents;
using Contracts.Events.UserEvents;
using Mapster;
using MassTransit;
using MediatR;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public sealed class AddUserToQueueConsumer : IConsumer<UserEnterQueuePlayEventRequest>
{
    private readonly IMediator _mediator;

    public AddUserToQueueConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<UserEnterQueuePlayEventRequest> context)
    {
        var queueData = context.Message.Adapt<UserQueueRequest>();

        var result = await _mediator.Send(new AddToQueueCommand(queueData), context.CancellationToken);

        var adapted = result.Adapt<IResponse<AddUserQueueResponse>>();

        await context.RespondAsync<UserEnterQueuePlayEventResponse>(new UserEnterQueuePlayEventResponse() { 
            Token = adapted.Data.Token
        });
    }
}
