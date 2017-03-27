using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Maps;
using Tera.Libs.Network;
using Tera.WorldServer.World.Spells;

namespace Tera.WorldServer.World.Fights
{
    public abstract class VirtualFighter : Fighter
    {
        public AIProcessor Mind
        {
            get;
            set;
        }

        public VirtualFighter(Fight Fight, GameActorTypeEnum ActorType, Fighter Invocator = null)
            : base(Fight, ActorType, Invocator)
        {
            
        }

        public override void Send(PacketBase Packet)
        {
        }

        public override void JoinFight()
        {
            this.Mind = new AIProcessor(Fight, this);
        }

        public abstract List<SpellLevel> getSpells();
    }
}
