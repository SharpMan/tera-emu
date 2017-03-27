using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Utils
{
    public class Couple<L, R>
    {
        public L first;
        public R second;

        public Couple(L s, R i)
        {
            this.first = s;
            this.second = i;
        }
    }
}
