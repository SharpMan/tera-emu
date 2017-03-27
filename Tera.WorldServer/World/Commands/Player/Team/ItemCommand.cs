using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Exchanges;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Player.Team
{
    public sealed class ItemCommand : PlayerCommand
    {
        public override string Prefix
        {
            get
            {
                return "item";
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
                return "Joindre la boutique";
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
            if (Settings.AppSettings.GetBoolElement("NpcShop.Use"))
            {
                if (parameters.Lenght < 1)
                {
                    StringBuilder sb = new StringBuilder("Listes des commandes d'achats d'items (" + Settings.AppSettings.GetIntElement("NpcShop.Cost") + " Points)\n");
                    sb.Append("<b>.item coiffe</b> ouvre la boutique des chapeau\n");
                    sb.Append("<b>.item cape</b> ouvre la boutique des capes\n");
                    sb.Append("<b>.item amulette</b> ouvre la boutique des amulettes\n");
                    sb.Append("<b>.item arc</b> ouvre la boutique des arc\n");
                    sb.Append("<b>.item baguette</b> ouvre la boutique des baguettes\n");
                    sb.Append("<b>.item baton</b> ouvre la boutique des batons\n");
                    sb.Append("<b>.item dague</b> ouvre la boutique des dagues\n");
                    sb.Append("<b>.item epee</b> ouvre la boutique des épes\n");
                    sb.Append("<b>.item marteau</b> ouvre la boutique des marteaux\n");
                    sb.Append("<b>.item pelle</b> ouvre la boutique des pelles\n");
                    sb.Append("<b>.item anneau</b> ouvre la boutique des anneaux\n");
                    sb.Append("<b>.item ceinture</b> ouvre la boutique des ceintures\n");
                    sb.Append("<b>.item botte</b> ouvre la boutique des bottes\n");
                    sb.Append("<b>.item famillier</b> ouvre la boutique des familliers\n");
                    sb.Append("<b>.item hache</b> ouvre la boutique des haches\n");
                    sb.Append("<b>.item dofus</b> ouvre la boutique des dofus\n");
                    sb.Append("<b>.item d2</b> ouvre la boutique des items 2.0\n");
                    sb.Append("<b>.fm coiffe/cape pa/po/pm</b>  Fm votre item\n");
                    Client.Send(new ChatGameMessage(sb.ToString(), "0281A1"));
                    return;
                }
                ItemTypeEnum Type;
                switch (parameters.GetParameter(0))
                {
                    case "coiffe":
                        Type = ItemTypeEnum.ITEM_TYPE_COIFFE;
                        break;
                    case "cape":
                        Type = ItemTypeEnum.ITEM_TYPE_CAPE;
                        break;
                    case "amulette":
                        Type = ItemTypeEnum.ITEM_TYPE_AMULETTE;
                        break;
                    case "arc":
                        Type = ItemTypeEnum.ITEM_TYPE_ARC;
                        break;
                    case "baguette":
                        Type = ItemTypeEnum.ITEM_TYPE_BAGUETTE;
                        break;
                    case "baton":
                        Type = ItemTypeEnum.ITEM_TYPE_BATON;
                        break;
                    case "dague":
                        Type = ItemTypeEnum.ITEM_TYPE_DAGUES;
                        break;
                    case "epee":
                        Type = ItemTypeEnum.ITEM_TYPE_EPEE;
                        break;
                    case "marteau":
                        Type = ItemTypeEnum.ITEM_TYPE_MARTEAU;
                        break;
                    case "pelle":
                        Type = ItemTypeEnum.ITEM_TYPE_PELLE;
                        break;
                    case "anneau":
                        Type = ItemTypeEnum.ITEM_TYPE_ANNEAU;
                        break;
                    case "ceinture":
                        Type = ItemTypeEnum.ITEM_TYPE_CEINTURE;
                        break;
                    case "botte":
                        Type = ItemTypeEnum.ITEM_TYPE_BOTTES;
                        break;
                    case "famillier":
                        Type = ItemTypeEnum.ITEM_TYPE_FAMILIER;
                        break;
                    case "hache":
                        Type = ItemTypeEnum.ITEM_TYPE_HACHE;
                        break;
                    case "dofus":
                        Type = ItemTypeEnum.ITEM_TYPE_DOFUS;
                        break;
                    case "d2":
                        Type = ItemTypeEnum.ITEM_TYPE_DONS;
                        break;
                    default:
                        Client.Send(new ChatGameMessage("Categorie non valide tapez .boutique pour plus d'informations", "0281A1"));
                        return;
                }
                if (!Client.CanGameAction(GameActionTypeEnum.EXCHANGE))
                {
                    Client.Send(new BasicNoOperationMessage());
                    return;
                }
                var Npc = ShopNpcTable.Cache[Type];
                var Exchange = new ShopNpcExchange(Client, Npc);
                Client.AddGameAction(new GameExchange(Client.Character, Exchange));
                Client.SetExchange(Exchange);
                Client.Send(new ExchangeCreateMessage(ExchangeTypeEnum.EXCHANGE_SHOP, Client.Character.ActorId.ToString()));
                Client.Send(new ExchangeShopItemListMessage(Npc));
                return;
            }
            else
            {
                Client.Send(new ChatGameMessage("Commande desactivée", "046380"));
            }
            

        }
    }
}
