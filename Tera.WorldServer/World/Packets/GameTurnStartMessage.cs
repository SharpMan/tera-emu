using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameTurnStartMessage : PacketBase
    {
        public long FighterId;
        public int TurnTime;

        public GameTurnStartMessage(long FighterId, int TurnTime)
        {
            this.FighterId = FighterId;
            this.TurnTime = TurnTime;
        }

        public override string Compile()
        {
            return "GTS" + this.FighterId + "|" + this.TurnTime;
        }
    }
}
