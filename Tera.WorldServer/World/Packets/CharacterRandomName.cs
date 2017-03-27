using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.Libs.Helper;

namespace Tera.WorldServer.World.Packets
{
    public class CharacterRandomName : PacketBase
    {
        public override string Compile()
        {
            return "APK" + StringHelper.GetRandomName();
        }
    }
}
