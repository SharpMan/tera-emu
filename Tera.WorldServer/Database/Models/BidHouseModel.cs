using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Utils;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.BH;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Database.Models
{
    public class BidHouse
    {
        //FIX Cache All Items
        public int HdvID { get; set; }
        public short MapID { get; set; }
        public float SellTaxe { get; set; }
        public byte Count { get; set; }
        public short sellTime { get; set; }
        public short countItem { get; set; }
        public short levelMax { get; set; }
        public String CategoriesString { get; set; }
        public Dictionary<int, Categorie> Categories = new Dictionary<int, Categorie>();
        public Dictionary<int, Couple<int, int>> Path = new Dictionary<int, Couple<int, int>>();

        public void Initialize()
        {
            int iditem;
            foreach (String a in CategoriesString.Split(','))
            {
                if (!int.TryParse(a, out iditem))
                    continue;
                Categories.Add(iditem, new Categorie(iditem));
            }
        }

        public void addObject(BidHouseItem item)
        {
            if (item.Item == null)
                return;
            item.MapID = MapID;
            int objectType = item.Item.Template.Type;
            int templateID = item.Item.TemplateID;
            if (!Categories.ContainsKey(objectType))
            {
                Logger.Error("Bidhoute " + HdvID + " !Contains Cat " + objectType);
                return;
            }
            Categories[objectType].addModel(item);
            Path.Add(item.LineID, new Couple<int, int>(objectType, templateID));
            BidHouseTable.addBidHouseItem(item.Owner, HdvID, item);
        }


        public List<BidHouseItem> getAllEntry()
        {
            List<BidHouseItem> toReturn = new List<BidHouseItem>();
            foreach (Categorie curCat in Categories.Values)
            {
                toReturn.AddRange(curCat.getAllEntry());
            }

            return toReturn;
        }

        public String SerializeStringObjectType(int objectType)
        {
            return Categories[objectType].ItemsToString();
        }

        public String SerializeStringItemID(int templateID)
        {
            int type = ItemTemplateTable.GetTemplate(templateID).Type;
            return Categories[type].Items[templateID].SerializeLinePerObejct();
        }


        public Boolean DestroyObject(BidHouseItem BHI)
        {
            var item = BHI.Item;
            if (item == null)
                return false;
            Boolean canDelete = Categories[item.Template.Type].removeModel(BHI);
            if (canDelete)
            {
                Path.Remove(BHI.LineID);
                BidHouseTable.BHITEMS[BHI.Owner][BidHouseTable.Cache[MapID].HdvID].Remove(BHI);
            }
            return canDelete;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Boolean BuyObject(int LineID, int Count, long Price, Player Character)
        {
            Boolean possible = true;
            try
            {
                if (Character.Kamas < Price)
                    return false;
                var Line = getLine(LineID);
                var ObjectToBuy = Line.Have(Count, Price);
                Character.InventoryCache.SubstractKamas(Price);
                if (ObjectToBuy.Owner != -1)
                {
                    var OwnerClient = Network.WorldServer.Clients.Find(x => x.Account != null && x.Account.ID == ObjectToBuy.Owner);
                    if (OwnerClient != null)
                    {
                        OwnerClient.Account.Data.BankKamas += ObjectToBuy.Price;
                        OwnerClient.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 65, new String[] { Price.ToString(), ObjectToBuy.Item.TemplateID.ToString(), 1 + "" }));
                    }
                    else
                    {
                        AccountDataTable.UpdateKamas(ObjectToBuy.Price, ObjectToBuy.Owner);
                        //TODO Cache vous IM MSG
                    }
                }
                Character.Send(new AccountStatsMessage(Character));
                var OldItem = ObjectToBuy.Item;
                Character.InventoryCache.Add(OldItem);

                OldItem.Template.newSold(ObjectToBuy.getQuantity(true), Price);
                DestroyObject(ObjectToBuy);
                BidHouseTable.Delete(ObjectToBuy.Item.ID);
                if (ObjectToBuy.Owner == -1)
                {
                    InventoryItemTable.Add(OldItem);
                }
                ObjectToBuy = null;
            }
            catch (Exception e)
            {
                possible = false;
            }
            return possible;
        }

        public Line getLine(int lineID)
        {
            try
            {
                int type = Path[lineID].first;
                int templateID = Path[lineID].second;
                return Categories[type].Items[templateID].Lines[lineID];
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public String TaxRate()
        {
            return SellTaxe.ToString("N0"); //N2 .ToString("###,###", new NumberFormatInfo() { NumberGroupSeparator = "." });
        }


    }
}
