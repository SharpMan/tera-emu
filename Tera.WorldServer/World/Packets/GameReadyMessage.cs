using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameReadyMessage : PacketBase
    {
        public long ActorId;
        public bool Ready;

        public GameReadyMessage(long ActorId, bool Ready)
        {
            this.ActorId = ActorId;
            this.Ready = Ready;
        }

        public override string Compile()
        {
            return "GR" + (this.Ready ? '1' : '0') + this.ActorId;
        }
    }
}
