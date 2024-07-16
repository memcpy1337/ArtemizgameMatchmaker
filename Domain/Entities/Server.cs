using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Server
{
    public int Id { get; set; }
    public required string ServerId { get; set; }
    public bool IsReady { get; set; }
    public bool IsActive { get; set; }
    public string Ip { get; set; } = string.Empty;
    public int Port { get; set; }
    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;
}
