using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public class GameActionEnvironementMessage : PacketBase
    {
        public int type;
        public String Content;

        public GameActionEnvironementMessage(int type,String Content)
        {
            this.type = type;
            this.Content = Content;
        }

        public override string Compile()
        {
            String packet = (new StringBuilder("ECK")).Append(type).ToString();
            if (!Content.Equals(""))
            {
                packet = (new StringBuilder(packet.ToString()).Append("|").Append(Content).ToString());
            }
            return packet;
        }
    }
}
