using Application.Common.Interfaces;
using Forbids;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Behaviours;

public class UserIpContextBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserIpContextBehaviour(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            if (request is IUserIp userRequest)
            {
                var realIp = httpContext.Request.Headers["X-Real-Ip"].ToString();

                if (!string.IsNullOrEmpty(realIp))
                {
                    userRequest.UserIp = realIp;
                }
            }
        }

        return await next();
    }
}
