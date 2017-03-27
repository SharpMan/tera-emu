    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Network
{
    public abstract class PacketBase
    {
        public StringBuilder Data = null;
        public abstract string Compile();

        public string Build()
        {
            if (Data == null)
            {
                Data = new StringBuilder(this.Compile());
                Data.Append((char)0x00);
            }

            return Data.ToString();
        }
    }
}
