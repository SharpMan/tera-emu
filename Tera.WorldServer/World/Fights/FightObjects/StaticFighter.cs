using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Fights.FightObjects
{
    public abstract class StaticFighter: Fighter
    {
        public override FightObjectType ObjectType
        {
            get
            {
                return FightObjectType.OBJECT_STATIC;
            }
        }

        public StaticFighter(Fight Fight, GameActorTypeEnum ActorType, Fighter Invocator = null)
            : base(Fight, ActorType, Invocator)
        {
        }
        
        public override int MaxAP { get { return 0; } }

        public override int MaxMP { get { return 0; } }

        public override int AP { get { return 0; } }

        public override int MP { get { return 0; } }

        public override int UsedAP { get { return 0; } set { } }

        public override int UsedMP { get { return 0; } set { } }
    }
}
