using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Packets
{
    public enum GameActorShowEnum
    {
        SHOW_SPAWN = '+',
        SHOW_REFRESH = '~',
    }

    public sealed class GameActorShowMessage : PacketBase
    {
        public IGameActor Actor;
        public GameActorShowEnum ShowType;

        public GameActorShowMessage(GameActorShowEnum ShowType, IGameActor Actor)
        {
            this.Actor = Actor;
            this.ShowType = ShowType;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("GM|");

            Packet.Append((char)this.ShowType);
            Actor.SerializeAsGameMapInformations(Packet);

            return Packet.ToString();
        }
    }
}
