using Application.Common.Interfaces;
using Contracts.Events.UserEvents;
using Domain.Entities;
using Mapster;
using MassTransit;
using System.Threading.Tasks;

namespace Infrastructure.Consumers;

public sealed class UserRegisterConsumer : IConsumer<UserRegisterEvent>
{
    private readonly IUserRepository _userRepository;

    public UserRegisterConsumer(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Consume(ConsumeContext<UserRegisterEvent> context)
    {
        var user = new User() { UserId = context.Message.Id, Elo = 600 };
        await _userRepository.AddAsync(user, context.CancellationToken);
    }
}
