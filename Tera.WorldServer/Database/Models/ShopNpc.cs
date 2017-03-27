using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Utils;

namespace Tera.WorldServer.Database.Models
{
    public class ShopNpc
    {
        private Dictionary<int, ItemTemplateModel> myItems = new Dictionary<int, ItemTemplateModel>();
        private StringBuilder mySerializedItems;
        private bool myInitialized = false;
        public ItemTypeEnum Type;

        public void SerializeAsItemList(StringBuilder SerializedString)
        {
            if (this.mySerializedItems == null)
            {
                if (!this.myInitialized)
                    this.Initialize();
            }

            SerializedString.Append(this.mySerializedItems.ToString());
        }

        public void Initialize(String ItemList)
        {
            if (this.myInitialized)
                return;

            this.mySerializedItems = new StringBuilder();


            var Items = new List<ItemTemplateModel>();
            foreach (String s in ItemList.Split(','))
            {
                int templateId;
                if (!int.TryParse(s, out templateId))
                {
                    continue;
                }
                var Template = ItemTemplateTable.GetTemplate(templateId);
                if (Template != null)
                    Items.Add(Template);
            }
            Items.OrderByDescending(x => x.Level);

            foreach (var Template in Items)
            {
                if (Template != null)
                {
                    if (!this.myItems.ContainsKey(Template.ID))
                    {
                        this.mySerializedItems.Append(Template.ID);
                        this.mySerializedItems.Append(';');
                        this.mySerializedItems.Append(Template.StatsTemplate);
                        this.mySerializedItems.Append('|');

                        this.myItems.Add(Template.ID, Template);
                    }
                }
            }


            this.myInitialized = true;
        }

        private void Initialize()
        {
            if (this.myInitialized)
                return;

            this.mySerializedItems = new StringBuilder();

            var Items = ItemTemplateTable.Cache.Values.Where(x => x.ItemType == Type).ToList();

            if (Type != ItemTypeEnum.ITEM_TYPE_FAMILIER && Type != ItemTypeEnum.ITEM_TYPE_DOFUS)
            {
                Items = Items.Where(x => x.Level > 115 && x.ID < 10678).ToList();
            }

            Items.OrderByDescending(x => x.Level);

            foreach (var Template in Items)
            {
                if (Template != null)
                {
                    if (!this.myItems.ContainsKey(Template.ID))
                    {
                        this.mySerializedItems.Append(Template.ID);
                        this.mySerializedItems.Append(';');
                        this.mySerializedItems.Append(Template.StatsTemplate);
                        this.mySerializedItems.Append('|');

                        this.myItems.Add(Template.ID, Template);
                    }
                }
            }


            this.myInitialized = true;
        }

        public bool HasItemTemplate(int TemplateId)
        {
            return this.myItems.ContainsKey(TemplateId);
        }

        public ItemTemplateModel GetItemTemplate(int TemplateId)
        {
            return this.myItems[TemplateId];
        }

    }
}
