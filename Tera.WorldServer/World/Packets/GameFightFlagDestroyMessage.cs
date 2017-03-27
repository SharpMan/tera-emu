using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameFightFlagDestroyMessage : PacketBase
    {
        public int FightId;

        public GameFightFlagDestroyMessage(int FightId)
        {
            this.FightId = FightId;
        }

        public override string Compile()
        {
            return "Gc-" + this.FightId;
        }
    }
}
