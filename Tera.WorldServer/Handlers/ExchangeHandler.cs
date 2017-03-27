using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World;
using Tera.WorldServer.World.Character;
using Tera.WorldServer.World.Exchanges;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.GameRequests;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class ExchangeHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'A':
                    ExchangeHandler.ProcessExchangeAcceptRequest(Client);
                    break;

                case 'R':
                    ExchangeHandler.ProcessExchangeRequest(Client, Packet);
                    break;

                case 'r':
                    ExchangeHandler.ProcessExchangeMountParkRequest(Client, Packet);
                    break;

                case 'V':
                    ExchangeHandler.ProcessExchangeLeaveMessage(Client);
                    break;

                case 'H':
                    ExchangeHandler.ProcesssExchangeMarketRequest(Client, Packet);
                    break;

                case 'K':
                    ExchangeHandler.ProcessExchangeValidateRequest(Client);
                    break;

                case 'B':
                    ExchangeHandler.ProcessExchangeBuyRequest(Client, Packet);
                    break;

                case 'S':
                    ExchangeHandler.ProcessExchangeSellRequest(Client, Packet);
                    break;

                case 'M':
                    switch (Packet[2])
                    {
                        case 'G':
                            ExchangeHandler.ProcessExchangeMoveGoldRequest(Client, Packet);
                            break;

                        case 'O':
                            ExchangeHandler.ProcessExchangeMoveObjectRequest(Client, Packet);
                            break;
                    }
                    break;
            }
        }

        private static void ProcesssExchangeMarketRequest(WorldClient Client, String Data)
        {
            if (Data.Length < 3 || !Client.IsGameAction(GameActionTypeEnum.EXCHANGE))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Exchange = Client.GetExchange();

            if (Exchange.ExchangeType != (int)ExchangeTypeEnum.EXCHANGE_BIGSTORE_SELL && Exchange.ExchangeType != (int)ExchangeTypeEnum.EXCHANGE_BIGSTORE_BUY)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            switch (Data[2])
            {
                case 'P':
                    int ModelID = 0;
                    if (int.TryParse(Data.Substring(3), out ModelID))
                    {
                        if (ItemTemplateTable.Cache.ContainsKey(ModelID))
                        {
                            Client.Send(new ObjectHdvPriceMessage(ModelID));
                        }
                    }
                    else
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }

                    break;
                case 'T':
                    int typeObject = 0;

                    if (int.TryParse(Data.Substring(3), out typeObject))
                    {
                        if (!(Exchange as MarketBuyExchange).Market.Categories.ContainsKey(typeObject))
                        {
                            Client.Send(new BasicNoOperationMessage());
                            return;
                        }
                        Client.Send(new ExchangeBidHouseItemsList(typeObject, (Client.GetExchange() as MarketBuyExchange).Market.SerializeStringObjectType(typeObject)));
                    }
                    else
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }
                    break;
                case 'l':
                    ModelID = 0;
                    if (int.TryParse(Data.Substring(3), out ModelID))
                    {
                        //SerializeStringItemID
                        try
                        {
                            String str = (Client.GetExchange() as MarketBuyExchange).Market.SerializeStringItemID(ModelID);
                            Client.Send(new ExchangeLineBidHouseItemLine(str));
                        }
                        catch (Exception e)
                        {
                            Client.Send(new ExchangeBidHouseFailJoinLine("-", ModelID + ""));
                        }
                    }
                    else
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }
                    break;
                case 'B':
                    try
                    {
                        String[] info = Regex.Split(Data.Substring(3), "\\|");
                        int lineaID = int.Parse(info[0]);
                        int cantidad = int.Parse(info[1]);
                        long precio = long.Parse(info[2]);
                        if ((Exchange as MarketBuyExchange).Market.BuyObject(lineaID, cantidad, precio, Client.Character))
                        {
                            Client.Send(new ExchangeBidHouseFailJoinLine("-", lineaID + ""));
                            if ((Exchange as MarketBuyExchange).Market.getLine(lineaID) != null && (Exchange as MarketBuyExchange).Market.getLine(lineaID).CategorieIsBlank())
                            {
                                Client.Send(new ExchangeBidHouseFailJoinLine("+", (Exchange as MarketBuyExchange).Market.getLine(lineaID).SerializePriceList()));
                            }
                            Client.Send(new InventoryWeightMessage(0, 2000));
                            Client.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 68));
                        }
                        else
                        {
                            Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 72));
                        }
                    }
                    catch (Exception e)
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }
                    break;
            }

        }



        public static void ProcessExchangeRequest(WorldClient Client, string Packet)
        {
            if (!Packet.Contains('|'))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Data = Packet.Substring(2).Split('|');

            if (Data.Length != 2)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var ExchangeType = 0;
            long ActorId = 0;

            if (!int.TryParse(Data[0], out ExchangeType))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!Enum.IsDefined(typeof(ExchangeTypeEnum), ExchangeType))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!long.TryParse(Data[1], out ActorId) && (ExchangeTypeEnum)ExchangeType != ExchangeTypeEnum.EXCHANGE_MOUNT && (ExchangeTypeEnum)ExchangeType != ExchangeTypeEnum.EXCHANGE_BIGSTORE_BUY && (ExchangeTypeEnum)ExchangeType != ExchangeTypeEnum.EXCHANGE_BIGSTORE_SELL)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Actor = Client.Character.myMap.GetActor(ActorId);

            // inexistant ?
            if (Actor == null && (ExchangeTypeEnum)ExchangeType != ExchangeTypeEnum.EXCHANGE_MOUNT && (ExchangeTypeEnum)ExchangeType != ExchangeTypeEnum.EXCHANGE_BIGSTORE_BUY && (ExchangeTypeEnum)ExchangeType != ExchangeTypeEnum.EXCHANGE_BIGSTORE_SELL)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            // Il peu echanger ?
            if (!Client.CanGameAction(GameActionTypeEnum.EXCHANGE) && !(Client.GetExchange() != null && Client.GetExchange() is MarketSellExchange && ExchangeType == 11))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            switch ((ExchangeTypeEnum)ExchangeType)
            {
                case ExchangeTypeEnum.EXCHANGE_BIGSTORE_BUY:
                    if (Client.Character.Deshonor >= 5)
                    {
                        Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 83));
                        Client.Send(new ExchangeLeaveMessage());
                        return;
                    }
                    if (Client.GetExchange() != null)
                    {
                        ProcessExchangeLeaveMessage(Client);
                    }
                    if (!BidHouseTable.Cache.ContainsKey(Client.Character.Map))
                    {
                        //TODO IM Message HDV Not Exist
                        Client.Send(new ExchangeLeaveMessage());
                        return;
                    }
                    var Market = BidHouseTable.Cache[Client.Character.Map];

                    var MbExchange = new MarketBuyExchange(Client, Market);

                    Client.AddGameAction(new GameExchange(Client.Character, MbExchange));
                    Client.SetExchange(MbExchange);

                    Client.Send(new ExchangeCreateMessage(ExchangeTypeEnum.EXCHANGE_BIGSTORE_BUY, "1,10,100;" + Market.CategoriesString + ";" + Market.TaxRate() + ";" + Market.levelMax + ";" + Market.countItem + ";-1;" + Market.sellTime));

                    break;
                case ExchangeTypeEnum.EXCHANGE_SHOP:

                    // lol ?
                    if (Actor.ActorType != GameActorTypeEnum.TYPE_NPC)
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }

                    var Npc = Actor as Npc;
                    var Exchange = new NpcExchange(Client, Npc);


                    Client.AddGameAction(new GameExchange(Client.Character, Exchange));
                    Client.SetExchange(Exchange);

                    Client.Send(new ExchangeCreateMessage((ExchangeTypeEnum)ExchangeType, Npc.ActorId.ToString()));
                    Client.Send(new ExchangeItemListMessage(Npc));
                    break;

                case ExchangeTypeEnum.EXCHANGE_BIGSTORE_SELL:
                    if (Client.Character.Deshonor >= 5)
                    {
                        Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 83));
                        Client.Send(new ExchangeLeaveMessage());
                        return;
                    }
                    if (!BidHouseTable.Cache.ContainsKey(Client.Character.Map))
                    {
                        //TODO IM Message HDV Not Exist
                        Client.Send(new ExchangeLeaveMessage());
                        return;
                    }
                    Market = BidHouseTable.Cache[Client.Character.Map];

                    var MaExchange = new MarketSellExchange(Client, Market);

                    Client.AddGameAction(new GameExchange(Client.Character, MaExchange));
                    Client.SetExchange(MaExchange);

                    Client.Send(new ExchangeCreateMessage(ExchangeTypeEnum.EXCHANGE_BIGSTORE_SELL, "1,10,100;" + Market.CategoriesString + ";" + Market.TaxRate() + ";" + Market.levelMax + ";" + Market.countItem + ";-1;" + Market.sellTime));
                    Client.Send(new ExchangeItemListMessagePerAccount(Client.Account.Data, Market.HdvID));

                    break;

                case ExchangeTypeEnum.EXCHANGE_MOUNT:

                    var Mount = Client.Character.Mount;

                    if (Mount == null)
                    {
                        return;
                    }

                    var MExchange = new MountExchange(Client, Mount);


                    Client.AddGameAction(new GameExchange(Client.Character, MExchange));
                    Client.SetExchange(MExchange);

                    Client.Send(new ExchangeCreateMessage((ExchangeTypeEnum)ExchangeType, Mount.ID.ToString()));
                    Client.Send(new ExchangeMountItemListMessage(Mount));
                    Client.Send(new MountActualPodMessage(Mount));
                    break;

                case ExchangeTypeEnum.EXCHANGE_PLAYER:

                    // lol?
                    if (Actor.ActorType != GameActorTypeEnum.TYPE_CHARACTER)
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }

                    // cast
                    var Target = (Actor as Player).Client;

                    if (!Target.CanGameAction(GameActionTypeEnum.BASIC_REQUEST) ||
                       Target.Character.HasRestriction(RestrictionEnum.RESTRICTION_CANT_EXCHANGE) ||
                       Client.Character.HasRestriction(RestrictionEnum.RESTRICTION_CANT_EXCHANGE))
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }

                    var Request = new ExchangeRequest(Client, Target);
                    var RequestAction = new GameRequest(Client.Character, Request);

                    Client.AddGameAction(RequestAction);
                    Target.AddGameAction(RequestAction);

                    Client.SetBaseRequest(Request);
                    Target.SetBaseRequest(Request);

                    var Message = new ExchangeRequestMessage(Client.Character.ID, Target.Character.ID);

                    Client.Send(Message);
                    Target.Send(Message);
                    break;
                case ExchangeTypeEnum.EXCHANGE_TAXCOLLECTOR:
                    if (Packet.Length < 4 || !Client.Character.HasGuild())
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }
                    long PercepteurID = long.Parse(Packet.Substring(4));
                    TaxCollector perco = TaxCollectorTable.GetPerco(PercepteurID);

                    if (perco == null || perco.inFight > 0 || perco.inExchange)
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }

                    perco.inExchange = true;

                    if (Actor.ActorType != GameActorTypeEnum.TYPE_TAX_COLLECTOR)
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }

                    var TC = Actor as TaxCollector;
                    var TExchange = new TaxCollectorExchange(Client, TC);


                    Client.AddGameAction(new GameExchange(Client.Character, TExchange));
                    Client.SetExchange(TExchange);

                    Client.Send(new ExchangeCreateMessage((ExchangeTypeEnum)ExchangeType, TC.ActorId.ToString()));
                    Client.Send(new ExchangeTaxItemListMessage(TC.SerializeAsItemList()));

                    break;
            }
        }

        public static void ProcessExchangeAcceptRequest(WorldClient Client)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.BASIC_REQUEST))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!(Client.GetBaseRequest() is ExchangeRequest))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            if (Client == Client.GetBaseRequest().Requester)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.GetBaseRequest().Accept())
            {
                Client.EndGameAction(GameActionTypeEnum.BASIC_REQUEST);

                var Trader = Client.GetBaseRequest().Requester;

                Client.SetBaseRequest(null);
                Trader.SetBaseRequest(null);

                var Exchange = new PlayerExchange(Client, Trader);
                var ExchangeAction = new GameExchange(Client.Character, Exchange);

                Client.SetExchange(Exchange);
                Trader.SetExchange(Exchange);

                Client.AddGameAction(ExchangeAction);
                Trader.AddGameAction(ExchangeAction);

                var Message = new ExchangeCreateMessage(ExchangeTypeEnum.EXCHANGE_PLAYER);

                Client.Send(Message);
                Trader.Send(Message);

                return;
            }

            Client.Send(new BasicNoOperationMessage());
        }

        public static void ProcessExchangeLeaveMessage(WorldClient Client)
        {
            if (Client.IsGameAction(GameActionTypeEnum.EXCHANGE))
            {
                if (Client.GetExchange().CloseExchange())
                {
                    Client.AbortGameAction(GameActionTypeEnum.EXCHANGE);
                    return;
                }
            }
            else if (Client.Character.isInBank)
            {
                Client.Character.isAaway = false;
                Client.Character.isInBank = false;
                Client.Send(new ExchangeLeaveMessage());
            }
            else if (Client.IsGameAction(GameActionTypeEnum.BASIC_REQUEST))
            {
                if (!(Client.GetBaseRequest() is ExchangeRequest))
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }

                if (Client.GetBaseRequest().Declin())
                {
                    Client.EndGameAction(GameActionTypeEnum.BASIC_REQUEST);
                    return;
                }
            }
            else if (Client.IsGameAction(GameActionTypeEnum.CELL_ACTION))
            {
                Client.EndGameAction(GameActionTypeEnum.CELL_ACTION);
                Client.Send(new ExchangeLeaveMessage());
                return;
            }
            else if (Client.miniActions.Count > 0)
            {
                Client.Send(new ExchangeLeaveMessage());
            }
            else
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }


            Client.SetExchange(null);
            Client.SetBaseRequest(null);
            Client.Send(new BasicNoOperationMessage());
        }

        public static void ProcessExchangeBuyRequest(WorldClient Client, string Packet)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.EXCHANGE))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Data = Packet.Split('|');

            if (Data.Length != 2)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            uint TemplateId = 0;
            ushort Quantity = 1;

            if (!uint.TryParse(Data[0].Substring(2), out TemplateId))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            ushort.TryParse(Data[1], out Quantity);

            if (Client.GetExchange().BuyItem(Client, TemplateId, Quantity))
            {
                Client.Send(new ExchangeBuySuccessMessage());
            }
            else if (!(Client.GetExchange() is ShopNpcExchange))
                Client.Send(new ExchangeBuyFailMessage());
        }

        public static void ProcessExchangeSellRequest(WorldClient Client, string Packet)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.EXCHANGE))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Data = Packet.Split('|');
            long ItemGuid = 0;
            ushort Quantity = 0;

            if (!long.TryParse(Data[0].Substring(2), out ItemGuid))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!ushort.TryParse(Data[1], out Quantity))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            var Item = Client.Character.InventoryCache.GetItem(ItemGuid);

            if (Item == null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (!Client.GetExchange().SellItem(Client, Item, Quantity))
                Client.Send(new ObjectSellFailMessage());
        }

        public static void ProcessExchangeValidateRequest(WorldClient Client)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.EXCHANGE))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            Client.GetExchange().Validate(Client);
        }

        public static void ProcessExchangeMoveGoldRequest(WorldClient Client, string Packet)
        {
            if (Client.IsGameAction(GameActionTypeEnum.EXCHANGE))
            {
                long Kamas = 0;

                if (!long.TryParse(Packet.Substring(3), out Kamas))
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }

                Client.GetExchange().MoveKamas(Client, Math.Abs(Kamas));
            }
            else if (Client.Character.isInBank)
            {
                long Kamas = 0;

                if (!long.TryParse(Packet.Substring(3), out Kamas))
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }
                if (Kamas == 0)
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }
                if (Kamas > 0)
                {
                    if (Client.GetCharacter().Kamas < Kamas)
                    {
                        Kamas = Client.GetCharacter().Kamas;
                    }
                    Client.Account.Data.BankKamas += Kamas;
                    Client.GetCharacter().InventoryCache.SubstractKamas(Kamas);
                    Client.Send(new AccountStatsMessage(Client.GetCharacter()));
                    Client.Send(new BankUpdateMessage("G" + Client.Account.Data.BankKamas));
                }
                else
                {
                    Kamas = -Kamas;
                    if (Client.Account.Data.BankKamas < Kamas)
                    {
                        Kamas = Client.Account.Data.BankKamas;
                    }
                    Client.Account.Data.BankKamas -= Kamas;
                    Client.GetCharacter().Kamas += Kamas;
                    Client.Send(new AccountStatsMessage(Client.GetCharacter()));
                    Client.Send(new BankUpdateMessage("G" + Client.Account.Data.BankKamas));
                }
                Client.Account.Data.Save();
            }
            else
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
        }

        public static void ProcessExchangeMoveObjectRequest(WorldClient Client, string Packet)
        {
            if (Client.IsGameAction(GameActionTypeEnum.EXCHANGE))
            {
                var Data = Packet.Split('|');

                var Add = Data[0].Substring(3)[0] == '+';

                long ItemGuid = 0;

                if (!long.TryParse(Data[0].Substring(4), out ItemGuid))
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }

                ushort Quantity = 0;

                if (!ushort.TryParse(Data[1], out Quantity))
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }

                var Item = Client.Character.InventoryCache.GetItem(ItemGuid);

                if (Client.GetExchange().ExchangeType == (int)ExchangeTypeEnum.EXCHANGE_MOUNT && !Add)
                {
                    Item = Client.Character.Mount.Items.Find(x => x.ID == ItemGuid);
                }

                if (Client.GetExchange().ExchangeType == (int)ExchangeTypeEnum.EXCHANGE_TAXCOLLECTOR && !Add)
                {
                    if (!(Client.GetExchange() as TaxCollectorExchange).Npc.Items.TryGetValue(ItemGuid, out Item))
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }
                }

                if (Client.GetExchange() is MarketSellExchange && !Add)
                {
                    (Client.GetExchange() as MarketSellExchange).ItemID = ItemGuid;
                }

                if (Client.GetExchange() is MarketSellExchange && Add)
                {
                    long Price = 0;
                    if (!long.TryParse(Data[2], out Price))
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }
                    if (Price <= 0)
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }
                    (Client.GetExchange() as MarketSellExchange).Price = Price;

                }

                if (Item == null && !(Client.GetExchange() is MarketSellExchange && !Add))
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }

                if (!(Client.GetExchange() is MarketSellExchange && !Add) && Item.Slot != ItemSlotEnum.SLOT_INVENTAIRE)
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }

                Client.GetExchange().MoveItem(Client, Item, Quantity, Add);
            }
            else if (Client.Character.isInBank)
            {
                int guid = 0;
                int qua = 0;
                try
                {
                    guid = int.Parse(Regex.Split(Packet.Substring(4), ("\\|"))[0]);
                    qua = int.Parse(Regex.Split(Packet.Substring(4), ("\\|"))[1]);
                }
                catch (Exception e)
                {
                }
                ;
                if (guid == 0 || qua <= 0 || 3 > Packet.Length)
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }
                switch (Packet[3])
                {
                    case '+':
                        BankHelper.addInBank(Client.Character, guid, qua);
                        break;
                    case '-':
                        BankHelper.removeFromBank(Client.Character, guid, qua);
                        break;
                }
            }
            else
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
        }

        private static void ProcessExchangeMountParkRequest(WorldClient Client, string Packet)
        {
            MountPark MP = Client.Character.inMountPark;
            if (MP != null)
            {
                char c = Packet[2];
                Packet = Packet.Substring(3);
                int guid = -1;
                try
                {
                    guid = int.Parse(Packet);
                }
                catch (Exception e)
                {
                };
                switch (c)
                {
                    case 'C'://Parcho => Etable (Stocker)
                        if (guid == -1 || !Client.Character.InventoryCache.hasItemGuid(guid))
                        {
                            return;
                        }
                        InventoryItemModel obj = Client.Character.InventoryCache.GetItem(guid);

                        //on prend la DD demandée
                        int DDid = -1;
                        Mount DD = null;
                        if (obj.GetStats().HasEffect(EffectEnum.MountOwner))
                        {
                            DDid = obj.GetStats().GetEffect(EffectEnum.MountOwner).Items;
                            DD = MountTable.getMount(DDid);
                        }
                        //FIXME mettre return au if pour ne pas créer des nouvelles dindes
                        if (DD == null)
                        {
                            int color = StaticMountTable.getMountColorByParchoTemplate(obj.TemplateID);
                            if (color < 1)
                            {
                                return;
                            }
                            DD = new Mount(color);
                        }
                        DD.Intialize();

                        //On enleve l'Item du Monde et du Perso
                        Client.Character.InventoryCache.remove(guid);
                        InventoryItemTable.removeItem(guid);
                        //on ajoute la dinde a l'étable
                        Client.Account.Data.Mounts.Add(DD.ID, DD);
                        Client.Account.Data.Save();

                        //On envoie les Packet
                        Client.Send(new ObjectRemoveMessage(obj.ID));
                        Client.Send(new ExchangeEndMessage('+', DD.parse()));
                        break;

                    case 'c'://Etable => Parcho(Echanger)
                        Mount DD1 = MountTable.getMount(guid);
                        //S'il n'a pas la dinde
                        if (!Client.Account.Data.Mounts.ContainsKey(DD1.ID) || DD1 == null)
                        {
                            return;
                        }
                        //on retire la dinde de l'étable
                        Client.Account.Data.Mounts.Remove(DD1.ID);

                        GenericStats Stat = new GenericStats();
                        Stat.AddItem(EffectEnum.MountOwner, DD1.ID);
                        Stat.AddSpecialEffect(EffectEnum.MountOwnerName, Client.Character.Name);
                        Stat.AddSpecialEffect(EffectEnum.MountName, DD1.Name);

                        var item = InventoryItemTable.TryCreateItem(StaticMountTable.getMountScroll(DD1.get_color()).ID, Client.Character, 1, -1, Stat.ToItemStats());

                        Client.Send(new ExchangeEndMessage('-', DD1.ID + ""));
                        Stat = null;
                        break;

                    case 'g'://Equiper
                        Mount DD3 = MountTable.getMount(guid);
                        //S'il n'a pas la dinde
                        if (DD3 == null || !Client.Account.Data.Mounts.ContainsKey(DD3.ID) || Client.Character.Mount != null)
                        {
                            return;
                        }
                        DD3.Intialize();
                        Client.Account.Data.Mounts.Remove(DD3.ID);
                        Client.Account.Data.Save();
                        Client.Character.Mount = DD3;

                        //Packets
                        Client.Send(new CharacterRideEventMessage("+", DD3));
                        Client.Send(new ExchangeEndMessage('-', DD3.ID + ""));
                        Client.Send(new CharacterMountXpGive(Client.Character.MountXPGive));
                        break;
                    case 'p'://Equipé => Stocker
                        //Si c'est la dinde équipé
                        if (Client.Character.Mount != null ? Client.Character.Mount.ID == guid : false)
                        {
                            //Si le perso est sur la monture on le fait descendre
                            if (Client.Character.isOnMount())
                            {
                                Client.Character.toogleOnMount();
                            }
                            //Si ca n'a pas réussie, on s'arrete lÃ  (Items dans le sac ?)
                            if (Client.Character.isOnMount())
                            {
                                return;
                            }

                            Mount DD2 = Client.Character.Mount;
                            DD2.Intialize();
                            Client.Account.Data.Mounts.Add(DD2.ID, DD2);
                            Client.Account.Data.Save();
                            Client.Character.Mount = null;

                            //Packets
                            Client.Send(new ExchangeEndMessage('+', DD2.parse()));
                            Client.Send(new CharacterRideEventMessage("-", null));
                            Client.Send(new CharacterMountXpGive(Client.Character.MountXPGive));
                        }
                        else//Sinon...
                        {
                        }
                        break;

                }
            }
            else
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
        }


    }
}
