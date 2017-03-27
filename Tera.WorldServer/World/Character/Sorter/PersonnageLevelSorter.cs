using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Character.Sorter
{
    public class PersonnageLevelSorter : IComparer<Player>
    {
        private Boolean croisant;

        public PersonnageLevelSorter(Boolean lth)
        {
            croisant = lth;
        }

        public int Compare(Player x, Player y)
        {
            return (y.Level- x.Level) * (croisant ? 1 : -1);
        }
    }
}
