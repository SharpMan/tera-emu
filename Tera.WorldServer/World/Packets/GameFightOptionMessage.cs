using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameFightOptionMessage : PacketBase
    {
        public char ToggleType;
        public bool Value;
        public long TeamId;

        public GameFightOptionMessage(char ToggleType, bool Value, long TeamId)
        {
            this.ToggleType = ToggleType;
            this.Value = Value;
            this.TeamId = TeamId;
        }

        public override string Compile()
        {
            return "Go" + (this.Value ? "+" : "-") + this.ToggleType + TeamId.ToString();
        }
    }
}
