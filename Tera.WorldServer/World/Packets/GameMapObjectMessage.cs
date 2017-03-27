using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameMapObjectMessage : PacketBase
    {
        public Map map;

        public GameMapObjectMessage(Map map)
        {
            this.map = map;
        }

        public override string Compile()
        {
            return map.getObjectsGDsPackets();
        }
    }
}
