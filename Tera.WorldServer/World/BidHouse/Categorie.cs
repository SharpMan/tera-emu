using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.BH
{
    public class Categorie
    {
        private int iditem;
        public Dictionary<int, Model> Items = new Dictionary<int, Model>();


        public Categorie(int iditem)
        {
            // TODO: Complete member initialization
            this.iditem = iditem;
        }

        public void addModel(BidHouseItem BHI)
        {
            if (!Items.ContainsKey(BHI.Item.TemplateID))
            {
                Items.Add(BHI.Item.TemplateID, new Model(BHI.Item.TemplateID, BHI));
            }
            else
            {
                Items[BHI.Item.TemplateID].addBidHouseItemToLine(BHI);
            }
        }

        public Boolean removeModel(BidHouseItem BHI)
        {
            Boolean canDelete = false;
            (Items[BHI.Item.TemplateID]).RemoveItem(BHI);
            if (canDelete = Items[BHI.Item.TemplateID].isEmpty())
                Items.Remove(BHI.Item.TemplateID);
            return canDelete;
        }


        internal string ItemsToString()
        {
            StringBuilder str = new StringBuilder();
            foreach (int modelID in Items.Keys)
            {
                if (str.Length > 0)
                    str.Append(";");
                str.Append(modelID);
            }
            return str.ToString();
        }

        public List<BidHouseItem> getAllEntry()
        {
            List<BidHouseItem> toReturn = new List<BidHouseItem>();

            foreach (var curTemp in Items.Values)
            {
                toReturn.AddRange(curTemp.getAllEntry());
            }
            return toReturn;
        }

    }
}
