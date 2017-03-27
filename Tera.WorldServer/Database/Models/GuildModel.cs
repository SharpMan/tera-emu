using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Helper;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Character;
using Tera.WorldServer.World.Chats;
using Tera.WorldServer.World.Events;

namespace Tera.WorldServer.Database.Models
{
    public class Guild : IWorldEventObserver
    {
        private delegate void GenericWorldClientPacket(PacketBase Packet);
        private event GenericWorldClientPacket Event_SendToGuild;

        private ChatChannel myChatChannel = new ChatChannel(ChatChannelEnum.CHANNEL_GUILD);
        public ChatChannel ChatChannel
        {
            get
            {
                return this.myChatChannel;
            }
        }

        public int ID { get; set; }
        public String Name { get; set; }
        public String Emblem { get; set; }
        public int Level { get; set; }
        public long Experience { get; set; }
        public int Capital { get; set; }
        public String Spells { get; set; }
        public String Stats { get; set; }
        public int PerceptorMaxCount { get; set; }

        private SpellBook mySpells = null;
        private GenericStats myStats = new GenericStats();
        private GenericStats myFightStats = new GenericStats();
        private bool myInitialized = false;
        public List<TaxCollector> TaxCollectorsCache = new List<TaxCollector>();


        public List<CharacterGuild> CharactersGuildCache = new List<CharacterGuild>();

        public GenericStats FightStats
        {
            get
            {
                return this.myFightStats;
            }
        }

        public GenericStats BaseStats
        {
            get
            {
                return this.myStats;
            }
        }

        public CachedString PatternSpells
        {
            get;
            private set;
        }

        public SpellBook GetSpellBook()
        {
            return this.mySpells;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize()
        {
            if (this.myInitialized)
                return;

            if (this.Spells.Length == 1)
                this.mySpells = SpellBook.GenerateForGuild();
            else
                this.mySpells = SpellBook.FromDatabase(this.Spells);



            if (this.Stats != string.Empty)
            {
                this.ParseStats();
            }

            this.myFightStats.AddBase(EffectEnum.AddForce, this.Level);
            this.myFightStats.AddBase(EffectEnum.AddSagesse, this.myStats.GetTotal(EffectEnum.AddSagesse));
            this.myFightStats.AddBase(EffectEnum.AddIntelligence, this.Level);
            this.myFightStats.AddBase(EffectEnum.AddChance, this.Level);
            this.myFightStats.AddBase(EffectEnum.AddAgilite, this.Level);
            this.myFightStats.AddBase(EffectEnum.AddReduceDamagePourcentNeutre, (int)Math.Floor((double)this.Level / 2));
            this.myFightStats.AddBase(EffectEnum.AddReduceDamagePourcentFeu, (int)Math.Floor((double)this.Level / 2));
            this.myFightStats.AddBase(EffectEnum.AddReduceDamagePourcentEau, (int)Math.Floor((double)this.Level / 2));
            this.myFightStats.AddBase(EffectEnum.AddReduceDamagePourcentAir, (int)Math.Floor((double)this.Level / 2));
            this.myFightStats.AddBase(EffectEnum.AddReduceDamagePourcentTerre, (int)Math.Floor((double)this.Level / 2));
            this.myFightStats.AddBase(EffectEnum.AddEsquivePA, (int)Math.Floor((double)this.Level / 2));
            this.myFightStats.AddBase(EffectEnum.AddEsquivePM, (int)Math.Floor((double)this.Level / 2));

            this.PatternSpells = new CachedString(new Func<String>(() =>
            {
                return string.Join("|", this.mySpells.GetMySpells().Values.Select(x => x.Id + ";" + x.Level));
            }));

            /*
             * SpellBook.SpellInfo curSpell in mySpells.GetSpellInfos()*/

            this.myInitialized = true;
        }

        private void ParseStats()
        {
            try
            {
                foreach (var Stats in this.Stats.Split('|'))
                {
                    if (Stats != string.Empty)
                    {
                        var EffectId = int.Parse(Stats.Split(';')[0]);
                        var EffectValue = int.Parse(Stats.Split(';')[1]);

                        this.myStats.AddBase((EffectEnum)EffectId, EffectValue);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Guild::ParseStats unknow error during stats parsing. " + ex.ToString());
            }
        }

        public void SaveChanges()
        {
            this.Initialize();
            this.Stats = string.Join("|", this.myStats.GetEffects().Values.Select(x => (int)x.EffectType + ";" + x.Base));
            this.Spells = this.mySpells.ToDatabase();
        }

        public String compileSpell()
        {
            this.Initialize();
            String toReturn = "";
            Boolean isFirst = true;

            foreach (SpellBook.SpellInfo curSpell in mySpells.GetSpellInfos())
            {
                if (!isFirst)
                {
                    toReturn = toReturn + "|";
                }
                toReturn = toReturn + curSpell.Id + ";" + curSpell.Level;

                isFirst = false;
            }

            return toReturn;
        }

        public bool BoostSpell(CharacterGuild Player, int SpellId)
        {
            if (Player.Can(GuildRightEnum.RIGHT_BOOST))
            {
                if (this.mySpells.HasSpell(SpellId))
                {
                    if (this.Capital > 4)
                    {
                        this.mySpells.LevelUpSepll(SpellId);

                        this.Capital -= 5;

                        this.PatternSpells.NeedToBeRefresh();

                        return true;
                    }
                }
            }

            return false;
        }



        public void Register(WorldClient Client)
        {
            this.Event_SendToGuild += Client.Send;

            Client.RegisterChatChannel(this.myChatChannel);
        }

        public void UnRegister(WorldClient Client)
        {
            this.Event_SendToGuild -= Client.Send;

            Client.UnRegisterChatChannel(ChatChannelEnum.CHANNEL_GUILD);
        }

        public void Send(PacketBase Packet)
        {
            if (this.Event_SendToGuild != null)
                this.Event_SendToGuild(Packet);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddPlayer(CharacterGuild Player, GuildGradeEnum Grade)
        {
            this.CharactersGuildCache.Add(Player);

            Player.OnResetRights();

            Player.SetGuild(this, Grade);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemovePlayer(CharacterGuild Player)
        {
            this.CharactersGuildCache.Remove(Player);

            if (Player.GradeType == GuildGradeEnum.GRADE_BOSS)
            {
                if (this.CharactersGuildCache.Count == 0)
                {
                    GuildTable.TryDeleteGuild(this);

                    // TODO : Delete TaxCollector.
                }
                else
                {
                    CharacterGuild BestBoss = null;

                    // On recherche le second membre avec le meilleur grade ou alors le meilleur xp s'il y a des ex echo
                    foreach (var Member in this.CharactersGuildCache)
                        if (BestBoss == null ||
                            Member.Grade < BestBoss.Grade ||
                            BestBoss.GradeType == GuildGradeEnum.GRADE_ESSAI && Member.GradeType != GuildGradeEnum.GRADE_ESSAI ||
                            (Member.Grade == BestBoss.Grade && Member.Experience > BestBoss.Experience))
                            BestBoss = Member;

                    if (BestBoss != null)
                    {
                        // On lui change le grade
                        BestBoss.SetGrade(GuildGradeEnum.GRADE_BOSS);

                        // On actualise les droits.
                        BestBoss.SendGuildSettingsInfos();
                    }
                }
            }

            CharactersGuildTable.Delete(Player.ID);


        }

        public CharacterGuild GetMember(string Name)
        {
            return this.CharactersGuildCache.Find(x => x.Name == Name);
        }

        public CharacterGuild GetMember(long Id)
        {
            return this.CharactersGuildCache.Find(x => x.ID == Id);
        }

        public void addXp(long xp)
        {
            this.Experience += xp;

            while (Experience >= ExpFloorTable.getGuildXpMax(this.Level) && this.Level < Settings.AppSettings.GetIntElement("World.GuildMaxLevel"))
            {
                levelUp();
            }
        }

        public void levelUp()
        {
            this.Level++;
            this.Capital += 5;
        }

  

        public bool BoostStats(CharacterGuild Player, char Stats)
        {
            if (Player.Can(GuildRightEnum.RIGHT_BOOST))
            {
                switch (Stats)
                {
                    case 'p': // Prospection.
                        if (this.Capital > 0)
                        {
                            if (this.myStats.GetTotal(EffectEnum.AddProspection) < 500)
                            {
                                this.myStats.AddBase(EffectEnum.AddProspection, 1);
                                this.Capital--;
                            }
                        }
                        break;

                    case 'x': // Sagesse.
                        if (this.Capital > 0)
                        {
                            if (this.myStats.GetTotal(EffectEnum.AddSagesse) < 400)
                            {
                                this.myStats.AddBase(EffectEnum.AddSagesse, 1);
                                this.Capital--;
                            }
                        }
                        break;

                    case 'o': // Pods
                        if (this.Capital > 0)
                        {
                            if (this.myStats.GetTotal(EffectEnum.AddPods) < 5000)
                            {
                                this.myStats.AddBase(EffectEnum.AddPods, 20);
                                this.Capital--;
                            }
                        }
                        break;

                    case 'k': // PerceptorCount.
                        if (this.Capital > 9)
                        {
                            if (this.PerceptorMaxCount < 50)
                            {
                                this.PerceptorMaxCount++;
                                this.Capital -= 10;
                            }
                        }
                        break;

                    default: // UNKNOW STATS ID
                        return false;
                }

                return true;
            }

            return false;
        }
    }

}
