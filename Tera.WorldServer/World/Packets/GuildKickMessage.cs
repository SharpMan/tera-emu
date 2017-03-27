using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public enum GuildKickReason
    {
        REASON_UNKNOW = 'a',
        REASON_NO_RIGHT = 'd',

        REASON_NONE,
    }

    public sealed class GuildKickMessage : PacketBase
    {
        public char Result;
        public GuildKickReason Reason;
        public string Args;

        public GuildKickMessage(char Result, GuildKickReason Reason = GuildKickReason.REASON_NONE, string Args = "")
        {
            this.Result = Result;
            this.Reason = Reason;
            this.Args = Args;
        }

        public override string Compile()
        {
            return "gK" + this.Result + (this.Reason == GuildKickReason.REASON_NONE ? "" : ((char)this.Reason).ToString()) + this.Args;
        }
    }
}
