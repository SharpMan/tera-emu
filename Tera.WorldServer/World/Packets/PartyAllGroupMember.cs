using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.World.Packets
{
    public class PartyAllGroupMember : PacketBase
    {
        public Party p;

        public PartyAllGroupMember(Party x)
        {
            p = x;
        }

        public override string Compile()
        {
            StringBuilder sb = new StringBuilder("PM+");
            Boolean first = true;
            if (p == null || p.Players == null)
            {
                return "BN";
            }
            foreach (Player pp in p.Players)
            {
                if (!first)
                {
                    sb.Append("|");
                }
                sb.Append(pp.ToPM());
                first = false;
            }
            return sb.ToString();
        }
    }
}
