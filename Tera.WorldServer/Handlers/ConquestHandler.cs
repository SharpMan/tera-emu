using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;
using Tera.Libs.Utils;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class ConquestHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'W':
                    ConquestHandler.ProcessConquestGepositionRequest(Client, Packet);
                    break;
                case 'I':
                    ConquestHandler.ProcessConquestDefenseRequest(Client, Packet);
                    break;
                case 'B':
                    float porc = AreaSubTable.getWorldBalance(Client.Character.Alignement);
                    float porcN = (float)Math.Round((double)Client.Character.Level / 2.5F + 1.0D);
                    Client.Send(new ConquestBonusMessage(porc + "," + porc + "," + porc + ";" + porcN + "," + porcN + "," + porcN + ";" + porc + "," + porc + "," + porc));
                    break;
                case 'b':
                    Client.Send(new ConquestBalanceMessage(AreaSubTable.getWorldBalance(Client.Character.Alignement) + ";" + AreaTable.getAreaBalance(Client.Character.myMap.subArea.area, Client.Character.Alignement)));
                    break;
                case 'F':
                    ConquestHandler.ProcessConquestJoinDefenseRequest(Client, Packet);
                    break;
            }
        }

        private static void ProcessConquestJoinDefenseRequest(WorldClient Client, string Packet)
        {
            if (Packet.Length < 2)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            switch (Packet[2])
            {
                case 'J':
                    long prismID = Client.Character.myMap.subArea.Prisme;
                    var Prism = PrismeTable.getPrism(prismID);
                    if (Prism == null || Prism.Alignement != Client.Character.Alignement || Client.GetFight() != null || Prism.CurrentFight == null)
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }

                    if (Client.GetFight() == null && !Client.GetCharacter().isAaway)
                    {
                        if (Client.GetCharacter().myMap.Id != Prism.Mapid)
                        {
                            Client.GetCharacter().isJoiningTaxFight = true;
                            Client.GetCharacter().OldPosition = new Couple<Map, int>(Client.Character.myMap, Client.Character.CellId);
                            Client.GetCharacter().Teleport(Prism.Map, Prism.CellId);
                            try
                            {
                                System.Threading.Thread.Sleep(700);
                            }
                            catch (Exception e)
                            {
                                Logger.Error(e);
                            }
                        }

                        if (Client.GetCharacter().HasRestriction(RestrictionEnum.RESTRICTION_IS_TOMBESTONE))
                        {
                            Client.Send(new BasicNoOperationMessage());
                            return;
                        }

                        var Team = Prism.CurrentFight.GetTeam2();

                        if (Prism.CurrentFight.CanJoin(Team, Client.GetCharacter()))
                        {
                            var Fighter = new CharacterFighter(Prism.CurrentFight, Client);

                            var FightAction = new GameFight(Fighter, Prism.CurrentFight);

                            Client.AddGameAction(FightAction);

                            Prism.CurrentFight.JoinFightTeam(Fighter, Team, false, -1, false);
                        }
                    }

                    //AnalyseDefense
                    var Packets = new List<PacketBase>();
                    foreach (var Prisme in PrismeTable.Cache.Values.Where(x => x.Alignement == Client.Character.Alignement && (x.inFight == 0 || x.inFight == -2)))
                    {
                        Packets.Add(new PrismInformationsDefenseMessage(Prisme.PrismDefenders(Prisme.ActorId, Prisme.CurrentFight)));
                    }
                    foreach (var packet in Packets)
                    {
                        Network.WorldServer.GetChatController().getAlignementChannel(Client.Character.AlignmentType).Send(packet);
                    }

                    break;
            }
        }

        private static void ProcessConquestDefenseRequest(WorldClient Client, string Packet)
        {
            switch (Packet[2])
            {
                case 'J':
                    var Prism = PrismeTable.getPrism(Client.Character.myMap.subArea.Prisme);
                    if (Prism != null)
                    {
                        Prisme.AnalyzeAttack(Client.Character);
                        Prisme.AnalyzeDefense(Client.Character);
                    }
                    Client.Send(new ConquestInfoJoinPrismMessage(Client.Character.PrismHelper()));
                    break;
                case 'V':
                    Client.Send(new ConquestClosePanelMessage());
                    break;
            }
        }

        private static void ProcessConquestGepositionRequest(WorldClient Client, string Packet)
        {
            if (Packet.Length < 3)
            {
                return;
            }
            switch (Packet[2])
            {
                case 'J':
                    Client.Send(new ConquestGepositionInfosMessage(PrismeTable.SerializePrismGeposition(Client.GetCharacter().Alignement)));
                    break;
            }

        }

    }
}
