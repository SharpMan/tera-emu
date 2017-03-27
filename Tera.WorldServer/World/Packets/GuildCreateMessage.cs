using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GuildCreateMessage : PacketBase
    {
        public bool Success;
        public string Reason;

        public GuildCreateMessage(bool Success, string Reason = "")
        {
            this.Success = Success;
            this.Reason = Reason;
        }

        public override string Compile()
        {
            return "gC" + (this.Success ? "K" : "E" + this.Reason);
        }
    }
}
