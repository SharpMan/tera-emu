using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterFriendDeleteMessage : PacketBase
    {
        public string Content;

        public CharacterFriendDeleteMessage(String str)
        {
            this.Content = str;
        }

        public override string Compile()
        {
            return "FD" + Content;
        }

    }
}
