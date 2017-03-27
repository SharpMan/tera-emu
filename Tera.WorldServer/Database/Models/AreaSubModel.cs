using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.Database.Models
{
    public class AreaSub
    {
        public int ID;
        public Area area;
        public String Name;
        public int Alignement;
        public List<Map> Maps = new List<Map>();

        public Boolean CanConquest;
        public long Prisme;
        public static int Bontas = 0;
        public static int Brakmars = 0;

        public int areaID; 

        public void Intialize()
        {
            area = AreaTable.Get(areaID);
        }

        public void onPrismLoaded()
        {
            if (PrismeTable.getPrism(Prisme) == null)
            {
                Alignement = 0;
                Prisme = 0;
            }
            if (Alignement == 1)
            {
                Bontas += 1;
            }
            else if (Alignement == 2)
            {
                Brakmars += 1;
            }
        }

        public void setAlignement(int align)
        {
            if ((Alignement == 1) && (align == -1))
            {
                Bontas -= 1;
            }
            else if ((Alignement == 2) && (align == -1))
            {
                Brakmars -= 1;
            }
            else if ((Alignement == -1) && (align == 1))
            {
                Bontas += 1;
            }
            else if ((Alignement == -1) && (align == 2))
            {
                Brakmars += 1;
            }
            Alignement = align;
        }

    }
}
