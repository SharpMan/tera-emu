using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Utils;
using Tera.WorldServer.Network;
using Tera.Libs.Network;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Maps;
using Tera.Libs.Helper;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World;
using Tera.WorldServer.World.Character;
using Tera.Libs;
using Tera.WorldServer.World.Fights;
using Tera.Libs.Utils;
using Tera.WorldServer.World.Chats;

namespace Tera.WorldServer.Database.Models
{
    public class Player : IGameActor
    {
        public long ID { get; set; }
        public int Owner { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Color1 { get; set; }
        public int Color2 { get; set; }
        public int Color3 { get; set; }
        public int Cote { get; set; }
        public int Look { get; set; }
        public int Sexe { get; set; }
        public int Classe { get; set; }
        public short Map { get; set; }
        public short Title { get; set; }
        public int CellId { get; set; }
        public long Experience { get; set; }
        public long Kamas { get; set; }
        public int CaractPoint { get; set; }
        public int SpellPoint { get; set; }
        public int LifePer { get; set; }
        public int Energy { get; set; }
        public int AP { get; set; }
        public int MP { get; set; }
        public int Vitality { get; set; }
        public int Wisdom { get; set; }
        public int Strength { get; set; }
        public int Intell { get; set; }
        public int Agility { get; set; }
        public int Chance { get; set; }
        public int MountID { get; set; }
        public int MountXPGive { get; set; }
        public string WornItem { get; set; }
        public GenericStats myStats;
        public CharacterInventory InventoryCache;
        public SpellBook mySpells;
        private CharacterGuild CharacterGuildCache;
        public AccountModel Account;
        public String EnabledChannels { get; set; }
        public int Restriction { get; set; }
        public bool myInitialized = false;
        public int SkinSize = 100;
        public Map myMap { get; set; }
        public Couple<Map, int> OldPosition { get; set; }
        public int Alignement { get; set; }
        public bool showWings = false;
        public int Life { get; set; }
        public int Honor { get; set; }
        public int Deshonor { get; set; }
        public String Stuff { get; set; }
        public String SpellString { get; set; }
        public int AlignementLevel = 0;
        public int FightType = -1;
        public String SavePos { get; set; }
        public List<short> Zaaps = new List<short>();
        public String ZaapString;
        public Mount Mount;
        public MountPark inMountPark;
        public bool isAaway = false;
        public bool isInBank = false;
        public bool isJoiningTaxFight = false;
        public Dictionary<long, Player> Follower = new Dictionary<long, Player>();
        public Player Follows = null;
        public int Orientation
        {
            get;
            set;
        }


        public long ActorId
        {
            get
            {
                return this.ID;
            }
        }

        public int Points
        {
            get
            {
                return AccountTable.getPoints(this.Account);
            }
        }

        //Chat Party
        private List<ChatChannelEnum> myChatChannelEnabled = new List<ChatChannelEnum>();

        public void EnableChatChannel(ChatChannelEnum Channel)
        {
            if (!this.myChatChannelEnabled.Contains(Channel))
                this.myChatChannelEnabled.Add(Channel);
            if (!this.EnabledChannels.Contains((char)Channel))
                this.EnabledChannels += (char)Channel;
        }


        public void DisableChatChannel(ChatChannelEnum Channel)
        {
            if (this.myChatChannelEnabled.Contains(Channel))
                this.myChatChannelEnabled.Remove(Channel);
            if (this.EnabledChannels.Contains((char)Channel))
                this.EnabledChannels = EnabledChannels.Remove(EnabledChannels.IndexOf((char)Channel), 1);
        }
        public List<ChatChannelEnum> GetEnabledChatChannels()
        {
            return this.myChatChannelEnabled;
        }

        public bool IsChatChannelEnable(ChatChannelEnum Channel)
        {
            return this.myChatChannelEnabled.Contains(Channel);
        }



        //Cache party
        public WorldClient Client;

        private bool myCached = false;
        CachedBuffer myBuffer = null;
        public void BeginCachedBuffer()
        {
            if (this.Client != null)
            {
                this.myBuffer = new CachedBuffer(this.Client);
                this.myCached = true;
            }
        }

        public void EndCachedBuffer()
        {
            if (this.myBuffer != null && this.Client != null)
            {
                this.myBuffer.Dispose();
                this.myBuffer = null;
                this.myCached = false;
            }
        }

        public void Send(PacketBase Packet)
        {
            if (this.Client != null)
                if (this.myCached)
                    this.myBuffer.Append(Packet);
                else
                    this.Client.Send(Packet);
        }

        public void SetOnline(WorldClient Client)
        {
            this.Client = Client;
            this.Orientation = 1;
            // init des channels activées
            foreach (char Channel in this.EnabledChannels)
            {
                if ((ChatChannelEnum)Channel == ChatChannelEnum.CHANNEL_ALIGNMENT)
                    Network.WorldServer.GetChatController().RegisterClient(Client, AlignmentType);
                this.EnableChatChannel((ChatChannelEnum)Channel);
            }

            // Enregistre au event de la guilde
            if (this.HasGuild())
            {
                this.GetGuild().Register(Client);
            }

            // init map
            this.myMap = MapTable.Cache.FirstOrDefault(x => x.Key == this.Map).Value;

            // init du persos
            if (!this.myInitialized)
                this.Initialize();
        }

        private void Initialize()
        {
            this.myStats = new GenericStats(this);
            for (int i = 1; i < 16; i++)
            {
                var Item = this.InventoryCache.GetItemInSlot((ItemSlotEnum)i);

                if (Item != null)
                {
                    this.myStats.Merge(Item.GetStats());
                    this.Life += Item.GetStats().GetTotal(EffectEnum.AddVitalite);
                }
            }
            if (this.ZaapString != null)
            {
                foreach (String str in ZaapString.Split(','))
                {
                    short zaap;
                    if (!short.TryParse(str, out zaap))
                    {
                        continue;
                    }
                    try
                    {
                        Zaaps.Add(zaap);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }
            }
            else
            {
                if (Settings.AppSettings.GetBoolElement("World.AllZaap"))
                {
                    foreach (short map in ZaapTable.Cache.Keys)
                    {
                        this.Zaaps.Add(map);
                    }
                }
            }

            if (this.MountID != -1)
            {
                Mount = MountTable.getMount(this.MountID);
                if (Mount != null)
                    Mount.Intialize();
            }

            if (this.SpellString == null || this.SpellString == "")
            {
                this.mySpells = SpellBook.GenerateForBreed((ClassEnum)this.Classe);
                for (int i = 1; i < Client.GetCharacter().Level; i++)
                {
                    this.mySpells.GenerateLevelUpSpell((ClassEnum)Client.GetCharacter().Classe, i);
                }
                Client.Send(new BasicNoOperationMessage());
                Client.Send(new SpellsListMessage(Client.Character));
            }
            else
                this.mySpells = SpellBook.FromDatabase(this.SpellString);

            if (LifePer <= 0)
                LifePer = 1;

            Life = (MaxLife * LifePer / 100);
            if (Life == 0) Life = 1;
            _exPdv = Life;

            this.myInitialized = true;
        }

        public Boolean onMount = false;

        public Boolean isOnMount()
        {
            return onMount;
        }

        private int _emoteActive = 0;

        public int emoteActive()
        {
            return _emoteActive;
        }

        public void setEmoteActive(int emoteActive)
        {
            _emoteActive = emoteActive;
        }

        public long lastMountRide, lastAlignementUpdate;

        public void toogleOnMount()
        {
            if ((Program.currentTimeMillis() - lastMountRide) < 2000)
            {
                return;
            }
            onMount = !onMount;

            InventoryItemModel obj = this.InventoryCache.GetItemInSlot(ItemSlotEnum.SLOT_FAMILIER);


            if (onMount && obj != null)
            {
                obj.Position = (int)ItemSlotEnum.SLOT_INVENTAIRE;
                Client.Send(new ObjectMoveSucessMessage(obj.ID, (short)obj.Position));
                this.myStats.UnMerge(obj.GetStats());
                this.Life -= obj.GetStats().GetTotal(EffectEnum.AddVitalite);
            }

            if (Mount.Energy <= 0)
            {
                Mount.Energy = 0;
                return;
            }

            if (Client.GetFight() != null && Client.GetFight().FightState == FightState.STATE_PLACE)
            {
                Client.GetFight().SendToFight(new CharacterFighterMount(Client.GetFighter(), this.ID));
            }
            else
            {
                this.RefreshOnMap();
            }

            if (onMount)
            {
                this.myStats.Merge(Mount.GetStats());
                this.Life += Mount.GetStats().GetTotal(EffectEnum.AddVitalite);
            }
            else
            {
                this.myStats.UnMerge(Mount.GetStats());
                this.Life -= Mount.GetStats().GetTotal(EffectEnum.AddVitalite);
            }

            Client.Send(new CharacterRideEventMessage("+", Mount));

            Client.Send(new CharacterRideMessage(onMount ? "+" : "-"));
            Client.Send(new AccountStatsMessage(this));
            Mount.Energy -= 10;
            lastMountRide = Program.currentTimeMillis();
        }

        public bool HasRestriction(RestrictionEnum Restriction)
        {
            return (this.Restriction & (int)Restriction) == (int)Restriction;
        }

        public Boolean isSitted()
        {
            return _sitted;
        }

        public void setSitted(bool b)
        {
            _sitted = b;
            int diff = Life - _exPdv;
            int time = b ? 1000 : 2000;

            _exPdv = Life;

            if (IsOnline() && Client.GetFight() == null)
            {
                //PacketsManager.GAME_SEND_ILF_PACKET(this, diff);

                //  PacketsManager.GAME_SEND_ILS_PACKET(this, time);
            }

            //_sitTimer.setDelay(time);

            if (((_emoteActive == 1) || (_emoteActive == 19)) && (!b))
            {
                _emoteActive = 0;
            }
        }

        private bool _sitted;
        private int _exPdv;

        public void SetRestriction(RestrictionEnum Restriction, bool CanOrIs)
        {
            if (CanOrIs)
            {
                if (!this.HasRestriction(Restriction))
                    this.Restriction |= (int)Restriction;
            }
            else
                if (this.HasRestriction(Restriction))
                    this.Restriction ^= (int)Restriction;
        }

        public void SpawnToMap()
        {
            if (this.myMap != null)
                this.myMap.SpawnActor(this);
        }

        public GameActorTypeEnum ActorType
        {
            get
            {
                return GameActorTypeEnum.TYPE_CHARACTER;
            }
        }

        public void SerializeAsGameMapInformations(StringBuilder SerializedString)
        {
            SerializedString.Append(CellId).Append(';');
            SerializedString.Append(Orientation).Append(';'); ;
            SerializedString.Append((int)ActorType).Append(';');
            SerializedString.Append(ID).Append(';');
            SerializedString.Append(Name).Append(';');
            SerializedString.Append(Classe).Append((Title > 0 ? ("," + TitleTable.getTitle(this) + ";") : (";")));
            SerializedString.Append(Look).Append('^');
            SerializedString.Append(SkinSize).Append(';');
            SerializedString.Append(Sexe).Append(';');
            SerializedString.Append(AlignmentPatternInfos).Append(';');
            SerializedString.Append(PatternColors(';')).Append(';');
            InventoryCache.SerializeAsDisplayEquipment(SerializedString);
            SerializedString.Append(';');
            SerializedString.Append((Level >= 200 ? '2' : (Level >= 100 ? '1' : '0'))).Append(';'); //Display Aura
            SerializedString.Append("").Append(';'); // DisplayEmotes
            SerializedString.Append("").Append(';'); // EmotesTimer
            if (this.HasGuild())
            {
                SerializedString.Append(this.GetGuild().Name).Append(';');
                SerializedString.Append(this.GetGuild().Emblem).Append(';');
            }
            else
            {
                SerializedString.Append("").Append(';'); // GuildInfos
                SerializedString.Append("").Append(';');
            }
            SerializedString.Append(StringHelper.EncodeBase36(Restriction)).Append(';');
            SerializedString.Append(onMount && Mount != null ? Mount.get_color(ParseMountColor()).ToString() : "").Append(';'); // MountLightInfos            
        }

        public String ParseMountColor()
        {
            return (Color1 == -1 ? "" : Color1.ToString("X")) + "," + (Color2 == -1 ? "" : Color2.ToString("X")) + "," + (Color3 == -1 ? "" : Color3.ToString("X"));
        }

        public int getGrade()
        {
            int mRank = 1;
            for (int i = 1; (i <= ExpFloorTable.MaxLevel); i++)
            {
                if ((ExpFloorTable.GetFloorByLevel(i).PvP == -1))
                {
                    break;
                }
                if ((Honor >= ExpFloorTable.GetFloorByLevel(i).PvP))
                {
                    mRank = i;
                }
            }
            return mRank;
        }

        public int Initiative
        {
            get
            {
                int fact = 4;
                int pvmax = MaxLife - BreedTable.Cache[Classe].StartLife;
                int pv = Life - BreedTable.Cache[Classe].StartLife;
                if (pv < 0)
                {
                    pv = 1;
                }
                if (Classe == (int)ClassEnum.CLASS_SACRIEUR)
                {
                    fact = 8;
                }
                double coef = pvmax / fact;

                coef += this.myStats.GetTotal(EffectEnum.AddInitiative);
                coef += this.myStats.GetTotal(EffectEnum.AddAgilite);
                coef += this.myStats.GetTotal(EffectEnum.AddChance);
                coef += this.myStats.GetTotal(EffectEnum.AddIntelligence);
                coef += this.myStats.GetTotal(EffectEnum.AddForce);

                int init = 1;
                if (pvmax != 0)
                {
                    init = (int)(coef * ((double)pv / (double)pvmax));
                }
                if (init < 0)
                {
                    init = 0;
                }
                return init;
                //return (int)Math.Floor((double)(this.MaxLife / 4 + this.myStats.GetTotal(EffectEnum.AddInitiative)) * (double)(this.Life / this.MaxLife));
                //return (int)((this.myStats.GetTotal(EffectEnum.AddInitiative) < 0 ? 0 : this.myStats.GetTotal(EffectEnum.AddInitiative)) * ((double)(MaxLife > 0 ? (double)Life / (double)MaxLife : 0)));
            }
        }

        public int Prospection
        {
            get
            {
                return (int)Math.Floor((double)(this.myStats.GetTotal(EffectEnum.AddChance) / 10)) + this.myStats.GetTotal(EffectEnum.AddProspection);
            }
        }

        public int MaxLife
        {
            get
            {
                return this.myStats.GetTotal(EffectEnum.AddVitalite) + this.myStats.GetTotal(EffectEnum.AddVie) + (Level * 5) + 50;
            }
        }

        public string PatternColors(char separator)
        {
            return (Color1 == -1 ? "-1" : Color1.ToString("x")) + separator + (Color2 == -1 ? "-1" : Color2.ToString("x")) + separator + (Color3 == -1 ? "-1" : Color3.ToString("x"));
        }

        public long lastTeleportTime;

        public void Teleport(Map NextMap, int NextCell)
        {


            if (this.myMap != null && NextMap.Id == this.myMap.Id)
            {
                //Sur la même map.
                if (NextCell == this.CellId)
                {
                    return;
                }
                else
                {
                    this.DestroyFromMap();
                    this.CellId = NextCell;
                    this.myMap.SpawnActor(this);
                    return;
                }
            }
            this.DestroyFromMap();
            this.myMap = NextMap;
            this.Map = NextMap.Id;
            this.CellId = NextCell;



            if (this.Client != null)
            {
                if (Client.IsGameAction(GameActionTypeEnum.MAP_MOVEMENT))
                {
                    Client.EndGameAction(GameActionTypeEnum.MAP_MOVEMENT);
                }
                if (Client.IsGameAction(GameActionTypeEnum.CELL_ACTION))
                {
                    Client.EndGameAction(GameActionTypeEnum.CELL_ACTION);
                }
                this.Client.SetState(WorldState.STATE_GAME_INFORMATION);

                using (CachedBuffer Buffer = new CachedBuffer(this.Client))
                {
                    Buffer.Append(new GameActionMessage((int)GameActionTypeEnum.CHANGE_MAP, this.ActorId));
                    Buffer.Append(new MapDataMessage(this.myMap));
                }
            }

            foreach (var Player in Follower.Values)
            {
                if (Player == null)
                    continue;
                if (Player.IsOnline())
                {
                    Player.Send(new CharacterFlagMessage(this));
                }
                else
                {
                    Follower.Remove(Player.ActorId);
                }
            }

        }

        public void RemoveHonor(int Value)
        {
            int oldGrade = this.getGrade();
            if (this.Alignement > 0)
            {
                this.Honor -= Value;
            }
            if (this.Client != null)
            {
                this.Client.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 81, Value.ToString()));

                if (this.getGrade() < oldGrade)
                    this.Client.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 83, this.getGrade().ToString()));
            }
        }

        public string AlignmentPatternInfos
        {
            get
            {
                return string.Concat(this.AlignmentType == AlignmentTypeEnum.ALIGNMENT_NEUTRAL ? "0" : Alignement.ToString(), ",",
                                     showWings ? Alignement.ToString() : "0", ",",
                                     showWings ? getGrade().ToString() : "0", ",", //getGrade
                                     Level + ID);
            }
        }

        public AlignmentTypeEnum AlignmentType
        {
            get
            {
                return (AlignmentTypeEnum)this.Alignement;
            }
        }

        public BreedModel GetBreed
        {
            get
            {
                return BreedTable.GetBreed(this.Classe);
            }
        }

        public GenericStats GetStats()
        {
            return this.myStats;
        }

        public String getStringVar(String str)
        {
            if (str.Equals("name"))
            {
                return this.Name;
            }
            if (str.Equals("bankCost"))
            {
                if (Client == null)
                    return "" + 0;//getBankCost()
                else
                    return Client.Account.Data.bankItems.Count.ToString();
            }
            return "";
        }

        public void LevelUP()
        {
            if (this.Level == ExpFloorTable.Cache.Count)
            {
                return;
            }
            this.Level++;
            this.CaractPoint += 5;
            this.SpellPoint++;
            this.Life = this.MaxLife;
            if (this.Level == 100)
            {
                this.AP++;
                this.myStats.AddBase(EffectEnum.AddPA, 1);
            }
            this.mySpells.GenerateLevelUpSpell((ClassEnum)this.Classe, this.Level);
            this.Experience = ExpFloorTable.GetFloorByLevel(this.Level).Character;

            if (this.HasGuild())
            {
                this.getCharacterGuild().Level = this.Level;
                CharactersGuildTable.Add(this.getCharacterGuild());
            }

        }

        public void setisForgetingSpell(Boolean isForgetingSpell)
        {
            _isForgetingSpell = isForgetingSpell;
        }

        public Boolean isForgetingSpell()
        {
            return _isForgetingSpell;
        }

        private Boolean _isForgetingSpell = false;

        public void AddExperience(long Value)
        {
            if (!this.myInitialized)
                this.Initialize();

            this.Experience += Value;

            if (this.Level != ExpFloorTable.Cache.Count)
            {
                ExpFloorModel Floor;

                var LastLevel = this.Level;

                do
                {
                    Floor = ExpFloorTable.GetFloorByLevel(this.Level + 1);

                    if (Floor.Character < this.Experience)
                    {
                        this.Level++;
                        this.CaractPoint += 5;
                        this.SpellPoint++;

                        if (this.Level == 100)
                        {
                            this.AP++;
                            this.myStats.AddBase(EffectEnum.AddPA, 1);
                        }

                        // Apprend des nouveaux sorts
                        this.mySpells.GenerateLevelUpSpell((ClassEnum)this.Classe, this.Level);
                    }
                }
                while (Floor.Character < this.Experience && this.Level != 200);

                if (this.Level != LastLevel)
                {
                    this.Life = this.MaxLife;
                    this.Send(new CharacterNewLevelMessage(this.Level));
                }

                if (this.Client != null)
                    this.Client.Send(new AccountStatsMessage(this));
            }
        }

        public Guild GetGuild()
        {
            if (CharacterGuildCache == null)
            {
                return null;
            }
            return CharacterGuildCache.GuildCache;
        }

        public CharacterGuild getCharacterGuild()
        {
            return this.CharacterGuildCache;
        }


        public bool HasGuild()
        {
            return CharacterGuildCache != null;
        }

        public void setCharacterGuild(CharacterGuild _guild)
        {
            CharacterGuildCache = _guild;
        }

        public String parseItemsToDB(char Splitter = '|')
        {
            //TODO Linq.Join
            String str = "";
            foreach (KeyValuePair<long, InventoryItemModel> entry in this.InventoryCache.getCache())
            {
                str = str + entry.Key + Splitter;
            }
            return str;
        }

        public void DestroyFromMap()
        {
            if (this.myMap != null)
                this.myMap.DestroyActor(this);
        }

        public void OnDisconnect()
        {
            try
            {
                CharacterTable.Update(this);
                if (this.myMap != null && this.Client != null && this.Client.GetState() == WorldState.STATE_IN_GAME)
                    this.DestroyFromMap();
                if (InventoryCache != null)
                {
                    foreach (var item in InventoryCache.ItemsCache)
                    {
                        if (item.Value.SpeakingItem != null)
                        {
                            SpeakingTable.Add(item.Value.SpeakingItem);
                            SpeakingTable.Cache.Remove(item.Value.SpeakingID);
                        }
                        InventoryItemTable.Items.Remove(item.Key);
                    }
                }
                CharacterTable.DelCharacter(this);
                this.Client = null;
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }



        public bool IsOnline()
        {
            return this.Client != null;
        }

        public SpellBook GetSpellBook()
        {
            return mySpells;
        }

        public Map GetMap()
        {
            return this.myMap;
        }

        public WorldClient GetClient()
        {
            return this.Client;
        }

        public void AddHonor(int Value)
        {
            if (this.Deshonor <= 0)
            {
                if (this.Alignement > 0)
                {
                    this.Honor += Value;

                    var CurrentLevel = this.getGrade();


                    if (this.GetClient() != null)
                    {
                        this.GetClient().Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 80, Value.ToString()));

                        if (this.getGrade() > CurrentLevel)
                            this.GetClient().Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 82, this.getGrade().ToString()));
                    }
                }
            }
        }

        public int GetPDVper()
        {
            int pdvper = 100;
            pdvper = 100 * Life / MaxLife;
            return pdvper;
        }

        public bool isZaaping = false;

        public void openZaapMenu()
        {
            if (Client.GetFight() == null)
            {
                if (Deshonor >= 3)
                {
                    this.GetClient().Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 83));
                    return;
                }
                isZaaping = true;
                if (!this.Zaaps.Contains(this.myMap.Id))
                {
                    this.Zaaps.Add(this.myMap.Id);
                    this.GetClient().Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 24));
                }
                this.GetClient().Send(new GameZaapMessage(this));
            }
        }

        public void openZaapiMenu()
        {
            if (Client.GetFight() == null)
            {
                if (Deshonor >= 3)
                {
                    this.GetClient().Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 83));
                    return;
                }

                StringBuilder ZaapiList = new StringBuilder();

                if (this.myMap.subArea.areaID == 7 && (this.Alignement == 1 || this.Alignement == 0 || this.Alignement == 3))
                {
                    int count = 0;
                    int price = 20;
                    if (this.Alignement == 1)
                        price = 10;
                    foreach (var zaapi in ZaapiTable.Cache[AlignmentTypeEnum.ALIGNMENT_BONTARIAN])
                    {
                        if (count == ZaapiTable.Cache[AlignmentTypeEnum.ALIGNMENT_BONTARIAN].Count)
                            ZaapiList.Append(zaapi).Append(";").Append(price);
                        else
                            ZaapiList.Append(zaapi).Append(";").Append(price).Append("|");
                        count++;
                    }
                    Client.Send(new PlayerZaapiMessage(this.Map, ZaapiList.ToString()));
                    isZaaping = true;
                }


                if (this.myMap.subArea.areaID == 11 && (this.Alignement == 2 || this.Alignement == 0 || this.Alignement == 3))//Démons, Neutre ou Sérianne
                {
                    int count = 0;
                    int price = 20;
                    if (this.Alignement == 2)
                        price = 10;
                    foreach (var zaapi in ZaapiTable.Cache[AlignmentTypeEnum.ALIGNMENT_BRAKMARIAN])
                    {
                        if (count == ZaapiTable.Cache[AlignmentTypeEnum.ALIGNMENT_BONTARIAN].Count)
                            ZaapiList.Append(zaapi).Append(";").Append(price);
                        else
                            ZaapiList.Append(zaapi).Append(";").Append(price).Append("|");
                        count++;
                    }
                    Client.Send(new PlayerZaapiMessage(this.Map, ZaapiList.ToString()));
                    isZaaping = true;
                }


            }
        }

        public String parseZaaps()
        {
            String str = "";
            Boolean first = true;
            foreach (short zaap in this.Zaaps)
            {

                if (!first)
                {
                    str += ",";
                }
                first = false;
                str += zaap;
            }
            return str;
        }

        public String PrismHelper()
        {
            Prisme prism = PrismeTable.getPrism(myMap.subArea.Prisme);
            if (prism == null)
            {
                return "-3";
            }
            else if (prism.inFight == 0)
            {
                return "0;" + prism.TimeTurn + ";" + (int)TurnTimeEnum.PRISME + ";7";
            }
            else
            {
                return prism.inFight + "";
            }
        }

        public void AlignementReset()
        {
            this.Deshonor = 0;
            this.Honor = 0;
            this.Alignement = 0;
            this.showWings = false;
            Client.UnRegisterChatChannel(ChatChannelEnum.CHANNEL_ALIGNMENT);
        }

        public void RefreshOnMap()
        {
            if (this.myMap != null && this.IsOnline())
            {
                this.myMap.SendToMap(new GameActorShowMessage(GameActorShowEnum.SHOW_REFRESH, this));
            }
        }


        internal void WarpToSavePos()
        {
            try
            {
                String[] infos = this.SavePos.Split(',');
                Teleport(MapTable.Get(short.Parse(infos[0])), int.Parse(infos[1]));
            }
            catch (Exception e) { Logger.Error(e); };
        }

        internal String ToPM()
        {
            StringBuilder sb = new StringBuilder(ActorId.ToString()).Append(';');
            sb.Append(Name).Append(';');
            sb.Append(Look).Append(';');
            sb.Append(Color1).Append(';');
            sb.Append(Color2).Append(';');
            sb.Append(Color3).Append(';');
            sb.Append(InventoryCache.SerializeAsDisplayEquipment()).Append(';');
            sb.Append(Life).Append(',').Append(MaxLife).Append(';');
            sb.Append(Initiative).Append(';');
            sb.Append(myStats.GetTotal(EffectEnum.AddProspection)).Append(';');
            sb.Append(1);
            return sb.ToString();
        }

        
    }
}
