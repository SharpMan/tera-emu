using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ChatChannelMessage : PacketBase
    {
        public ChatChannelEnum Channel;
        public long ActorId;
        public string ActorName;
        public string Message;

        public ChatChannelMessage(ChatChannelEnum Channel, long ActorId, string ActorName, string Message)
        {
            this.Channel = Channel;
            this.ActorId = ActorId;
            this.ActorName = ActorName;
            this.Message = Message;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("cMK");

            Packet.Append((char)Channel).Append('|');
            Packet.Append(ActorId).Append('|');
            Packet.Append(ActorName).Append('|');
            Packet.Append(Message).Append('|');

            return Packet.ToString();
        }
    }
}
