using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class PlayerAlignementUpdateMessage : PacketBase
    {
        public int Alignement;

        public PlayerAlignementUpdateMessage(int align)
        {
            this.Alignement = align;
        }

        public override string Compile()
        {
            return "ZC" + Alignement;
        }

    }
}
