using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Challenges;

namespace Tera.WorldServer.World.Packets
{
    public sealed class FightChallengeFail : PacketBase
    {
        public Challenge Challenge;

        public FightChallengeFail(Challenge c)
        {
            this.Challenge = c;
        }

        public override string Compile()
        {
            return "GdOO" + Challenge.Id;
        }
    }
}
