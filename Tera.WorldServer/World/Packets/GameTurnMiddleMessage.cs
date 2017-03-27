using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameTurnMiddleMessage : PacketBase
    {
        public IEnumerable<Fighter> Fighters;

        public GameTurnMiddleMessage(IEnumerable<Fighter> Fighters)
        {
            this.Fighters = Fighters;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("GTM");

            foreach (var Fighter in this.Fighters)
            {
                Packet.Append('|');
                Packet.Append(Fighter.ActorId).Append(';');
                Packet.Append(Fighter.Dead ? '1' : '0').Append(';');
                Packet.Append(Fighter.Life).Append(';');
                Packet.Append(Fighter.AP).Append(';');
                Packet.Append(Fighter.MP).Append(';');
                Packet.Append(Fighter.Cell.Id).Append(";;"); // TODO si invisible mataichcell
                Packet.Append(Fighter.MaxLife);
            }

            return Packet.ToString();
        }
    }
}
