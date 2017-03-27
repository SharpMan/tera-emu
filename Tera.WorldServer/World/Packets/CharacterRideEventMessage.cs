using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterRideEventMessage : PacketBase
    {
        public String sign;
        public Mount mount;

        public CharacterRideEventMessage(String sign, Mount DD)
        {
            this.sign = sign;
            this.mount = DD;
        }

        public override string Compile()
        {
            String packet = "Re" + sign;
            if (sign.Equals("+"))
            {
                packet += mount.parse();
            }
            return packet;
        }
    }
}
