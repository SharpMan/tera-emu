using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Team
{
    public class AllCommand : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "all";
            }
        }

        public override int AccessLevel
        {
            get
            {
                return 2;
            }
        }

        public override string Description
        {
            get
            {
                return "Envoyez un message à tout les clients";
            }
        }

        public override bool NeedLoaded
        {
            get
            {
                return true;
            }
        }

        public override void Execute(WorldClient client, CommandParameters parameters)
        {
            if (parameters.Lenght > 0)
            {
                try
                {
                    char[] a = { ' ' };
                    var message = parameters.GetFullPameters.Split(a, 1, StringSplitOptions.None);
                    String prefix = "(Staff) <b>" + client.Character.Name + "</b> : ";
                    Network.WorldServer.Clients.ForEach(x => x.Send(new ChatGameMessage(prefix + message[0], "FF0000")));
                    client.Send(new ConsoleMessage("Le message a été envoyé ! ", ConsoleColorEnum.GREEN));
                }
                catch (Exception e)
                {
                    client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
                    return;
                }
            }
            else
            {
                client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
            }
        }
    }
}
