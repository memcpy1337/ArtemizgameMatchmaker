using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions;

public class TicketIncorrentException : Exception
{
    private TicketIncorrentException() : base("Ticket invalid") { }
    public static TicketIncorrentException Instance { get; } = new();
}
