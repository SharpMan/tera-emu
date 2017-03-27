using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Helper;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.Database.Models
{
    public class Speaking
    {
        public int LastEatYear, LastEatTime, LastEatHour, Humour, Masque, Type;
        public int YearInter, DateInter, HourInter, YearReceive, DateReceived, HourReceived, Associated, TemplateReal;
        public long EXP, ID, LivingItem, LinkedItem;
        public String Stats;
        public bool Intialized = false;


        public Speaking()
        {

        }

        public Speaking(long ID, int LastEatYear, int LastEatTime, int LastEatHour, int Humour, int Masque, int Type,long LinkedItem, long XP, int YearInter, int DateInter, int HourInter, int YearReceive, int DateReceived, int HourReceived,int Associated, int TemplateReal, long livingitem, String stats)
        {
            this.ID = ID;
            this.LastEatYear = LastEatYear;
            this.LastEatTime = LastEatTime;
            this.LastEatHour = LastEatHour;
            this.Humour = Humour;
            this.Masque = Masque;
            this.Type = Type;
            this.LinkedItem = LinkedItem;
            this.EXP = XP;
            this.YearInter = YearInter;
            this.DateInter = DateInter;
            this.HourInter = HourInter;
            this.YearReceive = YearReceive;
            this.DateReceived = DateReceived;
            this.HourReceived = HourReceived;
            this.Associated = Associated;
            this.TemplateReal = TemplateReal;
            this.LivingItem = livingitem;
            this.Stats = stats;
        }

         public void Intialize()
        {
            if (Intialized)
                return;
            if (this.LivingItem != 0)
                InventoryItemTable.Load(this.LivingItem);
            Intialized = true;
        }


        public String convertirAString()
        {
            /*String str = "328#" + LastEatYear.ToString("X") + "#" + LastEatTime.ToString("X") + "#"
            + LastEatHour.ToString("X") + "," + "3cb#0#0#" + Humour.ToString("X") + "," + "3cc#0#0#"
            + Masque.ToString("X") + "," + "3cd#0#0#" + Type.ToString("X") + "," + "3ca#0#0#"
            + TemplateReal.ToString("X") + "," + "3ce#0#0#" + EXP.ToString("X") + "," + "3d7#"
            + YearInter.ToString("X") + "#" + DateInter.ToString("X") + "#" + HourInter.ToString("X")
            + "," + "325#" + YearReceive.ToString("X") + "#" + DateReceived.ToString("X") + "#"
            + HourReceived.ToString("X");*/
            StringBuilder sb = new StringBuilder("328#").Append(LastEatYear.ToString("X")).Append("#").Append(LastEatTime.ToString("X")).Append("#");
            sb.Append(LastEatHour.ToString("X")).Append(',').Append("3cb#0#0#").Append(Humour.ToString("X")).Append(",").Append("3cc#0#0#");
            sb.Append(Masque.ToString("X")).Append(",").Append("3cd#0#0#").Append(Type.ToString("X")).Append(",").Append("3ca#0#0#");
            sb.Append(TemplateReal.ToString("X")).Append(",").Append("3ce#0#0#").Append(EXP.ToString("X")).Append(",").Append("3d7#");
            sb.Append(YearInter.ToString("X")).Append("#").Append(DateInter.ToString("X")).Append("#").Append(HourInter.ToString("X"));
            sb.Append(",").Append("325#").Append(YearReceive.ToString("X")).Append("#").Append(DateReceived.ToString("X")).Append("#").Append(HourReceived.ToString("X"));
            return sb.ToString();
        }

    }
}
