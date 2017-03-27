using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.Libs.Enumerations;

namespace Tera.Libs.Packets
{
    public class HelloKeyMessagePacket : TeraPacket
    {
        public HelloKeyMessagePacket(string key): base(PacketHeaderEnum.HelloKeyMessage)
        {
            Writer.Write(key);
        }
    }
}
