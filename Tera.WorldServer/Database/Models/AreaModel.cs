using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.Database.Models
{
    public class Area
    {
        public int ID;
        public AreaSuper superArea;
        public String Name;
        public List<AreaSub> subAreas = new List<AreaSub>();

        public static int Bontas = 0;
        public static int Brakmars = 0;
        public long Prisme = 0;
        public int Alignement;

        public int val;

        public void Intialize()
        {
            superArea = AreaTable.GetSuperArea(val);
            if (superArea == null)
            {
                superArea = new AreaSuper(val);
                AreaTable.Add(superArea);
            }
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
