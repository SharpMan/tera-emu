using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ChatChannelEnabledMessage : PacketBase
    {
        List<ChatChannelEnum> EnabledChannels;

        public ChatChannelEnabledMessage(List<ChatChannelEnum> EnabledChannels)
        {
            this.EnabledChannels = EnabledChannels;
        }

        /*public override string Compile()
        {
            return "cC+*#%!pi$:?^";
        }*/

        public override string Compile()
        {
            var Packet = new StringBuilder("cC+");

            foreach (var Channel in this.EnabledChannels)
                Packet.Append((char)Channel);

            return Packet.ToString();
        }
    }
}
