using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Utils;

namespace Tera.WorldServer.Database.Tables
{
    public static class ShopNpcTable
    {
        public static Dictionary<ItemTypeEnum, ShopNpc> Cache = new Dictionary<ItemTypeEnum, ShopNpc>();

        public static void Initialize()
        {
            Cache.Add(ItemTypeEnum.ITEM_TYPE_COIFFE, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_COIFFE });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_CAPE, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_CAPE });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_AMULETTE, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_AMULETTE });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_ARC, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_ARC });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_BAGUETTE, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_BAGUETTE });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_BATON, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_BATON });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_DAGUES, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_DAGUES });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_EPEE, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_EPEE });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_MARTEAU, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_MARTEAU });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_PELLE, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_PELLE });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_ANNEAU, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_ANNEAU });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_CEINTURE, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_CEINTURE });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_BOTTES, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_BOTTES });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_FAMILIER, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_FAMILIER });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_HACHE, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_HACHE });
            Cache.Add(ItemTypeEnum.ITEM_TYPE_DOFUS, new ShopNpc() { Type = ItemTypeEnum.ITEM_TYPE_DOFUS });
            //var npc = new ShopNpc();
            //npc.Initialize(Settings.AppSettings.GetStringElement("NpcShop.Item20"));
            //Cache.Add(ItemTypeEnum.ITEM_TYPE_DONS,npc);
            Logger.Info("Loaded @'" + Cache.Count + "'@ Shopping NPC");
        }
    }
}
