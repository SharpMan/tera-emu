using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class AccountQueuPacket : PacketBase
    {
        private int position, totalAbo, totalNonAbo, queueID;
        private String subscribe;

        public AccountQueuPacket(int position, int totalAbo, int totalNonAbo, String subscribe,int queueID)
        {
            this.position = position;
            this.totalAbo = totalAbo;
            this.totalNonAbo = totalNonAbo;
            this.subscribe = subscribe;
            this.queueID = queueID;
        }

        public override string Compile()
        {
            return "Af" + position + "|" + totalAbo + "|" + totalNonAbo + "|" + subscribe + "|" + queueID;
        }
    }
}
