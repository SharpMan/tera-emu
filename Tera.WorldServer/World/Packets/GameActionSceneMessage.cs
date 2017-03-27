using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameActionSceneMessage : PacketBase
    {
        public long guid;

        public GameActionSceneMessage(long guid)
        {
            this.guid = guid;
        }

        public override string Compile()
        {
            return "GAS" + guid; 
        }
    }
}
