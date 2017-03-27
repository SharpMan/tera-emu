using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Controllers;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.Handlers
{
    public sealed class AuthHandler
    {
        public static object ticketProcess = new object();

        public static void ProcessAuth(WorldClient Client, string Packet)
        {
            // fake Packet
            if (Packet.Substring(0, 2) != "AT")
            {
                if (Client.Out.getDecryptor() != null)
                {
                    return;
                }
                Client.Send(new AccountTicketErrorMessage());
                return;
            }

            // fake Packet
            if (Packet.Length != 34)
            {
                Client.Send(new AccountTicketErrorMessage());
                return;
            }

            ProcessTicketResponce(Client, Packet.Substring(2));
        }

        public static void ProcessTicketResponce(WorldClient Client, string Ticket)
        {
            var a_ticket = TicketController.GetTicket(Ticket);
            lock (ticketProcess)
            {
                if (a_ticket == null)
                {
                    Client.Send(new AccountTicketErrorMessage());
                    return;
                }
                else
                {
                    Client.Account = a_ticket.Account;
                    if (CharacterTable.myCharacterById.Values.Where(x => x.Account != null && x.Account.ID == Client.Account.ID).Count() > 0)
                    {
                        CharacterTable.myCharacterById.Values.Where(x => x.Account != null && x.GetClient() != null && x.Account.ID == Client.Account.ID).ToList().ForEach(x => x.Client.Disconnect());
                        Client.Send(new AccountTicketErrorMessage());
                        return;
                    }
                    AccountTable.UpdateLogged(Client.Account.ID, true);
                    Client.Account.Characters = CharacterTable.FindAll(Client.Account.ID);
                    Client.Account.Characters.Values.ToList().ForEach(x => x.Account = Client.Account);
                    Client.SetState(WorldState.STATE_AUTHENTIFIED);
                    Client.Send(new AccountTicketSuccessMessage());
                    TicketController.DestroyTicket(a_ticket);
                    Client.Account.loadData();
                }
            }
        }
    }
}
