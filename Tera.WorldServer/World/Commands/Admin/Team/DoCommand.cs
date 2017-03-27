using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Team
{
    public class DoCommand : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "do";
            }
        }

        public override int AccessLevel
        {
            get
            {
                return 3;
            }
        }

        public override string Description
        {
            get
            {
                return "Genere un packet";
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
                    var packet = parameters.GetParameter(0);
                    client.Send(packet);
                    client.Send(new ConsoleMessage("Le packet a été envoyé avec succès !", ConsoleColorEnum.GREEN));
            }
            else
            {
                client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
            }
        }
    }
}
