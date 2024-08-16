using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Server
{
    public required string Id { get; set; }
    public bool IsReady { get; set; }
    public bool IsActive { get; set; }
    public required string MatchId { get; set; }
    public Match Match { get; set; } = null!;
}
