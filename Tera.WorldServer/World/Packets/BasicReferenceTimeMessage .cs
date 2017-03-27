using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class BasicReferenceTimeMessage : PacketBase
    {
        public static DateTime UNIX_REFERENCE = new DateTime(1970, 1, 1);

        public override string Compile()
        {
            return "BT" + Math.Round(DateTime.Now.Subtract(UNIX_REFERENCE).TotalMilliseconds);
        }
    }
}
