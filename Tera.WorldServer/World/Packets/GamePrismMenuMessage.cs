using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GamePrismMenuMessage : PacketBase
    {
        public Player character;

        public GamePrismMenuMessage(Player character)
        {
            this.character = character;
        }


        public override string Compile()
        {
            //Je pense que mettre ça en cache est con x)
            StringBuilder sb = new StringBuilder("Wp");
            sb.Append(character.Map);
            int subAreaID = character.myMap.subArea.area.superArea.ID;
            foreach (Prisme prisma in PrismeTable.Cache.Values.Where(x => x.Alignement == character.Alignement))
            {
                short mapid = prisma.Mapid;
                if (MapTable.Get(mapid) == null || MapTable.Get(mapid).subArea.area.superArea.ID != subAreaID)
                    continue;
                if (prisma.CurrentFight != null || prisma.inFight == -2)
                {
                    sb.Append("|" + mapid + ";*");
                }
                else
                {
                    int costo = ZaapTable.calculZaapCost(character.myMap, (MapTable.Get(mapid)));
                    if (mapid == character.myMap.Id)
                        costo = 0;
                    sb.Append("|" + mapid + ";" + costo);
                }
            }

            return sb.ToString();
        }
    }
}
