using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights
{
    public enum FightObjectType
    {
        OBJECT_FIGHTER = 1,
        OBJECT_TRAP = 2,
        OBJECT_GLYPHE = 3,
        OBJECT_STATIC = 4,
        OBJECT_BLYPHE = 5,
    }

    public interface IFightObject
    {
        FightObjectType ObjectType { get; }
        int CellId { get; }
        bool CanWalk();
        bool CanStack();
    }
}
