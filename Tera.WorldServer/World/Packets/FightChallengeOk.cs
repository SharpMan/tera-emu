using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Challenges;

namespace Tera.WorldServer.World.Packets
{
    public sealed class FightChallengeOk : PacketBase
    {
        public Challenge Challenge;

        public FightChallengeOk(Challenge c)
        {
            this.Challenge = c;
        }

        public override string Compile()
        {
            return "GdKK" + Challenge.Id;
        }
    }
}
