using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Challenges;

namespace Tera.WorldServer.World.Packets
{
    public sealed class FightShowChallenge : PacketBase
    {
        public Challenge Challenge;

        public FightShowChallenge(Challenge c)
        {
            this.Challenge = c;
        }

        public override string Compile()
        {
            return "Gd" + Challenge.Id + ";" + (Challenge.ShowTarget ? "1" : "0") + ";" + Challenge.TargetId + ";" + Challenge.BasicXpBonus + ";" + Challenge.TeamXpBonus + ";" + Challenge.BasicDropBonus + ";" + Challenge.TeamDropBonus + ";" + Challenge.State;
        }
    }
}
