using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class SpellUpdateSuccessMessage : PacketBase
    {
        public int SpellId;
        public int SpellLevel;

        public SpellUpdateSuccessMessage(int SpellId, int SpellLevel)
        {
            this.SpellId = SpellId;
            this.SpellLevel = SpellLevel;
        }

        public override string Compile()
        {
            return "SUK" + this.SpellId + '~' + this.SpellLevel;
        }
    }
}
