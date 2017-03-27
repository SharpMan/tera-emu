using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Chats
{
    public sealed class ChatController
    {
        private Dictionary<ChatChannelEnum, ChatChannel> myChannels = new Dictionary<ChatChannelEnum, ChatChannel>();

        public Dictionary<AlignmentTypeEnum, ChatChannel> AlignChannels = new Dictionary<AlignmentTypeEnum, ChatChannel>(){
            //Pas sur faire les FieldTask en C#
            { AlignmentTypeEnum.ALIGNMENT_NEUTRAL, new ChatChannel(ChatChannelEnum.CHANNEL_ALIGNMENT) },
            { AlignmentTypeEnum.ALIGNMENT_BONTARIAN, new ChatChannel(ChatChannelEnum.CHANNEL_ALIGNMENT) },
            { AlignmentTypeEnum.ALIGNMENT_BRAKMARIAN, new ChatChannel(ChatChannelEnum.CHANNEL_ALIGNMENT) },
            { AlignmentTypeEnum.ALIGNMENT_MERCENARY, new ChatChannel(ChatChannelEnum.CHANNEL_ALIGNMENT) },
        };

        public void GenerateChannels()
        {
            //this.myChannels.Add(ChatChannelEnum.CHANNEL_ALIGNMENT, new ChatChannel(ChatChannelEnum.CHANNEL_ALIGNMENT));
            this.myChannels.Add(ChatChannelEnum.CHANNEL_DEALING, new ChatChannel(ChatChannelEnum.CHANNEL_DEALING));
            this.myChannels.Add(ChatChannelEnum.CHANNEL_RECRUITMENT, new ChatChannel(ChatChannelEnum.CHANNEL_RECRUITMENT));
        }

        public ChatChannel getAlignementChannel(AlignmentTypeEnum Align)
        {
            if (AlignChannels.ContainsKey(Align))
                return AlignChannels[Align];
            return null;
        }

        public void RegisterClient(WorldClient Client, AlignmentTypeEnum Alignement)
        {
            if (AlignChannels.ContainsKey(Alignement))
                Client.RegisterChatChannel(AlignChannels[Alignement]);
        }

        public void RegisterClient(WorldClient Client)
        {
            foreach (var Channel in Client.Character.GetEnabledChatChannels())
                if (this.myChannels.ContainsKey(Channel))
                    Client.RegisterChatChannel(this.myChannels[Channel]);
        }

        public void RegisterClient(WorldClient Client, ChatChannelEnum ChannelType)
        {
            if (this.myChannels.ContainsKey(ChannelType))
                Client.RegisterChatChannel(this.myChannels[ChannelType]);
        }

        public void RegisterChannel(ChatChannel Channel)
        {
            if (!this.myChannels.ContainsKey(Channel.ChannelType))
                this.myChannels.Add(Channel.ChannelType, Channel);
            else 
                this.myChannels[Channel.ChannelType] = Channel;
        }

        public Boolean canSendMessage(WorldClient Client,ChatChannelEnum ChannelType)
        {;
            switch (ChannelType)
            {
                case ChatChannelEnum.CHANNEL_ALIGNMENT:
                    if (Client.Character.Deshonor >= 1)
                    {
                        Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 83, getTimeEplased(Client, ChannelType).ToString()));
                        return false;
                    }
                    if (Client.Character.Alignement == 0)
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return false;
                    }
                    else if (getTimeLeft(Client, ChannelType) < ChatTiming.getTime(ChannelType))
                    {
                        Client.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 115, getTimeEplased(Client, ChannelType).ToString()));
                        return false;
                    }
                    break;
                case ChatChannelEnum.CHANNEL_GENERAL:
                    if (getTimeLeft(Client, ChannelType) < ChatTiming.getTime(ChannelType))
                    {
                        Client.Send(new TextInformationMessage(TextInformationTypeEnum.UNK, 184));
                        return false;
                    }
                    break;
                case ChatChannelEnum.CHANNEL_GUILD:
                    if (getTimeLeft(Client, ChannelType) < ChatTiming.getTime(ChannelType))
                    {
                        Client.Send(new TextInformationMessage(TextInformationTypeEnum.UNK, 184));
                        return false;
                    }
                    break;
                case ChatChannelEnum.CHANNEL_GROUP:
                    if (getTimeLeft(Client, ChannelType) < ChatTiming.getTime(ChannelType))
                    {
                        Client.Send(new TextInformationMessage(TextInformationTypeEnum.UNK, 184));
                        return false;
                    }
                    break;
                case ChatChannelEnum.CHANNEL_RECRUITMENT:
                    if (getTimeLeft(Client, ChannelType) < ChatTiming.getTime(ChannelType))
                    {

                        Client.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 115, getTimeEplased(Client, ChannelType).ToString()));
                        return false;
                    }
                    break;
                case ChatChannelEnum.CHANNEL_DEALING:
                    if (getTimeLeft(Client, ChannelType) < ChatTiming.getTime(ChannelType))
                    {
                        Client.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 115, getTimeEplased(Client, ChannelType).ToString()));
                        return false;
                    }
                    break;
                case ChatChannelEnum.CHANNEL_PRIVATE_SEND:
                    if (getTimeLeft(Client, ChannelType) < ChatTiming.getTime(ChannelType))
                    {
                        Client.Send(new TextInformationMessage(TextInformationTypeEnum.UNK, 184));
                        return false;
                    }
                    break;
             
            }
            return true;
        }

        public long getTimeLeft(WorldClient Client, ChatChannelEnum ChannelType)
        {
            return Program.currentTimeMillis() - Client.myChatRestrictions[ChannelType];
            //return Convert.ToInt32(Math.Ceiling((Client.myChatRestrictions[ChannelType] - Environment.TickCount) / 1000d));
        }

        public long getTimeEplased(WorldClient Client, ChatChannelEnum ChannelType)
        {
            return Convert.ToInt32(Math.Ceiling((ChatTiming.getTime(ChannelType) - (Program.currentTimeMillis() - Client.myChatRestrictions[ChannelType])) / 1000d) + 1);
        }

        
    }
}
