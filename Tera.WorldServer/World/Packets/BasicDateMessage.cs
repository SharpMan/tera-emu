using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class BasicDateMessage : PacketBase
    {
        public override string Compile()
        {
            return "BD" + (DateTime.Now.Year - 1970) + '|' + (DateTime.Now.Month - 1) + '|' + DateTime.Now.Day;
        }
    }
}
