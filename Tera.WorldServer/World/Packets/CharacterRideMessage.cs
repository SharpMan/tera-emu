using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterRideMessage : PacketBase
    {
        public String message;

        public CharacterRideMessage(String str)
        {
            this.message = str;
        }


        public override string Compile()
        {
            return "Rr" + message;
        }

    }
}
