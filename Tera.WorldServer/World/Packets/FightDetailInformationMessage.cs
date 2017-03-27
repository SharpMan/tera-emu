using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Packets
{
    public sealed class FightDetailInformationMessage : PacketBase
    {
        public Fight Fight;

        public FightDetailInformationMessage(Fight Fight)
        {
            this.Fight = Fight;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("fD").Append(this.Fight.FightId).Append('|');

            foreach (var Fighter in this.Fight.Team1.GetAliveFighters())
                Packet.Append(Fighter.Name).Append('~').Append(Fighter.Level).Append(';');

            Packet.Append('|');

            foreach (var Fighter in this.Fight.Team2.GetAliveFighters())
                Packet.Append(Fighter.Name).Append('~').Append(Fighter.Level).Append(';');

            return Packet.ToString();
        }
    }
}
