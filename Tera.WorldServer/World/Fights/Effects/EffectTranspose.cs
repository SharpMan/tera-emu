using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectTranspose : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            foreach (var Target in CastInfos.Targets)
            {
                if (Target.ObjectType == FightObjectType.OBJECT_STATIC)
                {
                    continue;
                }
                if (CastInfos.SpellId == 445)
                {
                    if (Target.Team == CastInfos.Caster.Team)                    
                        continue;                    
                }
                else if (CastInfos.SpellId == 438)
                {
                    if (Target.Team != CastInfos.Caster.Team)
                        continue;
                }

                var TargetTeleport = new EffectCast(EffectEnum.Teleport, CastInfos.SpellId, CastInfos.Caster.CellId, 0, 0, 0, 0, 0, Target, null);
                var CasterTeleport = new EffectCast(EffectEnum.Teleport, CastInfos.SpellId, Target.CellId, 0, 0, 0, 0, 0, CastInfos.Caster, null);

                CastInfos.Caster.SetCell(null);
                Target.SetCell(null);

                if (BuffSacrifice.Teleport.ApplyEffect(TargetTeleport) == -3)
                    return -3;

                if (BuffSacrifice.Teleport.ApplyEffect(CasterTeleport) == -3)
                    return -3;
            }

            return -1;
        }
    }
}
