using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public class ImMessage : PacketBase
    {
        public String Message;

        public ImMessage(String mess)
        {
            this.Message = mess;
        }

        public override string Compile()
        {
            return "Im"+Message;
        }
    }
    
}
