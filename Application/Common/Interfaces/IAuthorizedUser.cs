using Domain.Entities;

namespace Application.Common.Interfaces;

public interface IAuthorizedUser
{
    User User { get; set; }
}
