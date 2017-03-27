using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.BH
{
    public class Model
    {
        public int TemplateID;
        public Dictionary<long, Line> Lines = new Dictionary<long, Line>();

        public Model(int templateID, BidHouseItem BHI)
        {
            this.TemplateID = templateID;
            addBidHouseItemToLine(BHI);
        }

        public void addBidHouseItemToLine(BidHouseItem BHI)
        {
            foreach (var line in Lines.Values)
            {
                if (line.addBidHouseObjectAtLine(BHI))
                    return;
            }
            int lineID = BidHouseTable.NextLineID();
            Lines.Add(lineID, new Line(lineID, BHI));
        }


        public Boolean RemoveItem(BidHouseItem BHI)
        {
            Boolean canDelete = Lines[BHI.LineID].RemoveBidHouseItemAtLine(BHI);
            if (Lines[BHI.LineID].CategorieIsBlank())
            {
                Lines.Remove(BHI.LineID);
            }
            return canDelete;
        }

        public Boolean isEmpty()
        {
            return Lines.Count == 0;
        }


        public String SerializeLinePerObejct()
        {
            StringBuilder str = new StringBuilder(TemplateID + "|");
            Boolean first = true;
            foreach (var line in Lines.Values)
            {
                if (!first)
                    str.Append("|");
                str.Append(line.SerializePriceList());
                first = false;
            }
            return str.ToString();
        }

        public List<BidHouseItem> getAllEntry()
        {
            List<BidHouseItem> toReturn = new List<BidHouseItem>();

            foreach (var curLine in Lines.Values)
            {
                toReturn.AddRange(curLine.getAll());
            }
            return toReturn;
        }
    }
}
