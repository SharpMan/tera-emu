using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectPushFear : EffectBase
    {
        public override int ApplyEffect(EffectCast CastInfos)
        {
            int Direction = 0;
            char d = Pathfinder.getDirBetweenTwoCase(CastInfos.Caster.CellId, CastInfos.CellId, CastInfos.Caster.Fight.Map, true);
            int tcellID = Pathfinder.DirToCellID(CastInfos.Caster.CellId, d, CastInfos.Caster.Fight.Map, true);

            var cell = CastInfos.Caster.Fight.GetCell(tcellID);
            Logger.Error("Cell => "+cell);
            if (cell == null)
                return -1;

            if (!cell.HasGameObject(FightObjectType.OBJECT_FIGHTER))
                return -1;

            var Target = cell.GetObjects<Fighter>().FirstOrDefault();
            if (Target.ObjectType == FightObjectType.OBJECT_STATIC)
            {
                return -1;
            }
            Logger.Error("TargetName => " + Target.Name);

            Direction = Pathfinder.GetDirection(Target.Fight.Map, CastInfos.Caster.CellId, Target.CellId);
            Logger.Error("Direction => " + Direction);

            if (EffectPush.ApplyPush(CastInfos, Target, Direction, Pathfinder.GoalDistance(Target.Fight.Map, Target.CellId, CastInfos.CellId)) == -3)
                return -3;

            return -1;
        }
    }
}
