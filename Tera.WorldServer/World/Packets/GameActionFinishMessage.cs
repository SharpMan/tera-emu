using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameActionFinishMessage : PacketBase
    {
        public long ActorId;

        public GameActionFinishMessage(long ActorId)
        {
            this.ActorId = ActorId;
        }

        public override string Compile()
        {
            return "GAF0|" + this.ActorId;
        }
    }
}
