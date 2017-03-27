using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameInformationCoordinateMessage : PacketBase
    {
        public IEnumerable<Fighter> Fighters;

        public GameInformationCoordinateMessage(IEnumerable<Fighter> Fighters)
        {
            this.Fighters = Fighters;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("GIC");

            foreach (var Fighter in this.Fighters)
            {
                if (Fighter == null)
                    continue;
                Packet.Append('|').Append(Fighter.ActorId).Append(';').Append(Fighter.Cell.Id).Append(";1");
            }
            return Packet.ToString();
        }
    }
}
