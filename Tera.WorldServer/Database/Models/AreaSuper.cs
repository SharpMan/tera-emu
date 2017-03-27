using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.Database.Models
{
    public class AreaSuper
    {
        public int ID;
        private List<Area> Areas = new List<Area>();

        public AreaSuper(int a_id)
        {
            ID = a_id;
        }

        public void addArea(Area A)
        {
            Areas.Add(A);
        }


        public Area getArea(int id)
        {
            return Areas.FirstOrDefault(x => x.ID == id);
        }
    }
}
