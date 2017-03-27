using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class FightGameActionMessage : PacketBase
    {
        public int ActionID;
        public String s1, s2;

        public FightGameActionMessage(int action, String s1, String s2)
        {
            this.ActionID = action;
            this.s1 = s1;
            this.s2 = s2;
        }

        public override string Compile()
        {
            StringBuilder sb = new StringBuilder("GA;").Append(ActionID).Append(";").Append(s1);
            if (!s2.Equals(""))
            {
                sb.Append(";").Append(s2);
            }
            return sb.ToString();
        }
    }
}
