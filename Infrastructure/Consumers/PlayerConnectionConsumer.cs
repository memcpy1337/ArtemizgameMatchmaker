using Contracts.Events.ServerEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public class PlayerConnectionConsumer : IConsumer<ServerPlayerConnectedEvent>
{
    public Task Consume(ConsumeContext<ServerPlayerConnectedEvent> context)
    {
        throw new NotImplementedException();
    }
}
