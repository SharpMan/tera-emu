using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Packets;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights
{
    public sealed class ChallengeFight : Fight
    {
        public ChallengeFight(Map Map, WorldClient Attacker, WorldClient Defender)
            : base(FightType.TYPE_CHALLENGE, Map)
        {
            // Init du combat
            var AttFighter = new CharacterFighter(this, Attacker);
            var DefFighter = new CharacterFighter(this, Defender);

            Attacker.AddGameAction(new GameFight(AttFighter, this));
            Defender.AddGameAction(new GameFight(DefFighter, this));

            base.InitFight(AttFighter, DefFighter);
        }

        public override int GetStartTimer()
        {
            return -1;
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
                this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER).Append(';');
                this.mySerializedString.Append((this.Team1.Leader as CharacterFighter).Character.Alignement).Append('|');

                this.mySerializedString.Append(this.Team2.LeaderId).Append(';');//Infos Team2
                this.mySerializedString.Append(this.Team2.Leader.MapCell).Append(';');
                this.mySerializedString.Append((int)TeamTypeEnum.TEAM_TYPE_PLAYER).Append(';');
                this.mySerializedString.Append((this.Team2.Leader as CharacterFighter).Character.Alignement);
            }

            Packet.Append(this.mySerializedString.ToString());
        }

        public override string SerializeAs_FightListInformations()
        {
            var Infos = new StringBuilder(this.FightId.ToString()).Append(';');//FightID
            Infos.Append(this.FightState == FightState.STATE_ACTIVE ? (this.FightTime) : (-1)).Append(';');//FightTime

            Infos.Append(TeamTypeEnum.TEAM_TYPE_PLAYER).Append(","); //Infos Team1
            Infos.Append((this.Team1.Leader as CharacterFighter).Character.Alignement).Append(",");
            Infos.Append(this.Team1.GetAliveFighters().Count).Append(';');

            Infos.Append(TeamTypeEnum.TEAM_TYPE_PLAYER).Append(","); //Infos Team2
            Infos.Append((this.Team2.Leader as CharacterFighter).Character.Alignement).Append(",");
            Infos.Append(this.Team2.GetAliveFighters().Count).Append(';');

            return Infos.ToString();
        }

        /// <summary>
        /// Kické ou alors tout simplement annulé
        /// </summary>
        /// <param name="Character"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override void OverridableLeaveFight(Fighter Fighter)
        {
            // Un persos quitte le combat
            switch (this.FightState)
            {
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
                    else
                    {
                        this.Map.SendToMap(new GameFightTeamFlagFightersMessage(new List<Fighter> { Fighter }, Fighter.Team.LeaderId, false));

                        this.SendToFight(new GameActorDestroyMessage(Fighter.ActorId));

                        Fighter.LeaveFight();

                        Fighter.Send(new GameLeaveMessage());
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
            foreach (var Fighter in Loosers.GetFighters())
                this.myResult.AddResult(Fighter, false);

            foreach (var Fighter in Winners.GetFighters())
                this.myResult.AddResult(Fighter, true);

            base.EndFight();
        }
    }
}
