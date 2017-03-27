using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.Libs;

namespace Tera.WorldServer.World.Controllers
{
    public static class TicketController
    {
        public static Dictionary<string, AccountTicket> Tickets = new Dictionary<string, AccountTicket>();

        public static void RegisterTicket(AccountTicket ticket)
        {
            ticket.ExpireTime = Environment.TickCount;

            lock (Tickets)
                Tickets.Add(ticket.Ticket, ticket);

            Logger.Debug("Registered new ticket for account @'" + ticket.Account.Username + "'@");
        }

        public static AccountTicket GetTicket(string ticket)
        {
            lock (Tickets)
            {
                if (Tickets.ContainsKey(ticket))
                {
                    return Tickets[ticket];
                }
                else
                {
                    return null;
                }
            }
        }

        public static void DestroyTicket(AccountTicket ticket)
        {
            lock (Tickets)
            {
                if (Tickets.ContainsKey(ticket.Ticket))
                {
                    Tickets.Remove(ticket.Ticket);
                    Logger.Debug("Destroyed ticket for account @'" + ticket.Account.Username + "'@");
                }
            }
        }
    }
}
