using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameMapComplementaryInformationsMessage : PacketBase
    {
        public IEnumerable<IGameActor> Actors;

        public GameMapComplementaryInformationsMessage(IEnumerable<IGameActor> Actors)
        {
            this.Actors = Actors;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("GM");

            foreach (var Actor in Actors)
            {
                Packet.Append("|+");
                Actor.SerializeAsGameMapInformations(Packet);
            }

            return Packet.ToString();
        }
    }
}
