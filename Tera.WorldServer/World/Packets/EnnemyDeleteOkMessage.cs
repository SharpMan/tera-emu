using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class EnnemyDeleteOkMessage : PacketBase
    {
        public string Content;

        public EnnemyDeleteOkMessage(String str)
        {
            this.Content = str;
        }

        public override string Compile()
        {
            return "iD" + Content;
        }
    }
}
