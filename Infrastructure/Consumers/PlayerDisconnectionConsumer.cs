using Contracts.Events.ServerEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public class PlayerDisconnectionConsumer : IConsumer<ServerPlayerDisconnectedEvent>
{
    public Task Consume(ConsumeContext<ServerPlayerDisconnectedEvent> context)
    {
        throw new NotImplementedException();
    }
}