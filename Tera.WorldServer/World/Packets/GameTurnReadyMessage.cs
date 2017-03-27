using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameTurnReadyMessage : PacketBase
    {
        public long FighterId;

        public GameTurnReadyMessage(long FighterId)
        {
            this.FighterId = FighterId;
        }

        public override string Compile()
        {
            return "GTR" + this.FighterId;
        }
    }
}
