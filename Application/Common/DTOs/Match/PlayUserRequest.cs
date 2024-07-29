using Contracts.Common.Models.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.DTOs.Match
{
    public class PlayUserRequest
    {
        [SwaggerSchema(Required = new[] { "Player Type (crew or hunt)" })]
        public PlayerTypeEnum PlayerType { get; set; }

        [SwaggerSchema(Required = new[] { "Game Regime (like 1vs1 or other)" })]
        public GameTypeEnum GameRegime { get; set; }
    }
}
