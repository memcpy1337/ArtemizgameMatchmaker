using Contracts.Events.UserEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Publishers;

public class UserRequest
{
    IRequestClient<UserInfoRequest> _clientUserInfo;

    public async Task<UserInfoResponse> 
}
