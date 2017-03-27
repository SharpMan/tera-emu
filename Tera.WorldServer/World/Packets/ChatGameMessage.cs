using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public class ChatGameMessage: PacketBase
    {
        public String Message;
        public String Color;

        public ChatGameMessage(String Mess, String color)
        {
            Message = Mess;
            Color = color;
        }

        public override string Compile()
        {
             var Packet = new StringBuilder("cs<font color='#");

            Packet.Append(this.Color);
            Packet.Append("'>");
            Packet.Append(Message);
            Packet.Append("</font>");

            return Packet.ToString();
            //return "";
        }
    }
}
