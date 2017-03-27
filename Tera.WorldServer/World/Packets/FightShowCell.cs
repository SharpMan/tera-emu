using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class FightShowCell : PacketBase
    {
        public long actor;
        public int cell;

        public FightShowCell(long actor, int cell)
        {
            this.actor = actor;
            this.cell = cell;
        }
        

        public override string Compile()
        {
            return "Gf" + actor + "|" + cell;
        }
    }
}
