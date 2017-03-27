using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Network;
using Tera.WorldServer.World;
using Tera.WorldServer.World.Actions;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.GameRequests;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class GameActionHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'A':
                    GameActionHandler.ProcessGameActionStartRequest(Client, Packet);
                    break;

                case 'K':
                    switch (Packet[2])
                    {
                        case 'K':
                            GameActionHandler.ProcessGameActionFinishRequest(Client);
                            break;

                        case 'E':
                            GameActionHandler.ProcessGameMapMovementAbortRequest(Client, Packet);
                            break;
                    }
                    break;

                case 'F':
                    if (Client.Character.HasRestriction(RestrictionEnum.RESTRICTION_IS_TOMBESTONE))
                    {
                        //Client.GetCharacter().OnFreeSoul();
                    }
                    break;

                case 'R':
                    FightHandler.ProcessGameReadyRequest(Client);
                    break;

                case 'T':
                    FightHandler.ProcessGameTurnReadyRequest(Client);
                    break;

                case 't':
                    FightHandler.ProcessGameTurnPassRequest(Client);
                    break;

                case 'Q':
                    FightHandler.ProcessGameRageQuitRequest(Client, Packet);
                    break;

                case 'p':
                    FightHandler.ProcessGamePlacementRequest(Client, Packet);
                    break;

                case 'P':
                    AlignmentHandler.ProcessAlignmentEnableRequest(Client, Packet);
                    break;

                case 'd':
                    if (Packet.Length < 3)
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }
                    switch (Packet[2])
                    {
                        case 'i':
                            if (Client.GetFight() != null && Client.GetFight().Challanges.Where(c => c.Id == int.Parse(Packet.Substring(3))).Count() != 0)
                            {
                                var chall = Client.GetFight().Challanges.FirstOrDefault(c =>  c.Id == int.Parse(Packet.Substring(3)));
                                var target = Client.GetFight().Fighters.FirstOrDefault(f => !f.Dead && f.ActorId == chall.TargetId);
                                if (target == null)
                                {
                                    Client.Send(new BasicNoOperationMessage());
                                    return;
                                }
                                Client.Send(new FightShowCell(chall.TargetId, target.CellId));
                            }
                            break;
                    }
                    break;
            }
        }

        public static void ProcessGameMapMovementAbortRequest(WorldClient Client, string Packet)
        {
            Client.Send(new BasicNoOperationMessage());

            if (!Client.IsGameAction(GameActionTypeEnum.MAP_MOVEMENT))
                return;

            if (Client.IsGameAction(GameActionTypeEnum.FIGHT))
                return;

            if (!Packet.Contains('|'))
                return;

            var Data = Packet.Split('|');

            int CellId = 0;

            if (Data.Length != 2)
                return;

            if (!int.TryParse(Data[1], out CellId))
                return;

            Client.AbortGameAction(GameActionTypeEnum.MAP_MOVEMENT, CellId);
        }

        public static void ProcessGameActionFinishRequest(WorldClient Client)
        {
            Client.Send(new BasicNoOperationMessage());

            if (Client.IsGameAction(GameActionTypeEnum.FIGHT))
            {
                Client.GetFight().StopAction(Client.GetFighter());
            }

            if (Client.IsGameAction(GameActionTypeEnum.MAP_MOVEMENT))
            {
                Client.EndGameAction(GameActionTypeEnum.MAP_MOVEMENT);
            }

            if (Client.IsGameAction(GameActionTypeEnum.CELL_ACTION))
            {
                Client.EndGameAction(GameActionTypeEnum.CELL_ACTION);
            }

        }

        public static void ProcessGameActionStartRequest(WorldClient Client, string Packet)
        {
            // fake Packet
            if (Packet.Length < 5)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            int ActionId = 0;

            if (!int.TryParse(Packet.Substring(2, 3), out ActionId))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            // action existante ?
            if (!Enum.IsDefined(typeof(GameActionTypeEnum), ActionId))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!Client.CanGameAction((GameActionTypeEnum)ActionId) && (GameActionTypeEnum)ActionId != GameActionTypeEnum.CELL_ACTION)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            switch ((GameActionTypeEnum)ActionId)
            {
                case GameActionTypeEnum.MAP_MOVEMENT:
                    GameActionHandler.ProcessGameMapMovementRequest(Client, Packet);
                    break;

                case GameActionTypeEnum.CHALLENGE_REQUEST:
                    GameActionHandler.ProcessGameMapChallengeRequest(Client, Packet);
                    break;

                case GameActionTypeEnum.CELL_ACTION:
                    GameActionHandler.ProcessGameCellActionRequest(Client, Packet);
                    break;

                case GameActionTypeEnum.CHALLENGE_DENY:
                    GameActionHandler.ProcessGameChallengeDenyRequest(Client);
                    break;

                case GameActionTypeEnum.CHALLENGE_ACCEPT:
                    GameActionHandler.ProcessGameChallengeAcceptRequest(Client);
                    break;

                case GameActionTypeEnum.FIGHT_AGGRESSION:
                    GameActionHandler.ProcessGameAggressionRequest(Client, Packet);
                    break;

                case GameActionTypeEnum.FIGHT_LAUNCHSPELL:
                    GameActionHandler.ProcessGameFightLaunchSpellRequest(Client, Packet);
                    break;
                case GameActionTypeEnum.FIGHT_USEWEAPON:
                    GameActionHandler.ProcessGameFightLaunchWeaponRequest(Client, Packet);
                    break;

                case GameActionTypeEnum.FIGHT_JOIN:
                    FightHandler.ProcessGameFightJoinRequest(Client, Packet);
                    break;

                case GameActionTypeEnum.TAXCOLLECTOR_AGRESSION:
                    FightHandler.ProcessStartTaxCollectorFightRequest(Client, Packet);
                    break;

                case GameActionTypeEnum.OPEN_PRISM_MENU:
                    GameActionHandler.OpenPrismMenuRequest(Client, Packet);
                    break;

                case GameActionTypeEnum.PRISM_ATTACK:
                    FightHandler.ProcessStartPrismFightRequest(Client, Packet);
                    break;
                    
            }
        }

        private static void OpenPrismMenuRequest(WorldClient Client, string Packet)
        {
            if (Client.GetFight() != null || Client.Character.isZaaping || Client.GetExchange() != null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            if(Client.Character.Deshonor >= 3)
            {
                Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 83));
                return;
            }
            Client.Character.isZaaping = false;
            Client.Send(new GamePrismMenuMessage(Client.Character));
        }

        public static void ProcessGameFightLaunchWeaponRequest(WorldClient Client, string Packet)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.FIGHT))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            int CellId;
            if (!int.TryParse(Packet.Substring(5), out CellId))
            {
                return;
            }
            Client.GetFight().LaunchWeapon(Client.GetFighter(), CellId);
        }

        /* public static void ProcessGameMapCellAction(WorldClient Client, string Packet)
         {
             /*if (!Client.CanGameAction(GameActionTypeEnum.CELL_ACTION))
             {
                 Client.Send(new BasicNoOperationMessage());
                 return;
             }
             int nextGameActionID = 0;
             if (Client.miniActions.Count > 0)
             {
                 nextGameActionID = Client.miniActions.Count + 1;
             }
             MiniGameAction GA = new MiniGameAction(nextGameActionID, (int)GameActionTypeEnum.CELL_ACTION, Packet);
             String Packet = Packet.Substring(5);
             int cellID = -1;
             int actionID = -1;
             try
             {
                 cellID = int.Parse(Packet.Substring(5).Split(';')[0]);
                 actionID = int.Parse(Packet.Substring(5).Split(';')[1]);
             }
             catch (Exception e)
             {
             }
             if (cellID == -1 || actionID == -1 || Client.Character == null || Client.Character.myMap == null || Client.Character.myMap.getCell(cellID) == null)
             {
                 return;
             }
             GA._args = cellID + ";" + actionID;
             Client.miniActions.Add(nextGameActionID, GA);
             if (!Client.Character.myMap.getCell(cellID).canDoAction(actionID))
             {
                 Logger.Error("hna");
                 Client.Send(new BasicNoOperationMessage());
                 return;
             }
             Client.Character.myMap.getCell(cellID).startAction(Client.Character, GA);
         }*/


        public static void ProcessGameAggressionRequest(WorldClient Client, string Packet)
        {
            if (!Client.CanGameAction(GameActionTypeEnum.FIGHT))
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

            if (Actor == null || Actor.ActorType != GameActorTypeEnum.TYPE_CHARACTER)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Target = (Actor as Player).Client;

            if (Target.GetCharacter().Alignement == Client.GetCharacter().Alignement ||
                Client.GetCharacter().Alignement == 0 ||
                Client.GetCharacter().Alignement == (int)AlignmentTypeEnum.ALIGNMENT_NEUTRAL ||
                Client.GetCharacter().HasRestriction(RestrictionEnum.RESTRICTION_CANT_ATTACK) ||
                Target.GetCharacter().HasRestriction(RestrictionEnum.RESTRICTION_CANT_ATTACK))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Target == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!Client.GetCharacter().showWings)
            {
                Client.GetCharacter().showWings = true;
            }

            Target.AbortGameActions();

            Client.GetCharacter().GetMap().SendToMap(new GameActionMessage((int)GameActionTypeEnum.FIGHT_AGGRESSION, Client.GetCharacter().ActorId, TargetId.ToString()));

            Client.GetCharacter().GetMap().AddFight(new AggressionFight(Client.GetCharacter().GetMap(), Client, Target));
        }

        public static void ProcessGameChallengeDenyRequest(WorldClient Client)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.BASIC_REQUEST))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!(Client.GetBaseRequest() is ChallengeFightRequest))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetBaseRequest().Declin())
            {
                var AttId = Client.GetBaseRequest().Requester.GetCharacter().ActorId;
                var DefId = Client.GetBaseRequest().Requested.GetCharacter().ActorId.ToString();

                var Message = new GameActionMessage((int)GameActionTypeEnum.CHALLENGE_DENY, AttId, DefId);

                Client.GetBaseRequest().Requester.Send(Message);
                Client.GetBaseRequest().Requested.Send(Message);

                Client.EndGameAction(GameActionTypeEnum.BASIC_REQUEST);
            }
        }

        public static void ProcessGameChallengeAcceptRequest(WorldClient Client)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.BASIC_REQUEST))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!(Client.GetBaseRequest() is ChallengeFightRequest))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client == Client.GetBaseRequest().Requester)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            // Lancement du combat
            if (Client.GetBaseRequest().Accept())
            {
                var Fight = new ChallengeFight(Client.GetCharacter().GetMap(), Client.GetBaseRequest().Requester, Client.GetBaseRequest().Requested);

                Client.EndGameAction(GameActionTypeEnum.BASIC_REQUEST);

                Client.GetCharacter().GetMap().AddFight(Fight);
            }
        }

        public static void ProcessGameFightLaunchSpellRequest(WorldClient Client, string Packet)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.FIGHT))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Data = Packet.Substring(5).Split(';');
            var SpellId = int.Parse(Data[0]);
            var CellId = int.Parse(Data[1]);

            var Spell = Client.GetCharacter().GetSpellBook().GetSpellLevel(SpellId);

            // Sort existant ?
            if (Spell != null)
            {
                Client.GetFight().LaunchSpell(Client.GetFighter(), Spell, CellId);
            }
        }

        public static void ProcessGameCellActionRequest(WorldClient Client, string Packet)
        {
            if (Client.Character.HasRestriction(RestrictionEnum.RESTRICTION_IS_TOMBESTONE))
            {
                return;
            }
            if (Client.IsGameAction(GameActionTypeEnum.CELL_ACTION))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            Client.AddGameAction(new GameCellAction(Client, Packet));
        }

        public static void ProcessGameMapMovementRequest(WorldClient Client, string Packet)
        {

            if (Client.Character.HasRestriction(RestrictionEnum.RESTRICTION_IS_TOMBESTONE))
            {
                return;
            }

            string EncodedPath = Packet.Substring(5);

            Client.Send(new BasicNoOperationMessage());

            // minimum requis : header = direction + cellChar = 3
            if (EncodedPath.Length < 3)
            {
                return;
            }

            // En combat ?
            if (Client.IsGameAction(GameActionTypeEnum.FIGHT))
            {
                var Path = Pathfinder.IsValidPath(Client.GetFight(), Client.GetFighter(), Client.GetFighter().CellId, EncodedPath);

                if (Path != null)
                {
                    if (Client.GetFighter().Dead)
                    {
                        Client.Send(new BasicNoOperationMessage());
                        Client.GetFight().EndTurn();
                        return;
                    }
                    var GameMovement = Client.GetFight().TryMove(Client.GetFighter(), Path);

                    if (GameMovement != null)
                    {
                        Client.AddGameAction(GameMovement);
                    }
                }
                return;
            }


            var MovementPath = Pathfinder.IsValidPath(Client.Character.myMap, Client.Character.CellId, EncodedPath);
            if (MovementPath == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }


            Client.AddGameAction(new GameMapMovement(Client.Character.myMap, Client.Character, MovementPath));

            Client.Character.myMap.SendToMap(new GameActionMessage((int)GameActionTypeEnum.MAP_MOVEMENT, Client.Character.ActorId, MovementPath.ToString()));
        }

        public static void ProcessGameMapChallengeRequest(WorldClient Client, string Packet)
        {
            long TargetId = 0;

            if (!long.TryParse(Packet.Substring(5), out TargetId))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Actor = Client.GetCharacter().GetMap().GetActor(TargetId);

            if (Actor == null || Actor.ActorType != GameActorTypeEnum.TYPE_CHARACTER)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Target = (Actor as Player).GetClient();

            if (Target == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            // Occupé ?
            if (!Target.CanGameAction(GameActionTypeEnum.BASIC_REQUEST) ||
                Target.GetCharacter().HasRestriction(RestrictionEnum.RESTRICTION_CANT_CHALLENGE) ||
                Client.GetCharacter().HasRestriction(RestrictionEnum.RESTRICTION_CANT_CHALLENGE))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Request = new ChallengeFightRequest(Client, Target);
            var RequestAction = new GameRequest(Client.GetCharacter(), Request);

            Client.SetBaseRequest(Request);
            Target.SetBaseRequest(Request);

            Client.AddGameAction(RequestAction);
            Target.AddGameAction(RequestAction);

            var Message = new GameActionMessage((int)GameActionTypeEnum.CHALLENGE_REQUEST, Client.GetCharacter().ActorId, TargetId.ToString());

            Client.GetCharacter().GetMap().SendToMap(Message);
        }

    }
}
