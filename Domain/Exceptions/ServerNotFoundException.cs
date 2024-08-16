using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions;

public class ServerNotFoundException : Exception
{
    private ServerNotFoundException() : base("Server can't be found.") { }
    public static ServerNotFoundException Instance { get; } = new();
}
