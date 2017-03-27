using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Utils;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharactersListMessage : PacketBase
    {
        public Dictionary<long, Player> Characters;

        public CharactersListMessage(Dictionary<long , Player> Characters)
        {
            this.Characters = Characters;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("ALK"+Definitions.SubscriptionTime.ToString());
            if (Characters.Count > 0)
            {
                Packet.Append('|').Append(Characters.Count);

                foreach (var Character in Characters.Values)
                {
                    Packet.Append('|').Append(Character.ID);
                    Packet.Append(';').Append(Character.Name);
                    Packet.Append(';').Append(Character.Level);
                    Packet.Append(';').Append(Character.Look);
                    Packet.Append(';').Append(Character.Color1 == -1 ? "-1" : Character.Color1.ToString("x"));
                    Packet.Append(';').Append(Character.Color2 == -1 ? "-1" : Character.Color2.ToString("x"));
                    Packet.Append(';').Append(Character.Color3 == -1 ? "-1" : Character.Color3.ToString("x"));
                    Packet.Append(';');
                    Packet.Append(Character.WornItem); //Character.InventoryCache.SerializeAsDisplayEquipment(Packet);
                    Packet.Append(';').Append('0'); // MARCHAND
                    Packet.Append(';').Append(Settings.Server);
                    Packet.Append(';').Append(0); //Character.Dead ? '1' : '0'
                    Packet.Append(';').Append(0); //Character.DeathCount
                    Packet.Append(';').Append(0); //Character.MaxLevel
                }
            }

            return Packet.ToString();
        }
    }
}
