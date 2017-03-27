using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Helper;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Fights.FightObjects;
using Tera.Libs;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectTrap : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            var currCell = CastInfos.Caster.Fight.GetCell(CastInfos.CellId);
            if (!currCell.IsWalkable() || currCell.HasGameObject(FightObjectType.OBJECT_FIGHTER) || currCell.HasGameObject(FightObjectType.OBJECT_STATIC))
            {
                return -1;
            }
            foreach (var Trap in currCell.GetObjects<FightTrapLayer>())
            {
                if (Trap.CellId == CastInfos.CellId)
                {
                    return -1;
                }
            }
            int spellID = CastInfos.Value1;
            int level = CastInfos.Value2;
            var spell = SpellTable.Cache[spellID].GetLevel(level);
            spell.Initialize();
            new FightTrapLayer(CastInfos.Caster, spell, CastInfos.Caster.Fight.GetCell(CastInfos.CellId)
                , CastInfos.SpellId, CastInfos.SpellLevel.SpellCache.SpriteID, CastInfos.SpellLevel.Range);
            return -1;
        }
    }
}
