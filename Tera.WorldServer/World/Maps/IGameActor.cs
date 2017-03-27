using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Maps
{
    public enum GameActorTypeEnum
    {
        TYPE_CHARACTER = 0,
        TYPE_MONSTER = -3,
        TYPE_NPC = -4,
        TYPE_MERCHANT = -5,
        TYPE_TAX_COLLECTOR = -6,
        TYPE_MUTANT = -8,
        TYPE_MOUNT_PARK = -9,
        TYPE_PRISM = -10,
    }

    public interface IGameActor
    {
        long ActorId
        {
            get;
        }

        int Orientation
        {
            get;
            set;
        }

        int CellId
        {
            get;
            set;
        }

        GameActorTypeEnum ActorType
        {
            get;
        }

        void SerializeAsGameMapInformations(StringBuilder SerializedString);
    }
}
