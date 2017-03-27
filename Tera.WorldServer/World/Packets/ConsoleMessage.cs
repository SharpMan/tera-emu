using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    class ConsoleMessage : PacketBase
    {
        public String Message;
        public ConsoleColorEnum Color;

        public ConsoleMessage(String Mess, ConsoleColorEnum color = ConsoleColorEnum.GREEN)
        {
            Message = Mess;
            Color = color;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("BAT");
            Packet.Append((int)Color);
            Packet.Append(this.Message);

            return Packet.ToString();
        }
    }
}
