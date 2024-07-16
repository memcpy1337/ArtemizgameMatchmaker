using Application.Common.DTOs;
using Application.Common.Interfaces;
using FluentValidation;

namespace Application.Common.Validators.Match;

public class UserQueueDtoValidator : AbstractValidator<UserQueueRequest>
{
    private readonly IUserRepository _userRepository;
    public UserQueueDtoValidator(IUserRepository userRepository)
    {

        _userRepository = userRepository;

        RuleFor(x => x.UserId).NotEmpty().NotNull().Must(NotInQueue).WithMessage("User already in queue or match");

        RuleFor(x => (int)x.PlayerType)
            .InclusiveBetween(0, 1)
            .WithMessage("Incorrect player type");

        RuleFor(x => (int)x.GameRegime).InclusiveBetween(0, 4).WithMessage("Incorrect game regime");
    }

    private bool NotInQueue(string userId)
    {
        return !_userRepository.Any(userId);
    }
}
