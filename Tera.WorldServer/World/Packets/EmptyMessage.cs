using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class EmptyMessage : PacketBase
    {
        public string Content;

        public EmptyMessage(string a)
        {
            Content = a;
        }

        public override string Compile()
        {
            return Content;
        }

    }
}
