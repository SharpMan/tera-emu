using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Challenges;

namespace Tera.WorldServer.World.Fights
{
    public class MonsterFight : Fight
    {
        public MonsterGroup MonsterGroup
        {
            get;
            set;
        }

        public MonsterFight(Map Map, WorldClient Player, MonsterGroup Monsters)
            : base(FightType.TYPE_PVM, Map)
        {
            try
            {
                Logger.Debug("PVM_FIGHT Launched : Player=" + Player.GetCharacter().Name + " MapId=" + Map.Id);

                var AttFighter = new CharacterFighter(this, Player);
                var DefFighter = new MonsterFighter(this, Monsters.Monsters[-1], this.NextActorId, Monsters);

                this.MonsterGroup = Monsters;

                Player.AddGameAction(new GameFight(AttFighter, this));

                base.InitFight(AttFighter, DefFighter);

                bool first = true;
                foreach (var Monster in Monsters.Monsters.Values)
                {
                    if (!first)
                        base.JoinFightTeam(new MonsterFighter(this, Monster, this.NextActorId, Monsters), this.myTeam2);
                    first = false;
                }

                bool inDungeon = false;
                if (this.Map.hasEndFightAction((int)FightType))
                    inDungeon = true;

                int challengeNumber = (inDungeon ? 2 : 1);

                int challInitz = 0;

                while (challInitz != challengeNumber)
                {
                    var Challenge = ChallengeHandler.GetRandomChallenge(this);
                    while (!Challenge.CanSet() || Challanges.Any(x => x.Id == Challenge.Id))
                    {
                        Challenge = ChallengeHandler.GetRandomChallenge(this);
                    }
                    this.Challanges.Add(Challenge);
                    this.RegisterFightListener(Challenge);
                    challInitz++;
                }

            }
            catch (Exception ex)
            {
                Logger.Error("MonsterFight::MonsterFight() "+ ex.ToString());
            }
        }

        public override int GetStartTimer()
        {
            return 30000;
        }

        public override int GetTurnTime()
        {
            return 30000;
        }

        StringBuilder mySerializedString;
        public override void SerializeAs_FlagDisplayInformations(StringBuilder Packet)
        {
            if (this.mySerializedString == null)
            {
                this.mySerializedString = new StringBuilder();
                this.mySerializedString.Append(this.FightId).Append(';');//Infos Fight
                this.mySerializedString.Append((int)this.FightType).Append('|');

                this.mySerializedString.Append(this.myTeam1.LeaderId).Append(';');//Infos Team1
                this.mySerializedString.Append(this.Map.GetNearestFreeCell(this.myTeam1.Leader.MapCell)).Append(';');
               
                if (mustShowAlign())
                {
                    this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER_ALIGNED).Append(';');
                    this.mySerializedString.Append((this.Team1.Leader as CharacterFighter).Character.Alignement);
                }
                else
                {
                    this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER).Append(';');
                    this.mySerializedString.Append((int)AlignmentTypeEnum.ALIGNMENT_WITHOUT);
                }

                this.mySerializedString.Append('|');//Separation

                this.mySerializedString.Append(this.myTeam2.LeaderId).Append(';');//Infos Team2
                this.mySerializedString.Append(this.myTeam2.Leader.MapCell).Append(';');
                this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_MONSTER).Append(';');
                if (mustShowAlign())
                    this.mySerializedString.Append(this.MonsterGroup.Alignement);
                else
                    this.mySerializedString.Append((int)AlignmentTypeEnum.ALIGNMENT_WITHOUT);
            }
            Packet.Append(this.mySerializedString.ToString());
        }

        private Boolean mustShowAlign()
        {
            return !withoutAlign(MonsterGroup.AlignmentType) 
                && !withoutAlign((AlignmentTypeEnum)(this.Team1.Leader as CharacterFighter).Character.AlignmentType) 
                && ((AlignmentTypeEnum)(this.Team1.Leader as CharacterFighter).Character.AlignmentType != MonsterGroup.AlignmentType);
        }

        private bool withoutAlign(AlignmentTypeEnum type)
        {
            return type == AlignmentTypeEnum.ALIGNMENT_WITHOUT || type == AlignmentTypeEnum.ALIGNMENT_NEUTRAL;
        }

        public override string SerializeAs_FightListInformations()
        {
            var Infos = new StringBuilder(this.FightId.ToString()).Append(';');//FightID
            Infos.Append(this.FightState == FightState.STATE_ACTIVE ? (this.FightTime) : (-1)).Append(';');//FightTime

            
            if (mustShowAlign())
            {
                Infos.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER_ALIGNED).Append(","); //Infos Team1
                Infos.Append((this.Team1.Leader as CharacterFighter).Character.Alignement).Append(",");
            }
            else
            {
                Infos.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER).Append(","); //Infos Team1
                Infos.Append((int)AlignmentTypeEnum.ALIGNMENT_WITHOUT).Append(",");
            }

            Infos.Append(this.myTeam1.GetAliveFighters().Count).Append(';');

            Infos.Append((int)TeamTypeEnum.TEAM_TYPE_MONSTER).Append(","); //Infos Team2
            if (mustShowAlign())
                Infos.Append(this.MonsterGroup.Alignement).Append(",");
            else
                Infos.Append((int)AlignmentTypeEnum.ALIGNMENT_WITHOUT).Append(",");
            Infos.Append(this.myTeam2.GetAliveFighters().Count).Append(';');

            return Infos.ToString();
        }

        public override bool CanJoin(FightTeam Team, Player Character)
        {
            if (Team.Leader.ActorType == GameActorTypeEnum.TYPE_MONSTER)
                return false;

            return base.CanJoin(Team, Character);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void OverridableLeaveFight(Fighter Fighter)
        {
            // Un persos quitte le combat
            switch (this.FightState)
            {
                case Fights.FightState.STATE_PLACE: // TODO : Uniquement si kické
                    this.Map.SendToMap(new GameFightTeamFlagFightersMessage(new List<Fighter> { Fighter }, Fighter.Team.LeaderId, false));

                    this.SendToFight(new GameActorDestroyMessage(Fighter.ActorId));

                    Fighter.LeaveFight();

                    Fighter.Send(new GameLeaveMessage());
                  
                    if (Team1.GetAliveFighters().Count == 0 || Team2.GetAliveFighters().Count == 0)
                    {
                        FightTeam ft = null;
                        if (Team1.GetAliveFighters().Count == 0)
                            ft = Team1;
                        else
                            ft = Team2;

                        foreach (var TeamFighter in ft.GetFighters())
                        {
                            TeamFighter.Life = 0;
                        }

                        Fighter.Left = true;

                        this.OverridableEndFight(this.GetEnnemyTeam(ft), ft);
                    }
                    break;

                case FightState.STATE_ACTIVE:
                    if (Fighter.TryDie(Fighter.ActorId, true) != -3)
                    {
                        Fighter.LeaveFight();

                        Fighter.Send(new GameLeaveMessage());
                    };
                    break;
            }
        }

        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void OverridableEndFight(FightTeam Winners, FightTeam Loosers)
        {
            /*foreach (var Chal in Challanges)
            {
                if (Chal.State && !Chal.Signaled)
                    Chal.Ok();
            }*/

            var WinChallenges = Challanges.Where(c => c.State).ToList();
            var ChallengeXpBonus = 1;
            var ChallengeDropBonus = 1;
            if (WinChallenges.Count > 0)
            {
                ChallengeXpBonus = (int)Math.Round(Convert.ToDouble((100 + WinChallenges.Sum(c => c.BasicXpBonus + c.TeamXpBonus)) / 100));
                ChallengeDropBonus = (int)Math.Round(Convert.ToDouble((100 + WinChallenges.Sum(c => c.BasicDropBonus + c.TeamDropBonus)) / 100));
            }


            var WinnersCount = Winners.GetFighters().Count;
            var LoosersCount = Loosers.GetFighters().Count;
            var WinnersLevel = Winners.GetFighters().Sum(x => x.Level);
            var LoosersLevel = Loosers.GetFighters().Sum(x => x.Level);
            var WinnersProspection = Winners.GetFighters().Sum(x => x.Stats.GetTotal(EffectEnum.AddProspection));
            var MonsterWon = Winners.Leader.ActorType == GameActorTypeEnum.TYPE_MONSTER;

            var PossibleItemLoot = new List<ItemLoot>();
            long PossibleKamasLoot = 0;
            var PossibleDrops = new Dictionary<Drop, int>();
            var MinKamas = 0;
            var MaxKamas = 0;
            long BaseXp = 0;

            // Les monstres perdent ?
            if (!MonsterWon)
            {
                PossibleItemLoot.AddRange(this.MonsterGroup.ItemLoot);
                PossibleKamasLoot = this.MonsterGroup.KamasLoot;
            }

            foreach (var Fighter in Loosers.GetFighters())
            {
                switch (Fighter.ActorType)
                {
                    case GameActorTypeEnum.TYPE_MONSTER:
                        var Monster = Fighter as MonsterFighter;

                        // Ajout des drops et kamas
                        MinKamas += Monster.Grade.Monster.MinKamas;
                        MaxKamas += Monster.Grade.Monster.MaxKamas;
                        BaseXp += Monster.Grade.BaseXP;

                        // On augmente le Taux si possible
                        foreach (var Drop in Monster.Grade.Monster.DropsCache)
                        {
                            if (Drop.ItemTemplateCache != null)
                            {
                                if (!PossibleDrops.ContainsKey(Drop))
                                {
                                    if (Drop.Seuil <= WinnersProspection)
                                    {
                                        var Taux = (int)((WinnersProspection * Drop.Taux * Settings.AppSettings.GetIntElement("Rate.Drop") * ChallengeDropBonus / 100));

                                        PossibleDrops.Add(Drop, Taux);
                                    }
                                    else
                                    {
                                        PossibleDrops.Add(Drop, (int)Drop.Taux);
                                    }
                                }
                            }
                        }
                        break;
                }

                this.myResult.AddResult(Fighter, false);
            }

            var WinnersOrderedByProspect = Winners.GetFighters().OrderByDescending(x => x.Stats.GetTotal(EffectEnum.AddProspection));
            var AlreadyDropItems = new Dictionary<int, int>();
            var ItemLootPerFighter = PossibleItemLoot.Count / WinnersCount;
            var KamasLootPerFighter = PossibleKamasLoot / WinnersCount;
            var Modulo = PossibleItemLoot.Count % (this.Map.GetActors().Where(x => x.ActorType == GameActorTypeEnum.TYPE_TAX_COLLECTOR).Count() > 0  ? WinnersCount +1 : WinnersCount);

            foreach (var Drop in PossibleDrops)
                if (!AlreadyDropItems.ContainsKey(Drop.Key.ItemTemplateCache.ID))
                    AlreadyDropItems.Add(Drop.Key.ItemTemplateCache.ID, 0);

            foreach (var Fighter in WinnersOrderedByProspect)
            {
                long WinXp = 0;
                long WinKamas = 0;
                var Drops = new Dictionary<int, int>();

                if (!Fighter.Left)
                {
                    switch (Fighter.ActorType)
                    {
                        case GameActorTypeEnum.TYPE_CHARACTER:
                            var Character = Fighter as CharacterFighter;

                            WinXp = Algo.CalculatePVMXp(Character, Winners.GetFighters(), Loosers.GetFighters(), LoosersLevel, WinnersLevel, BaseXp);
                            WinXp *= ChallengeXpBonus;
                            WinKamas = Algo.CalculatePVMKamas(MaxKamas, MinKamas);
                            WinKamas += KamasLootPerFighter;
                            WinKamas *= ChallengeDropBonus;

                            try
                            {
                                Character.Character.BeginCachedBuffer();

                                Character.Character.InventoryCache.AddKamas(WinKamas);
                                Character.Character.AddExperience(WinXp);

                                foreach (var Drop in PossibleDrops)
                                {
                                    var Taux = Drop.Value * 100;
                                    var Jet = Algo.Random(0, 100 * 100);

                                    if (Jet < Taux)
                                    {
                                        if (AlreadyDropItems[Drop.Key.TemplateId] < Drop.Key.Max)
                                        {
                                            if (Drops.ContainsKey(Drop.Key.TemplateId))
                                            {
                                                Drops[Drop.Key.TemplateId]++;
                                            }
                                            else
                                                Drops.Add(Drop.Key.TemplateId, 1);
                                        }
                                    }
                                }

                                foreach (var Drop in Drops)
                                {
                                    InventoryItemTable.TryCreateItem(Drop.Key, Character.Character, Drop.Value);
                                }

                                for (int i = 0; i < ItemLootPerFighter; i++)
                                {
                                    if (Drops.ContainsKey(PossibleItemLoot[i].TemplateId))
                                    {
                                        Drops[PossibleItemLoot[i].TemplateId]++;
                                    }
                                    else
                                        Drops.Add(PossibleItemLoot[i].TemplateId, PossibleItemLoot[i].Quantity);

                                    InventoryItemTable.TryCreateItem(PossibleItemLoot[i].TemplateId, Character.Character, PossibleItemLoot[i].Quantity, Stats: PossibleItemLoot[i].ItemStats.ToItemStats());
                                }

                                if (ItemLootPerFighter > 0)
                                    PossibleItemLoot.RemoveRange(0, ItemLootPerFighter);

                                if (Modulo > 0)
                                {
                                    if (Drops.ContainsKey(PossibleItemLoot[0].TemplateId))
                                    {
                                        Drops[PossibleItemLoot[0].TemplateId]++;
                                    }
                                    else
                                        Drops.Add(PossibleItemLoot[0].TemplateId, PossibleItemLoot[0].Quantity);

                                    InventoryItemTable.TryCreateItem(PossibleItemLoot[0].TemplateId, Character.Character, PossibleItemLoot[0].Quantity, Stats: PossibleItemLoot[0].ItemStats.ToItemStats());

                                    PossibleItemLoot.RemoveAt(0);
                                    Modulo--;
                                }

                                // Fin de la mise en cache
                                Character.Character.EndCachedBuffer();
                            }
                            catch (Exception exc)
                            {
                                Logger.Error("MonsterFight::EndFight() "+ exc.ToString());
                            }

                            // Ajout du resultat
                            this.myResult.AddResult(Fighter, true, WinKamas, WinXp, WinItems: Drops);

                            break;

                        case GameActorTypeEnum.TYPE_MONSTER:
                            var Monster = Fighter as MonsterFighter;

                            WinKamas += KamasLootPerFighter;

                            for (int i = 0; i < ItemLootPerFighter; i++)
                            {
                                if (Drops.ContainsKey(PossibleItemLoot[i].TemplateId))
                                {
                                    Drops[PossibleItemLoot[i].TemplateId]++;
                                }
                                else
                                    Drops.Add(PossibleItemLoot[i].TemplateId, PossibleItemLoot[i].Quantity);
                            }

                            if (ItemLootPerFighter > 0)
                                PossibleItemLoot.RemoveRange(0, ItemLootPerFighter);

                            if (Modulo > 0)
                            {
                                if (Drops.ContainsKey(PossibleItemLoot[0].TemplateId))
                                {
                                    Drops[PossibleItemLoot[0].TemplateId]++;
                                }
                                else
                                    Drops.Add(PossibleItemLoot[0].TemplateId, PossibleItemLoot[0].Quantity);

                                PossibleItemLoot.RemoveAt(0);
                                Modulo--;
                            }

                            this.myResult.AddResult(Fighter, true, WinKamas, WinXp, WinItems: Drops);

                            break;
                    }
                }
            }


            if (!MonsterWon && this.Map.GetActors().Where(x => x.ActorType == GameActorTypeEnum.TYPE_TAX_COLLECTOR).Count() > 0)
            {
                var TCollector = this.Map.GetActors().Where(x => x.ActorType == GameActorTypeEnum.TYPE_TAX_COLLECTOR).First() as TaxCollector;
                long winxp = (int)Math.Floor((double)Algo.CalculateXpWinPerco(TCollector, Winners.GetFighters(), Loosers.GetFighters(), BaseXp) / 100);
                long winkamas = (int)Math.Floor((double)Algo.CalculatePVMKamas(MinKamas, MaxKamas) / 100);
                TCollector.XP += winxp;
                TCollector.Kamas += winkamas;
                var Drops = new Dictionary<int, int>();

                foreach (var Drop in PossibleDrops)
                {
                    var Taux = Drop.Value * 100;
                    var Jet = Algo.Random(0, 100 * 100);

                    if (Jet < Taux)
                    {
                        if (AlreadyDropItems[Drop.Key.TemplateId] < Drop.Key.Max)
                        {
                            if (Drops.ContainsKey(Drop.Key.TemplateId))
                            {
                                Drops[Drop.Key.TemplateId]++;
                            }
                            else
                                Drops.Add(Drop.Key.TemplateId, 1);
                        }
                    }
                }

                foreach (var Drop in Drops)
                {
                    InventoryItemTable.TryCreateItem(Drop.Key, TCollector, Drop.Value);
                }

                for (int i = 0; i < ItemLootPerFighter; i++)
                {
                    if (Drops.ContainsKey(PossibleItemLoot[i].TemplateId))
                    {
                        Drops[PossibleItemLoot[i].TemplateId]++;
                    }
                    else
                        Drops.Add(PossibleItemLoot[i].TemplateId, PossibleItemLoot[i].Quantity);

                    InventoryItemTable.TryCreateItem(PossibleItemLoot[i].TemplateId, TCollector, PossibleItemLoot[i].Quantity, Stats: PossibleItemLoot[i].ItemStats.ToItemStats());
                }

                if (ItemLootPerFighter > 0)
                    PossibleItemLoot.RemoveRange(0, ItemLootPerFighter);

                if (Modulo > 0)
                {
                    if (Drops.ContainsKey(PossibleItemLoot[0].TemplateId))
                    {
                        Drops[PossibleItemLoot[0].TemplateId]++;
                    }
                    else
                        Drops.Add(PossibleItemLoot[0].TemplateId, PossibleItemLoot[0].Quantity);

                    InventoryItemTable.TryCreateItem(PossibleItemLoot[0].TemplateId, TCollector, PossibleItemLoot[0].Quantity, Stats: PossibleItemLoot[0].ItemStats.ToItemStats());

                    PossibleItemLoot.RemoveAt(0);
                    Modulo--;
                }

                this.myResult.TCollectorResult = new Tera.WorldServer.World.Fights.GameFightEndResult.TaxCollectorResult()
                {
                    TaxCollector = TCollector,
                    WinExp = winxp,
                    WinKamas = winkamas,
                    WinItems = Drops
                };

                TaxCollectorTable.Update(TCollector);
            }

            if (!MonsterWon)
                this.Map.SpawnMonsterGroup(1);
            else if(!MonsterGroup.IsFix)
                this.Map.SpawnActor(this.MonsterGroup); 

            base.EndFight();
        }
    }
}
