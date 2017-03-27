using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeLeaveMessage : PacketBase
    {
        public bool Success = false;

        public ExchangeLeaveMessage(bool Success = false)
        {
            this.Success = Success;
        }

        public override string Compile()
        {
            return "EV" + (Success ? "a" : "");
        }
    }
}
