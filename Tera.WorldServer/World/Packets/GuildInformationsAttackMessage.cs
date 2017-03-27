using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GuildInformationsAttackMessage : PacketBase
    {
        public String Content;
        public bool defense;

        public GuildInformationsAttackMessage(String c,bool defense)
        {
            Content = c;
            this.defense = defense;
        }

        public override string Compile()
        {
            StringBuilder sb = new StringBuilder("gIT");

            if (!defense)
                sb.Append("p");
            else
                sb.Append("P");
                
            sb.Append(Content);

            return sb.ToString();
        }
    }
}
