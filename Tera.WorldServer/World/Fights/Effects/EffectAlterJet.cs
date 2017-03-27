using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Effects
{
    public class EffectAlterJet : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            if (CastInfos.Duration > 0)
            {
                foreach (var Target in CastInfos.Targets)
                {
                    Target.States.RemoveState(FighterStateEnum.STATE_MINIMIZE_EFFECTS);
                    Target.States.RemoveState(FighterStateEnum.STATE_MAXIMIZE_EFFECTS);
                    Target.Buffs.AddBuff(new BuffState(CastInfos, Target));
                }
            }

            return -1;
        }
    }
}
