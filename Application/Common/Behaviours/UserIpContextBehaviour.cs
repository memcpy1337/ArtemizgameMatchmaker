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
                var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].ToString();
                var forwardedForIp = httpContext.Request.Headers["X-Real-Ip"].ToString();

                Console.WriteLine(forwardedForIp);

                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    Console.WriteLine(forwardedFor);
                    var ipAddresses = forwardedFor.Split(',');
                    userRequest.UserIp = ipAddresses[0];
                }
            }
        }

        return await next();
    }
}
