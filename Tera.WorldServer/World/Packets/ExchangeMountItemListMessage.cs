using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeMountItemListMessage : PacketBase
    {
        public Mount Mount;

        public ExchangeMountItemListMessage(Mount Mount)
        {
            this.Mount = Mount;
        }


        public override string Compile()
        {
            var Packet = new StringBuilder("EL");

            this.Mount.SerializeAsItemList(Packet);

            return Packet.ToString();
        }
    }
}
