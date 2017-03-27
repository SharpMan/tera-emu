using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Fights;
using Tera.Libs;

namespace Tera.WorldServer.World.Packets
{
    public sealed class FightListMessage : PacketBase
    {
        public IEnumerable<Fight> Fights = new List<Fight>();

        public FightListMessage(IEnumerable<Fight> Fights)
        {
            this.Fights = Fights;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("fL");

            Packet.Append(string.Join("|", this.Fights.Select(x => x.SerializeAs_FightListInformations())));
            Logger.Debug("FIGHT LIST=> " + Packet.ToString());
            return Packet.ToString();
        }
    }
}
