using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Team
{
    public class RebootCommand : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "reboot";
            }
        }

        public override int AccessLevel
        {
            get
            {
                return 6;
            }
        }

        public override string Description
        {
            get
            {
                return "Metttre hors ligne le serveur";
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
                DatabaseCache.Save();
                Network.WorldServer.Clients.ForEach(x => x.Disconnect());
                Environment.Exit(0);
            }
            else
            {
                client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
            }
        }
    }
}
