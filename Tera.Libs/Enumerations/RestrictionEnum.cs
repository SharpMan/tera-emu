using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Enumerations
{
    public enum RestrictionEnum
    {
        RESTRICTION_CANT_ASSAULT = 1,
        RESTRICTION_CANT_CHALLENGE = 2,
        RESTRICTION_CANT_EXCHANGE = 4,
        RESTRICTION_CANT_ATTACK = 8,
        RESTRICTION_FORCEWALK = 16,
        RESTRICTION_SLOWED = 32,
        RESTRICTION_CANT_SWITCH_TOCREATURE = 64,
        RESTRICTION_IS_TOMBESTONE = 128,
    }
}
