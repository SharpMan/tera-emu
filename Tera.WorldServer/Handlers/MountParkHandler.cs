using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class MountParkHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'b':
                    MountParkHandler.BuyRequest(Client, Packet);
                    break;
                case 's':
                    MountParkHandler.SellRequest(Client, Packet);
                    break;
                case 'd':
                    MountParkHandler.MountDescriptionRequest(Client, Packet);
                    break;
                case 'n':
                    MountParkHandler.MountChangeNameRequest(Client, Packet);
                    break;
                case 'v':
                    Client.Send(new GameRideMessage("v"));
                    break;
                case 'x':
                    MountParkHandler.MountOnUpdateGiveXP(Client, Packet);
                    break;
                case 'r':
                    if (Client.Character.Level < 60 || Client.Character.Mount == null || !Client.Character.Mount.isMountable())
                    {
                        Client.Send(new CharacterRideEventMessage("Er", null));
                        return;
                    }
                    Client.Character.toogleOnMount();
                    break;
            }
        }

        private static void MountOnUpdateGiveXP(WorldClient Client, string Packet)
        {
            try
            {
                int xp = int.Parse(Packet.Substring(2));
                if (xp < 0)
                {
                    xp = 0;
                }
                if (xp > 90)
                {
                    xp = 90;
                }
                Client.Character.MountXPGive = xp;
                Client.Send(new CharacterMountXpGive(Client.Character.MountXPGive));
            }
            catch (Exception e)
            {
            }
        }

        private static void MountChangeNameRequest(WorldClient Client, string Packet)
        {
            Packet = Packet.Substring(2);
            if (Client.Character.Mount == null)
                return;
            Client.Character.Mount.Name = Packet;
            Client.Send(new MountNameChangeMessage(Packet));
        }

        private static void MountDescriptionRequest(WorldClient Client, string Packet)
        {
            int DDid = -1;
            try
            {
                DDid = int.Parse(Regex.Split(Packet.Substring(2),("\\|"))[0]);
                //on ignore le temps?
            }
            catch (Exception e)
            {
            }
            if (DDid == -1)
            {
                return;
            }
            Mount DD = MountTable.getMount(DDid);
            if (DD == null)
            {
                return;
            }
            Client.Send(new MountDescriptionMessage(DD));
        }

        private static void SellRequest(WorldClient Client, string Packet)
        {
            Client.Send(new GameRideMessage("v"));
            int price = int.Parse(Packet.Substring(2));
            MountPark MP1 = Client.Character.myMap.mountPark;
            if (MP1 == null) return;
            if (MP1.get_owner() == -1)
            {
                Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 94));
                return;
            }
            if (MP1.get_owner() != Client.Character.ID)
            {
                Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 95));
                return;
            }
            MP1.set_price(price);
            MountParkTable.Update(MP1);
            Client.Character.myMap.SendToMap(new MapMountParkMessage(MP1));
        }

        private static void BuyRequest(WorldClient Client, string Packet)
        {
            Client.Send(new GameRideMessage("v"));
            MountPark MP = Client.Character.myMap.mountPark;
            if (MP == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            if (MP.get_owner() == -1)
            {
                Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 96));
                return;
            }
            if (MP.get_price() == 0)
            {
                Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 97));
                return;
            }
            if (!Client.Character.HasGuild())
            {
                Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 135));
                return;
            }
            if (Client.GetCharacter().getCharacterGuild().GradeType != GuildGradeEnum.GRADE_BOSS)
            {
                Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 98));
                return;
            }
            byte enclosMax = (byte)Math.Floor((double)Client.Character.GetGuild().Level / 10);
            byte TotalEncloGuild = (byte)MountParkTable.CountByGuild(Client.Character.GetGuild().ID);
            if (TotalEncloGuild >= enclosMax)
            {
                Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 103));
                return;
            }
            if (Client.Character.Kamas < MP.get_price())
            {
                Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 82));
                return;
            }
            Client.GetCharacter().InventoryCache.SubstractKamas(MP.get_price());
            if (MP.get_owner() > 0)
            {
                var Owner = CharacterTable.GetCharacter(MP.get_owner());
                if (Owner != null && Owner.Account.curPlayer != null)
                {
                    Owner.Send(new ChatGameMessage("Un enclo a ete vendu a " + MP.get_price() + ".", "CC0000"));
                    Owner.Account.Data.BankKamas += MP.get_price();
                    Owner.Account.Data.Save();
                }
                else
                {
                    AccountDataTable.Update(MP.get_price(), MP.get_owner());
                }
            }
            MP.set_price(0);
            MP.set_owner(Client.Character.ID);
            MP.set_guild(Client.Character.GetGuild());
            MountParkTable.Update(MP);
            CharacterTable.Update(Client.Character);
            Client.GetCharacter().myMap.SendToMap(new MapMountParkMessage(MP));
        }
    }
}
