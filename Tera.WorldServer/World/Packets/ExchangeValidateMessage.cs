using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeValidateMessage : PacketBase
    {
        public long ActorId;
        public bool Validated = false;

        public ExchangeValidateMessage(long ActorId, bool Validated)
        {
            this.ActorId = ActorId;
            this.Validated = Validated;
        }

        public override string Compile()
        {
            return "EK" + (this.Validated ? "1" : "0") + this.ActorId;
        }
    }
}
