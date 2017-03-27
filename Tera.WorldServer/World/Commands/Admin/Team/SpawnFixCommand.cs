using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.Libs.Utils;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Team
{
    public class SpawnFixCommand : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "spawnfix";
            }
        }

        public override int AccessLevel
        {
            get
            {
                return 4;
            }
        }

        public override string Description
        {
            get
            {
                return "Ajoute un group de monstre static";
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
                client.GetCharacter().myMap.AddStaticGroup(new Couple<int,String>(client.Character.CellId, packet));
                MobGroupFixTable.Update(client.Character.myMap.Id, client.Character.CellId, packet);
                client.Character.RefreshOnMap();
                client.Send(new ConsoleMessage("Le groupe de monstre a été ajouté avec succès !", ConsoleColorEnum.GREEN));
            }
            else
            {
                client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
            }
        }
    }
}
