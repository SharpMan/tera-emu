using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.World.Packets
{
    public sealed class PartyListMessage : PacketBase
    {
        public Party p;

        public PartyListMessage(Party p)
        {
            this.p = p;
        }
        public override string Compile()
        {
            return "PL" + p.Chief.ActorId;
        }
    }
}
