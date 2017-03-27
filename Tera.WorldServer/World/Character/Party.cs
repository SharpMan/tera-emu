using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Chats;
using Tera.WorldServer.World.Events;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Character
{
    public class Party : IWorldEventObserver
    {
        private delegate void GenericWorldClientPacket(PacketBase Packet);
        private event GenericWorldClientPacket Event_SendToGuild;

        private ChatChannel myChatChannel = new ChatChannel(ChatChannelEnum.CHANNEL_GROUP);
        public ChatChannel ChatChannel
        {
            get
            {
                return this.myChatChannel;
            }
        }

        public List<Player> Players = new List<Player>();
        public Player Chief;

        public Party(Player p1, Player p2)
        {
            Chief = p1;
        }

        public void Leave(Player p)
        {
            if (p == null || Players == null)
            {
                return;
            }
            if (!Players.Contains(p))
            {
                return;
            }
            UnRegister(p.Client);
            Players.Remove(p);
            if (Players.Count == 1)
            {
                Player last = Players[0];
                if (last != null)
                {
                    last.Client.EndGameAction(GameActionTypeEnum.GROUP);
                    last.Send(new PartyLeaveMessage(""));
                }
                Clear();
            }
            else
            {
                this.Send(new PartyRemovePlayer(p.ActorId));
            }
            if (Players != null && Players.Count <= 0)
            {
                Clear();
            }
        }

        public int GetPersosNumber()
        {
            if (Players == null)
            {
                return 0;
            }
            return Players.Count;
        }

        public void Clear()
        {
            Players.Clear();
            Players = null;
            Chief = null;
        }


        public void Register(WorldClient Client)
        {
            this.Event_SendToGuild += Client.Send;

            Client.RegisterChatChannel(this.myChatChannel);
        }

        public void UnRegister(WorldClient Client)
        {
            this.Event_SendToGuild -= Client.Send;

            Client.UnRegisterChatChannel(ChatChannelEnum.CHANNEL_GROUP);
        }

        public void Send(PacketBase Packet)
        {
            if (this.Event_SendToGuild != null)
                this.Event_SendToGuild(Packet);
        }

        public Boolean inKolizeum = false;

        public int GetMoyLevel()
        {
            int lvls = 0;
            int count = 0;
            foreach (Player p in Players)
            {
                lvls += p.Level;
                count++;
            }
            if (count == 0)
            {
                return 0;
            }
            return lvls / count;
        }

        public int GetGroupLevel()
        {
            if (Players == null)
            {
                return 0;
            }
            int lvls = 0;
            Players.ForEach(x => lvls += x.Level);
            return lvls;
        }

        public Boolean isChief(long guid)
        {
            return Chief != null && Chief.ActorId == guid;
        }
    }
}
