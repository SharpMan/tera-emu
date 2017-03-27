using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public class NpcDialogStartMessage : PacketBase
    {
        public int ID;

        public NpcDialogStartMessage(int id)
        {
            this.ID = id;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("DCK");
            Packet.Append(this.ID);
            return Packet.ToString();
        }
    
    }
}
