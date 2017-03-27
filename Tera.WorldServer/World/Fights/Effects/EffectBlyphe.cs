using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Fights.FightObjects;
using Tera.WorldServer.Database.Tables;
using Tera.Libs.Helper;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectBlyphe : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            if (!CastInfos.Caster.Fight.GetCell(CastInfos.CellId).IsWalkable())
            {
                return -1;
            }
            int spellID = CastInfos.Value1;
            int level = CastInfos.Value2;
            short duration = (short)CastInfos.Value3;
            String po = CastInfos.SpellLevel.Range;
            var spell = SpellTable.Cache[spellID].GetLevel(level);
            spell.Initialize();
            new FightBlypheLayer(CastInfos.Caster, spell, CastInfos.Caster.Fight.GetCell(CastInfos.CellId), CastInfos.SpellId, duration, po);
            return -1;
        }
    }
}
