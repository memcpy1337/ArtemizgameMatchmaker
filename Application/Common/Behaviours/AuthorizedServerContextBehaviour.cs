using Application.Common.Interfaces;
using Domain.Exceptions;
using Forbids;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours;

public class AuthorizedServerContextBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IForbid _forbid;
    private readonly ILogger<AuthorizedServerContextBehaviour<TRequest, TResponse>> _logger;

    public AuthorizedServerContextBehaviour(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IForbid forbid, ILogger<AuthorizedServerContextBehaviour<TRequest, TResponse>> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _forbid = forbid;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            if (request is IAuthorizedServer serverRequest)
            {
                var serverId = httpContext.User.Identity!.Name;
                var matchId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(serverId) || string.IsNullOrEmpty(matchId))
                {
                    _logger.LogWarning($"Server authorization failed");
                    throw ServerNotFoundException.Instance;
                }

                serverRequest.ServerId = serverId;
                serverRequest.MatchId = matchId;
            }
        }

        return await next();
    }
}
