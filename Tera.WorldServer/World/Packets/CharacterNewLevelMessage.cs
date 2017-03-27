using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterNewLevelMessage : PacketBase
    {
        public int Level;

        public CharacterNewLevelMessage(int Level)
        {
            this.Level = Level;
        }

        public override string Compile()
        {
            return "AN" + this.Level;
        }
    }
}
