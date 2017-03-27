using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterEnnemyDeleteMessage : PacketBase
    {
       public String Content;

        public CharacterEnnemyDeleteMessage(String str)
        {
            this.Content = str;
        }

        public override string Compile()
        {
            return "FD" + this.Content;
        }
    }
}
