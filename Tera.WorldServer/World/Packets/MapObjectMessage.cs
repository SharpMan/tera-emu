using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Packets
{
    public sealed class MapObjectMessage : PacketBase
    {
        public Map map;
        public DofusCell cell;

        public MapObjectMessage(Map map, DofusCell cell)
        {
            this.map = map;
            this.cell = cell;
        }

        public override string Compile()
        {
            var sb = new StringBuilder("GDF|");
            sb.Append(cell.Id);
            sb.Append(";");
            sb.Append(cell.Object.getState());
            sb.Append(";");
            sb.Append(cell.Object.isInteractive() ? "1" : "0");
            return sb.ToString();
        }

    }
}
