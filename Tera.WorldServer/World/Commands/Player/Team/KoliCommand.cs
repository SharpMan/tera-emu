using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Exchanges;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Player.Team
{
    public sealed class KoliCommand : PlayerCommand
    {
        public override string Prefix
        {
            get
            {
                return "koli";
            }
        }

        public override int AccessLevel
        {
            get
            {
                return 0;
            }
        }

        public override string Description
        {
            get
            {
                return "Participer au Kolizeum";
            }
        }

        public override bool NeedLoaded
        {
            get
            {
                return true;
            }
        }

        public override void Execute(WorldClient Client, CommandParameters parameters)
        {
            if (parameters.Lenght > 0)
            {
                var packet = parameters.GetParameter(0);
                if (packet.Equals("on"))
                {
                    if (Client.IsGameAction(GameActionTypeEnum.KOLIZEUM))
                    {
                        Client.Send(new ChatGameMessage("Vous êtes déjà inscrit au Kolizeum !", "046380"));
                        return;
                    }
                    else if (Client.IsGameAction(GameActionTypeEnum.FIGHT))
                    {
                        Client.Send(new ChatGameMessage("Vous êtes en combat !", "046380"));
                        return;
                    }
                    Client.AddGameAction(new GameKolizeum(Client.GetCharacter()));
                    Client.Send(new ChatGameMessage("Vous vous êtes inscrit au Kolizeum !", "046380"));
                    return;
                }
                else if (packet.Equals("off"))
                {
                    if (!Client.IsGameAction(GameActionTypeEnum.KOLIZEUM))
                    {
                        Client.Send(new ChatGameMessage("Vous n'êtes pas inscrit au Kolizeum !", "046380"));
                        return;
                    }
                    Client.EndGameAction(GameActionTypeEnum.KOLIZEUM);
                    Client.Send(new ChatGameMessage("Vous avez quitté le Kolizeum !", "046380"));
                    return;
                }
                /*else if (packet.Equals("shop"))
                {
                    if (!Client.CanGameAction(GameActionTypeEnum.EXCHANGE))
                    {
                        Client.Send(new BasicNoOperationMessage());
                        return;
                    }
                    var Npc = ShopNpcTable.Cache[ItemTypeEnum.ITEM_TYPE_TROPHE];
                    var Exchange = new KoliShopExchange(Client, Npc);
                    Client.AddGameAction(new GameExchange(Client.Character, Exchange));
                    Client.SetExchange(Exchange);
                    var CachedP = Client.Character.InventoryCache.getCache().Values.Where(x => x.TemplateID == Settings.AppSettings.GetIntElement("Kolizeum.WinItem")).Count();
                    Client.Send(new ChatGameMessage("Bienvenue dans le marché de Kolizeum vous disposez de "+CachedP+" Kolizetons", "046380"));
                    Client.Send(new ExchangeCreateMessage(ExchangeTypeEnum.EXCHANGE_SHOP, Client.Character.ActorId.ToString()));
                    Client.Send(new ExchangeShopItemListMessage(Npc));
                    return;
                }*/
            }
            Client.Send(new ChatGameMessage("<b>Kolizeum : </b>\n"
                                                + ".koli on/off - s'inscrire / Quitter au kolizeum\n"
                                                + ".kolig on/off - inscris votre groupe / Quitter\n"
                                                + ".koli shop - marché de kolizeum", "046380"));

        }
    }
}
