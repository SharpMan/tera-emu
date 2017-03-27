using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameFightTeamFlagFightersMessage : PacketBase
    {
        public IEnumerable<Fighter> Fighters;
        public long LeaderId;
        bool Add = true;

        public GameFightTeamFlagFightersMessage(IEnumerable<Fighter> Fighters, long LeaderId, bool Add = true)
        {
            this.Fighters = Fighters;
            this.LeaderId = LeaderId;
            this.Add = true;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("Gt").Append(this.LeaderId);

            foreach (var Fighter in this.Fighters)
            {
                Packet.Append("|").Append(this.Add ? '+' : '-');
                Packet.Append(Fighter.ActorId).Append(';');
                Packet.Append(Fighter.Name).Append(';');
                Packet.Append(Fighter.Level);
            }

            return Packet.ToString();
        }
    }
}
