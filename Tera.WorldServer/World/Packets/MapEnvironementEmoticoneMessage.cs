using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class MapEnvironementEmoticoneMessage : PacketBase
    {
        public long guid;
        public int emote;

        public MapEnvironementEmoticoneMessage(long guid, int emote)
        {
            this.guid = guid;
            this.emote = emote;
        }

        public override string Compile()
        {
            return "eUK" + guid + "|" + emote;
        }
    }
}
