using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.BH
{
    public class Line
    {
        public int LineID, TemplateID;
        public List<List<BidHouseItem>> Categories = new List<List<BidHouseItem>>(3);
        public String StringStat { get; set; }

        public Line(int lineID, BidHouseItem BHI)
        {
            this.LineID = lineID;
            this.StringStat = BHI.Item.GetStats().ToItemStats();
            for (int i = 0; i < 3; i++)
            {
                Categories.Add(new List<BidHouseItem>());
            }
            addBidHouseObjectAtLine(BHI);
        }

        public Boolean addBidHouseObjectAtLine(BidHouseItem BHI)
        {
            if ((!CategorieIsBlank()) && (!HasEqualStats(BHI)))
                return false;
            BHI.LineID = this.LineID;
            byte index = (byte)(BHI.QuantityType(false) - 1);
            Categories[index].Add(BHI);
            Tri(index);
            return true;
        }

        public Boolean RemoveBidHouseItemAtLine(BidHouseItem BHI)
        {
            byte cat = (byte)(BHI.getQuantity(false) - 1);
            Boolean canDelete = Categories[cat].Remove(BHI);
            Tri(cat);
            return canDelete;
        }

        public void Tri(byte cat)
        {
            Categories[cat].Sort();
        }


        public Boolean HasEqualStats(BidHouseItem BHI)
        {
            return (StringStat.Equals(BHI.Item.GetStats().ToItemStats())) && (BHI.Item.Template.Type != 85);
        }

        public String SerializePriceList()
        {
            long[] nPrice = getLos3PricePerLine();
            String str = LineID + ";" + StringStat + ";" + (nPrice[0] == 0 ? "" : nPrice[0].ToString()) + ";" + (nPrice[1] == 0 ? "" : nPrice[1].ToString()) + ";" + (nPrice[2] == 0 ? "" : nPrice[2].ToString());
            return str;
        }

        public long[] getLos3PricePerLine()
        {
            long[] str = new long[3];
            for (int i = 0; i < Categories.Count; i++)
            {
                try
                {
                    str[i] = (Categories[i])[0].Price;
                }
                catch (Exception e)
                {
                    str[i] = 0;
                }
            }
            return str;
        }

        public BidHouseItem Have(int cat, long price)
        {
            int index = cat - 1;
            for (int i = 0; i < (Categories[index]).Count; i++)
            {
                if ((Categories[index][i]).Price == price)
                    return Categories[index][i];
            }
            return null;
        }

        public Boolean CategorieIsBlank()
        {
            for (int i = 0; i < Categories.Count; i++)
            {
                try
                {
                    if (Categories[i].Count > 0) //ElementAtOrDefault
                        return false;
                }
                catch (Exception e)
                {
                }
            }
            return true;
        }



        public List<BidHouseItem> getAll()
        {
            int totalSize = Categories[0].Count + Categories[1].Count + Categories[2].Count;
            List<BidHouseItem> toReturn = new List<BidHouseItem>(totalSize);

            for (int qte = 0; qte < Categories.Count; qte++)
            {
                toReturn.AddRange(Categories[qte]);
            }

            return toReturn;
        }
    }
}
