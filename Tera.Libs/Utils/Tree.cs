using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Utils
{
    [Serializable]
    public class Tree<L, R, S>
    {
        public L first;
        public R second;
        public S tree;

        public Tree(L s, R i, S o)
        {
            this.first = s;
            this.second = i;
            this.tree = o;
        }
    }
}
