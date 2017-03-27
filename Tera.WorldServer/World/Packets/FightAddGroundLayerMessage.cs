using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Fights.FightObjects;
using Tera.Libs;

namespace Tera.WorldServer.World.Packets
{
    public class FightAddGroundLayerMessage : PacketBase
    {
        private FightGroundLayer layer;

        public FightAddGroundLayerMessage(FightGroundLayer layer)
        {
            this.layer = layer;
        }

        public override string Compile()
        {
           return "GDZ|+" + layer.CellId + ";" + layer.Size + ";" + layer.Color;
        }
    }
}
