using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class MapDataMessage : PacketBase
    {
        public Map Map;
        public Player Character;

        public MapDataMessage(Map Map)
        {
            this.Map = Map;
        }

        public MapDataMessage(Map Map,Player c)
        {
            this.Map = Map;
            this.Character = c;
        }

        public override string Compile()
        {
            try
            {
                return "GDM|" + this.Map.Id + '|' + Map.Date + '|' + Map.Key;
            }
            catch (NullReferenceException e)
            {
                if (Map == null && Character != null) // Ne devrais pas arriver
                {
                    this.Map = Tera.WorldServer.Database.Tables.MapTable.GetRandomMap();
                    Character.Teleport(Map, Map.getRandomWalkableCell());
                }
                return "GDM|" + this.Map.Id + '|' + Map.Date + '|' + Map.Key;
            }
        }
    }
}
