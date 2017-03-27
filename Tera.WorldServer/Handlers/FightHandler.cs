using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Network;
using Tera.WorldServer.World;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.Fights.Types;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class FightHandler
    {
        public static void ProcessGameReadyRequest(WorldClient Client)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.FIGHT))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetFight().FightState != FightState.STATE_PLACE)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            Client.GetFight().SetFighterReady(Client.GetFighter());
        }

        public static void ProcessGameTurnReadyRequest(WorldClient Client)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.FIGHT))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetFight().FightState != FightState.STATE_ACTIVE)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetFighter() == null)
                return;

            Client.GetFighter().TurnReady = true;
        }

        public static void ProcessGameRageQuitRequest(WorldClient Client, string Packet)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.FIGHT))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var FighterId = -1;
            var Fighter = Client.GetFighter();

            if (Packet.Length > 2)
            {
                if (Client.GetFight().FightState != FightState.STATE_PLACE)
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }

                if (Client.GetFighter().Team.Leader != Client.GetFighter())
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }

                if (!int.TryParse(Packet.Substring(2), out FighterId))
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }

                Fighter = Client.GetFight().GetFighter(FighterId);

                if (Fighter == Client.GetFighter())
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }

                if (Fighter.Team != Client.GetFighter().Team)
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }
            }

            if (Fighter == null)
                Client.GetFight().LeaveSpectator(Client);
            else
                Client.GetFight().LeaveFight(Fighter);
        }

        public static void ProcessGamePlacementRequest(WorldClient Client, string Packet)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.FIGHT))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetFight().FightState != FightState.STATE_PLACE)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var CellId = 0;

            if (!int.TryParse(Packet.Substring(2), out CellId))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            Client.GetFight().SetFighterPlace(Client.GetFighter(), CellId);
        }

        public static void ProcessGameTurnPassRequest(WorldClient Client)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.FIGHT))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetFight().FightState != FightState.STATE_ACTIVE)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetFight().CurrentFighter == Client.GetFighter())
            {
                Client.GetFight().FightLoopState = FightLoopState.STATE_END_TURN;
            }
        }

        public static void ProcessGameFightJoinRequest(WorldClient Client, string Packet)
        {
            var Data = Packet.Substring(5).Split(';');

            if (Data.Length > 2)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var FightId = 0;

            if (!int.TryParse(Data[0], out FightId))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Fight = Client.GetCharacter().GetMap().GetFight(FightId);

            if (Data.Length == 1)
            {
                if (Fight.CanJoinSpectator())
                {
                    Fight.JoinFightSpectator(Client);
                }
                else
                {
                    Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 57));
                }

                return;
            }

            var LeaderId = 0;

            if (!int.TryParse(Data[1], out LeaderId))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Fight == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetCharacter().HasRestriction(RestrictionEnum.RESTRICTION_IS_TOMBESTONE))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Team = Fight.GetTeam(LeaderId);

            if (Fight.CanJoin(Team, Client.GetCharacter()))
            {
                var Fighter = new CharacterFighter(Fight, Client);

                var FightAction = new GameFight(Fighter, Fight);

                Client.AddGameAction(FightAction);

                Fight.JoinFightTeam(Fighter, Team);
            }
        }

        public static void ProcessToggleLockRequest(WorldClient Client, string Packet)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.FIGHT))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Packet.Length != 2)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var ToggleStr = Packet[1];

            if (!Enum.IsDefined(typeof(ToggleTypeEnum), (int)ToggleStr))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetFighter() != Client.GetFighter().Team.Leader)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            Client.GetFight().ToggleLock(Client.GetFighter(), (ToggleTypeEnum)ToggleStr);
        }

        public static void ProcessFightDetailsRequest(WorldClient Client, string Packet)
        {
            if (Packet.Length < 3)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var FightId = 0;

            if (!int.TryParse(Packet.Substring(2), out FightId))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Fight = Client.GetCharacter().GetMap().GetFight(FightId);

            if (Fight == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            Client.Send(new FightDetailInformationMessage(Fight));
        }


        public static void ProcessStartTaxCollectorFightRequest(WorldClient Client, string Packet)
        {
            if (Client.GetFight() != null || !Client.CanGameAction(GameActionTypeEnum.FIGHT))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            long TargetId = 0;

            if (!long.TryParse(Packet.Substring(5), out TargetId))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Actor = Client.GetCharacter().GetMap().GetActor(TargetId);

            if (Actor == null || Actor.ActorType != GameActorTypeEnum.TYPE_TAX_COLLECTOR)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Target = Actor as TaxCollector;

            if (Target.inFight > 0 || Target.Guild == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            Client.GetCharacter().GetMap().SendToMap(new GameActionMessage((int)GameActionTypeEnum.TAXCOLLECTOR_AGRESSION, Client.GetCharacter().ActorId, TargetId.ToString()));
            Client.GetCharacter().GetMap().AddFight(new PercepteurFight(Client.GetCharacter().GetMap(), Client, Target));
        }

        public static void ProcessStartPrismFightRequest(WorldClient Client, string Packet)
        {
            if (Client.GetFight() != null || !Client.CanGameAction(GameActionTypeEnum.FIGHT))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            long TargetId = 0;

            if (!long.TryParse(Packet.Substring(5), out TargetId))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Actor = Client.GetCharacter().GetMap().GetActor(TargetId);

            if (Actor == null || Actor.ActorType != GameActorTypeEnum.TYPE_PRISM)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Target = Actor as Prisme;

            if (Target.inFight == 0 || Target.inFight == -2)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            Client.GetCharacter().GetMap().SendToMap(new GameActionMessage((int)GameActionTypeEnum.TAXCOLLECTOR_AGRESSION, Client.GetCharacter().ActorId, TargetId.ToString()));
            Client.GetCharacter().GetMap().AddFight(new PrismFight(Client.GetCharacter().GetMap(), Client, Target));
        }
    }
}
