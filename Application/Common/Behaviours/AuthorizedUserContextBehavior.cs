using Application.Common.Interfaces;
using Domain.Exceptions;
using Forbids;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours;

public class AuthorizedUserContextBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IForbid _forbid;

    public AuthorizedUserContextBehavior(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IForbid forbid)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _forbid = forbid;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var userId = httpContext.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var userName = httpContext.User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;

            if (request is IAuthorizedUser userRequest)
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
                {
                    throw UserNotFoundException.Instance;
                }

                var userData = await _userRepository.GetAsync(userId, cancellationToken);

                _forbid.Null(userData, UserNotFoundException.Instance);

                userRequest.User = userData!;
            }
        }

        return await next();
    }
}
