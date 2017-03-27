using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Network;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameTurnListMessage : PacketBase
    {
        public IEnumerable<Fighter> Fighters;

        public GameTurnListMessage(IEnumerable<Fighter> Fighters)
        {
            this.Fighters = Fighters;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("GTL");

            foreach (var Fighter in this.Fighters.Where(x => !x.Dead))
            {
                Packet.Append('|').Append(Fighter.ActorId);
            }

            return Packet.ToString();
        }
    }
}
