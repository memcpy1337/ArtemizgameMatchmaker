using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using Contracts.Events.ServerEvents;
using MassTransit;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public class ServerReadyConsumer : IConsumer<ServerReadyEvent>
{
    private readonly IMatchService _matchService;
    public ServerReadyConsumer(IMatchService matchService)
    {
        _matchService = matchService;
    }

    public async Task Consume(ConsumeContext<ServerReadyEvent> context)
    {
        await _matchService.UpdateMatchStatus(MatchStatusEnum.WaitPlayerConnect, context.Message.MatchId);
    }
}
