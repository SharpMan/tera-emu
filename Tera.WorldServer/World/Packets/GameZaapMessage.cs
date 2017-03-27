using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameZaapMessage : PacketBase
    {
        public Player character;

        public GameZaapMessage(Player character)
        {
            this.character = character;
        }

        public override string Compile()
        {
            StringBuilder sb = new StringBuilder("WC");
            String map = character.myMap.Id + "";
            try
            {
                map = character.SavePos.Split(',')[0];
            }
            catch (Exception localException)
            {
                Logger.Error(localException);
            }
            sb.Append(map);
            int SubAreaID = character.myMap.subArea.area.superArea.ID;
            foreach (short i in character.Zaaps)
            {
                if (MapTable.Get(i) == null || MapTable.Get(i).subArea.area.superArea.ID != SubAreaID)
                {
                    continue;
                }
                int cost = ZaapTable.calculZaapCost(character.myMap, MapTable.Get(i));
                if (i == character.myMap.Id)
                {
                    cost = 0;
                }
                sb.Append("|").Append(i).Append(";").Append(cost);
            }
            return sb.ToString();
        }

    }
}
