using Contracts.Common.Models.Enums;
using Domain.Entities;
using Netjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces;

[InjectAsTransient]
public interface IPublishService
{
    Task NewMatchCreated(string matchId, GameTypeEnum regime);
    Task UserAddToMatch(string matchId, string userId, bool serverIsReady);

}
