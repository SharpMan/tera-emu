using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.Database.Models
{
    public class BidHouseItem : IComparable<BidHouseItem>
    {
        public int MapID { get; set; }
        public int Quantity { get; set; }
        public int LineID { get; set; }
        public int Owner { get; set; }
        public long Price { get; set; }
        public long ItemID { get; set; }
        public InventoryItemModel Item { get; set; }
        public BidHouse BH { get; set; }

        public void Initialize()
        {
            Item = InventoryItemTable.Load(ItemID);

            if (Item == null)
            {
                BidHouseTable.Delete(ItemID);
                return;
            }

            BH.addObject(this);
            BH = null;


        }

        public int QuantityType(Boolean RealCount)
        {
            if (RealCount)
            {
                return (int)(Math.Pow(10.0D, Quantity) / 10.0D);
            }
            return Quantity;
        }

        public void SerializeAsDisplayEquipment(StringBuilder Packet)
        {
            int count = getQuantity(true);
            Packet.Append(LineID).Append(';').Append(count).Append(';').Append(Item.TemplateID).Append(';').Append(Item.GetStats().ToItemStats()).Append(';').Append(Price).Append(";350");
        }

        public String SerializeAsDisplayEquipmentOnMarket()
        {
            return Item.ID + "|" + getQuantity(true) + "|" + Item.TemplateID + "|" + Item.GetStats().ToItemStats() + "|" + Price + "|350";
        }


        public int getQuantity(Boolean RealQuantity)
        {
            if (RealQuantity)
            {
                return (int)(Math.Pow(10.0D, Quantity) / 10.0D);
            }
            return Quantity;
        }


        public int CompareTo(BidHouseItem BHI)
        {
            long oldPrice = BHI.Price;
            if (Price > oldPrice)
                return -1;
            if (Price == oldPrice)
                return 0;
            if (Price < oldPrice)
                return 1;
            return 0;
        }
    }
}
