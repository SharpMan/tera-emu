using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Packets;
using Tera.Libs.Enumerations;
using System.Timers;
using System.Runtime.CompilerServices;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.Fights
{
    public sealed class PercepteurFight : Fight
    {
        public TaxCollector TaxCollector
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
            if (TaxCollector.TimeTurn <= 0)
            {
                _timer.Stop();
                _timer = null;
                return;
            }
            TaxCollector.TimeTurn -= 1000;
        }

        public PercepteurFight(Map Map, WorldClient Player, TaxCollector Monsters) : base(FightType.TYPE_PVT, Map)
        {
            try
            {
                Logger.Debug("PVT_FIGHT Launched : Player=" + Player.GetCharacter().Name + " MapId=" + Map.Id);
                this.TaxCollector = Monsters;
                StartTimer();

                var AttFighter = new CharacterFighter(this, Player);
                var DefFighter = new PercepteurFighter(this, TaxCollector, this.NextActorId);
                TaxCollector.Map.DestroyActor(TaxCollector);
                TaxCollector.Guild.Send(new GuildFightInformationsMesssage(TaxCollector.Guild));

                foreach (var member in TaxCollector.Guild.CharactersGuildCache.Where(x => x.getPerso() != null && x.getPerso().IsOnline()))
                {
                    TaxCollector.parseAttaque(member.getPerso(), TaxCollector.GuildID);
                    TaxCollector.parseDefense(member.getPerso(), TaxCollector.GuildID);
                    member.getPerso().Send(new ChatGameMessage("Un de vos percepteurs est attaqué!", "CC0000"));
                }

                

                Player.AddGameAction(new GameFight(AttFighter, this));

                base.InitFight(AttFighter, DefFighter);

            }
            catch (Exception ex)
            {
                Logger.Error("PercepteurFight::PercepteurFight() "+ ex.ToString());
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

        public override void OverridableEndFight(FightTeam Winners, FightTeam Loosers)
        {
            foreach (var Fighter in Loosers.GetFighters())
            {
                this.myResult.AddResult(Fighter, false);
                if (Fighter is PercepteurFighter)
                {
                    this.Map.DestroyActor((Fighter as PercepteurFighter).TaxCollector);
                    (Fighter as PercepteurFighter).TaxCollector.Guild.Send(new GuildFightInformationsMesssage((Fighter as PercepteurFighter).TaxCollector.Guild));
                    (Fighter as PercepteurFighter).TaxCollector.Guild.Send(new GuildAttackedTaxcollector(AttackedTaxcollectorState.DIED, (Fighter as PercepteurFighter).TaxCollector));
                    TaxCollectorTable.TryDeleteTax((Fighter as PercepteurFighter).TaxCollector);
                }
            }
            foreach (var Fighter in Winners.GetFighters())
            {
                this.myResult.AddResult(Fighter, true);
                if (Fighter is PercepteurFighter)
                {
                    (Fighter as PercepteurFighter).TaxCollector.Guild.Send(new GuildFightInformationsMesssage((Fighter as PercepteurFighter).TaxCollector.Guild));
                    (Fighter as PercepteurFighter).TaxCollector.Guild.Send(new GuildAttackedTaxcollector(AttackedTaxcollectorState.SURVIVED, (Fighter as PercepteurFighter).TaxCollector));
                    (Fighter as PercepteurFighter).TaxCollector.inFight = 0;
                    (Fighter as PercepteurFighter).TaxCollector.CurrentFight = null;
                    (Fighter as PercepteurFighter).TaxCollector.Map.SpawnActor((Fighter as PercepteurFighter).TaxCollector);
                }
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

                this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER).Append(';');
                this.mySerializedString.Append((int)AlignmentTypeEnum.ALIGNMENT_WITHOUT);

                this.mySerializedString.Append('|');//Separation

                this.mySerializedString.Append(this.myTeam2.LeaderId).Append(';');//Infos Team2
                this.mySerializedString.Append(this.myTeam2.Leader.MapCell).Append(';');
                this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_TAXCOLLECTOR).Append(';');
                this.mySerializedString.Append((int)AlignmentTypeEnum.ALIGNMENT_WITHOUT);
            }

            Packet.Append(this.mySerializedString.ToString());
        }

        public override string SerializeAs_FightListInformations()
        {
            var Infos = new StringBuilder(this.FightId.ToString()).Append(';');//FightID
            Infos.Append(this.FightState == FightState.STATE_ACTIVE ? (this.FightTime) : (-1)).Append(';');//FightTime

            Infos.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER).Append(","); //Infos Team1
            Infos.Append((int)AlignmentTypeEnum.ALIGNMENT_WITHOUT).Append(",");
            Infos.Append(this.myTeam1.GetAliveFighters().Count).Append(';');

            Infos.Append((int)TeamTypeEnum.TEAM_TYPE_TAXCOLLECTOR).Append(","); //Infos Team2
            Infos.Append((int)AlignmentTypeEnum.ALIGNMENT_WITHOUT).Append(",");
            Infos.Append(this.myTeam2.GetAliveFighters().Count).Append(';');

            return Infos.ToString();
        }
    }
}
