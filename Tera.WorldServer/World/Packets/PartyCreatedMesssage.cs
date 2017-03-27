using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.World.Packets
{
    public sealed class PartyCreatedMesssage : PacketBase
    {
        public Party Party;

        public PartyCreatedMesssage(Party p)
        {
            this.Party = p;
        }

        public override string Compile()
        {
            return "PCK" + Party.Chief.Name;
        }
    }
}
