using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public enum GuildJoinReason
    {
        REASON_UNKNOW = 'u',
        REASON_AWAY = 'o',
        REASON_IN_GUILD = 'a',
        REASON_NO_RIGHT = 'd',
        REASON_JOIN_DECLIN = 'c',
        REASON_JOIN_ACCEPT = 'j',
        REASON_NONE,
    }

    public sealed class GuildJoinMessage : PacketBase
    {
        public char Result;
        public GuildJoinReason Reason;
        public string Args;

        public GuildJoinMessage(char Result, GuildJoinReason Reason = GuildJoinReason.REASON_NONE, string Args = "")
        {
            this.Result = Result;
            this.Reason = Reason;
            this.Args = Args;
        }

        public override string Compile()
        {
            return "gJ" + this.Result + (this.Reason == GuildJoinReason.REASON_NONE ? "" : ((char)this.Reason).ToString()) + this.Args;
        }
    }
}
