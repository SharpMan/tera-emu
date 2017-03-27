using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public enum ChatChannelErrorEnum
    {
        ERROR_NOT_FOUND = 'f',
    }

    public sealed class ChatMessageErrorMessage : PacketBase
    {
        public ChatChannelErrorEnum Reason;

        public ChatMessageErrorMessage(ChatChannelErrorEnum Reason)
        {
            this.Reason = Reason;
        }

        public override string Compile()
        {
            return "cME" + (char)this.Reason;
        }
    }
}
