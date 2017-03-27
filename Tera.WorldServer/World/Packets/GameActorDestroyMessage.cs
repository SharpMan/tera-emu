using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameActorDestroyMessage : PacketBase
    {
        public long ActorId;

        public GameActorDestroyMessage(long ActorId)
        {
            this.ActorId = ActorId;
        }

        public override string Compile()
        {
            return "GM|-" + ActorId;
        }
    }
}
