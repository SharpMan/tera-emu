using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;

namespace Tera.Libs.Packets
{
    public class KickPlayerMessage : TeraPacket
    {
        public KickPlayerMessage(string account) : base(PacketHeaderEnum.KickPlayerMessage)
        {
            Writer.Write(account);
        }
    }
}
