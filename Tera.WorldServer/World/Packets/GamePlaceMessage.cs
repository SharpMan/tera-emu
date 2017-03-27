using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GamePlaceMessage : PacketBase
    {
        public string Places;
        public long TeamId;

        public GamePlaceMessage(string Places, long TeamId)
        {
            this.Places = Places;
            this.TeamId = TeamId;
        }

        public override string Compile()
        {
            return "GP" + this.Places + '|' + this.TeamId;
        }
    }
}
