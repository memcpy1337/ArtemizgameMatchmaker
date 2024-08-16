using Application.Common.Interfaces;
using Contracts.Common.Models.Enums;
using Contracts.Events.ServerEvents;
using MassTransit;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public class ServerDeployConsumer : IConsumer<ServerDeployEvent>
{
    private readonly IMatchService _matchService;
    public ServerDeployConsumer(IMatchService matchService)
    {
        _matchService = matchService;
    }

    public async Task Consume(ConsumeContext<ServerDeployEvent> context)
    {
        if (context.Message.IsSuccess)
        {
            await _matchService.UpdateMatchStatus(MatchStatusEnum.WaitForServer, context.Message.MatchId);
        }
        else
        {
            await _matchService.CancelMatch(context.Message.MatchId, MatchCancelEnum.InternalError);
        }
    }
}
