using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Fights.Fighters;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights.Types
{
    public sealed class PrismFight : Fight
    {
        public String DefenderList;

        public Prisme Prisme
        {
            get;
            set;
        }

        public CharacterFighter AttFighter
        {
            get;
            set;
        }

        public Timer _timer;

        public void StartTimer()
        {
            _timer = new Timer(1000);
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Enabled = true; // Enable it
        }

        public void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Prisme.TimeTurn <= 0)
            {
                _timer.Stop();
                _timer = null;
                return;
            }
            Prisme.TimeTurn -= 1000;
        }



        public PrismFight(Map Map, WorldClient Player, Prisme Monsters)
            : base(FightType.TYPE_PVMA, Map)
        {
            try
            {
                Logger.Debug("PVMA_FIGHT Launched : Player=" + Player.GetCharacter().Name + " MapId=" + Map.Id);
                this.Prisme = Monsters;
                StartTimer();
                AttFighter = new CharacterFighter(this, Player);
                var DefFighter = new PrismFighter(this, Prisme, this.NextActorId);

                Prisme.Map.DestroyActor(Prisme);

                Network.WorldServer.GetChatController().getAlignementChannel((AlignmentTypeEnum)Prisme.Alignement).Send(new ConquestPrismeAttackMessage(Prisme.Mapid + "|" + Prisme.Map.X + "|" + Prisme.Map.Y));

                Player.AddGameAction(new GameFight(AttFighter, this));

                base.InitFight(AttFighter, DefFighter);

            }
            catch (Exception ex)
            {
                Logger.Error("PrismFight::PrismFight() " + ex.ToString());
            }
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

        public override bool CanJoin(FightTeam Team, Player Character)
        {
            if (Team.Leader is PrismFighter)
            {
                if (Character.Alignement != (Team.Leader as PrismFighter).Prisme.Alignement)
                    return false;
            }

            if (Team.Leader is CharacterFighter)
            {
                if (Character.Alignement != (Team.Leader as CharacterFighter).Character.Alignement)
                    return false;
            }
            return base.CanJoin(Team, Character);
        }

        public override void OverridableEndFight(FightTeam Winners, FightTeam Loosers)
        {
            var WinnersFighter = Winners.GetFighters().OfType<CharacterFighter>().ToList();
            var LoosersFighter = Loosers.GetFighters().OfType<CharacterFighter>().ToList();
            var WinnersFighterP = Winners.GetFighters().OfType<PrismFighter>().ToList();
            var LoosersFighterP = Loosers.GetFighters().OfType<PrismFighter>().ToList();

            var WinnersTotalGrade = WinnersFighter.Sum(x => x.Character.getGrade());
            var WinnersTotalLevel = WinnersFighter.Sum(x => x.Level);

            WinnersTotalGrade += WinnersFighterP.Sum(x => x.Prisme.Level * 15 + 80);//Setting LEVEL PVP
            WinnersTotalLevel += WinnersFighterP.Sum(x => x.Level);

            var LoosersTotalGrade = LoosersFighter.Sum(x => x.Character.getGrade());
            var LoosersTotalLevel = LoosersFighter.Sum(x => x.Level);

            LoosersTotalGrade += LoosersFighterP.Sum(x => x.Prisme.Level * 15 + 80);
            LoosersTotalLevel += LoosersFighterP.Sum(x => x.Level);

            foreach (var Fighter in LoosersFighter)
            {
                int WinHonor = 0;
                if (Fighter.Character.AlignmentType != AlignmentTypeEnum.ALIGNMENT_NEUTRAL)
                {
                    WinHonor = Algo.CalculateAggressionHonor(Fighter, false, WinnersFighter.Count + WinnersFighterP.Count, WinnersTotalGrade, WinnersTotalLevel, LoosersTotalGrade, LoosersTotalLevel);
                    if (WinHonor > Fighter.Character.Honor)
                        WinHonor = Fighter.Character.Honor;
                    Fighter.Character.RemoveHonor(WinHonor);
                }


                if (Fighter.Character.GetClient() != null)
                    Fighter.Character.GetClient().Send(new AccountStatsMessage(Fighter.Character));


                this.myResult.AddResult(Fighter, false, WinHonor: -WinHonor);
            }

            var PhToWin = Algo.Random(150, 300);

            foreach (var Fighter in WinnersFighter)
            {
                var Drops = new Dictionary<int, int>();

                if (Fighter.Character.AlignmentType != AlignmentTypeEnum.ALIGNMENT_NEUTRAL)
                    Fighter.Character.AddHonor(PhToWin);

                this.myResult.AddResult(Fighter, true, WinHonor: PhToWin, WinItems: Drops);
            }

            foreach (var Fighter in LoosersFighterP)
            {
                int WinHonor = PhToWin;

                if (WinHonor > Fighter.Prisme.Honor)
                    WinHonor = Fighter.Prisme.Honor;
                Fighter.Prisme.AddHonor(-WinHonor);

                Network.WorldServer.GetChatController().getAlignementChannel(AlignmentTypeEnum.ALIGNMENT_NEUTRAL).Send(new SubAreaAlignMessage(Map.subArea.ID + "|0|1"));
                Network.WorldServer.GetChatController().getAlignementChannel((AlignmentTypeEnum)Prisme.Alignement).Send(new ConquestPrismDiedMessage(Prisme.Mapid + "|" + Prisme.Map.X + "|" + Prisme.Map.Y));

                foreach (var Key in Network.WorldServer.GetChatController().AlignChannels.Keys.Where(x => x != AlignmentTypeEnum.ALIGNMENT_NEUTRAL))
                {
                    Network.WorldServer.GetChatController().getAlignementChannel(Key).Send(new SubAreaAlignMessage(Map.subArea.ID + "|-1|0"));
                    Network.WorldServer.GetChatController().getAlignementChannel(Key).Send(new SubAreaAlignMessage(Map.subArea.ID + "|0|1"));
                    if (Prisme.Area == -1)
                        continue;
                    Network.WorldServer.GetChatController().getAlignementChannel(Key).Send(new AreaAlignMessage(Map.subArea.areaID + "|-1"));
                }
                if (Prisme.Area != -1)
                {
                    Map.subArea.area.Prisme = 0;
                    Map.subArea.area.setAlignement(0);
                }

                Map.subArea.Prisme = 0;
                Map.subArea.setAlignement(0);
                this.Map.DestroyActor((Fighter as PrismFighter).Prisme);
                PrismeTable.TryDeleteTax((Fighter as PrismFighter).Prisme);

                this.myResult.AddResult(Fighter, false, WinHonor: -WinHonor);

            }
            foreach (var Fighter in WinnersFighterP)
            {
                int WinHonor = Algo.CalculateAggressionHonor(Fighter, true, WinnersFighter.Count + WinnersFighterP.Count, WinnersTotalGrade, WinnersTotalLevel, LoosersTotalGrade, LoosersTotalLevel);
                Fighter.Prisme.AddHonor(WinHonor);
                Network.WorldServer.GetChatController().getAlignementChannel((AlignmentTypeEnum)Prisme.Alignement).Send(new ConquestPrismSurvivedMessage(Prisme.Mapid + "|" + Prisme.Map.X + "|" + Prisme.Map.Y));
                Prisme.inFight = -1;
                Prisme.CurrentFight = null;
                Prisme.Map.SpawnActor((Fighter as PrismFighter).Prisme);
                var Drops = new Dictionary<int, int>();
                this.myResult.AddResult(Fighter, true, WinHonor: WinHonor, WinItems: Drops);
            }
            base.EndFight();
        }

        public override int GetStartTimer()
        {
            return 45000;
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

                this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER_ALIGNED).Append(';');
                this.mySerializedString.Append(AttFighter.Character.Alignement);

                this.mySerializedString.Append('|');//Separation

                this.mySerializedString.Append(this.myTeam2.LeaderId).Append(';');//Infos Team2
                this.mySerializedString.Append(this.myTeam2.Leader.MapCell).Append(';');
                this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER_ALIGNED).Append(';');
                this.mySerializedString.Append(Prisme.Alignement);
            }

            Packet.Append(this.mySerializedString.ToString());
        }

        public override string SerializeAs_FightListInformations()
        {
            var Infos = new StringBuilder(this.FightId.ToString()).Append(';');//FightID
            Infos.Append(this.FightState == FightState.STATE_ACTIVE ? (this.FightTime) : (-1)).Append(';');//FightTime

            Infos.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER_ALIGNED).Append(","); //Infos Team1
            Infos.Append(AttFighter.Character.Alignement).Append(",");
            Infos.Append(this.myTeam1.GetAliveFighters().Count).Append(';');

            Infos.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER_ALIGNED).Append(","); //Infos Team2
            Infos.Append(Prisme.Alignement).Append(",");
            Infos.Append(this.myTeam2.GetAliveFighters().Count).Append(';');

            return Infos.ToString();
        }
    }
}
