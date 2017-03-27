using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Team
{
    public class TeleportCommand : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "teleport";
            }
        }

        public override int AccessLevel
        {
            get
            {
                return 1;
            }
        }

        public override string Description
        {
            get
            {
                return "Vous teleporte ou teleporte un joueur";
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
            var mapid = parameters.getShortParamater(0);
            var cellid = parameters.GetIntParameter(1);
            var toTeleport = client.Character;

            if (parameters.Lenght > 2)
            {
                var playerName = parameters.GetParameter(2);
                toTeleport = CharacterTable.GetCharacter(playerName);
            }

            if (toTeleport != null)
            {
                var NextMap = MapTable.Get(mapid);
                if (NextMap == null)
                {
                    client.Send(new ConsoleMessage("Mapid invalide"));
                    return;
                }
                toTeleport.Teleport(NextMap, cellid);
                client.Send(new ConsoleMessage("Teleportation effectuer"));
            }
            else
            {
                 client.Send(new ConsoleMessage("Impossible de trouver le joueur", ConsoleColorEnum.RED));
            }
        }
    }
}
