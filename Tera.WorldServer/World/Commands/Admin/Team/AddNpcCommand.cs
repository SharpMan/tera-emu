using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Team
{
    public sealed class AddNpcCommand : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "addnpc";
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
                return "Ajout un Pnj sur la map";
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
                int npcid;
                if (!int.TryParse(packet, out npcid))
                {
                    client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
                    return;
                }
                var npc = client.Character.myMap.addNpc(npcid, client.Character.CellId, client.Character.Orientation);
                if (npc == null)
                {
                    client.Send(new ConsoleMessage("Echoue de l'ajout du pnj invalide !", ConsoleColorEnum.RED));
                    return;
                }
                client.Character.myMap.InitNpc(npc);
                NpcTemplateTable.Add(npcid, client.Character);
                client.Send(new ConsoleMessage("PNJ ajouté sur la map avec succès !", ConsoleColorEnum.GREEN));
            }
            else
            {
                client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
            }
        }
    }
}
