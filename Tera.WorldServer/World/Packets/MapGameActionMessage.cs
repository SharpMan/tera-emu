using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public class MapGameActionMessage : PacketBase
    {
        public int GameActionID;
        public int actionId;
        public String s1;
        public String s2;

        public MapGameActionMessage(int gID, int aID, String s1, String s2)
        {
            this.GameActionID = gID;
            this.actionId = aID;
            this.s1 = s1;
            this.s2 = s2;
        }

        public override string Compile()
        {
            String packet = "GA" + GameActionID + ";" + actionId + ";" + s1;
            if (!s2.Equals(""))
            {
                packet += ";" + s2;
            }
            return packet;
        }
    }
}
