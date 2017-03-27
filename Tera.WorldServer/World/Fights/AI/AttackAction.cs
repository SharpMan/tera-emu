using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Fights.Effects;
using Tera.WorldServer.World.Spells;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Fights.FightObjects;

namespace Tera.WorldServer.World.Fights.AI
{
    public class AttackAction : AIAction
    {
        protected override double ScoreHeal(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false)
        {
            var Score = 0;
            var BaseScore = 13;
            BaseScore *= Effect.RandomJet;

            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                if (Target.States.HasState(FighterStateEnum.STATE_INVISIBLE) && Effect.EffectType == EffectEnum.Invisible)
                {
                    Score -= currScore;
                    continue;
                }
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 10)
                    currScore *= 12;
                if (PercentLife < 20)
                    currScore *= 8;
                else if (PercentLife < 30)
                    currScore *= 5;
                else if (PercentLife < 50)
                    currScore *= 3;
                else if (PercentLife < 75)
                    currScore *= 2;
                else if (PercentLife >= 95)
                {
                    continue;
                }

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;
                if (Reverse)
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreBuff_I(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false)
        {
            var Score = 0;
            var BaseScore = 10;

            if (!notUseJet)
            {
                BaseScore *= Effect.RandomJet;
            }

            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                if (Target.States.HasState(FighterStateEnum.STATE_INVISIBLE) && Effect.EffectType == EffectEnum.Invisible)
                {
                    Score -= currScore;
                    continue;
                }
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 20)
                    currScore *= 8;
                else if (PercentLife < 30)
                    currScore *= 5;
                else if (PercentLife < 50)
                    currScore *= 3;
                else if (PercentLife < 75)
                    currScore *= 2;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;
                if (Reverse)
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreBuff_II(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false)
        {
            var Score = 0;
            var BaseScore = 12;

            if (!notUseJet)
            {
                BaseScore *= Effect.RandomJet;
            }

            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                if (Target.States.HasState(FighterStateEnum.STATE_INVISIBLE) && Effect.EffectType == EffectEnum.Invisible)
                {
                    Score -= currScore;
                    continue;
                }
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 20)
                    currScore *= 8;
                else if (PercentLife < 30)
                    currScore *= 5;
                else if (PercentLife < 50)
                    currScore *= 3;
                else if (PercentLife < 75)
                    currScore *= 2;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;
                if (Reverse)
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreBuff_III(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false)
        {
            var Score = 0;
            var BaseScore = 15;

            if (!notUseJet)
            {
                BaseScore *= Effect.RandomJet;
            }

            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                if (Target.States.HasState(FighterStateEnum.STATE_INVISIBLE) && Effect.EffectType == EffectEnum.Invisible)
                {
                    Score -= currScore;
                    continue;
                }
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 20)
                    currScore *= 8;
                else if (PercentLife < 30)
                    currScore *= 5;
                else if (PercentLife < 50)
                    currScore *= 3;
                else if (PercentLife < 75)
                    currScore *= 2;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;
                if (Reverse)
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreDamage_0(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false)
        {
            var Score = 0;
            var BaseScore = 2;
            BaseScore *= Math.Abs(Effect.Value1);
            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 5)
                    currScore *= 8;
                else if (PercentLife < 10)
                    currScore *= 5;
                else if (PercentLife < 25)
                    currScore *= 3;
                else if (PercentLife < 50)
                    currScore *= 2;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;

                if (Reverse)
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 3;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 3;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreDamage_I(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false)
        {
            var Score = 0;
            var BaseScore = 25;
            BaseScore *= Math.Abs(Effect.Value1);
            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 5)
                    currScore *= 8;
                else if (PercentLife < 10)
                    currScore *= 5;
                else if (PercentLife < 25)
                    currScore *= 3;
                else if (PercentLife < 50)
                    currScore *= 2;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;

                if (Reverse)
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 3;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 3;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreDamage_II(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false)
        {
            var Score = 0;
            var BaseScore = 27;
            BaseScore *= Math.Abs(Effect.Value1);
            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 5)
                    currScore *= 8;
                else if (PercentLife < 10)
                    currScore *= 5;
                else if (PercentLife < 25)
                    currScore *= 3;
                else if (PercentLife < 50)
                    currScore *= 2;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;

                if (Reverse)
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 3;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 3;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreDamage_III(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false)
        {
            var Score = 0;
            var BaseScore = 29;
            BaseScore *= Math.Abs(Effect.Value1);
            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 5)
                    currScore *= 8;
                else if (PercentLife < 10)
                    currScore *= 5;
                else if (PercentLife < 25)
                    currScore *= 3;
                else if (PercentLife < 50)
                    currScore *= 2;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;

                if (Reverse)
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 3;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 3;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreDamagesPerPA(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false)
        {
            var Score = 0;

            var BaseScore = 11;

            BaseScore *= Effect.RandomJet;

            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                currScore *= Target.MaxAP;
                if (PercentLife < 20)
                    currScore *= 2;
                else if (PercentLife < 30)
                    currScore *= 3;
                else if (PercentLife < 50)
                    currScore *= 5;
                else if (PercentLife < 75)
                    currScore *= 8;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;

                if (Reverse)
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreSubBuff_I(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false)
        {
            var Score = 0;
            var BaseScore = 10;
            if (!notUseJet)
            {
                BaseScore *= Effect.RandomJet;
            }

            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 20)
                    currScore *= 2;
                else if (PercentLife < 30)
                    currScore *= 3;
                else if (PercentLife < 50)
                    currScore *= 5;
                else if (PercentLife < 75)
                    currScore *= 8;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;
                if (Reverse)
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreSubBuff_II(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false)
        {
            var Score = 0;
            var BaseScore = 12;
            if (!notUseJet)
            {
                BaseScore *= Effect.RandomJet;
            }

            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 20)
                    currScore *= 2;
                else if (PercentLife < 30)
                    currScore *= 3;
                else if (PercentLife < 50)
                    currScore *= 5;
                else if (PercentLife < 75)
                    currScore *= 8;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;
                if (Reverse)
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreSubBuff_III(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false)
        {
            var Score = 0;
            var BaseScore = 15;
            if (!notUseJet)
            {
                BaseScore *= Effect.RandomJet;
            }

            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 20)
                    currScore *= 2;
                else if (PercentLife < 30)
                    currScore *= 3;
                else if (PercentLife < 50)
                    currScore *= 5;
                else if (PercentLife < 75)
                    currScore *= 8;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;
                if (Reverse)
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreSubBuff_IV(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false)
        {
            var Score = 0;
            var BaseScore = 18;
            if (!notUseJet)
            {
                BaseScore *= Effect.RandomJet;
            }

            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 20)
                    currScore *= 2;
                else if (PercentLife < 30)
                    currScore *= 3;
                else if (PercentLife < 50)
                    currScore *= 5;
                else if (PercentLife < 75)
                    currScore *= 8;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;
                if (Reverse)
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }

            return Score;
        }

        protected override double ScoreAddStateGood(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false)
        {
            var Score = 0;
            var BaseScore = 11;
            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 20)
                    currScore *= 8;
                else if (PercentLife < 30)
                    currScore *= 5;
                else if (PercentLife < 50)
                    currScore *= 3;
                else if (PercentLife < 75)
                    currScore *= 2;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;

                if (Reverse)
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }
            return Score;
        }

        protected override double ScoreAddStateBad(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false)
        {
            var Score = 0;
            var BaseScore = 11;
            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 20)
                    currScore *= 2;
                else if (PercentLife < 30)
                    currScore *= 3;
                else if (PercentLife < 50)
                    currScore *= 5;
                else if (PercentLife < 75)
                    currScore *= 8;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;

                if (Reverse)
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }
            return Score;
        }

        protected override double ScoreRemStateGood(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false)
        {
            var Score = 0;
            var BaseScore = 11;
            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 20)
                    currScore *= 2;
                else if (PercentLife < 30)
                    currScore *= 3;
                else if (PercentLife < 50)
                    currScore *= 5;
                else if (PercentLife < 75)
                    currScore *= 8;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;

                if (Reverse)
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }
            return Score;
        }

        protected override double ScoreRemStateBad(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false)
        {
            var Score = 0;
            var BaseScore = 11;
            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                var PercentLife = Math.Ceiling((double)(Target.Life / Target.MaxLife) * 100);
                if (PercentLife < 20)
                    currScore *= 8;
                else if (PercentLife < 30)
                    currScore *= 5;
                else if (PercentLife < 50)
                    currScore *= 3;
                else if (PercentLife < 75)
                    currScore *= 2;

                if (Effect.Duration > 0)
                    currScore *= Effect.Duration;

                if (Reverse)
                {
                    if (Target.Team == AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
                else
                {
                    if (Target.Team != AI.myFighter.Team)
                        Score -= currScore * 2;
                    else
                        Score += currScore;
                }
            }
            return Score;
        }

        protected override double ScoreDebuff(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false)
        {
            if (Reverse)//On evite la boucle infinie
            {
                return 0;
            }
            var Score = 0;
            var BaseScore = 11;

            //BaseScore *= Effect.RandomJet;

            foreach (var Target in Targets)
            {
                int currScore = BaseScore;
                List<Fighter> target = new List<Fighter>() { Target };
                foreach (BuffEffect buff in Target.Buffs.getBuffs())
                {
                    if (buff.IsDebuffable)
                    {
                        currScore += (int)this.GetEffectScore(AI, -1, -1, new EffectInfos(buff.CastInfos), target, true);
                    }
                }
                Score += currScore;
            }

            return Score;
        }

        protected override double ScoreInvocation(AIProcessor AI, Spells.EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool InvokPreview = false)
        {
            if (Reverse)//On evite la boucle infinie
            {
                return 0;
            }
            var BaseScore = 11;
            var Score = BaseScore;

            var InvocationId = Effect.Value1;
            var InvocationLevel = Effect.Value2;
            if (InvokPreview)
            {
                return BaseScore * InvocationLevel;
            }
            if (!AI.myNeuron.myScoreInvocations.ContainsKey(InvocationId))
            {
                var Monster = MonsterTable.GetMonster(InvocationId);
                // Template de monstre existante
                if (Monster != null)
                {
                    Monster.Initialize();
                    var MonsterLevel = Monster.GetLevelOrNear(InvocationLevel);
                    // Level de monstre existant
                    if (MonsterLevel != null)
                    {
                        List<Fighter> possibleTargets = AI.myFight.GetAllyTeam(AI.myFighter.Team).GetFighters().FindAll(x => !x.Dead && !x.Left);
                        foreach (var Spell in MonsterLevel.Spells.GetSpells())
                        {
                            foreach (var spellEffect in Spell.Effects)
                            {
                                int currScore = (int)this.GetEffectScore(AI, -1, -1, spellEffect, possibleTargets, InvokPreview: true);
                                if (currScore > 0)
                                {
                                    Score += currScore;
                                }
                            }
                        }
                        possibleTargets = AI.myFight.GetEnnemyTeam(AI.myFighter.Team).GetFighters().FindAll(x => !x.Dead && !x.Left);
                        foreach (var Spell in MonsterLevel.Spells.GetSpells())
                        {
                            foreach (var spellEffect in Spell.Effects)
                            {
                                int currScore = (int)this.GetEffectScore(AI, -1, -1, spellEffect, possibleTargets, InvokPreview: true);
                                if (currScore > 0)
                                {
                                    Score += currScore;
                                }
                            }
                        }
                        foreach (var State in MonsterLevel.Stats.GetEffects().Values)
                        {
                            var total = State.Total;
                            if (total > 0)
                            {
                                Score += total;
                            }
                        }
                        Score *= MonsterLevel.Level;
                        AI.myNeuron.myScoreInvocations.Add(InvocationId, Score);
                        return Score;
                    }
                }
            }
            else
            {
                return AI.myNeuron.myScoreInvocations[InvocationId];
            }
            return 0;
        }

        protected override double ScoreInvocationStatic(AIProcessor AI, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool InvokPreview = false)
        {
            if (Reverse)//On evite la boucle infinie
            {
                return 0;
            }
            var BaseScore = 11;
            var Score = BaseScore;

            var InvocationId = Effect.Value1;
            var InvocationLevel = Effect.Value2;
            if (InvokPreview)
            {
                return BaseScore * InvocationLevel;
            }
            if (!AI.myNeuron.myScoreInvocations.ContainsKey(InvocationId))
            {
                var Monster = MonsterTable.GetMonster(InvocationId);
                // Template de monstre existante
                if (Monster != null)
                {
                    Monster.Initialize();
                    var MonsterLevel = Monster.GetLevelOrNear(InvocationLevel);
                    // Level de monstre existant
                    if (MonsterLevel != null)
                    {
                        List<Fighter> possibleTargets = AI.myFight.GetAllyTeam(AI.myFighter.Team).GetFighters().FindAll(x => !x.Dead && !x.Left);
                        foreach (var Spell in MonsterLevel.Spells.GetSpells())
                        {
                            foreach (var spellEffect in Spell.Effects)
                            {
                                int currScore = (int)this.GetEffectScore(AI, -1, -1, spellEffect, possibleTargets, InvokPreview: true);
                                if (currScore > 0)
                                {
                                    Score += currScore;
                                }
                            }
                        }
                        foreach (var State in MonsterLevel.Stats.GetEffects().Values)
                        {
                            var total = State.Total;
                            if (total > 0)
                            {
                                Score += total;
                            }
                        }
                        Score *= MonsterLevel.Level;
                        AI.myNeuron.myScoreInvocations.Add(InvocationId, Score);
                        return Score;
                    }
                }
            }
            else
            {
                return AI.myNeuron.myScoreInvocations[InvocationId];
            }
            return 0;
        }

        protected override double ScoreRepulse(AIProcessor AI, int CastCell, EffectInfos Effect, List<Fighter> Targets, bool InvokPreview, bool isFear)
        {
            if (InvokPreview)
            {
                return 0;
            }
            var Score = 0;
            if (isFear)
            {
                char d = Pathfinder.getDirBetweenTwoCase(AI.myFighter.CellId, CastCell, AI.myFight.Map, true);
                var StartCell = AI.myFight.GetCell(Pathfinder.DirToCellID(AI.myFighter.CellId, d, AI.myFight.Map, true));
                var EndCell = AI.myFight.GetCell(CastCell);
                if (StartCell != null && EndCell != null)
                {
                    var target = StartCell.GetFighter();
                    if (target != null)
                    {
                        Score += ScorePush(AI, target, Pathfinder.GetDirection(AI.myFight.Map, AI.myFighter.CellId, CastCell), Pathfinder.GoalDistance(AI.myFight.Map, AI.myFighter.CellId, CastCell), true);
                    }
                }
            }
            else
            {
                var StartCell = AI.myFight.GetCell(CastCell);
                if (StartCell != null)
                {
                    var target = StartCell.GetFighter();
                    if (target != null)
                    {
                        Score += ScorePush(AI, target, Pathfinder.GetDirection(AI.myFight.Map, AI.myFighter.CellId, CastCell), Effect.Value1, false);
                    }
                }
            }

            return Score;
        }

        protected override double ScoreAttract(AIProcessor AI, int CastCell, EffectInfos Effect, List<Fighter> Targets, bool InvokPreview)
        {
            if (InvokPreview)
            {
                return 0;
            }

            int Score = 0;
            char d = Pathfinder.getDirBetweenTwoCase(CastCell, AI.myFighter.CellId, AI.myFight.Map, true);
            var EndCell = AI.myFight.GetCell(Pathfinder.DirToCellID(AI.myFighter.CellId, d, AI.myFight.Map, true));
            var StartCell = AI.myFight.GetCell(CastCell);
            if (StartCell != null && EndCell != null)
            {
                var target = StartCell.GetFighter();
                if (target != null)
                {
                    Score += ScorePush(AI, target, Pathfinder.GetDirection(AI.myFight.Map, CastCell, AI.myFighter.CellId), Pathfinder.GoalDistance(AI.myFight.Map, CastCell, AI.myFighter.CellId), true);
                }
            }

            return Score;
        }

        private int ScorePush(AIProcessor AI, Fighter Target, int Direction, int Length, bool Fear)
        {
            bool isAlly = Target.Team == AI.myFighter.Team;
            List<Fighter> TargetList = new List<Fighter>() { Target };
            var LastCell = Target.Cell;
            int Score = 0;

            foreach (var Layer in Target.Cell.GetObjects<FightGroundLayer>())//On cherche à savoir si décaller de cette cellule est utile
            {
                int LayerScore = 0;
                foreach (var Effect in Layer.CastSpell.Effects)
                {
                    LayerScore = (int)Math.Floor(AIAction.AIActions[AIActionEnum.SELF_ACTING].GetEffectScore(AI, -1, -1, Effect, TargetList, true, true));
                }
                if (Layer is FightBlypheLayer)
                {
                    LayerScore *= 2;
                }
                Score += LayerScore;
            }

            int PathScore = 4;
            int FinalLength = 0;
            for (int i = 0; i < Length; i++)
            {
                var NextCell = Target.Fight.GetCell(Pathfinder.NextCell(Target.Fight.Map, LastCell.Id, Direction));
                if (NextCell != null)
                {
                    LastCell = NextCell;
                }

                if (NextCell != null && NextCell.IsWalkable())
                {
                    if (NextCell.HasGameObject(FightObjectType.OBJECT_FIGHTER) || NextCell.HasGameObject(FightObjectType.OBJECT_STATIC) || Target.States.HasState(FighterStateEnum.STATE_ENRACINER))
                    {
                        if (!Fear)
                        {
                            PathScore *= EffectPush.RANDOM_PUSHDAMAGE.Next(4, 7);
                            if (isAlly)
                            {
                                PathScore *= -1;
                            }
                        }
                        break;
                    }
                    else if (NextCell.HasGameObject(FightObjectType.OBJECT_TRAP))
                    {//On Stop seulement : ce genre de calcul se fera a la fin.
                        break;
                    }
                }
                else
                {
                    if (!Fear)
                    {
                        PathScore *= EffectPush.RANDOM_PUSHDAMAGE.Next(4, 7);
                        if (isAlly)
                        {
                            PathScore *= -1;
                        }
                    }
                    break;
                }
                FinalLength += 1;
            }
            Score += FinalLength * PathScore;
            if (LastCell != Target.Cell)
            {
                foreach (var Layer in LastCell.GetObjects<FightGroundLayer>())
                {
                    int LayerScore = 0;
                    foreach (var Effect in Layer.CastSpell.Effects)
                    {
                        LayerScore += (int)Math.Floor(AIAction.AIActions[AIActionEnum.SELF_ACTING].GetEffectScore(AI, -1, -1, Effect, TargetList, false, true));
                    }
                    if (Layer is FightTrapLayer)// TODO : Calculate if traplayer others targets
                    {
                        LayerScore *= 4;//Immediat
                    }
                    else if (Layer is FightBlypheLayer)
                    {
                        LayerScore *= 2;//Debut de tour
                    }
                    Score += LayerScore;
                }
            }

            return Score;
        }

        protected override double ScoreDeplace(AIProcessor AI, int CastCell, EffectInfos Effect, List<Fighter> Targets, bool InvokPreview, bool isThrow)
        {
            if (InvokPreview)
            {
                return 0;
            }
            int Score = 0;

            if (isThrow)
            {
                var TargetCell = AI.myFight.GetCell(CastCell);
                if (TargetCell != null)
                {
                    var Infos = AI.myFighter.States.FindState(FighterStateEnum.STATE_PORTEUR);
                    if (Infos != null)
                    {
                        var Target = Infos.Target;
                        if (Target != null && Target.States.HasState(FighterStateEnum.STATE_PORTE))
                        {
                            List<Fighter> TargetList = new List<Fighter>() { Target };
                            foreach (var Layer in TargetCell.GetObjects<FightGroundLayer>())
                            {
                                int LayerScore = 0;
                                foreach (var EffectLayer in Layer.CastSpell.Effects)
                                {
                                    LayerScore += (int)Math.Floor(AIAction.AIActions[AIActionEnum.SELF_ACTING].GetEffectScore(AI, -1, -1, Effect, TargetList, false, true));
                                }
                                if (Layer is FightTrapLayer)
                                {
                                    LayerScore *= 4;//Immediat
                                }
                                else if (Layer is FightBlypheLayer)
                                {
                                    LayerScore *= 2;//Debut de tour
                                }
                                Score += LayerScore;
                            }
                        }
                    }
                }
            }
            else
            {

            }

            return Score;
        }

        protected override double ScoreExchangePlace(AIProcessor AI, int CasterCell, int CastCell, EffectInfos Effect, List<Fighter> Targets, bool InvokPreview)
        {
            int Score = 0;
            var TargetCell = AI.myFight.GetCell(CastCell);
            var LaunchCell = AI.myFight.GetCell(CasterCell);
            if (TargetCell != null)
            {
                var Target = TargetCell.GetFighter();
                if (Target != null)
                {
                    List<Fighter> TargetList = new List<Fighter>() { AI.myFighter };
                    foreach (var Layer in TargetCell.GetObjects<FightGroundLayer>())
                    {
                        int LayerScore = 0;
                        foreach (var EffectLayer in Layer.CastSpell.Effects)
                        {
                            LayerScore += (int)Math.Floor(AIAction.AIActions[AIActionEnum.SELF_ACTING].GetEffectScore(AI, -1, -1, Effect, TargetList, false, true));
                        }
                        if (Layer is FightTrapLayer)
                        {
                            LayerScore *= 4;//Immediat
                        }
                        else if (Layer is FightBlypheLayer)
                        {
                            LayerScore *= 2;//Debut de tour
                        }
                        Score += LayerScore;
                    }

                    TargetList = new List<Fighter>() { Target };
                    foreach (var Layer in LaunchCell.GetObjects<FightGroundLayer>())
                    {
                        int LayerScore = 0;
                        foreach (var EffectLayer in Layer.CastSpell.Effects)
                        {
                            LayerScore += (int)Math.Floor(AIAction.AIActions[AIActionEnum.SELF_ACTING].GetEffectScore(AI, -1, -1, Effect, TargetList, false, true));
                        }
                        if (Layer is FightTrapLayer)
                        {
                            LayerScore *= 4;//Immediat
                        }
                        else if (Layer is FightBlypheLayer)
                        {
                            LayerScore *= 2;//Debut de tour
                        }
                        Score += LayerScore;
                    }
                }
            }
            return Score;
        }

        protected override double ScoreUseLayer(AIProcessor AI, int CastCell, EffectInfos Effect, List<Fighter> Targets, bool Reverse = false, bool notUseJet = false)
        {
            int Score = 0;
            var spell = SpellTable.Cache[Effect.Value1];
            if (spell != null)
            {
                var spellLevel = spell.GetLevel(Effect.Value2);
                if (spellLevel != null)
                {
                    List<Fighter> possibleTargets = AI.myFight.GetEnnemyTeam(AI.myFighter.Team).GetFighters().FindAll(x => !x.Dead && !x.Left);
                    int LayerScore = 0;
                    foreach (var EffectLayer in spellLevel.Effects)
                    {
                        LayerScore += (int)Math.Floor(AIAction.AIActions[AIActionEnum.SELF_ACTING].GetEffectScore(AI, -1, -1, Effect, possibleTargets, false, true));
                    }
                    if (LayerScore <= 0)
                    {
                        LayerScore = 0;
                        possibleTargets = AI.myFight.GetAllyTeam(AI.myFighter.Team).GetFighters().FindAll(x => !x.Dead && !x.Left);
                        foreach (var EffectLayer in spellLevel.Effects)
                        {
                            LayerScore += (int)Math.Floor(AIAction.AIActions[AIActionEnum.SELF_ACTING].GetEffectScore(AI, -1, -1, Effect, possibleTargets, false, true));
                        }
                    }
                    Score += LayerScore;
                }
            }

            return Score;
        }
    }
}
