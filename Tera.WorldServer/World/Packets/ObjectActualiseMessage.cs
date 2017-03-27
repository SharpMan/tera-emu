using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ObjectActualiseMessage : PacketBase
    {
        public Player Character;

        public ObjectActualiseMessage(Player Character)
        {
            this.Character = Character;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("Oa");

            Packet.Append(this.Character.ID);
            Packet.Append('|');
            this.Character.InventoryCache.SerializeAsDisplayEquipment(Packet);

            return Packet.ToString();
        }
    }
}
