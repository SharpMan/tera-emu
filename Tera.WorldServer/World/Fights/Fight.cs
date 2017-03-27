using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Challenges;
using Tera.WorldServer.World.Character;
using Tera.WorldServer.World.Chats;
using Tera.WorldServer.World.Events;
using Tera.WorldServer.World.Fights.Effects;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Spells;
using Tera.Libs.Utils;
using System.Threading;
using Tera.WorldServer.World.Fights.FightObjects;
using Tera.WorldServer.World.Fights.Types;

namespace Tera.WorldServer.World.Fights
{
    public enum FightType
    {
        TYPE_CHALLENGE = 0,
        TYPE_AGGRESSION = 1,
        TYPE_PVMA = 2,
        TYPE_MXVM = 3,
        TYPE_PVM = 4,
        TYPE_PVT = 5,
        TYPE_PVMU = 6,
        TYPE_KOLIZEUM = 7,
    }
    public enum FightState
    {
        STATE_PLACE = 2,
        STATE_INIT = 1,
        STATE_ACTIVE = 3,
        STATE_FINISH = 4,
    }
    public enum FightLoopState
    {
        STATE_WAIT_START,
        STATE_WAIT_TURN,
        STATE_WAIT_ACTION,
        STATE_WAIT_READY,
        STATE_WAIT_END,
        STATE_WAIT_AI,
        STATE_END_TURN,
        STATE_END_FIGHT,
    }

    public abstract class Fight : IWorldEventObserver, IWorldField
    {
        private static Dictionary<int, Dictionary<int, List<int>>> MAP_FIGHTCELLS = new Dictionary<int, Dictionary<int, List<int>>>();
        public static Random RANDOM = new Random();

        # region Evenements

        private delegate void GenericWorldClientPacket(PacketBase Packet);
        private event GenericWorldClientPacket Event_SendToFight;


        private delegate void Generic_onLeaveFight(Fighter Fighter);
        private event Generic_onLeaveFight Event_onLeaveFight;

        private delegate void Generic_onEndFight(FightTeam Winners, FightTeam Loosers);
        private event Generic_onEndFight Event_onEndFight;

        private delegate void Generic_onStartFight();
        private event Generic_onStartFight Event_onStartFight;

        private delegate void Generic_onLaunchWeapon(Fighter Launcher, InventoryItemModel Weapon, int TargetCellId, Fighter TargetFighter, Dictionary<WeaponEffect, List<Fighter>> TargetEffects, bool IsCC, bool IsEchec, bool isPunch);
        private event Generic_onLaunchWeapon Event_onLaunchWeapon;

        private delegate void Generic_onLaunchSpell(Fighter Launcher, SpellLevel Spell, int TargetCellId, Fighter TargetFighter, Dictionary<EffectInfos, List<Fighter>> TargetEffects, bool IsCC, bool IsEchec);
        private event Generic_onLaunchSpell Event_onLaunchSpell;

        private delegate void Generic_onActorMoved(Fighter Fighter, MovementPath Path, FightCell NewCell);
        private event Generic_onActorMoved Event_onActorMoved;

        private delegate void Generic_onBeginTurn(Fighter newFighter);
        private event Generic_onBeginTurn Event_onBeginTurn;

        private delegate void Generic_onMiddleTurn(Fighter fighter);
        private event Generic_onMiddleTurn Event_onMiddleTurn;

        private delegate void Generic_onEndTurn(Fighter endFighter, bool Finish = false);
        private event Generic_onEndTurn Event_onEndTurn;

        private delegate void Generic_onDie(Fighter fighter, Fighter Caster);
        private event Generic_onDie Event_onDie;

        private delegate void Generic_onApplyGroundLayer(FightGroundLayer layer, Fighter Caster, SpellLevel Spell, int TargetCellId, Fighter TargetFighter, Dictionary<EffectInfos, List<Fighter>> TargetEffects);
        private event Generic_onApplyGroundLayer Event_onApplyGroundLayer;

        public void onFighterDie(Fighter fighter, Fighter Caster)
        {
            if (Event_onDie != null) Event_onDie.Invoke(fighter, Caster);
        }

        public void onApplyGroundLayer(FightGroundLayer layer, Fighter Caster, SpellLevel Spell, int TargetCellId, Fighter TargetFighter, Dictionary<EffectInfos, List<Fighter>> TargetEffects)
        {
            if (Event_onApplyGroundLayer != null) Event_onApplyGroundLayer.Invoke(layer, Caster, Spell, TargetCellId, TargetFighter, TargetEffects);
        }

        public void RegisterFightListener(FightListener listener)
        {
            Event_onLeaveFight += listener.onLeaveFight;
            Event_onEndFight += listener.onEndFight;
            Event_onStartFight += listener.onStartFight;
            Event_onLaunchWeapon += listener.onLaunchWeapon;
            Event_onLaunchSpell += listener.onLaunchSpell;
            Event_onActorMoved += listener.onActorMoved;
            Event_onBeginTurn += listener.onBeginTurn;
            Event_onMiddleTurn += listener.onMiddleTurn;
            Event_onEndTurn += listener.onEndTurn;
            Event_onDie += listener.onDie;
            Event_onApplyGroundLayer += listener.onApplyGroundLayer;
        }

        public void unRegisterFightListener(FightListener listener)
        {
            Event_onLeaveFight -= listener.onLeaveFight;
            Event_onEndFight -= listener.onEndFight;
            Event_onStartFight -= listener.onStartFight;
            Event_onLaunchWeapon -= listener.onLaunchWeapon;
            Event_onLaunchSpell -= listener.onLaunchSpell;
            Event_onActorMoved -= listener.onActorMoved;
            Event_onBeginTurn -= listener.onBeginTurn;
            Event_onMiddleTurn -= listener.onMiddleTurn;
            Event_onEndTurn -= listener.onEndTurn;
            Event_onDie -= listener.onDie;
            Event_onApplyGroundLayer -= listener.onApplyGroundLayer;
        }

        # endregion

        /// <summary>
        /// Kické ou alors tout simplement annulé
        /// </summary>
        /// <param name="Character"></param>
        public void LeaveFight(Fighter Fighter)
        {
            if (Event_onLeaveFight != null) Event_onLeaveFight.Invoke(Fighter);
            OverridableLeaveFight(Fighter);
        }
        public abstract void OverridableLeaveFight(Fighter Fighter);

        /// <summary>
        /// Fin du combat
        /// </summary>
        /// <param name="Winners"></param>
        /// <param name="Loosers"></param>
        public void EndFight(FightTeam Winners, FightTeam Loosers)
        {
            if (Event_onEndFight != null) Event_onEndFight.Invoke(Winners, Loosers);
            OverridableEndFight(Winners, Loosers);
        }
        public abstract void OverridableEndFight(FightTeam Winners, FightTeam Loosers);

        /// <summary>
        /// Temps avant lancement du combat automatique
        /// </summary>
        /// <returns></returns>
        public abstract int GetStartTimer();

        /// <summary>
        /// Temps de tour par joueurs
        /// </summary>
        /// <returns></returns>
        public abstract int GetTurnTime();

        /// <summary>
        /// Serialisation pour les Packet d'epée de combat
        /// </summary>
        /// <param name="Packet"></param>
        public abstract void SerializeAs_FlagDisplayInformations(StringBuilder Packet);

        /// <summary>
        /// Serialisation pour le Packet de la liste des combats sur la map
        /// </summary>
        /// <returns></returns>
        public abstract string SerializeAs_FightListInformations();

        /// <summary>
        /// Id unique du combat
        /// </summary>
        public int FightId
        {
            get;
            set;
        }

        /// <summary>
        /// Team attaquants
        /// </summary>
        public FightTeam Team1
        {
            get
            {
                return this.myTeam1;
            }
        }

        /// <summary>
        /// Team defenseurs
        /// </summary>
        public FightTeam Team2
        {
            get
            {
                return this.myTeam2;
            }
        }

        /// <summary>
        /// Type de combat
        /// </summary>
        public FightType FightType
        {
            get;
            set;
        }

        /// <summary>
        /// Etat du combat
        /// </summary>
        public FightState FightState
        {
            get;
            set;
        }

        /// <summary>
        /// Etat de l'actualisation du combat
        /// </summary>
        public FightLoopState FightLoopState
        {
            get;
            set;
        }

        /// <summary>
        /// Map sur laquelle se passe le combat
        /// </summary>
        public Map Map
        {
            get;
            set;
        }

        /// <summary>
        /// Combattant actuel
        /// </summary>
        public Fighter CurrentFighter
        {
            get;
            set;
        }

        /// <summary>
        /// Liste des combattants encore vivants
        /// </summary>
        public List<Fighter> AliveFighters
        {
            get
            {
                return this.myTeam1.GetAliveFighters().Concat(this.myTeam2.GetAliveFighters()).ToList();
            }
        }

        /// <summary>
        /// Liste des combattans mort ou vivants
        /// </summary>
        public List<Fighter> Fighters
        {
            get
            {
                return this.myTeam1.GetFighters().Concat(this.myTeam2.GetFighters()).ToList();
            }
        }

        /// <summary>
        /// Recuperation d'un Id unique pour les combattants
        /// </summary>
        public int NextActorId
        {
            get
            {
                int id = this.myNextActorId--;
                while (this.Fighters.Exists(x => x.ActorId == id))
                {
                    id = this.myNextActorId--;
                }
                return id;
            }
        }

        public int NextActorInvocationId(Fighter caster)
        {
            var Id = this.NextActorId - (caster.InvokTotal+1);
            while (this.Fighters.Exists(x => x.ActorId == Id))
            {
                    Id--;
            }
            return Id;
        }

        protected ChatChannel myChatChannel = new ChatChannel(ChatChannelEnum.CHANNEL_GENERAL);
        protected Dictionary<FightTeam, Dictionary<int, FightCell>> myFightCells = new Dictionary<FightTeam, Dictionary<int, FightCell>>();
        protected Dictionary<string, FightTimer> myTimers = new Dictionary<string, FightTimer>();
        protected Dictionary<int, FightCell> myCells = new Dictionary<int, FightCell>();

        protected List<FightGroundLayer> Layers = new List<FightGroundLayer>();

        public List<Challenge> Challanges = new List<Challenge>();
        protected FightWorker myWorker = new FightWorker();
        protected FightTeam myTeam1 = new FightTeam(0);
        protected FightTeam myTeam2 = new FightTeam(1);
        protected List<WorldClient> mySpectators = new List<WorldClient>();

        protected int myLoopTimeOut = -1;
        protected int myLoopActionTimeOut;
        protected int myNextActorId = -1000;
        protected GameFightEndResult myResult;
        protected List<GameAction> myActions = new List<GameAction>();
        //protected long StartedTime;

        public long startTime
        {
            get;
            set;
        }
        public long FightTime
        {
            get;
            set;
        }

        /// <summary>
        /// Contructeur unique
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Map"></param>
        public Fight(FightType Type, Map Map)
        {
            this.FightState = FightState.STATE_PLACE;
            this.startTime = 0;
            this.FightTime = -1;
            this.FightType = Type;
            this.Map = Map;
            this.FightId = this.Map.NextFightId;
            this.myResult = new GameFightEndResult(this);
            this.InitCells();
        }

        // recupere un combattant par son id
        public Fighter GetFighter(long FighterId)
        {
            return this.Fighters.Find(x => x.ActorId == FighterId);
        }

        public FightTeam GetTeam1()
        {
            return myTeam1;
        }

        public FightTeam GetTeam2()
        {
            return myTeam2;
        }

        public void addLayer(FightGroundLayer layer)
        {
            this.Layers.Add(layer);
        }

        public void removeLayer(FightGroundLayer layer)
        {
            this.Layers.Remove(layer);
        }

        public List<FightGroundLayer> GetLayers()
        {
            return this.Layers;
        }

        // Initialisation des cells de combat par equipes
        private void InitCells()
        {
            // Ajout des cells
            foreach (var Cell in this.Map.GetCells())
            {
                this.myCells.Add(Cell.Id, new FightCell(Cell.Id, Cell.Walkable, Cell.LoS));
            }

            // Ajout
            this.myFightCells.Add(this.myTeam1, new Dictionary<int, FightCell>());
            this.myFightCells.Add(this.myTeam2, new Dictionary<int, FightCell>());

            // Verifi si les cells deja split sinon les splits
            if (Fight.MAP_FIGHTCELLS.ContainsKey(this.Map.Id))
            {
                // Ajout
                lock (Fight.MAP_FIGHTCELLS)
                {
                    foreach (var Cell in Fight.MAP_FIGHTCELLS[this.Map.Id][0])
                        this.myFightCells[this.myTeam1].Add(Cell, this.myCells[Cell]);
                    foreach (var Cell in Fight.MAP_FIGHTCELLS[this.Map.Id][1])
                        this.myFightCells[this.myTeam2].Add(Cell, this.myCells[Cell]);
                }
            }
            else
            {
                if (this.Map.FightCell != string.Empty && this.Map.FightCell.Contains('|') && this.Map.FightCell != "|" && this.Map.FightCell.Split('|').Length == 2)
                {
                    var Data = this.Map.FightCell.Split('|');
                    bool validStartCells = true;
                    for (int i = 0; i < Data[0].Length - 2; i += 2)
                    {
                        var Cell = this.myCells[MapCrypter.CellCharCodeToId(Data[0].Substring(i, 2))];
                        if (Map.getCell(Cell.Id) == null || !Map.getCell(Cell.Id).isWalkable(true))
                        {
                            validStartCells = false;
                            break;
                        }
                        this.myFightCells[this.myTeam1].Add(MapCrypter.CellCharCodeToId(Data[0].Substring(i, 2)), Cell);
                    }
                    if (validStartCells)
                    {
                        for (int i = 0; i < Data[1].Length - 2; i += 2)
                        {
                            var Cell = this.myCells[MapCrypter.CellCharCodeToId(Data[1].Substring(i, 2))];
                            if (Map.getCell(Cell.Id) == null || !Map.getCell(Cell.Id).isWalkable(true))
                            {
                                validStartCells = false;
                                break;
                            }
                            this.myFightCells[this.myTeam2].Add(MapCrypter.CellCharCodeToId(Data[1].Substring(i, 2)), Cell);
                        }
                    }
                    if (!validStartCells)
                    {
                        Logger.Info("New FightCells generated on this map!");
                        this.myFightCells[this.myTeam1].Clear();
                        this.myFightCells[this.myTeam2].Clear();
                        Couple<List<FightCell>, List<FightCell>> startCells = Algo.GenRandomFightPlaces(this);

                        foreach (var Cell in startCells.first)
                            this.myFightCells[this.myTeam1].Add(Cell.Id, Cell);
                        foreach (var Cell in startCells.second)
                            this.myFightCells[this.myTeam2].Add(Cell.Id, Cell);

                        this.Map.FightCell = Algo.SerializeAsStartFightCells(startCells);
                    }
                    lock (Fight.MAP_FIGHTCELLS)
                    {
                        Fight.MAP_FIGHTCELLS.Add(this.Map.Id, new Dictionary<int, List<int>>());
                        Fight.MAP_FIGHTCELLS[this.Map.Id].Add(0, this.myFightCells[this.myTeam1].Select(x => x.Key).ToList());
                        Fight.MAP_FIGHTCELLS[this.Map.Id].Add(1, this.myFightCells[this.myTeam2].Select(x => x.Key).ToList());
                    }
                }
                else
                {
                    Logger.Info("New FightCells generated on this map!");
                    this.myFightCells[this.myTeam1].Clear();
                    this.myFightCells[this.myTeam2].Clear();
                    Couple<List<FightCell>, List<FightCell>> startCells = Algo.GenRandomFightPlaces(this);

                    foreach (var Cell in startCells.first)
                        this.myFightCells[this.myTeam1].Add(Cell.Id, Cell);
                    foreach (var Cell in startCells.second)
                        this.myFightCells[this.myTeam2].Add(Cell.Id, Cell);

                    this.Map.FightCell = Algo.SerializeAsStartFightCells(startCells);

                    lock (Fight.MAP_FIGHTCELLS)
                    {
                        Fight.MAP_FIGHTCELLS.Add(this.Map.Id, new Dictionary<int, List<int>>());
                        Fight.MAP_FIGHTCELLS[this.Map.Id].Add(0, this.myFightCells[this.myTeam1].Select(x => x.Key).ToList());
                        Fight.MAP_FIGHTCELLS[this.Map.Id].Add(1, this.myFightCells[this.myTeam2].Select(x => x.Key).ToList());
                    }
                }
            }
        }

        /// <summary>
        /// Initialisation du combat
        /// </summary>
        /// <param name="Attacker"></param>
        /// <param name="Defender"></param>
        protected void InitFight(Fighter Attacker, Fighter Defender)
        {
            // Les leaders d'equipes
            this.myTeam1.SetLeader(Attacker);
            this.myTeam2.SetLeader(Defender);

            // On despawn avant la vue du flag de combat
            Attacker.JoinFight();
            Defender.JoinFight();

            // Flags de combat
            this.Map.SendToMap(new GameFightFlagDisplayMessage(this));

            // Rejoins les combats
            this.JoinFightTeam(Attacker, this.myTeam1, true);
            this.JoinFightTeam(Defender, this.myTeam2, true);

            // Si un timer pour le lancement du combat
            if (this.GetStartTimer() != -1)
            {
                this.StartTimer("StartTimer", this.StartFight, this.GetStartTimer());
            }
        }

        /// <summary>
        /// Un ocmbattant et pret a combattre
        /// </summary>
        /// <param name="Fighter"></param>
        public void SetFighterReady(Fighter Fighter)
        {
            // Si combat deja commencé on arrete
            if (this.FightState != Fights.FightState.STATE_PLACE)
                return;

            Fighter.TurnReady = Fighter.TurnReady == false;

            this.SendToFight(new GameReadyMessage(Fighter.ActorId, Fighter.TurnReady));

            // Debut du combat si tout le monde ready
            if (this.IsAllTurnReady() && FightType != Fights.FightType.TYPE_PVT && FightType != Fights.FightType.TYPE_PVMA)
            {
                this.StartFight();
            }
        }

        /// <summary>
        /// Change de place
        /// </summary>
        /// <param name="Fighter"></param>
        public void SetFighterPlace(Fighter Fighter, int CellId)
        {
            // Deja pret ?
            if (Fighter.TurnReady)
                return;

            FightCell Cell = null;

            // Existante ?
            if (this.myFightCells[Fighter.Team].TryGetValue(CellId, out Cell))
            {
                // Aucun persos dessus ?
                if (Cell.CanWalk())
                {
                    // Affectation
                    Fighter.SetCell(Cell);

                    // Changement de position
                    this.SendToFight(new GameInformationCoordinateMessage(new List<Fighter>() { Fighter }));
                }
            }
        }

        /// <summary>
        /// Demarre un timer de combat
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Milliseconds"></param>
        /// <param name="Callback"></param>
        /// <param name="DueTime"></param>
        /// <param name="State"></param>
        public void StartTimer(string Name, Action<Object> Callback, int WaitTime = 0, int DueTime = -1, Object State = null)
        {
            var Timer = new FightTimer();
            Timer.RegisterCallback(Callback);

            lock (this.myTimers)
                this.myTimers.Add(Name, Timer);

            Timer.Start(WaitTime, DueTime, State);
        }

        /// <summary>
        /// Stop un timer de combat
        /// </summary>
        /// <param name="Name"></param>
        public void StopTimer(string Name)
        {
            FightTimer Timer;

            lock (this.myTimers)
            {
                if (this.myTimers.TryGetValue(Name, out Timer))
                {
                    Timer.Stop();

                    this.myTimers.Remove(Name);
                }
            }
        }

        /// <summary>
        /// Debut d'une action
        /// </summary>
        /// <param name="Action"></param>
        public void StartAction(GameAction Action)
        {
            lock (this.myActions)
                this.myActions.Add(Action);

            if (Action.Actor.ActorType == GameActorTypeEnum.TYPE_CHARACTER)
            {

                this.SendToFight(new GameActionStartMessage(Action.Actor.ActorId));
            }

            this.myLoopActionTimeOut = Environment.TickCount + 5000;
        }

        /// <summary>
        /// Fin de l'action
        /// </summary>
        /// <param name="Action"></param>
        public void StopAction(Fighter Fighter)
        {
            lock (this.myActions)
            {
                if (this.myActions.Any(x => x.Actor == Fighter))
                {
                    var Action = this.myActions.Find(x => x.Actor == Fighter);

                    Action.EndExecute();

                    if (Action.Actor is CharacterFighter)
                    {
                        this.SendToFight(new GameActionFinishMessage(Fighter.ActorId));
                    }
                    this.myActions.Remove(Action);
                }
            }
        }

        /// <summary>
        /// Veifie si toute les actions son terminé
        /// </summary>
        /// <returns></returns>
        public bool IsActionsFinish()
        {
            return this.myActions.Count == 0;
        }

        /// <summary>
        /// Rejointe du combat.
        /// </summary>
        /// <param name="Character"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void JoinFightTeam(Fighter Fighter, FightTeam Team, bool Leader = false, int cell = -1, bool sendInfos = true)
        {
            // On detruit l'entité de la map;
            if (!Leader)
                Fighter.JoinFight();

            // On envois l'ajout du joueur a la team sur la map
            this.Map.SendToMap(new GameFightTeamFlagFightersMessage(new List<Fighter>() { Fighter }, Team.LeaderId));

            // Ajout a la team
            Team.FighterJoin(Fighter);

            // Cell de combat
            if (cell == -1)
                Fighter.SetCell(this.GetFreeSpawnCell(Team));
            else
                Fighter.SetCell(this.GetCell(cell));


            // Spawn d'une nouvelle entitée     
            this.SendToFight(new GameActorShowMessage(GameActorShowEnum.SHOW_SPAWN, Fighter));

            // Envois uniquement si personnage
            if (Fighter is CharacterFighter)
            {
                var CharacterFighter = Fighter as CharacterFighter;

                if (CharacterFighter.Client != null)
                {
                    this.RegisterToChat(CharacterFighter.Client);

                    using (var Buffer = new CachedBuffer(CharacterFighter.Client))
                    {
                        Buffer.Append(new GameJoinMessage((short)this.FightState, (short)(this.FightType == Fights.FightType.TYPE_CHALLENGE ? 1 : 0), 1, 0, (short)this.GetStartTimer()));
                        Buffer.Append(new GamePlaceMessage(this.Map.FightCell, Team.Id));
                        Buffer.Append(new GameMapComplementaryInformationsMessage(this.Fighters));
                    }
                }
            }
            if (sendInfos && FightType == Fights.FightType.TYPE_PVT)
            {
                //(this as PercepteurFight).TaxCollector.inFight = (byte)2;
                // On actualise la guilde+Message d'attaque FIXME
                foreach (var z in (this as PercepteurFight).TaxCollector.Guild.CharactersGuildCache.Where(x => x.getPerso() != null && x.getPerso().IsOnline()))
                {
                    TaxCollector.parseAttaque(z.getPerso(), (this as PercepteurFight).TaxCollector.GuildID);
                    TaxCollector.parseDefense(z.getPerso(), (this as PercepteurFight).TaxCollector.GuildID);
                }
            }
        }

        /// <summary>
        /// Rejointe d'un combat en mode spectateur.
        /// </summary>
        /// <param name="Client"></param>
        public void JoinFightSpectator(WorldClient Client)
        {
            // On l'ajoute au spectateurs
            lock (this.mySpectators)
                this.mySpectators.Add(Client);

            // On enleve l'entitée de la map
            Client.GetCharacter().DestroyFromMap();

            // On lui ajoute la gameaction
            Client.AddGameAction(new GameFightSpectator(Client, this));

            // On enregiste le client
            Client.RegisterWorldEvent(this);

            // On lui affecte le combat
            Client.SetFight(this);

            // On lui envoi les données sur le combat
            using (var Buffer = new CachedBuffer(Client))
            {
                Buffer.Append(new GameJoinMessage((short)this.FightState, 0, 0, 1, 0));
                Buffer.Append(new GameMapComplementaryInformationsMessage(this.AliveFighters));
                foreach (var Challenge in Challanges)
                {
                    Buffer.Append(new FightShowChallenge(Challenge));
                }
                Buffer.Append(new GameStartMessage());
                Buffer.Append(new GameTurnListMessage(this.myWorker.Fighters));
                Buffer.Append(new GameTurnStartMessage(this.CurrentFighter.ActorId, this.GetTurnTime()));
            }
            // Envoi l'infos dans le combat comme quoi il join le combat
            this.SendToFight(new TextInformationMessage(TextInformationTypeEnum.INFO, 36, Client.GetCharacter().Name));
        }

        /// <summary>
        /// Kick tous les spectateur du combat
        /// </summary>
        public void KickSpectators(bool End = false)
        {
            lock (this.mySpectators)
            {
                var Spectators = new List<WorldClient>();
                Spectators.AddRange(this.mySpectators);
                Spectators.ForEach(x => this.LeaveSpectator(x, End));
            }
        }

        /// <summary>
        /// Retourne liste des combattants
        /// </summary>
        public IEnumerable<Fighter> getWorkerFighters()
        {
            return this.myWorker.Fighters;
        }

        /// <summary>
        ///  Init des tours
        /// </summary>
        public void RemakeTurns()
        {
            this.myWorker.RemakeTurns(this.Fighters);
        }



        /// <summary>
        /// Quitte le combat (spectateur)
        /// </summary>
        /// <param name="Client"></param>
        public void LeaveSpectator(WorldClient Client, bool End = false)
        {
            // Retire le client de la liste
            lock (this.mySpectators)
                this.mySpectators.Remove(Client);

            // On le deabonne
            Client.UnRegisterWorldEvent(this);

            // On stop le combat
            Client.EndGameAction(GameActionTypeEnum.FIGHT);

            // On reinit
            Client.SetFight(null);

            // On change le status
            Client.SetState(WorldState.STATE_GAME_CREATE);

            // Il quitte le combat
            if (!End)
                Client.Send(new GameLeaveMessage());
        }

        /// <summary>
        ///  Bloque les spectateur/jointe et demande d'aide
        /// </summary>
        /// <param name="Fighter"></param>
        /// <param name="Type"></param>
        public void ToggleLock(Fighter Fighter, ToggleTypeEnum Type)
        {
            bool Value = Fighter.Team.IsToggle(Type) == false;

            Fighter.Team.Toggle(Type, Value);

            if (this.FightState == FightState.STATE_PLACE)
            {
                this.Map.SendToMap(new GameFightOptionMessage(Type == ToggleTypeEnum.TYPE_NEW_PLAYER ? 'A' : (char)Type, Value, Fighter.Team.LeaderId));
            }

            PacketBase Message = null;

            switch (Type)
            {
                case ToggleTypeEnum.TYPE_NEW_PLAYER:
                    if (Value)
                        Message = new TextInformationMessage(TextInformationTypeEnum.INFO, 95);
                    else
                        Message = new TextInformationMessage(TextInformationTypeEnum.INFO, 96);
                    break;

                case ToggleTypeEnum.TYPE_HELP:
                    if (Value)
                        Message = new TextInformationMessage(TextInformationTypeEnum.INFO, 103);
                    else
                        Message = new TextInformationMessage(TextInformationTypeEnum.INFO, 104);
                    break;

                case ToggleTypeEnum.TYPE_PARTY:
                    if (Value)
                        Message = new TextInformationMessage(TextInformationTypeEnum.INFO, 93);
                    else
                        Message = new TextInformationMessage(TextInformationTypeEnum.INFO, 94);
                    break;

                case ToggleTypeEnum.TYPE_SPECTATOR:
                    if (Value)
                    {
                        Message = new TextInformationMessage(TextInformationTypeEnum.INFO, 40);

                        // on kick les spectateurs
                        this.KickSpectators();
                    }
                    else
                        Message = new TextInformationMessage(TextInformationTypeEnum.INFO, 39);
                    break;
            }

            this.SendToFight(Message);
        }

        /// <summary>
        /// Debut du combat
        /// </summary>
        /// <param name="Obj"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void StartFight(Object Obj = null)
        {
            // Si combat deja lancé
            if (this.FightState != Fights.FightState.STATE_PLACE)
                return;
            
            //Percepteur
            if (this is PercepteurFight)
            {
                (this as PercepteurFight).TaxCollector.inFight = (byte)2;
                // On actualise la guilde+Message d'attaque FIXME
                GuildAttackedTaxcollector packetAttacked = new GuildAttackedTaxcollector(AttackedTaxcollectorState.ATTACKED, (this as PercepteurFight).TaxCollector);
                GuildFightInformationsMesssage packetInfos = new GuildFightInformationsMesssage((this as PercepteurFight).TaxCollector.Guild);
                foreach (var z in (this as PercepteurFight).TaxCollector.Guild.CharactersGuildCache.Where(x => x.getPerso() != null && x.getPerso().IsOnline()))
                {
                    z.getPerso().Send(packetInfos);
                    TaxCollector.parseAttaque(z.getPerso(), (this as PercepteurFight).TaxCollector.GuildID);
                    TaxCollector.parseDefense(z.getPerso(), (this as PercepteurFight).TaxCollector.GuildID);
                    z.getPerso().Send(packetAttacked);
                }
            }

            if (this is PrismFight)
            {
                (this as PrismFight).Prisme.inFight = -2;
                //AnalyseAttack
                var Packets = new List<PacketBase>();
                foreach (var Prisme in PrismeTable.Cache.Values.Where(x => x.Alignement == (this as PrismFight).Prisme.Alignement && (x.inFight == 0 || x.inFight == -2)))
                {
                    Packets.Add(new PrismInformationsAttackMessage(Prisme.PrismAttackers(Prisme.ActorId, Prisme.CurrentFight)));
                }
                //AnazlyuzeDefense
                foreach (var Prisme in PrismeTable.Cache.Values.Where(x => x.Alignement == (this as PrismFight).Prisme.Alignement && (x.inFight == 0 || x.inFight == -2)))
                {
                    Packets.Add(new PrismInformationsDefenseMessage(Prisme.PrismDefenders(Prisme.ActorId, Prisme.CurrentFight)));
                }
                foreach (var packet in Packets)
                {
                    Network.WorldServer.GetChatController().getAlignementChannel((this as PrismFight).Prisme.AlignmentType).Send(packet);
                }
            }

            // Destruction de la blade de combat
            this.Map.SendToMap(new GameFightFlagDestroyMessage(this.FightId));

            // Preparation du lancement
            this.FightState = FightState.STATE_INIT;

            foreach(var Chall in Challanges){
                PacketBase packet = new FightShowChallenge(Chall);
                SendToFight(packet);
            }

            // Arret du timer
            this.StopTimer("StartTimer");

            //Inscrit le temps de lancement.
            this.startTime = Program.currentTimeMillis();
            this.FightTime = this.startTime - Program.getDiffUTCTime();

            // Init des tours
            this.myWorker.InitTurns(this.Fighters);

            // Positions des joueurs
            this.SendToFight(new GameInformationCoordinateMessage(this.Fighters));

            // GameStart
            this.SendToFight(new GameStartMessage());

            // Liste des tours
            this.SendToFight(new GameTurnListMessage(this.myWorker.Fighters));

            //Etat Chevauchant
            this.Fighters.Where(x => x.ActorType == GameActorTypeEnum.TYPE_CHARACTER && (x as CharacterFighter).Character.isOnMount()).ToList().ForEach(x => this.SendToFight(new FightGameActionMessage(950, x.ActorId + "", x.ActorId + "," + 11 + ",1")));

            // Reset du ready
            this.SetAllUnReady();

            // En attente de lancement
            this.FightLoopState = Fights.FightLoopState.STATE_WAIT_START;

            if (Event_onStartFight != null) Event_onStartFight.Invoke();

            // Lancement du gameLoop 10 ms d'interval.
            this.StartTimer("GameLoop", this.GameLoop, 10, 10);
        }

        /// <summary>
        /// Fonction principale du combat
        /// </summary>
        /// <param name="Obj"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void GameLoop(Object Obj = null)
        {
            try
            {
                // Switch sur le status et verif fin de tour
                switch (this.FightLoopState)
                {
                    case FightLoopState.STATE_WAIT_START: // En attente de lancement
                        this.FightState = FightState.STATE_ACTIVE;
                        this.FightLoopState = FightLoopState.STATE_WAIT_READY;
                        this.BeginTurn();
                        break;

                    case FightLoopState.STATE_WAIT_TURN: // Fin du tour par force a cause du timeout
                        if (this.myLoopTimeOut < Environment.TickCount)
                        {
                            if (this.IsActionsFinish() || this.myLoopActionTimeOut < Environment.TickCount)
                            {
                                this.EndTurn(); // Fin du tour
                            }
                        }
                        break;

                    case FightLoopState.STATE_END_TURN: // Fin du tour par le joueur
                        if (this.IsActionsFinish() || this.myLoopActionTimeOut < Environment.TickCount)
                        {
                            this.EndTurn(); // Fin du tour
                        }
                        break;

                    case FightLoopState.STATE_WAIT_READY: // En attente des joueurs x ...
                        if (this.IsAllTurnReady())
                        {
                            this.MiddleTurn();
                            this.BeginTurn();
                        }
                        else if (this.myLoopTimeOut < Environment.TickCount)
                        {
                            // TODO SEND UNREADY
                            this.MiddleTurn();
                            this.BeginTurn();
                        }
                        break;

                    case FightLoopState.STATE_WAIT_AI: // Artificial intelligence
                        if (this.CurrentFighter is VirtualFighter)
                        {
                            // Lancement de l'IA pour 30 secondes maximum
                            (this.CurrentFighter as VirtualFighter).Mind.runAI();
                            //new AIProcessor(this, this.CurrentFighter).applyIA(Environment.TickCount + 30000); 
                        }
                        else if (this.CurrentFighter.ObjectType == FightObjectType.OBJECT_STATIC)
                        {
                            Thread.Sleep(500);
                        }
                        // Fin de tour
                        if (this.FightLoopState != FightLoopState.STATE_WAIT_END)
                            this.FightLoopState = FightLoopState.STATE_END_TURN;
                        break;

                    case FightLoopState.STATE_WAIT_END: // Fin du combat
                        if (!HasFinished || this.IsActionsFinish() || this.myLoopActionTimeOut < Environment.TickCount)
                        {
                            this.EndTurn(true);
                            System.Threading.Thread.Sleep(500);
                            this.myTeam1.EndFight();
                            this.myTeam2.EndFight();
                            this.EndFight(this.GetWinners(), this.GetEnnemyTeam(this.GetWinners()));
                            HasFinished = true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Fight::GameLoop() " + ex.ToString());
            }

        }

        public bool HasFinished = false;

        internal bool IsAvailableCell(int aCell)
        {
            return GetCell(aCell).IsWalkable() && !GetCell(aCell).HasGameObject(FightObjectType.OBJECT_FIGHTER) && !GetCell(aCell).HasGameObject(FightObjectType.OBJECT_STATIC);
        }




        /// <summary>
        /// Tentative de deplacement
        /// </summary>
        /// <param name="Fighter"></param>
        /// <param name="Path"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public GameMapMovement TryMove(Fighter Fighter, MovementPath Path)
        {
            // Pas a lui de jouer
            if (Fighter != this.CurrentFighter)
            {
                return null;
            }

            // Pas assez de point de mouvement
            if (Path.MovementLength > Fighter.MP ||Path.MovementLength == -1)
            {
                return null;
            }

            var TacledChance = Pathfinder.TryTacle(Fighter);

            // Si tacle
            if (TacledChance != -1 && !this.CurrentFighter.States.HasState(FighterStateEnum.STATE_ENRACINER))
            {
                // XX A été taclé
                this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_TACLE, Fighter.ActorId));

                var LostAP = Fighter.AP * TacledChance / 100;
                var LostMP = LostAP;

                if (LostAP < 0) LostAP = 0;

                Fighter.UsedAP += LostAP;

                this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_LOSTPA, Fighter.ActorId, Fighter.ActorId + ",-" + LostAP));

                if (LostMP > Fighter.MP) LostMP = Fighter.MP;

                Fighter.UsedMP += LostMP;

                this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_LOSTPM, Fighter.ActorId, Fighter.ActorId + ",-" + LostMP));

                // Annule le deplacement
                return null;
            }

            this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.MAP_MOVEMENT, Fighter.ActorId, Path.ToString()));

            Fighter.UsedMP += Path.MovementLength;

            var GameMovement = new GameMapMovement(this, Fighter, Path);

            this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_LOSTPM, Fighter.ActorId, Fighter.ActorId + ",-" + Path.MovementLength));

            this.StartAction(GameMovement);

            //this.GetCell(Path.EndCell).GetObjects<FightTrap>().ForEach(x => x.onTraped(Fighter));

            Fighter.SetCell(this.GetCell(Path.EndCell));

            return GameMovement;
        }

        /// <summary>
        /// Verifi s'il peu lancer un sort
        /// </summary>
        /// <param name="Fighter"></param>
        /// <param name="Spell"></param>
        /// <param name="CellId"></param>
        /// <returns></returns>
        public bool CanLaunchSpell(Fighter Fighter, SpellLevel Spell, int CurrentCell, int CellId, long TargetId)
        {
            // Fake caster
            if (Fighter != this.CurrentFighter)
                return false;

            // Fake cellId
            if (!this.myCells.ContainsKey(CellId))
                return false;

            FightCell cell = myCells[CellId];
            if (!cell.IsWalkable() || !cell.LineOfSight())
            {
                return false;
            }

            // PA manquant ?
            if (Fighter.AP < Spell.APCost)
                return false;

            // Distance
            var Distance = Pathfinder.GoalDistance(this.Map, CurrentCell, CellId);

            // PO max du sort + stats du lanceur
            var MaxPo = Spell.MaxPO + (Spell.AllowPOBoost ? Fighter.Stats.GetTotal(EffectEnum.AddPO) : 0);

            // S'il a des malus qui reduisent trop
            if (MaxPo - Spell.MinPO < 1) MaxPo = Spell.MinPO;

            // Mauvaise distance ?
            if (Distance > MaxPo || Distance < Spell.MinPO)
                return false;

            if (!cell.CanWalk() && Spell.NeedEmptyCell)
            {
                return false;
            }

            if (!Fighter.SpellsController.CanLaunchSpell(Spell, TargetId))
            {
                return false;
            }

            if (CurrentCell == CellId)
            {
                return true;
            }

            if (Spell.InLine && !Pathfinder.InLine(this.Map, CurrentCell, CellId))
            {
                return false;
            }

            if (Spell.Effects.Exists(x => x.EffectType == EffectEnum.UseTrap))
            {
                foreach(FightTrapLayer layer in cell.GetObjects<FightTrapLayer>()){
                    if (layer.myCell == cell)
                    {
                        return false;
                    }
                }
                
            }
            
            return true;
        }

        /*public bool CanLaunchSpell(Fighter Fighter, SpellLevel Spell, long TargetId)
        {
            // Fake caster
            if (Fighter != this.CurrentFighter)
                return false;

            // PA manquant ?
            if (Fighter.AP < Spell.APCost)
                return false;


            return Fighter.SpellsController.CanLaunchSpell(Spell, TargetId);
        }*/

        public bool OnlyOneTeam()
        {
            bool Team1 = false;
            bool Team2 = false;

            foreach (var Player in Fighters)
            {
                if ((Player.Team.Id == 0) && (!Player.Dead) && (!Player.Left)) Team1 = true;
                if ((Player.Team.Id == 1) && (!Player.Dead) && (!Player.Left)) Team2 = true;
            }

            return !(Team1 && Team2);
        }

        /// <summary>
        /// Lancement du CAC
        /// </summary>
        /// <param name="Fighter"></param>
        /// <param name="InvetoryItem"></param>
        /// <param name="CellId"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LaunchWeapon(Fighter Fighter, int CellId)
        {
            // Combat encore en cour ?
            if (this.FightState != Fights.FightState.STATE_ACTIVE)
                return;
            var TargetE = this.HasEnnemyInCell(CellId, Fighter.Team);
            // Fake caster
            if (Fighter != this.CurrentFighter)
                return;

            InventoryItemModel Weapon = (Fighter as CharacterFighter).Character.InventoryCache.GetItemInSlot(ItemSlotEnum.SLOT_ARME);

            if (Weapon == null)
            {
                this.LaunchSpell(Fighter, SpellTable.Cache[0].GetLevel(1), CellId, isPunch: true);
            }
            else
            {
                if ((ItemTypeEnum)Weapon.Template.Type == ItemTypeEnum.ITEM_TYPE_PIERRE_AME)
                {
                    this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_USEWEAPON_ECHEC, Fighter.ActorId));
                    this.SendToFight(new GameActionFightMessage(0, Fighter.ActorId));
                    EndTurn();
                }

                int APCost = Weapon.Template.PACost;

                if (Fighter.AP < APCost)
                    return;

                this.SendToFight(new GameActionSceneMessage(Fighter.ActorId));


                // Calcul d'echec
                Fighter.UsedAP += APCost;
                var IsEchec = false;
                var TauxEchec = Weapon.Template.TauxEC - Fighter.Stats.GetTotal(EffectEnum.AddEchecCritic);
                if (TauxEchec < 2) TauxEchec = 2;
                if (Weapon.Template.TauxEC != 0 && (Fight.RANDOM.Next(1, Weapon.Template.TauxEC) == 1))
                    IsEchec = true;

                if (IsEchec)
                {
                    this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_USEWEAPON_ECHEC, Fighter.ActorId));
                    this.SendToFight(new GameActionFightMessage(0, Fighter.ActorId));
                    if (Event_onLaunchWeapon != null) Event_onLaunchWeapon.Invoke(Fighter, Weapon, CellId, this.GetCell(CellId).GetFighter(), null, false, IsEchec, false);
                    if (this.FightLoopState != Fights.FightLoopState.STATE_END_FIGHT)
                        if (IsEchec)
                            this.FightLoopState = Fights.FightLoopState.STATE_END_TURN;
                }
                else
                {
                    this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_USEWEAPON, Fighter.ActorId, CellId + ""));
                    var IsCc = false;
                    if (Weapon.Template.TauxCC != 0)
                    {
                        var TauxCC = Weapon.Template.TauxCC - Fighter.Stats.GetTotal(EffectEnum.AddDamageCritic);
                        if (TauxCC < 2) TauxCC = 2;
                        if (Fight.RANDOM.Next(1, TauxCC) == 1)
                            IsCc = true;
                    }
                    IsCc &= !Fighter.States.HasState(FighterStateEnum.STATE_MINIMIZE_EFFECTS);
                    if (IsCc)
                    {
                        this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_LAUNCHSPELL_CRITIC, Fighter.ActorId, "0"));
                    }
                    //si cible invisible dehide

                    var Effects = Weapon.GetStats().GetWeaponEffects();
                    var Targets = new Dictionary<WeaponEffect, List<Fighter>>();
                    foreach (var Effect in Effects.Values)
                    {
                        Targets.Add(Effect, new List<Fighter>());

                        foreach (var Cell in CellZone.GetWeaponCells(Weapon.Template.Type, Map, this.GetCell(CellId), Fighter.Cell.Id))
                        {
                            var FightCell = this.GetCell(Cell);
                            if (FightCell != null)
                                if (FightCell.HasGameObject(FightObjectType.OBJECT_FIGHTER) | FightCell.HasGameObject(FightObjectType.OBJECT_STATIC))
                                    Targets[Effect].AddRange(FightCell.GetObjects<Fighter>());
                        }
                    }
                    foreach (var Effect in Effects.Values)
                    {
                        // Actualisation des morts
                        Targets[Effect].RemoveAll(F => F.Dead);

                        var CastInfos = new EffectCast(Effect.EffectType, 102, CellId, Effect.Min, Effect.Max, 0, 0, 0, Fighter, Targets[Effect], true);

                        if (EffectBase.TryApplyEffect(CastInfos) == -3)
                            break;
                    }
                    this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_LOSTPA, Fighter.ActorId, Fighter.ActorId + ",-" + APCost.ToString()));
                    this.SendToFight(new GameActionFightMessage(0, Fighter.ActorId));
                    if (Event_onLaunchWeapon != null) Event_onLaunchWeapon.Invoke(Fighter, Weapon, CellId, this.GetCell(CellId).GetFighter(), Targets, IsCc, IsEchec, false);
                }

            }

        }


        /// <summary>
        /// Lancement d'un sort
        /// </summary>
        /// <param name="Fighter"></param>
        /// <param name="SpellLevel"></param>
        /// <param name="CellId"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LaunchSpell(Fighter Fighter, SpellLevel SpellLevel, int CellId, bool friend = false, bool isPunch = false)
        {
            // Combat encore en cour ?
            if (this.FightState != Fights.FightState.STATE_ACTIVE)
                return;

            // La cible si elle existe
            var TargetE = this.HasEnnemyInCell(CellId, Fighter.Team);
            if (friend && TargetE == null)
            {
                TargetE = this.HasFriendInCell(CellId, Fighter.Team);
            }
            long TargetId = TargetE == null || TargetE.Dead ? -1 : TargetE.ActorId;

            // Peut lancer le sort ?
            if (!this.CanLaunchSpell(Fighter, SpellLevel, Fighter.CellId, CellId, TargetId))
            {
                Fighter.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 175));
                Fighter.Send(new EmptyMessage("GA0"));
                return;
            }

            Fighter.UsedAP += SpellLevel.APCost;

            // Calcul d'echec
            var IsEchec = false;
            var TauxEchec = SpellLevel.TauxEC - Fighter.Stats.GetTotal(EffectEnum.AddEchecCritic);
            if (TauxEchec < 2) TauxEchec = 2;
            if (SpellLevel.TauxEC != 0 && (Fight.RANDOM.Next(1, SpellLevel.TauxEC) == 1))
                IsEchec = true;

            // Eche critique
            if (IsEchec)
            {
                this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_LAUNCHSPELL_ECHEC, Fighter.ActorId, SpellLevel.SpellCache.ID.ToString()));
                if (!isPunch)
                {
                    if (Event_onLaunchSpell != null) Event_onLaunchSpell.Invoke(Fighter, SpellLevel, CellId, this.GetCell(CellId).GetFighter(), null, false, true);
                }
                else
                {
                    if (Event_onLaunchWeapon != null) Event_onLaunchWeapon.Invoke(Fighter, null, CellId, this.GetCell(CellId).GetFighter(), null, false, true, true);
                }
            }
            else
            {
                SpellLevel.Initialize();
                // Calcul coup critique
                Fighter.SpellsController.Actualise(SpellLevel, TargetId);

                this.StartAction(new GameFightCastSpell(Fighter));

                this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_LAUNCHSPELL, Fighter.ActorId, SpellLevel.SpellCache.ID + "," + CellId + "," + SpellLevel.SpellCache.SpriteID + "," + SpellLevel.Level + "," + SpellLevel.SpellCache.SpriteInfos));

                var IsCc = false;
                if (SpellLevel.TauxCC != 0 && SpellLevel.CriticEffects.Count > 0)
                {
                    var TauxCC = SpellLevel.TauxCC - Fighter.Stats.GetTotal(EffectEnum.AddDamageCritic);
                    if (TauxCC < 2) TauxCC = 2;
                    if (Fight.RANDOM.Next(1, TauxCC) == 1)
                        IsCc = true;
                }
                IsCc &= !Fighter.States.HasState(FighterStateEnum.STATE_MINIMIZE_EFFECTS);
                if (IsCc)
                {
                    this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_LAUNCHSPELL_CRITIC, Fighter.ActorId, SpellLevel.SpellCache.ID.ToString()));
                }
                var Effects = IsCc ? SpellLevel.CriticEffects : SpellLevel.Effects;
                if (Effects == null)
                    Effects = SpellLevel.CriticEffects;
                var Targets = new Dictionary<EffectInfos, List<Fighter>>();
                int TE = 0;
                int num = 0;
                var cellsTargetsCache = new Dictionary<string, List<int>>();
                foreach (var Effect in Effects)
                {
                    Targets.Add(Effect, new List<Fighter>());
                    
                    if (SpellLevel.SpellCache != null ? SpellLevel.SpellCache.effectTargets.Count > num : false)
                    {
                        TE = SpellLevel.SpellCache.effectTargets.ToArray()[num];
                    }
                    if (Effect.RangeType == "C_")
                    {
                        Targets[Effect].AddRange(this.Fighters);
                        //Reperage...
                        if (Effect.EffectType == EffectEnum.Perception)
                        {
                            var traps = this.Layers.OfType<FightTrapLayer>();
                            foreach (var trap in traps)
                            {
                                trap.Show(Fighter);
                            }
                        }

                        foreach (var fighter in this.Fighters)
                        {
                            //Ne touches pas les alliés
                            if (((TE & 1) == 1) && (fighter.Team.Id == Fighter.Team.Id))
                            {
                                Targets[Effect].Remove(fighter);
                            }
                            if ((((TE >> 1) & 1) == 1) && (fighter.ActorId == Fighter.ActorId))
                            {
                                Targets[Effect].Remove(fighter);
                            }
                            //Ne touche pas les ennemies
                            if ((((TE >> 2) & 1) == 1) && (fighter.Team.Id != Fighter.Team.Id))
                            {
                                Targets[Effect].Remove(fighter);
                            }
                            //Ne touche pas les combatants (seulement invocations)
                            if ((((TE >> 3) & 1) == 1) && (fighter.Invocator == null))
                            {
                                Targets[Effect].Remove(fighter);
                            }
                            //Ne touche pas les invocations
                            if ((((TE >> 4) & 1) == 1) && (fighter.Invocator != null))
                            {
                                Targets[Effect].Remove(fighter);
                            }
                            //N'affecte que le lanceur
                            if ((((TE >> 5) & 1) == 1) && (fighter.ActorId != Fighter.ActorId))
                            {
                                Targets[Effect].Remove(fighter); ;
                            }
                        }
                    }
                    else
                    {
                        List<int> TargetCells = null;
                        if (!cellsTargetsCache.ContainsKey(Effect.RangeType))
                        {
                            TargetCells = CellZone.GetCells(Map, CellId, Fighter.CellId, Effect.RangeType);
                            if(!TargetCells.Contains(CellId))TargetCells.Add(CellId);
                            cellsTargetsCache.Add(Effect.RangeType, TargetCells);
                        }
                        else
                        {
                            TargetCells = cellsTargetsCache[Effect.RangeType];
                        }

                        foreach (var Cell in TargetCells)
                        {
                            var FightCell = this.GetCell(Cell);
                            if (FightCell != null)
                            {
                                if (FightCell.HasGameObject(FightObjectType.OBJECT_FIGHTER) || FightCell.HasGameObject(FightObjectType.OBJECT_STATIC))
                                    Targets[Effect].AddRange(FightCell.GetObjects<Fighter>());

                                //Reperage...
                                if (Effect.EffectType == EffectEnum.Perception && FightCell.HasGameObject(FightObjectType.OBJECT_TRAP))
                                {
                                    FightCell.GetObjects<FightTrapLayer>().ForEach(x => x.Show(Fighter));
                                }
                                
                                foreach (var fighter in FightCell.GetObjects<Fighter>())
                                {
                                    //Ne touches pas les alliés
                                    if (((TE & 1) == 1) && (fighter.Team.Id == Fighter.Team.Id))
                                    {
                                        Targets[Effect].Remove(fighter);
                                    }
                                    if ((((TE >> 1) & 1) == 1) && (fighter.ActorId == Fighter.ActorId))
                                    {
                                        Targets[Effect].Remove(fighter);
                                    }
                                    //Ne touche pas les ennemies
                                    if ((((TE >> 2) & 1) == 1) && (fighter.Team.Id != Fighter.Team.Id))
                                    {
                                        Targets[Effect].Remove(fighter);
                                    }
                                    //Ne touche pas les combatants (seulement invocations)
                                    if ((((TE >> 3) & 1) == 1) && (fighter.Invocator == null))
                                    {
                                        Targets[Effect].Remove(fighter);
                                    }
                                    //Ne touche pas les invocations
                                    if ((((TE >> 4) & 1) == 1) && (fighter.Invocator != null))
                                    {
                                        Targets[Effect].Remove(fighter);
                                    }
                                    //N'affecte que le lanceur
                                    if ((((TE >> 5) & 1) == 1) && (fighter.ActorId != Fighter.ActorId))
                                    {
                                        Targets[Effect].Remove(fighter); ;
                                    }
                                }
                            }
                        }
                    }
                    
                    //Si le sort n'affecte que le lanceur et que le lanceur n'est pas dans la zone
                    if (((TE >> 5) & 1) == 1)
                    {
                        if (Targets.ContainsKey(Effect) && !Targets[Effect].Contains(Fighter))
                        {
                            Targets[Effect].Add(Fighter);
                        }
                    }
                    num++;
                }

                var ActualChance = 0;
                foreach (var Effect in Effects)
                {
                    if (Effect.Chance > 0)
                    {
                        if (Fight.RANDOM.Next(1, 100) > (Effect.Chance + ActualChance))
                        {
                            ActualChance += Effect.Chance;
                            continue;
                        }
                        ActualChance -= 100;
                    }

                    // Actualisation des morts
                    Targets[Effect].RemoveAll(F => F.Dead);

                    var CastInfos = new EffectCast(Effect.EffectType, SpellLevel.SpellCache.ID, CellId, Effect.Value1, Effect.Value2, Effect.Value3, Effect.Chance, Effect.Duration, Fighter, Targets[Effect], false, EffectEnum.None, 0, Effect.Spell);

                    if (EffectBase.TryApplyEffect(CastInfos) == -3)
                        break;
                }
                if (!isPunch)
                {
                    if (Event_onLaunchSpell != null) Event_onLaunchSpell.Invoke(Fighter, SpellLevel, CellId, this.GetCell(CellId).GetFighter(), Targets, IsCc, false);
                }
                else
                {
                    if (Event_onLaunchWeapon != null) Event_onLaunchWeapon.Invoke(Fighter, null, CellId, this.GetCell(CellId).GetFighter(), null, IsCc, false, true);
                }
            }

            this.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_LOSTPA, Fighter.ActorId, Fighter.ActorId + ",-" + SpellLevel.APCost.ToString()));

            if (this.FightLoopState != Fights.FightLoopState.STATE_END_FIGHT)
                if (SpellLevel.IsECEndTurn && IsEchec)
                    this.FightLoopState = Fights.FightLoopState.STATE_END_TURN;
        }

        /// <summary>
        /// Deabonne le client de la cell et l'ajoute sur la new
        /// </summary>
        /// <param name="Actor"></param>
        /// <param name="NewCell"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ActorMoved(MovementPath Path, IGameActor Actor, int NewCell)
        {
            var Fighter = Actor as Fighter;

            Fighter.SetCell(this.myCells[NewCell]);
            if (Actor.ActorType == GameActorTypeEnum.TYPE_CHARACTER)
            {
                this.SendToFight(new GameActionFinishMessage(Actor.ActorId));
            }
            if (Event_onActorMoved != null) Event_onActorMoved.Invoke(Fighter, Path, this.GetCell(NewCell));
        }

        /// <summary>
        /// Debut du tour
        /// </summary>
        /// <param name="Obj"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void BeginTurn()
        {
            // Mise a jour du combattant
            this.CurrentFighter = this.myWorker.GetNextFighter();

            // Activation des buffs et fightObjects
            var BeginTurnIndice = this.CurrentFighter.BeginTurn();

            // Mort du joueur ou fin de combat
            if (BeginTurnIndice == -3 || BeginTurnIndice == -2)
                return;

            if (Event_onBeginTurn != null) Event_onBeginTurn(this.CurrentFighter);

            if (CurrentFighter.Dead) return;

            // Timeout du tour
            this.myLoopTimeOut = Environment.TickCount + this.GetTurnTime();

            // Envois debut du tour
            this.SendToFight(new GameTurnStartMessage(this.CurrentFighter.ActorId, this.GetTurnTime()));

            // Status en attente de fin de tour
            this.FightLoopState = FightLoopState.STATE_WAIT_TURN;

            // Monstre passe le tour
            //if (this.CurrentFighter.ActorType == GameActorTypeEnum.TYPE_MONSTER || this.CurrentFighter is DoubleFighter)
            if (this.CurrentFighter is VirtualFighter || this.CurrentFighter.ObjectType == FightObjectType.OBJECT_STATIC)
                this.FightLoopState = Fights.FightLoopState.STATE_WAIT_AI;
        }

        /// <summary>
        /// A la fin d'un tour, reset etc
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void MiddleTurn()
        {
            this.CurrentFighter.MiddleTurn();
            if (Event_onMiddleTurn != null) Event_onMiddleTurn(this.CurrentFighter);
            this.SendToFight(new GameTurnMiddleMessage(this.Fighters));
        }

        /// <summary>
        /// Fin du tour
        /// </summary>
        /// <param name="Obj"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void EndTurn(bool Finish = false)
        {
            // Fin du tour, activation des buffs, pieges etc
            if (this.CurrentFighter.EndTurn() == -3)
                return; // Combat fini a la fin de son tour

            if (Event_onEndTurn != null) Event_onEndTurn(this.CurrentFighter, Finish);

            // Tout le monde doit se synchro
            this.SetAllUnReady();
            if (!Finish)
                // En attente des joueurs
                this.FightLoopState = FightLoopState.STATE_WAIT_READY;

            // Tour fini
            this.SendToFight(new GameTurnFinishMessage(this.CurrentFighter.ActorId));

            if (!Finish)
                // En attente des joueurs xx
                this.SendToFight(new GameTurnReadyMessage(this.CurrentFighter.ActorId));

        }

        /// <summary>
        /// Verification d'une fin de combat apres des actions, attaque, deplacement etc ...
        /// </summary>
        /// <param name="Obj"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool TryEndFight()
        {
            if (this.GetWinners() != null)
            {
                this.FightLoopState = FightLoopState.STATE_WAIT_END;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retourne l'equipe gagnante
        /// </summary>
        /// <param name="Obj"></param>
        public FightTeam GetWinners()
        {
            if (!this.myTeam1.HasFighterAlive())
            {
                return this.myTeam2;
            }
            else if (!this.myTeam2.HasFighterAlive())
            {
                return this.myTeam1;
            }

            return null;
        }

        /// <summary>
        /// Fin du combat
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected void EndFight()
        {
            switch (this.FightState)
            {
                case FightState.STATE_PLACE:
                    this.Map.SendToMap(new GameFightFlagDestroyMessage(this.FightId));
                    break;

                case Fights.FightState.STATE_FINISH:
                    return;
            }

            this.StopTimer("GameLoop");

            if (!(this is ChallengeFight))
            {
                this.GetWinners().GetFighters().Where(x => x.ActorType == GameActorTypeEnum.TYPE_CHARACTER).ToList().ForEach(x => (x as CharacterFighter).Character.FightType = (int)this.FightType);
                this.GetEnnemyTeam(this.GetWinners()).GetFighters().Where(x => x.ActorType == GameActorTypeEnum.TYPE_CHARACTER).ToList().ForEach(x => (x as CharacterFighter).Character.FightType = -2);
                if (this.FightType == Fights.FightType.TYPE_PVT || this.FightType == Fights.FightType.TYPE_PVMA || this.FightType == Fights.FightType.TYPE_KOLIZEUM)
                    this.Fighters.Where(x => x.ActorType == GameActorTypeEnum.TYPE_CHARACTER && (x as CharacterFighter).Character.OldPosition != null).ToList().ForEach(x => (x as CharacterFighter).Character.FightType = -3);
            }

            this.SendToFight(new GameFightEndMessage(this.myResult));

            this.startTime = 0;
            this.FightTime = 0;

            foreach (var Fighter in this.Fighters)
                Fighter.EndFight();

            this.KickSpectators(true);

            this.myFightCells.Clear();
            this.myCells.Clear();
            this.myTimers.Clear();
            this.Layers.Clear();
            this.myWorker.Dispose();
            this.myTeam1.Dispose();
            this.myTeam2.Dispose();

            this.myFightCells = null;
            this.myCells = null;
            this.myTeam1 = null;
            this.myTeam2 = null;
            this.myWorker = null;
            this.myTimers = null;
            this.Layers = null;

            this.Map.RemoveFight(this);

            this.FightState = Fights.FightState.STATE_FINISH;
            this.FightLoopState = FightLoopState.STATE_END_FIGHT;
        }

        #region SubMethods

        /// <summary>
        /// Envoi des combattants et options dans chaque flag
        /// </summary>
        /// <param name="Client"></param>
        public void SendFightFlagInfos(WorldClient Client)
        {
            // Enois des infos combat
            using (var Buffer = new CachedBuffer(Client))
            {
                Buffer.Append(new GameFightFlagDisplayMessage(this));

                Buffer.Append(new GameFightTeamFlagFightersMessage(this.myTeam1.GetFighters(), this.myTeam1.LeaderId));

                Buffer.Append(new GameFightTeamFlagFightersMessage(this.myTeam2.GetFighters(), this.myTeam2.LeaderId));

                Buffer.Append(new GameFightOptionMessage('A', this.myTeam1.IsToggle(ToggleTypeEnum.TYPE_NEW_PLAYER), this.myTeam1.LeaderId));
                Buffer.Append(new GameFightOptionMessage((char)ToggleTypeEnum.TYPE_HELP, this.myTeam1.IsToggle(ToggleTypeEnum.TYPE_HELP), this.myTeam1.LeaderId));
                Buffer.Append(new GameFightOptionMessage((char)ToggleTypeEnum.TYPE_PARTY, this.myTeam1.IsToggle(ToggleTypeEnum.TYPE_PARTY), this.myTeam1.LeaderId));
                Buffer.Append(new GameFightOptionMessage((char)ToggleTypeEnum.TYPE_SPECTATOR, this.myTeam1.IsToggle(ToggleTypeEnum.TYPE_SPECTATOR), this.myTeam1.LeaderId));

                Buffer.Append(new GameFightOptionMessage('A', this.myTeam2.IsToggle(ToggleTypeEnum.TYPE_NEW_PLAYER), this.myTeam2.LeaderId));
                Buffer.Append(new GameFightOptionMessage((char)ToggleTypeEnum.TYPE_HELP, this.myTeam2.IsToggle(ToggleTypeEnum.TYPE_HELP), this.myTeam2.LeaderId));
                Buffer.Append(new GameFightOptionMessage((char)ToggleTypeEnum.TYPE_PARTY, this.myTeam2.IsToggle(ToggleTypeEnum.TYPE_PARTY), this.myTeam2.LeaderId));
                Buffer.Append(new GameFightOptionMessage((char)ToggleTypeEnum.TYPE_SPECTATOR, this.myTeam2.IsToggle(ToggleTypeEnum.TYPE_SPECTATOR), this.myTeam2.LeaderId));
            }
        }

        /// <summary>
        /// Met tous les joueurs non ready
        /// </summary>
        private void SetAllUnReady()
        {
            foreach (var Fighter in this.Fighters)
                Fighter.TurnReady = false;
            foreach (var Fighter in this.Fighters.Where(Fighter => Fighter is DoubleFighter))
                Fighter.TurnReady = true;
        }

        /// <summary>
        /// Les joueurs son prets ?
        /// </summary>
        private bool IsAllTurnReady()
        {
            return this.AliveFighters.All(Fighter => Fighter.TurnReady);
        }

        /// <summary>
        /// Retourne la team par l'Id
        /// </summary>
        /// <param name="TeamId"></param>
        /// <returns></returns>
        public FightTeam GetTeam(int LeaderId)
        {
            return (this.myTeam1.LeaderId == LeaderId ? this.myTeam1 : this.myTeam2);
        }

        /// <summary>
        /// Retourne l'equipe adverse
        /// </summary>
        /// <param name="Team"></param>
        /// <returns></returns>
        public FightTeam GetEnnemyTeam(FightTeam Team)
        {
            return (Team == this.myTeam1 ? this.myTeam2 : this.myTeam1);
        }

        /// <summary>
        /// Retourne l'equipe adverse
        /// </summary>
        /// <param name="Team"></param>
        /// <returns></returns>
        public FightTeam GetAllyTeam(FightTeam Team)
        {
            return (Team == this.myTeam1 ? this.myTeam1 : this.myTeam2);
        }

        /// <summary>
        ///  Verifi si un joueur peu rejoindre
        /// </summary>
        /// <param name="TeamId"></param>
        /// <param name="Character"></param>
        /// <returns></returns>
        public virtual bool CanJoin(FightTeam Team, Player Character)
        {
            return Team.CanJoin(Character) && this.GetFreeSpawnCell(Team) != null;
        }

        /// <summary>
        /// Ouvert au spectateurs ?
        /// </summary>
        /// <returns></returns>
        public bool CanJoinSpectator()
        {
            return this.FightState == Fights.FightState.STATE_ACTIVE && !this.myTeam1.IsToggle(ToggleTypeEnum.TYPE_SPECTATOR) && !this.myTeam2.IsToggle(ToggleTypeEnum.TYPE_SPECTATOR);
        }

        /// <summary>
        /// Pathfind
        /// </summary>
        /// <param name="CellId"></param>
        /// <returns></returns>
        public bool IsCellWalkable(int CellId)
        {
            if (!this.myCells.ContainsKey(CellId))
                return false;

            return this.myCells[CellId].CanWalk();
        }

        /// <summary>
        /// Retourne la cell de combat
        /// </summary>
        /// <param name="CellId"></param>
        /// <returns></returns>
        public FightCell GetCell(int CellId)
        {
            if (this.myCells.ContainsKey(CellId))
                return this.myCells[CellId];
            return null;
        }

        /// <summary>
        ///  Un personnage au alentour
        /// </summary>
        /// <param name="CellId"></param>
        /// <returns></returns>
        public Fighter HasEnnemyInCell(int CellId, FightTeam Team)
        {
            return this.myCells[CellId].HasEnnemy(Team);
        }


        /// <summary>
        ///  Un ami au alentour
        /// </summary>
        /// <param name="CellId"></param>
        /// <returns></returns>
        public Fighter HasFriendInCell(int CellId, FightTeam Team)
        {
            return this.myCells[CellId].HasFriend(Team);
        }

        /// <summary>
        /// Une cell quand une entitée rejoin le combat
        /// </summary>
        /// <param name="Team"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private FightCell GetFreeSpawnCell(FightTeam Team)
        {
            foreach (var Cell in this.myFightCells[Team].Values)
                if (Cell.CanWalk())
                    return Cell;

            return null;
        }

        #endregion

        #region IWorldEvent

        public void SendToFight(PacketBase Packet)
        {
            if (this.Event_SendToFight != null)
                this.Event_SendToFight.Invoke(Packet);
        }

        public void Register(WorldClient Client)
        {
            this.Event_SendToFight += Client.Send;
        }

        public void RegisterToChat(WorldClient Client)
        {
            Client.RegisterChatChannel(this.myChatChannel);
        }

        public void UnRegister(WorldClient Client)
        {
            this.Event_SendToFight -= Client.Send;

            Client.UnRegisterChatChannel(ChatChannelEnum.CHANNEL_GENERAL);
        }

        #endregion

    }
}
