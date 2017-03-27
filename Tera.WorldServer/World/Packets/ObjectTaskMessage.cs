using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ObjectTaskMessage : PacketBase
    {
        public int ID = -1;

        public ObjectTaskMessage(int id)
        {
            this.ID = id;
        }

        public override string Compile()
        {
            if (ID > 0)
                return "OT" + ID;
            return "OT";
        }


    }
}
