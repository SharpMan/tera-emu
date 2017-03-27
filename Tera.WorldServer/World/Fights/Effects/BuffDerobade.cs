using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffDerobade : BuffEffect
    {
        public BuffDerobade(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_ATTACKED_POST_JET, BuffDecrementType.TYPE_ENDTURN)
        {
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            if (!DamageInfos.IsCAC)
                return -1;

            DamageValue = 0; // Annihilation des dommages;

            var SubInfos = new EffectCast(EffectEnum.PushBack, 0, 0, 0, 0, 0, 0, 0, DamageInfos.Caster, null);
            var Direction = Pathfinder.GetDirection(Target.Fight.Map, DamageInfos.Caster.CellId, Target.CellId);

            // Application du push
            return EffectPush.ApplyPush(SubInfos, this.Target, Direction, 1,true);
        }
    }
}
