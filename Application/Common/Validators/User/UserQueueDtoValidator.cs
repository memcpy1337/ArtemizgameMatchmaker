using Application.Common.DTOs;
using Application.Common.DTOs.Match;
using Application.Common.Interfaces;
using FluentValidation;

namespace Application.Common.Validators.Match;

public class PlayUserRequestValidator : AbstractValidator<PlayUserRequest>
{
    public PlayUserRequestValidator()
    {

        RuleFor(x => (int)x.PlayerType)
            .InclusiveBetween(0, 1)
            .WithMessage("Incorrect player type");

        RuleFor(x => (int)x.GameRegime).InclusiveBetween(0, 4).WithMessage("Incorrect game regime");
    }

}
