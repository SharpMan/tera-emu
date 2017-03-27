using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterSeeFriendConnection : PacketBase
    {
        public bool see;

        public CharacterSeeFriendConnection(bool see)
        {
            this.see = see;
        }

        public override string Compile()
        {
            return "FO" + (see ? "+" : "-");
        }
    }
}
