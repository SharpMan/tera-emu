using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class PartyRemovePlayer : PacketBase
    {
        public long Actor;

        public PartyRemovePlayer(long id)
        {
            this.Actor = id;
        }

        public override string Compile()
        {
            return "PM-" + Actor;
        }
    }
}
