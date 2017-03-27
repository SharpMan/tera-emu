using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
   public sealed class PartyRequestMessage : PacketBase
    {
       public string n1, n2;
       public PartyRequestMessage(String k, String l)
       {
           n1 = k;
           n2 = l;
       }
       public override string Compile()
       {
           return "PIK"  + n1 + "|"+ n2;
       }
    }
}
