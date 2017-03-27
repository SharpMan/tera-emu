using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SilverSock;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Handlers;
using Tera.Libs.Network;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Events;
using Tera.WorldServer.World.Chats;
using Tera.WorldServer.World;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Exchanges;
using Tera.WorldServer.World.GameRequests;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.Scripting;
using Tera.WorldServer.World.Actions;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.GameActions;
using System.Threading;
using Tera.WorldServer.World.Scripting.Commands;

namespace Tera.WorldServer.Network
{
    public class WorldClient : BaseClient
    {
        public string EncryptKey { get; set; }
        public IPrintWriterEncrypted Out;
        public AccountModel Account { get; set; }
        public Player Character { get; set; }
        private Exchange myExchange = null;
        private GameBaseRequest myBaseRequest = null;
        private WorldState myState = WorldState.STATE_NON_AUTHENTIFIED;
        private List<IWorldEventObserver> myObservedEvents = new List<IWorldEventObserver>();
        private Dictionary<GameActionTypeEnum, GameAction> myActions = new Dictionary<GameActionTypeEnum, GameAction>();
        public Dictionary<int, MiniGameAction> miniActions = new Dictionary<int, MiniGameAction>();
        private Dictionary<ChatChannelEnum, ChatChannel> myChatChannels = new Dictionary<ChatChannelEnum, ChatChannel>()
        {
            { ChatChannelEnum.CHANNEL_ADMIN, null },
            { ChatChannelEnum.CHANNEL_ALIGNMENT, null },
            { ChatChannelEnum.CHANNEL_DEALING, null },
            { ChatChannelEnum.CHANNEL_GENERAL, null },
            { ChatChannelEnum.CHANNEL_GROUP, null },
            { ChatChannelEnum.CHANNEL_GUILD, null },
            { ChatChannelEnum.CHANNEL_RECRUITMENT, null },
            { ChatChannelEnum.CHANNEL_TEAM, null },
        };
        public Dictionary<ChatChannelEnum, long> myChatRestrictions = new Dictionary<ChatChannelEnum, long>()
        {
            { ChatChannelEnum.CHANNEL_ALIGNMENT, 0 },
            { ChatChannelEnum.CHANNEL_DEALING, 0 },
            { ChatChannelEnum.CHANNEL_GENERAL, 0 },
            { ChatChannelEnum.CHANNEL_GUILD, 0 },
            { ChatChannelEnum.CHANNEL_GROUP, 0 },
            { ChatChannelEnum.CHANNEL_RECRUITMENT, 0 },
            { ChatChannelEnum.CHANNEL_POINT , 0 },
        };

        private Fight myFight;
        private Fighter myFighter;

        public object BoostStatsSync = new object();
        public object BoostSpellSync = new object();
        public bool hasFightMessage = false;

        public WorldClient(SilverSocket socket)
            : base(socket)
        {
            this.Write("<?xml version=\"1.0\" encoding=\"UTF-8\"?><cross-domain-policy><allow-access-from domain=\"*\" to-ports=\"*\" secure=\"false\" /><site-control permitted-cross-domain-policies=\"master-only\" /></cross-domain-policy>");
            this.Out = new IPrintWriterEncrypted(this);
            this.Out.GenAndSendKey();
            this.Send(new HelloGameMessage());
        }

        public override void OnClose()
        {
            lock (WorldServer.Clients)
            {
                try
                {
                    this.AbortGameActions();
                }
                catch (Exception ex)
                {

                }
                Logger.Debug("Client disconnected !");
                if (Account != null)
                    Account.OnDisconnect();
                if (Character != null)
                    Character.OnDisconnect();
                try
                {
                    this.ReleaseWorldEvents();
                }
                catch (Exception ex)
                {

                }
                WorldServer.Clients.Remove(this);
            }
        }

        public override void OnPacket(String message)
        {
            if (this.Out.getDecryptor() != null)
            {
                try
                {
                    message = Out.getDecryptor().decrypt(message);
                }
                catch (Exception e)
                {
                    Logger.Error("Fail to Parse -> " + e.ToString());
                    return;
                }
            }
            Logger.Debug("Received packet from client : " + message);
            try
            {
                PacketProcessor.ProcessPacket(this, message);
            }
            catch (Exception e)
            {
                Logger.Error("Can't handle packet : " + e.ToString());
            }
        }

        public override void Send(String message)
        {
            Logger.Debug("Send packet : " + message);
            this.Out.print(message);
        }

        public void Write(String message)
        {
            base.Send(message);
        }

        public void Send(PacketBase packet)
        {
            this.Send(packet.Build());
        } 
        
        public WorldState GetState()
        {
            return this.myState;
        }

        public void SetState(WorldState State)
        {
            this.myState = State;
        }

        public void UnRegisterChatChannel(ChatChannelEnum Channel)
        {
            lock (this.myChatChannels)
            {
                if (this.myChatChannels.ContainsKey(Channel) && this.myChatChannels[Channel] != null)
                {
                    this.UnRegisterWorldEvent(this.myChatChannels[Channel]);
                    this.myChatChannels[Channel] = null;
                }
            }
        }

        public void RegisterChatChannel(ChatChannel Instance)
        {
            this.UnRegisterChatChannel(Instance.ChannelType);

            lock (this.myChatChannels)
                this.myChatChannels[Instance.ChannelType] = Instance;

            this.RegisterWorldEvent(Instance);
        }

        public void RegisterWorldEvent(IWorldEventObserver Observer)
        {
            lock (this.myObservedEvents)
                this.myObservedEvents.Add(Observer);

            Observer.Register(this);
        }

        public void UnRegisterWorldEvent(IWorldEventObserver Observer)
        {
            lock (this.myObservedEvents)
                if (this.myObservedEvents.Contains(Observer))
                    this.myObservedEvents.Remove(Observer);

            Observer.UnRegister(this);
        }

        public void SetCharacter(Player Character)
        {
            this.Character = Character;
            this.Character.SetOnline(this);
        }

        private void ReleaseWorldEvents()
        {
            var Events = new List<IWorldEventObserver>();
            Events.AddRange(this.myObservedEvents);

            lock (this.myObservedEvents)
                foreach (IWorldEventObserver Event in Events)
                    this.UnRegisterWorldEvent(Event);
        }

        public void AbortGameActions()
        {
            var Copy = new List<GameAction>(this.myActions.Values);

            lock (this.myActions)
                foreach (GameAction action in Copy)
                    action.Abort();

            this.myActions.Clear();
        }

        public bool CanGameAction(GameActionTypeEnum ActionType)
        {
            if (Character != null && ActionType == GameActionTypeEnum.BASIC_REQUEST && Character.isAaway)
            {
                return false;
            }
            lock (this.myActions)
                return this.myActions.Values.All(x => x.CanSubAction(ActionType));
        }

        public void AddGameAction(GameAction Action)
        {
            try
            {
                lock (this.myActions)
                    this.myActions.Add(Action.ActionType, Action);

                Action.RegisterEnd(this.DelGameAction);
                Action.Execute();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public int getGameActionCount()
        {
            return this.myActions.Count;
        }

        public void EndGameAction(GameActionTypeEnum Action)
        {
            lock (this.myActions)
                if (this.myActions.ContainsKey(Action))
                    this.myActions[Action].EndExecute();
        }

        public GameAction GetGameAction(GameActionTypeEnum Action)
        {
            lock (this.myActions)
                if (this.myActions.ContainsKey(Action))
                    return this.myActions[Action];
            return null;
        }

        public void AbortGameAction(GameActionTypeEnum Action, params object[] Args)
        {
            lock (this.myActions)
                if (this.myActions.ContainsKey(Action))
                    this.myActions[Action].Abort(Args);
        }

        private void DelGameAction(GameAction GameAction)
        {
            lock (this.myActions)
                this.myActions.Remove(GameAction.ActionType);
        }

        public void DelGameAction(GameActionTypeEnum Action)
        {
            lock (this.myActions)
                this.myActions.Remove(Action);
        }

        public bool IsGameAction(GameActionTypeEnum Action)
        {
            lock (this.myActions)
                return this.myActions.ContainsKey(Action);
        }

        public void SetExchange(Exchange Exchange)
        {
            this.myExchange = Exchange;
        }

        public void SetBaseRequest(GameBaseRequest Request)
        {
            this.myBaseRequest = Request;
        }

        public GameBaseRequest GetBaseRequest()
        {
            return this.myBaseRequest;
        }

        public Exchange GetExchange()
        {
            return this.myExchange;
        }

        public void SetFight(Fight Fight)
        {
            this.myFight = Fight;
        }

        public void SetFighter(Fighter Fighter)
        {
            this.myFighter = Fighter;
        }

        public Fight GetFight()
        {
            return this.myFight;
        }

        public Fighter GetFighter()
        {
            return this.myFighter;
        }

        public long lastCheckPoint;

        public void RaiseChatMessage(ChatChannelEnum Channel, string Content)
        {
            if (!this.Character.IsChatChannelEnable(Channel))
                return;
            if (this.myChatChannels.ContainsKey(Channel))
            {
                if (this.myChatChannels[Channel] != null && WorldServer.GetChatController().canSendMessage(this, Channel))
                {
                    if (ChatTiming.Exist(Channel))
                    {
                        this.myChatRestrictions[Channel] = Program.currentTimeMillis();
                    }
                    if (Content != String.Empty && Content[0] == '.' && this.myFight == null)
                    {
                        var parameters = new CommandParameters(Content.Substring(1), false);
                        var command = JSKernel.CommandsPlayer.Get(parameters.Prefix);
                        if (command != null)
                        {
                            bool result = command.callAction(this, parameters);
                            if (!result)
                            {
                                Send(new ChatGameMessage("Commande invalide , tapez .help pour plus d'infos", "FF0000"));
                                return;
                            }
                            return;
                        }
                        else
                        {
                            Send(new ChatGameMessage("Commande invalide , tapez .help pour plus d'infos", "FF0000"));
                            return;
                        }
                        /*TeraScript Script = ScriptKernel.getScript(Content.Substring(1).Split(' ')[0].ToLower());
                        if (Script != null)
                        {
                            Script.apply(this.Character);
                            return;
                        }
                        else
                        {
                            Send(new ChatGameMessage("Commande invalide , tapez .help pour plus d'infos", "FF0000"));
                            return;
                        }*/
                    }
                }
            }
        }

        public Player GetCharacter()
        {
            return this.Character;
        }
    }
}
