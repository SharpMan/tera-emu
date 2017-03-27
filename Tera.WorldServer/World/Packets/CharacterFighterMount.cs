using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterFighterMount : PacketBase
    {
        public Fighter f;
        public long guid;

        public CharacterFighterMount(Fighter f, long guid)
        {
            this.f = f;
            this.guid = guid;
        }

        public override string Compile()
        {
            StringBuilder packet = new StringBuilder("GM|-").Append(guid).Append((char)0x00).Append("~");
            (f as CharacterFighter).SerializeAsGameMapInformations(packet);
            return packet.ToString();
        }

    }
}
