using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterSelectedInformationsMessage : PacketBase
    {
        public Player Character;

        public CharacterSelectedInformationsMessage(Player Character)
        {
            this.Character = Character;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("ASK|");

            Packet.Append(Character.ID).Append('|');
            Packet.Append(Character.Name).Append('|');
            Packet.Append(Character.Level).Append('|');
            Packet.Append(Character.Classe).Append('|');
            Packet.Append(Character.Sexe).Append('|');
            Packet.Append(Character.Look).Append('|');
            Packet.Append(Character.Color1).Append('|');
            Packet.Append(Character.Color2).Append('|');
            Packet.Append(Character.Color3).Append('|');
            Character.InventoryCache.SerializeAsInventoryContent(Packet);
            Packet.Append('|'); // ITEMS DATA
            return Packet.ToString();
        }
    }
}
