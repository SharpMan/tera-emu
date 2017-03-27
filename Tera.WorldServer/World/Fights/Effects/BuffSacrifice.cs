using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class BuffSacrifice : BuffEffect
    {
        public static EffectTeleport Teleport = new EffectTeleport();

        public BuffSacrifice(EffectCast CastInfos, Fighter Target)
            : base(CastInfos, Target, BuffActiveType.ACTIVE_ATTACKED_AFTER_JET, BuffDecrementType.TYPE_ENDTURN)
        {
        }

        public override int ApplyEffect(ref int DamageValue, EffectCast DamageInfos = null)
        {
            if (Target.ObjectType != FightObjectType.OBJECT_STATIC)
            {
                var TargetTeleport = new EffectCast(EffectEnum.Teleport, this.CastInfos.SpellId, this.CastInfos.Caster.CellId, 0, 0, 0, 0, 0, this.Target, null);
                var CasterTeleport = new EffectCast(EffectEnum.Teleport, this.CastInfos.SpellId, this.Target.CellId, 0, 0, 0, 0, 0, this.CastInfos.Caster, null);

                this.Caster.SetCell(null);
                this.Target.SetCell(null);

                if (BuffSacrifice.Teleport.ApplyEffect(TargetTeleport) == -3)
                    return -3;

                if (BuffSacrifice.Teleport.ApplyEffect(CasterTeleport) == -3)
                    return -3;
            }

            if (EffectDamage.ApplyDamages(DamageInfos, this.CastInfos.Caster, ref DamageValue) == -3)
                return -3;

            DamageValue = 0;

            return base.ApplyEffect(ref DamageValue, DamageInfos);
        }
    }
}
