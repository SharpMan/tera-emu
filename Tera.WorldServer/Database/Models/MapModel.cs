using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Events;
using Tera.Libs.Network;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Chats;
using Tera.Libs;
using System.Text.RegularExpressions;
using Tera.WorldServer.Network;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Fights;
using System.Runtime.CompilerServices;
using Tera.WorldServer.World;
using Tera.WorldServer.World.Actions;
using Tera.Libs.Utils;

namespace Tera.WorldServer.Database.Models
{
    public interface IWorldField
    {
        void ActorMoved(MovementPath Path, IGameActor Actor, int CellId);
    }

    public partial class Map : IWorldEventObserver, IWorldField
    {
        private static Random RANDOM = new Random();
        private delegate void GenericWorldClientPacket(PacketBase Packet);
        private event GenericWorldClientPacket Event_SendToMap;
        private ChatChannel myChannel;
        private Dictionary<long, Player> myCharactersById = new Dictionary<long, Player>();
        private Dictionary<string, Player> myCharactersByName = new Dictionary<string, Player>();
        private Dictionary<long, IGameActor> myGameActors = new Dictionary<long, IGameActor>();
        private List<Npc> myNpcs = new List<Npc>();
        private FightController myFightController = new FightController();
        private List<MonsterLevel> myPossibleMonsters = new List<MonsterLevel>();
        public List<Couple<int, String>> StaticGroup = new List<Couple<int, string>>();

        public short Id;
        public String Date;
        public byte Width;
        public byte Height;
        public String Key;
        public String FightCell;
        public String Monsters;
        private Dictionary<int, DofusCell> myCells = new Dictionary<int, DofusCell>();
        public List<CellAction> CellActionsCache = new List<CellAction>();
        private Dictionary<int, List<ActionModel>> endFightAction = new Dictionary<int, List<ActionModel>>();
        public int NextObjectID = -1;
        public int X = 0;
        public int Y = 0;
        public byte maxGroup = 3;
        public byte GrouMaxSize;
        public String mapData, CellData, MapInfos;
        public bool myInitialized = false;
        public AreaSub subArea;
        public MountPark mountPark;


        public void initPos()
        {
            String[] mapInfos = MapInfos.Split(',');
            try
            {
                this.X = int.Parse(mapInfos[0]);
                this.Y = int.Parse(mapInfos[1]);
                int subArea = int.Parse(mapInfos[2]);
                this.subArea = AreaSubTable.Get(subArea);
                if (this.subArea != null)
                {
                    this.subArea.Maps.Add(this);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        public void Init()
        {
            if (!String.IsNullOrEmpty(mapData))
            {
                /*lock (myCells)
                {*/
                myCells = MapCrypter.DecompileMapData(this, mapData);
                //}
            }
            else
            {
                String[] cellsDataArray = Regex.Split(CellData, "\\|");
                myCells = new Dictionary<int, DofusCell>();

                foreach (String o in cellsDataArray)
                {
                    bool Walkable = true;
                    bool LineOfSight = true;
                    int Number = -1;
                    int obj = -1;
                    String[] cellInfos = o.Split(',');
                    try
                    {
                        Walkable = cellInfos[2].Equals("1");
                        LineOfSight = cellInfos[1].Equals("1");
                        Number = int.Parse(cellInfos[0]);
                        if (!cellInfos[3].Trim().Equals(""))
                        {
                            obj = int.Parse(cellInfos[3]);
                        }
                    }
                    catch (Exception d)
                    {
                    };
                    if (Number == -1)
                    {
                        continue;
                    }
                    var cell = new DofusCell()
                    {
                        Map = this.Id,
                        Id = Number,
                        Walkable = Walkable,
                        LoS = LineOfSight,
                    };
                    if (obj != -1)
                    {
                        cell.Object = new IObject(this, cell, obj);
                    }
                    myCells.Add(Number, cell);

                }
            }
            mapData = null;
            CellData = null;
            MapInfos = null;
            ObjectCount = myCells.Values.Count(x => x.Object != null);
            myInitialized = true;
            this.myChannel = new ChatChannel(ChatChannelEnum.CHANNEL_GENERAL);
            foreach (var Action in this.CellActionsCache)
                if (this.myCells.ContainsKey(Action.CellID))
                {
                    this.myCells[Action.CellID].AddAction(Action);
                }
            foreach (var Npc in this.myNpcs)
                if (this.myCells.ContainsKey(Npc.CellId))
                {
                    Npc.Initialize(this.myNextActorId--);
                    this.SpawnActor(Npc);
                }

            foreach (var MonsterInfo in this.Monsters.Split('|'))
            {
                if (MonsterInfo != string.Empty)
                {
                    try
                    {
                        var MonsterData = MonsterInfo.Split(',');

                        var MonsterId = int.Parse(MonsterData[0]);
                        var MonsterLevel = int.Parse(MonsterData[1]);

                        var Monster = MonsterTable.GetMonster(MonsterId);

                        if (Monster != null)
                        {
                            Monster.Initialize();
                            var MonsterGrade = Monster.GetLevel(MonsterLevel);

                            if (MonsterGrade != null)
                            {
                                this.myPossibleMonsters.Add(MonsterGrade);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            this.SpawnMonsterGroup(3);
            this.AddStaticGroup();
        }

        public void Register(WorldClient Client)
        {
            this.Event_SendToMap += Client.Send;

            Client.RegisterChatChannel(this.myChannel);
        }

        public void UnRegister(WorldClient Client)
        {
            this.Event_SendToMap -= Client.Send;

            Client.UnRegisterChatChannel(ChatChannelEnum.CHANNEL_GENERAL);
        }

        public void ActorMoved(MovementPath Path, IGameActor Actor, int NewCell)
        {
            Actor.Orientation = Path.GetDirection(Path.LastStep);
            this.myCells[Actor.CellId].DelActor(Actor);
            this.myCells[NewCell].AddActor(Actor);
            /*if (this.FightCell.Equals("|") || this.FightCell.Equals("-1"))
            {
                return;
            }*/
            if (Actor.ActorType == GameActorTypeEnum.TYPE_CHARACTER)
            {
                if ((Actor as Player).Map == this.Id)
                {
                    foreach (var MGroup in this.myGameActors.Values.OfType<MonsterGroup>())
                    {
                        if (Pathfinder.GoalDistance(this, NewCell, MGroup.CellId) <= MGroup.Aggrodistance)
                        {
                            if ((MGroup.AlignmentType == AlignmentTypeEnum.ALIGNMENT_NEUTRAL) || (Actor as Player).Alignement != MGroup.Alignement)
                            {
                               /* if (this.FightCell.Equals("|") || this.FightCell.Equals("-1"))
                                {
                                    continue; ;
                                }*/
                                this.LaunchMonsterFight((Actor as Player).GetClient(), MGroup);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void SendMonsterGroupGMsPackets()
        {
            var MonsterGroups = this.myGameActors.Values.OfType<MonsterGroup>();
            if (MonsterGroups.Count() < 1)
            {
                return;
            }

            StringBuilder Packet = new StringBuilder();
            Packet.Append("GM|");
            Boolean isFirst = true;
            foreach (MonsterGroup entry in MonsterGroups)
            {
                entry.SerializeAsGameMapInformations(Packet);
                if (!isFirst)
                {
                    Packet.Append("|");
                }
                isFirst = false;
            }
            SendToMap(new EmptyMessage(Packet.ToString()));
        }


        private object myFightLock = new object();
        public void LaunchMonsterFight(WorldClient Player, MonsterGroup Monsters)
        {
            lock (this.myFightLock)
            {
                if (Monsters != null && Player != null)
                {
                    // Ne peut pas lancer de combat ?
                    if (Player.CanGameAction(GameActionTypeEnum.FIGHT))
                    {
                        this.AddFight(new MonsterFight(this, Player, Monsters));
                        if (!Monsters.IsFix)
                            this.DestroyActor(Monsters);
                        else
                            SendMonsterGroupGMsPackets();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SpawnMonsterGroup(int Count)
        {
            if (Count < 1)
                return;

            for (int i = 0; i < Count; i++)
            {
                var Group = new MonsterGroup(this.myPossibleMonsters, this.GrouMaxSize, this.NextActorId);

                if (Group.Monsters.Count < 1)
                    continue;

                this.SpawnActor(Group);
            }
        }

        public int getRandomCell()
        {
            return this.myCells.Keys.FirstOrDefault(x => x == RANDOM.Next(this.myCells.Keys.Count));
        }

        public int getRandomWalkableCell()
        {
            int cell = getRandomCell();
            if (myCells[cell].isWalkable(true))
            {
                return cell;
            }
            else
            {
                return getRandomWalkableCell();
            }
        }

        public void SpawnActor(IGameActor Actor)
        {
            if (!this.myInitialized)
                this.Init();
            if (Actor.ActorType == GameActorTypeEnum.TYPE_CHARACTER)
            {
                var Character = Actor as Player;

                lock (this.myCharactersById)
                    this.myCharactersById.Add(Character.ID, Character);
                lock (this.myCharactersByName)
                    this.myCharactersByName.Add(Character.Name, Character);

                if (Character.Client != null)
                {
                    Character.Client.RegisterWorldEvent(this);
                    if (!Character.isJoiningTaxFight)
                        this.myFightController.SendFightInfos(Character.Client);
                }
            }
            lock (this.myGameActors)
                this.myGameActors.Add(Actor.ActorId, Actor);

            if (!(Actor.ActorType == GameActorTypeEnum.TYPE_CHARACTER && (Actor as Player).isJoiningTaxFight))
                this.SendToMap(new GameActorShowMessage(GameActorShowEnum.SHOW_SPAWN, Actor));

            if (this.myCells.ContainsKey(Actor.CellId))
            {
                if (this.myCells[Actor.CellId].Walkable || Actor.ActorType == GameActorTypeEnum.TYPE_NPC)
                {
                    this.myCells[Actor.CellId].AddActor(Actor);
                }
                else
                {
                    Actor.CellId = this.GetFreeCell();

                    this.myCells[Actor.CellId].AddActor(Actor);
                }
            }

        }

        public void SpawnActor(IGameActor Actor, int Cell)
        {
            if (!this.myInitialized)
                this.Init();
            if (Actor.ActorType == GameActorTypeEnum.TYPE_CHARACTER)
            {
                var Character = Actor as Player;

                lock (this.myCharactersById)
                    this.myCharactersById.Add(Character.ID, Character);
                lock (this.myCharactersByName)
                    this.myCharactersByName.Add(Character.Name, Character);

                if (Character.Client != null)
                {
                    Character.Client.RegisterWorldEvent(this);
                    if (!Character.isJoiningTaxFight)
                        this.myFightController.SendFightInfos(Character.Client);
                }
            }
            lock (this.myGameActors)
                this.myGameActors.Add(Actor.ActorId, Actor);

            if (!(Actor.ActorType == GameActorTypeEnum.TYPE_CHARACTER && (Actor as Player).isJoiningTaxFight))
                this.SendToMap(new GameActorShowMessage(GameActorShowEnum.SHOW_SPAWN, Actor));

            if (this.myCells.ContainsKey(Cell))
            {
                if (this.myCells[Cell].Walkable)
                {
                    this.myCells[Cell].AddActor(Actor);
                }
                else
                {
                    Actor.CellId = this.GetFreeCell();

                    this.myCells[Actor.CellId].AddActor(Actor);
                }
            }

        }

        public IGameActor GetActor(long Id)
        {
            if (!this.myGameActors.ContainsKey(Id))
                return null;

            return this.myGameActors[Id];
        }

        public void DestroyActor(IGameActor Actor)
        {
            if (Actor.ActorType == GameActorTypeEnum.TYPE_CHARACTER)
            {
                var Character = Actor as Player;

                lock (this.myCharactersById)
                    this.myCharactersById.Remove(Character.ID);
                lock (this.myCharactersByName)
                    this.myCharactersByName.Remove(Character.Name);

                if (Character.Client != null)
                {
                    Character.Client.UnRegisterWorldEvent(this);
                }
            }

            lock (this.myGameActors)
                this.myGameActors.Remove(Actor.ActorId);


            this.SendToMap(new GameActorDestroyMessage(Actor.ActorId));

            if (this.myCells.ContainsKey(Actor.CellId))
            {
                this.myCells[Actor.CellId].DelActor(Actor);
            }
        }

        public Npc addNpc(int npcID, int cellID, int dir)
        {
            if (!NpcTemplateTable.Cache.ContainsKey(npcID))
            {
                return null;
            }
            NpcTemplateModel temp = NpcTemplateTable.Cache[npcID];
            if (temp == null)
            {
                return null;
            }
            Npc npc = new Npc(temp, cellID, dir);
            myNpcs.Add(npc);
            return npc;
        }

        public Npc getNPC(int id)
        {
            if (id > this.myGameActors.Count)
            {
                return null;
            }
            var actor = this.myGameActors[id];
            if (actor.ActorType == GameActorTypeEnum.TYPE_NPC)
            {
                return (Npc)actor;
            }
            else
            {
                return null;
            }
        }

        private long myNextActorId = -1;

        public long NextActorId
        {
            get { return myNextActorId--; }
        }


        public int GetFreeCell()
        {
            int CellId = -1;

            do
            {
                CellId = RANDOM.Next(50, this.myCells.Count - 1);
            }
            while (!this.myCells[CellId].Walkable);

            return CellId;
        }



        public void SendToMap(PacketBase Packet)
        {
            if (this.Event_SendToMap != null)
                this.Event_SendToMap(Packet);
        }

        public IEnumerable<IGameActor> GetActors()
        {
            return this.myGameActors.Values.ToArray();
        }

        public long getActorsSize()
        {
            return this.myGameActors.Count;
        }

        public long getNpcSize()
        {
            return this.myNpcs.Count;
        }

        public IEnumerable<DofusCell> GetCells()
        {
            return this.myCells.Values;
        }

        public int CellsCount
        {
            get
            {
                return this.myCells.Count;
            }
        }

        public DofusCell getCell(int cell)
        {
            if (!myCells.ContainsKey(cell))
            {
                return null;
            }
            return this.myCells[cell];
        }

        public void AddFight(Fight Fight)
        {
            this.myFightController.AddFight(Fight);

            this.SendMapFightCountMessage();
        }

        public void RemoveFight(Fight Fight)
        {
            this.myFightController.RemoveFight(Fight);

            this.SendMapFightCountMessage();
        }

        public void SendMapFightCountMessage()
        {
            this.SendToMap(new MapFightCountMessage(this.myFightController.FightCount));
        }

        public Fight GetFight(int FightId)
        {
            return this.myFightController.GetFight(FightId);
        }

        public List<Fight> GetFights()
        {
            return this.myFightController.Fights;
        }

        public int NextFightId
        {
            get
            {
                return this.myFightController.NextFightId;
            }
        }

        public int ObjectCount = 0;



        public String getObjectsGDsPackets()
        {
            StringBuilder toreturn = new StringBuilder();
            bool first = true;
            foreach (DofusCell entry in myCells.Values.Where(x => x.Object != null))
            {
                if (!first)
                {
                    toreturn.Append((char)0x00);
                }
                first = false;
                int cellID = entry.Id;
                IObject objet = entry.Object;
                toreturn.Append("GDF|").Append(cellID).Append(";").Append(
                        objet.getState()).Append(";").Append(
                        (objet.isInteractive() ? "1" : "0"));

            }
            return toreturn.ToString();
        }

        public bool IsCellWalkable(int CellId)
        {
            if (CellId < 0 || CellId >= this.CellsCount)
                return true;

            return this.myCells[CellId].Walkable;
        }

        public void RegisterToChat(WorldClient Client)
        {
            Client.RegisterChatChannel(this.myChannel);
        }

        public int GetNearestFreeCell(int CellId)
        {
            for (int i = 0; i < 8; i++)
            {
                var Cell = Pathfinder.NextCell(this, CellId, i);

                if (this.myCells.ContainsKey(Cell))
                    return Cell;
            }

            return CellId;
        }

        public void applyEndFightAction(int type, Player perso)
        {
            if (!endFightAction.ContainsKey(type))
            {
                return;
            }
            foreach (ActionModel A in endFightAction[type])
            {
                A.apply(perso, null, -1, -1);
            }
        }

        public void addEndFightAction(int type, ActionModel A)
        {
            if (!endFightAction.ContainsKey(type))
            {
                endFightAction.Add(type, new List<ActionModel>());
            }
            delEndFightAction(type, A.ID);
            endFightAction[type].Add(A);
        }

        public Boolean hasEndFightAction(int type)
        {
            return endFightAction.ContainsKey(type);
        }

        public void delEndFightAction(int type, int aType)
        {
            if (!endFightAction.ContainsKey(type))
            {
                return;
            }
            List<ActionModel> copy = new List<ActionModel>();
            endFightAction[type].ForEach(x => copy.Add(x));
            foreach (ActionModel A in copy)
            {
                if (A.ID == aType)
                {
                    endFightAction[type].Remove(A);
                }
            }
        }

        public int GetBestCellBetween(int CellId, int NextCell, List<int> Closes)
        {
            int BestCell = -1;
            int BestDist = 1000;
            for (int i = 1; i < 8; i += 2)
            {
                var Cell = Pathfinder.NextCell(this, CellId, ((i + 2) % 8));

                if (this.myCells.ContainsKey(Cell))
                    if (!Closes.Contains(Cell))
                    {
                        var Dist = Pathfinder.GoalDistanceScore(this, Cell, NextCell);
                        if (Dist <= BestDist)
                        {
                            BestCell = Cell;
                            BestDist = Dist;
                        }
                    }
            }
            return BestCell;
        }

        public Player GetCharacter(string Name)
        {
            if (!this.myCharactersByName.ContainsKey(Name))
                return null;

            return this.myCharactersByName[Name];
        }

        public Object getMountParkDoor()
        {
            foreach (DofusCell c in myCells.Values)
            {
                if (c.Object == null)
                {
                    continue;
                }
                //Si enclose
                if (c.Object.getID() == 6763 || c.Object.getID() == 6766 || c.Object.getID() == 6767 || c.Object.getID() == 6772)
                {
                    return c.Object;
                }
            }
            return null;
        }

        public void InitNpc(Npc Npc)
        {
            Npc.Initialize(this.myNextActorId--);
            this.SpawnActor(Npc);
        }



        internal void AddStaticGroup()
        {
            foreach (var a in StaticGroup)
            {
                var Group = new MonsterGroup(this.NextActorId, a.second);

                if (Group.Monsters.Count < 1)
                    return;

                this.SpawnActor(Group, a.first);
            }
        }

        internal void AddStaticGroup(Couple<int, String> a)
        {
            var Group = new MonsterGroup(this.NextActorId, a.second);

            if (Group.Monsters.Count < 1)
                return;

            this.SpawnActor(Group, a.first);
            SendMonsterGroupGMsPackets();
        }
    }
}
