using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Events;
using Tera.Libs.Network;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;
using Tera.Libs;

namespace Tera.WorldServer.World.Chats
{
    public sealed class ChatChannel : IWorldEventObserver
    {
        public delegate void Generic_EventObserver(PacketBase Packet);
        private event Generic_EventObserver Event_ChatMessage;


        private ChatChannelEnum myChannelType;

        public ChatChannelEnum ChannelType
        {
            get
            {
                return this.myChannelType;
            }
        }

        public void RaiseChatMessage(long ActorId, string ActorName, string Content)
        {
            if (this.Event_ChatMessage != null)
                this.Event_ChatMessage(new ChatChannelMessage(this.ChannelType, ActorId, ActorName, Content));
        }

        public void Send(PacketBase Packet)
        {
            if (this.Event_ChatMessage != null)
                this.Event_ChatMessage.Invoke(Packet);
        }


        public ChatChannel(ChatChannelEnum ChannelType)
        {
            this.myChannelType = ChannelType;
        }

        public void Register(WorldClient Client)
        {
            this.Event_ChatMessage += Client.Send;
        }

        public void UnRegister(WorldClient Client)
        {
            this.Event_ChatMessage -= Client.Send;
        }
    }
}
