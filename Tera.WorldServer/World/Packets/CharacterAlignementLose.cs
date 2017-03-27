using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterAlignementLose : PacketBase
    {
        public int lose;

        public CharacterAlignementLose(int a)
        {
            this.lose = a;
        }

        public override string Compile()
        {
            return "GIP" + lose;
        }

    }
}
