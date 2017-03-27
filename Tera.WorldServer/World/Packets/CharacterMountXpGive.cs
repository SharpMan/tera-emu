using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterMountXpGive : PacketBase
    {
        public int CharacterMountXpGiv;

        public CharacterMountXpGive(int c)
        {
            this.CharacterMountXpGiv = c;
        }

        public override string Compile()
        {
            return "Rx" + CharacterMountXpGiv;
        }
    }
}
