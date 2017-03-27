using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectUnHide : EffectBase
    {

        public override int ApplyEffect(EffectCast CastInfos)
        {
            foreach (var Target in CastInfos.Targets)
            {
                if(Target.States.HasState(FighterStateEnum.STATE_INVISIBLE))
                    Target.States.RemoveState(FighterStateEnum.STATE_INVISIBLE);

            }
            return -1;
        }
    }
}
