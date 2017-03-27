using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class SpellsListMessage : PacketBase
    {
        public Player Character;

        public SpellsListMessage(Player Character)
        {
            this.Character = Character;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("SL");

            Character.GetSpellBook().SerializeAsSpellsListMessage(Packet);

            return Packet.ToString();
        }
    }
}
