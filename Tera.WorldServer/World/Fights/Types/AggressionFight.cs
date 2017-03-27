using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Fights.Fighters;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights
{
    public sealed class AggressionFight : Fight
    {
        public AggressionFight(Map Map, WorldClient Aggressor, WorldClient Victim)
            : base(FightType.TYPE_AGGRESSION, Map)
        {
            Logger.Debug("AGGRESSION_FIGHT Launched : Aggressor=" + Aggressor.GetCharacter().Name + " Victim=" + Victim.GetCharacter().Name + " MapId=" + Map.Id);

            // Init du combat
            var AttFighter = new CharacterFighter(this, Aggressor);
            var DefFighter = new CharacterFighter(this, Victim);

            Aggressor.AddGameAction(new GameFight(AttFighter, this));
            Victim.AddGameAction(new GameFight(DefFighter, this));

            base.InitFight(AttFighter, DefFighter);
        }

        public override int GetStartTimer()
        {
            return 30000;
        }

        public override int GetTurnTime()
        {
            return 30000;
        }

        private StringBuilder mySerializedString = null;
        public override void SerializeAs_FlagDisplayInformations(StringBuilder Packet)
        {
            if (this.mySerializedString == null)
            {
                this.mySerializedString = new StringBuilder();
                this.mySerializedString.Append(this.FightId).Append(';');//Infos Fight
                this.mySerializedString.Append((int)this.FightType).Append('|');

                this.mySerializedString.Append(this.Team1.LeaderId).Append(';');//Infos Team2
                this.mySerializedString.Append(this.Team1.Leader.MapCell).Append(';');
                this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER_ALIGNED).Append(';');
                this.mySerializedString.Append((this.Team1.Leader as CharacterFighter).Character.Alignement).Append('|');

                this.mySerializedString.Append(this.Team2.LeaderId).Append(';');//Infos Team2
                this.mySerializedString.Append(this.Team2.Leader.MapCell).Append(';');
                this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER_ALIGNED).Append(';');
                this.mySerializedString.Append((this.Team2.Leader as CharacterFighter).Character.Alignement);
            }

            Packet.Append(this.mySerializedString.ToString());
        }

        public override string SerializeAs_FightListInformations()
        {
            var Infos = new StringBuilder(this.FightId.ToString()).Append(';');//FightID
            Infos.Append(this.FightState == FightState.STATE_ACTIVE ? (this.FightTime) : (-1)).Append(';');//FightTime

            Infos.Append(TeamTypeEnum.TEAM_TYPE_PLAYER_ALIGNED).Append(","); //Infos Team1
            Infos.Append((this.Team1.Leader as CharacterFighter).Character.Alignement).Append(",");
            Infos.Append(this.Team1.GetAliveFighters().Count).Append(';');

            Infos.Append(TeamTypeEnum.TEAM_TYPE_PLAYER_ALIGNED).Append(","); //Infos Team2
            Infos.Append((this.Team2.Leader as CharacterFighter).Character.Alignement).Append(",");
            Infos.Append(this.Team2.GetAliveFighters().Count).Append(';');

            return Infos.ToString();
        }

        public override bool CanJoin(FightTeam Team, Player Character)
        {
            if (Character.Alignement != (Team.Leader as CharacterFighter).Character.Alignement)
                return false;

            return base.CanJoin(Team, Character);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void OverridableLeaveFight(Fighter Fighter)
        {
            switch (this.FightState)
            {
                // TODO deco du joueur
                case Fights.FightState.STATE_PLACE:
                    if (Fighter == Fighter.Team.Leader)
                    {
                        foreach (var TeamFighter in Fighter.Team.GetFighters())
                        {
                            TeamFighter.Life = 0;
                        }

                        Fighter.Left = true;

                        this.OverridableEndFight(this.GetEnnemyTeam(Fighter.Team), Fighter.Team);
                    }
                    break;

                case FightState.STATE_ACTIVE:
                    if (Fighter.TryDie(Fighter.ActorId, true) != -3)
                    {
                        Fighter.LeaveFight();

                        Fighter.Send(new GameLeaveMessage());
                    }
                    break;
            }
        }

        public override void OverridableEndFight(FightTeam Winners, FightTeam Loosers)
        {
            var Winner = (Winners.Leader as CharacterFighter).Character;
            var Looser = (Loosers.Leader as CharacterFighter).Character;

            // Pour les logs
            Logger.Debug("AGGRESSION_FIGHT Ended : Winner=" + Winner.Name + "(" + Winner.Level + ") Looser=" + Looser.Name + "(" + Looser.Level + ") MapId=" + Map.Id);

            var WinnersFighter = Winners.GetFighters().OfType<CharacterFighter>().ToList();
            var LoosersFighter = Loosers.GetFighters().OfType<CharacterFighter>().ToList();

            var WinnersTotalGrade = WinnersFighter.Sum(x => x.Character.getGrade());
            var WinnersTotalLevel = WinnersFighter.Sum(x => x.Level);

            var LoosersTotalGrade = LoosersFighter.Sum(x => x.Character.getGrade());
            var LoosersTotalLevel = LoosersFighter.Sum(x => x.Level);

            var PossibleItemLoot = new List<ItemLoot>();
            long PossibleKamasLoot = 0, PossibleXpLoot = 0;

            foreach (var Fighter in LoosersFighter)
            {
                int WinHonor = 0;
                if (Fighter.Character.AlignmentType != AlignmentTypeEnum.ALIGNMENT_NEUTRAL)
                {
                    WinHonor = Algo.CalculateAggressionHonor(Fighter, false, WinnersFighter.Count, WinnersTotalGrade, WinnersTotalLevel, LoosersTotalGrade, LoosersTotalLevel);
                    if (WinHonor > Fighter.Character.Honor)
                        WinHonor = Fighter.Character.Honor;
                    Fighter.Character.RemoveHonor(WinHonor);
                }

               
                if (Fighter.Character.GetClient() != null)
                    Fighter.Character.GetClient().Send(new AccountStatsMessage(Fighter.Character));


                this.myResult.AddResult(Fighter, false, WinHonor: -WinHonor);
            }

            var ItemLootPerFighter = PossibleItemLoot.Count / WinnersFighter.Count;
            var KamasLootPerFighter = PossibleKamasLoot / WinnersFighter.Count;
            var XpLootPerFighter = PossibleXpLoot / WinnersFighter.Count;
            var Modulo = PossibleItemLoot.Count % WinnersFighter.Count;

            foreach (var Fighter in WinnersFighter)
            {
                var Drops = new Dictionary<int, int>();
                var WinKamas = KamasLootPerFighter;
                int WinHonor = 0;
                if (Fighter.Character.AlignmentType != AlignmentTypeEnum.ALIGNMENT_NEUTRAL)
                    WinHonor = Algo.CalculateAggressionHonor(Fighter, true, WinnersFighter.Count, WinnersTotalGrade, WinnersTotalLevel, LoosersTotalGrade, LoosersTotalLevel);
                var WinXp = XpLootPerFighter;

                if (Fighter.Character.AlignmentType != AlignmentTypeEnum.ALIGNMENT_NEUTRAL)
                    Fighter.Character.AddHonor(WinHonor);
                Fighter.Character.InventoryCache.AddKamas(WinKamas);
                Fighter.Character.AddExperience(WinXp);

                this.myResult.AddResult(Fighter, true, WinKamas: WinKamas, WinExp: WinXp, WinHonor: WinHonor, WinItems: Drops);
            }

            base.EndFight();
        }
    }
}
