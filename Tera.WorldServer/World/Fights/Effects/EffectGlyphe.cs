using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Helper;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Fights.FightObjects;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectGlyphe : EffectBase
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
            new FightGlypheLayer(CastInfos.Caster, spell, CastInfos.Caster.Fight.GetCell(CastInfos.CellId), CastInfos.SpellId, duration, po);
            return -1;
        }
    }
}
