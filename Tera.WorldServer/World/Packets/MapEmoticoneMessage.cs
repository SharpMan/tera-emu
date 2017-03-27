using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public class MapEmoticoneMessage : PacketBase
    {
        public long Guid;
        public int ID;

        public MapEmoticoneMessage(long Guid, int Id)
        {
            this.Guid = Guid;
            this.ID = Id;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("cS");

            Packet.Append(Guid);
            Packet.Append("|");
            Packet.Append(ID);

            return Packet.ToString();
        }
    }
}
