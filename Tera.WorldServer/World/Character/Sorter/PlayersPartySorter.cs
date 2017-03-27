using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Character.Sorter
{
    public class PlayersPartySorter : IComparer<Party>
    {
        private Boolean croisant;

        public PlayersPartySorter(Boolean lth)
        {
            croisant = lth;
        }

        public int Compare(Party x, Party y)
        {
            return (y.GetMoyLevel() - x.GetMoyLevel()) * (croisant ? 1 : -1);
        }
    }
}
