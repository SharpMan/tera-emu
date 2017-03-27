using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Fights.FightObjects;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public class FightRemoveGroundLayerMessage : PacketBase
    {
        private FightGroundLayer layer;

        public FightRemoveGroundLayerMessage(FightGroundLayer layer)
        {
            this.layer = layer;
        }

        public override string Compile()
        {
            return "GDZ|-" + layer.CellId + ";" + layer.Size + ";" + ((byte)layer.Color);
        }
    }
}
