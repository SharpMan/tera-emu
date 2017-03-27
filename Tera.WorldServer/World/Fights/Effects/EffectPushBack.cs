using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Fights.FightObjects;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectPush : EffectBase
    {
        public static Random RANDOM_PUSHDAMAGE = new Random();

        public override int ApplyEffect(EffectCast CastInfos)
        {
            foreach (var Target in CastInfos.Targets)
            {
                if (Target.ObjectType == FightObjectType.OBJECT_STATIC)
                {
                    continue;
                }

                int Direction = 0;

                switch (CastInfos.EffectType)
                {
                    case EffectEnum.PushBack:
                        if (Pathfinder.InLine(Target.Fight.Map, CastInfos.CellId, Target.CellId) && CastInfos.CellId != Target.CellId)
                            Direction = Pathfinder.GetDirection(Target.Fight.Map, CastInfos.CellId, Target.CellId);
                        else if (Pathfinder.InLine(Target.Fight.Map, CastInfos.Caster.CellId, Target.CellId))
                            Direction = Pathfinder.GetDirection(Target.Fight.Map, CastInfos.Caster.CellId, Target.CellId);
                        else
                        {
                            continue;
                        }
                        break;

                    case EffectEnum.PushFront:
                        Direction = Pathfinder.GetDirection(Target.Fight.Map, Target.CellId, CastInfos.Caster.CellId);
                        break;
                }

                if (EffectPush.ApplyPush(CastInfos, Target, Direction, CastInfos.Value1) == -3)
                    return -3;
            }

            return -1;
        }

        public static int ApplyPush(EffectCast CastInfos, Fighter Target, int Direction, int Length, bool isDerobade = false)
        {
            var GameAction = (int)GameActionTypeEnum.MAP_PUSHBACK;
            var LastCell = Target.Cell;

            for (int i = 0; i < Length; i++)
            {
                if(isDerobade && i !=  0)continue;
                var NextCell = Target.Fight.GetCell(Pathfinder.NextCell(Target.Fight.Map, LastCell.Id, Direction));
                if (LastCell.HasGameObject(FightObjectType.OBJECT_TRAP) && i > 0)
                {
                    break;
                }
                if (NextCell != null && NextCell.IsWalkable())
                {
                    // Un objet deriere lui ?
                    if (NextCell.HasGameObject(FightObjectType.OBJECT_FIGHTER) || NextCell.HasGameObject(FightObjectType.OBJECT_STATIC) || Target.States.HasState(FighterStateEnum.STATE_ENRACINER))
                    {
                        if (i != 0)
                        {
                            Target.Fight.SendToFight(new GameActionMessage(GameAction, Target.ActorId, Target.ActorId + "," + LastCell.Id));

                            var PushTime = 200 + (i * 200);

                            System.Threading.Thread.Sleep(PushTime);

                            // Affecte la cell
                            var CellResult = Target.SetCell(LastCell);

                            LastCell.GetObjects<FightGroundLayer>().ForEach(x => x.onWalkOnLayer(Target, LastCell));

                            // Persos mort ou fin de combat ?
                            if (CellResult == -3 || CellResult == -2)
                                return CellResult;
                        }

                        // Application des dommages
                        if(CastInfos.EffectType == EffectEnum.PushBack)
                            return EffectPush.ApplyPushBackDamages(CastInfos, Target, Length, i);

                        return -1;
                    }
                }
                else
                {
                    if (i != 0)
                    {
                        Target.Fight.SendToFight(new GameActionMessage(GameAction, Target.ActorId, Target.ActorId + "," + LastCell.Id));

                        var PushTime = 200 + (i * 200);

                        System.Threading.Thread.Sleep(PushTime);

                        // Affecte la cell
                        var CellResult = Target.SetCell(LastCell);

                        LastCell.GetObjects<FightGroundLayer>().ForEach(x => x.onWalkOnLayer(Target, LastCell));

                        // Persos mort ou fin de combat ?
                        if (CellResult == -3 || CellResult == -2)
                            return CellResult;
                    }

                    // Application des dommages
                    if(CastInfos.EffectType == EffectEnum.PushBack)
                        return EffectPush.ApplyPushBackDamages(CastInfos, Target, Length, i);

                    return -1;
                }

                LastCell = NextCell;
            }

            Target.Fight.SendToFight(new GameActionMessage(GameAction, Target.ActorId, Target.ActorId + "," + LastCell.Id));

            if (Length > 0)
            {
                System.Threading.Thread.Sleep(200 + (Length * 200));
                var CallResult = Target.SetCell(LastCell);
                LastCell.GetObjects<FightGroundLayer>().ForEach(x => x.onWalkOnLayer(Target, LastCell));
                return CallResult;
            }
            return -1;
        }        

        public static int ApplyPushBackDamages(EffectCast CastInfos, Fighter Target, int Length, int CurrentLength)
        {
            int DamageCoef;
            if(Target.States.HasState(FighterStateEnum.STATE_MAXIMIZE_EFFECTS)) DamageCoef = 7;
            else if (Target.States.HasState(FighterStateEnum.STATE_MINIMIZE_EFFECTS))DamageCoef = 4;
            else DamageCoef = EffectPush.RANDOM_PUSHDAMAGE.Next(4, 7);
            
            double LevelCoef = CastInfos.Caster.Level / 50;
            if (LevelCoef < 0.1) LevelCoef = 0.1;
            int DamageValue = (int)Math.Floor(DamageCoef * LevelCoef) * (Length - CurrentLength + 1);

            var SubInfos = new EffectCast(EffectEnum.DamageBrut, CastInfos.SpellId, CastInfos.CellId, 0, 0, 0, 0, 0, CastInfos.Caster, null);

            return EffectDamage.ApplyDamages(SubInfos, Target, ref DamageValue);
        }
    }
}
