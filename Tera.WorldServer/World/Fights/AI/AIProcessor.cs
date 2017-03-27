using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Spells;
using Tera.WorldServer.World.Fights.Exceptions;
using Tera.Libs.Utils;
using Tera.WorldServer.World.Scripting.Fights;
using Tera.WorldServer.World.Scripting;
using Tera.WorldServer.World.Fights.Effects;
using Tera.WorldServer.Database.Tables;
using System.Threading;
using Tera.WorldServer.World.Fights.AI;

namespace Tera.WorldServer.World.Fights
{
    public class AIProcessor
    {
        public Fight myFight
        {
            get;
            protected set;
        }
        public VirtualFighter myFighter
        {
            get;
            protected set;
        }
        protected List<SpellLevel> mySpells = new List<SpellLevel>();
        public AINeuron myNeuron
        {
            private set;
            get;
        }

        public AIAction.AIActionEnum Mode
        {
            get;
            set;
        }

        public static IAMindEnum IAMindOf(Fighter f)
        {
            try
            {
                if (f == null)
                {
                    return IAMindEnum.PASSIVE;
                }
                else if (f is DoubleFighter)
                {
                    return IAMindEnum.BLOCKER;
                }
                else if (f is PercepteurFighter || f.ActorType == GameActorTypeEnum.TYPE_TAX_COLLECTOR)
                {
                    return IAMindEnum.TAXCOLLECTOR;
                }
                else if (f is MonsterFighter)
                {
                    return (IAMindEnum)(f as MonsterFighter).Grade.Monster.AI_TYPE;
                }
                else
                {
                    return IAMindEnum.PASSIVE;
                }
            }
            catch (Exception e)
            {
                return IAMindEnum.PASSIVE;
            }
        }

        public static JSIAMind GetMindOf(IAMindEnum type)
        {
            JSIAMind mind = JSKernel.FightIAMinds.Get((int)type);
            if (mind == null)
            {
                Logger.Error("JSIAMind[" + (int)type + "] doesn't exist");
                mind = JSKernel.FightIAMinds.Get(0);
            }
            return mind;
        }

        public long whenIStartIA
        {
            get;
            private set;
        }
        private int usedneurons = 0;

        public int UsedNeurons
        {
            get
            {
                return usedneurons;
            }
        }

        private IAMindEnum type;
        public AIProcessor(Fight Fight, VirtualFighter Fighter)
        {
            this.type = IAMindOf(Fighter);
            this.myFight = Fight;
            this.myFighter = Fighter;
            this.mySpells = Fighter.getSpells();
            this.mySpells.RemoveAll(x => x == null);
            this.Mode = AIAction.AIActionEnum.SELF_ACTING;
        }

        public void runAI()
        {
            whenIStartIA = Environment.TickCount;
            usedneurons = 0;
            JSIAMind mind = GetMindOf(type);
            try
            {
                do
                {
                    if (!CanPlay())
                    {
                        break;
                    }

                    myNeuron = new AINeuron();

                    usedneurons++;
                    try
                    {
                        mind.Play(this);
                    }
                    catch (FightException Fe)
                    {
                        throw Fe;
                    }
                    catch (FighterException fe)
                    {
                        throw fe;
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                    finally
                    {
                        if (myNeuron != null)
                        {
                            myNeuron.Dispose();
                            myNeuron = null;
                        }
                    }
                    Wait(10);
                } while (CanPlay());
            }
            catch (FightException Fe)
            {
                Fe.finalAction();
            }
            catch (FighterException fe)
            {
                fe.finalAction();
            }
            finally
            {
                whenIStartIA = 0;
                usedneurons = 0;
                mind = null;
            }
        }

        # region "Fonctions utiles pour dev une IA"

        public void Wait(int Millis)
        {
            System.Threading.Thread.Sleep(Millis);
        }

        public long RemainingTime
        {
            get
            {
                return (whenIStartIA + myFight.GetTurnTime()) - Environment.TickCount;
            }
        }

        public bool CanPlay()
        {
            return myFighter != null
                && !myFighter.Dead
                && (RemainingTime > 0)
                && (myFighter.AP > 0 || myFighter.MP > 0)
                && usedneurons <= 10
                && !myFight.OnlyOneTeam();
        }

        public void Stop()
        {
            throw new StopAIException("Arrêt de l'IA demandé", myFight, myFighter);
        }

        #endregion

        # region "Calculs"

        public void SelectBestAction()
        {
            AIAction Action = null;
            if (AIAction.AIActions.TryGetValue(Mode, out Action)){
                foreach (var Cell in myNeuron.myReachableCells)
                {
                    foreach (var Spell in this.mySpells)
                    {
                        this.SelectBestSpell(Action, Spell, Cell);
                    }
                }
             }
        }

        public void ApplyBestAction()
        {
            if (myNeuron.myBestSpell != null)
            {
                if (myNeuron.myBestMoveCell != this.myFighter.CellId && myNeuron.myBestMoveCell != 0)
                {
                    var Path = new Pathmaker().Pathing(this.myFight.Map, this.myFighter.CellId, myNeuron.myBestMoveCell, true, this.myFighter.MP, true, this.myFight);
                    if (Path != null)
                    {
                        Path = Pathfinder.IsValidPath(this.myFight, this.myFighter, this.myFighter.CellId, Path.ToString());
                        var Action = this.myFight.TryMove(this.myFighter, Path);
                        if (Action != null)
                        {
                            System.Threading.Thread.Sleep(100 + (Path.MovementTime));

                            this.myFight.StopAction(this.myFighter);
                        }
                    }
                }
                if (myNeuron.myFirstTargetIsMe)
                {
                    myNeuron.myBestCastCell = this.myFighter.CellId;
                }
                this.myFight.LaunchSpell(this.myFighter, myNeuron.myBestSpell, myNeuron.myBestCastCell, true);
                myNeuron.myAttacked = true;

                System.Threading.Thread.Sleep(1250 + Pathfinder.GoalDistance(myFight.Map, myFighter.CellId, myNeuron.myBestCastCell)*250);

                this.myFight.StopAction(this.myFighter);
            }
        }

        protected void SelectBestSpell(AIAction Action, SpellLevel Spell, int CurrentCell)
        {
            // PO max du sort + stats du lanceur
            var MaxPo = Spell.MaxPO + (Spell.AllowPOBoost ? this.myFighter.Stats.GetTotal(EffectEnum.AddPO) : 0);

            // S'il a des malus qui reduisent trop
            if (MaxPo - Spell.MinPO < 1)
                MaxPo = Spell.MinPO;
            List<int> cells = new List<int>();
            if (!Spell.InLine)
            {
                cells.AddRange(CellZone.GetCircleCells(this.myFight.Map, CurrentCell, MaxPo));
            }else{
                cells.AddRange(CellZone.GetCrossCells(this.myFight.Map, CurrentCell, MaxPo));
            }
            if (!cells.Contains(CurrentCell))
            {
                cells.Add(CurrentCell);
            }
            foreach (var Cell in cells)
            {
                var FightCell = this.myFight.GetCell(Cell);

                if (FightCell != null)
                {
                    Fighter FirstTarget = null;
                    if (FightCell.HasUnWalkableFighter())
                    {
                        FirstTarget = FightCell.GetObjects<Fighter>()[0];
                    }
                    if (this.myFight.CanLaunchSpell(this.myFighter, Spell, CurrentCell, Cell, FirstTarget == null ? -1 : FirstTarget.ActorId))
                    {
                        var Score = this.GetSpellScore(Action, Spell, CurrentCell, Cell);

                        var Distance = (Pathfinder.GoalDistance(this.myFight.Map, this.myFighter.CellId, CurrentCell) * 5);
                        if (Score > 0)
                            if (Score - Distance < 0)
                                Score = 1;
                            else
                                Score -= Distance;

                        if (FightCell.HasEnnemy(this.myFighter.Team) != null)
                        {
                            if (Score > 0)
                                Score += 50;
                        }
                        
                        if (Score >  myNeuron.myBestScore)
                        {
                            myNeuron.myBestScore = (int)Score;
                            myNeuron.myBestSpell = Spell;
                            myNeuron.myFirstTargetIsMe = FirstTarget == this.myFighter;
                            //if (myNeuron.myFirstTargetIsMe)
                            //{
                            //    myNeuron.myBestCastCell = this.myFighter.CellId;
                            //}
                            //else
                            //{
                            myNeuron.myBestCastCell = Cell;
                            //}
                            myNeuron.myBestMoveCell = CurrentCell;
                        }
                    }
                }
            }
        }

        public void InitCells()
        {
            myNeuron.myReachableCells.Clear();
            myNeuron.myReachableCells.Add(this.myFighter.Cell.Id);

            for (int i = 1; i < this.myFight.Map.CellsCount; i++)
            {
                if (this.myFight.IsCellWalkable(i))
                {
                    if (Pathfinder.GoalDistance(this.myFight.Map, this.myFighter.CellId, i) <= this.myFighter.MP)
                    {
                         myNeuron.myReachableCells.Add(i);
                    }
                }
            }
        }

        protected double GetSpellScore(AIAction Action, SpellLevel Spell, int CurrentCellId, int CastCell)
        {
            double Score = 0;

            var cellsTargetsCache = new Dictionary<string, List<int>>();
            int num = 0;
            int TE = 0;
            List<Fighter> Targets = null;
            foreach (var Effect in Spell.Effects)
            {
                if (Spell.SpellCache != null ? Spell.SpellCache.effectTargets.Count > num : false)
                {
                    TE = Spell.SpellCache.effectTargets.ToArray()[num];
                }
                if (Effect.RangeType == "C_")
                {
                    Targets = this.myFight.Fighters;
                    foreach (var fighter in this.myFight.Fighters)
                    {
                        if (fighter.Dead)
                        {
                            Targets.Remove(fighter);
                        }
                        //Ne touches pas les alliés
                        if (((TE & 1) == 1) && (fighter.Team.Id == myFighter.Team.Id))
                        {
                            Targets.Remove(fighter);
                        }
                        if ((((TE >> 1) & 1) == 1) && (fighter.ActorId == myFighter.ActorId))
                        {
                            Targets.Remove(fighter);
                        }
                        //Ne touche pas les ennemies
                        if ((((TE >> 2) & 1) == 1) && (fighter.Team.Id != myFighter.Team.Id))
                        {
                            Targets.Remove(fighter);
                        }
                        //Ne touche pas les combatants (seulement invocations)
                        if ((((TE >> 3) & 1) == 1) && (fighter.Invocator == null))
                        {
                            Targets.Remove(fighter);
                        }
                        //Ne touche pas les invocations
                        if ((((TE >> 4) & 1) == 1) && (fighter.Invocator != null))
                        {
                            Targets.Remove(fighter);
                        }
                        //N'affecte que le lanceur
                        if ((((TE >> 5) & 1) == 1) && (fighter.ActorId != myFighter.ActorId))
                        {
                            Targets.Remove(fighter); ;
                        }
                    }
                }
                else
                {
                    List<int> TargetCells = null;
                    if (!cellsTargetsCache.ContainsKey(Effect.RangeType))
                    {
                        TargetCells = CellZone.GetCells(this.myFight.Map, CastCell, CurrentCellId, Effect.RangeType);
                        if (!TargetCells.Contains(CastCell))
                        {
                            TargetCells.Add(CastCell);
                        }
                        cellsTargetsCache.Add(Effect.RangeType, TargetCells);
                    }
                    else
                    {
                        TargetCells = cellsTargetsCache[Effect.RangeType];
                    }
                    Targets = new List<Fighter>();
                    foreach (var Cell in TargetCells)
                    {
                        var FightCell = this.myFight.GetCell(Cell);
                        if (FightCell == null) continue;
                        //myFight.SendToFight(new FightShowCell(this.myFighter.ActorId, Cell));
                        Targets.AddRange(FightCell.GetObjects<Fighter>());
                    }
                    List<Fighter> newList = new List<Fighter>();
                    newList.AddRange(Targets);
                    foreach (var fighter in newList)
                    {
                        if (fighter.Dead)
                        {
                            Targets.Remove(fighter);
                        }
                        //Ne touches pas les alliés
                        if (((TE & 1) == 1) && (fighter.Team.Id == myFighter.Team.Id))
                        {
                            Targets.Remove(fighter);
                        }
                        if ((((TE >> 1) & 1) == 1) && (fighter.ActorId == myFighter.ActorId))
                        {
                            Targets.Remove(fighter);
                        }
                        //Ne touche pas les ennemies
                        if ((((TE >> 2) & 1) == 1) && (fighter.Team.Id != myFighter.Team.Id))
                        {
                            Targets.Remove(fighter);
                        }
                        //Ne touche pas les combatants (seulement invocations)
                        if ((((TE >> 3) & 1) == 1) && (fighter.Invocator == null))
                        {
                            Targets.Remove(fighter);
                        }
                        //Ne touche pas les invocations
                        if ((((TE >> 4) & 1) == 1) && (fighter.Invocator != null))
                        {
                            Targets.Remove(fighter);
                        }
                        //N'affecte que le lanceur
                        if ((((TE >> 5) & 1) == 1) && (fighter.ActorId != myFighter.ActorId))
                        {
                            Targets.Remove(fighter);
                        }
                    }
                }
                //Si le sort n'affecte que le lanceur et que le lanceur n'est pas dans la zone
                if (((TE >> 5) & 1) == 1)
                {
                        if(!Targets.Contains(myFighter))Targets.Add(myFighter);
                }

                if (Targets.Count > 0 || (Spell.NeedEmptyCell && Targets.Count == 0))
                {
                    Score += Math.Floor(Action.GetEffectScore(this, CurrentCellId, CastCell, Effect, Targets));
                }
                num++;
            }
            return Score;
        }

        #endregion

        private bool React()
        {
            SelectBestAction();
            if (myNeuron.myBestScore > 0)//En fonction des calculs, signifie "Action voulue trouvée"
            {
                ApplyBestAction();
                return true;
            }
            return false;
        }

        /*
         * Automatic Best Action
         */
        public bool SelfAction()
        {
            this.Mode = AIAction.AIActionEnum.SELF_ACTING;
            return React();
        }

        public bool MadSelfAction()
        {
            this.Mode = AIAction.AIActionEnum.MAD;
            return React();
        }

        /*
         * Attack
         */
        public bool Attack()
        {
            this.Mode = AIAction.AIActionEnum.ATTACK;
            return React();
        }

        /*
         * Buff Actions
         */
        public bool BuffMe()
        {
            this.Mode = AIAction.AIActionEnum.BUFF_HIMSELF;
            return React();
        }

        public bool BuffAlly()
        {
            this.Mode = AIAction.AIActionEnum.BUFF_ALLY;
            return React();
        }

        public bool Buff()
        {
            return BuffAlly() || BuffMe();
        }

        /*
         * Debuff Actions
         */
        public bool DebuffAlly()
        {
            this.Mode = AIAction.AIActionEnum.DEBUFF_ALLY;
            return React();
        }

        public bool DebuffEnnemy()
        {
            this.Mode = AIAction.AIActionEnum.DEBUFF_ENNEMY;
            return React();
        }

        public bool Debuff()
        {
            return DebuffAlly() || DebuffEnnemy();
        }

        /*
         * Heal Actions
         */
        public bool HealMe()
        {
            this.Mode = AIAction.AIActionEnum.HEAL_HIMSELF;
            return React();
        }

        public bool HealAlly()
        {
            this.Mode = AIAction.AIActionEnum.HEAL_ALLY;
            return React();
        }

        public bool Heal()
        {
            return HealAlly() || HealMe();
        }

        /*
         * Actions de support
         */
        public bool Support()
        {
            //return Repels() || HealAlly() || Debuff() || Subbuff() || BuffAlly() || Invocate();
            this.Mode = AIAction.AIActionEnum.SUPPORT;
            return React();
        }

        /*
         * Invocation
         */
        public bool Invocate()
        {
            this.Mode = AIAction.AIActionEnum.INVOK;
            return React();
        }

        /*
         * Repousse les ennemis
         */
        public bool Repels()
        {
            this.Mode = AIAction.AIActionEnum.REPELS;
            return React();
        }

        /*
         * Ralenti les ennemis 
         */
        public bool Subbuff()
        {
            this.Mode = AIAction.AIActionEnum.SUBBUFF;
            return React();
        }

        public bool MoveTo(Fighter target, int MaxDistance)
        {
            var BaseDistance = Pathfinder.GoalDistance(this.myFight.Map, this.myFighter.CellId, target.CellId);
            if (MaxDistance > BaseDistance)
            {
                MaxDistance = BaseDistance;
            }
            var BaseDistanceDeplacement = BaseDistance - this.myFighter.MP;
            if (BaseDistanceDeplacement <= 1) BaseDistanceDeplacement = 1;
            if (BaseDistanceDeplacement <= MaxDistance)
            {
                var BestCell = -1;
                var BestDistance = MaxDistance;

                foreach (var Cell in Pathfinding.GetCircleZone(target.CellId, MaxDistance, this.myFight.Map))
                {
                    FightCell fCell = myFight.GetCell(Cell);
                    if (fCell != null && fCell.CanWalk())
                    {
                        var Distance = Pathfinder.GoalDistance(this.myFight.Map, target.CellId, Cell);
                        if (Distance < BestDistance)
                        {
                            BestCell = Cell;
                            BestDistance = Distance;
                        }
                    }
                }

                if (BestCell != -1 && BestCell != this.myFighter.CellId)
                {
                    var Path = new Pathmaker().Pathing(this.myFight.Map, this.myFighter.CellId, BestCell, true, this.myFighter.MP, true, this.myFight);
                    Path = Pathfinder.IsValidPath(this.myFight, this.myFighter, this.myFighter.CellId, Path.ToString());
                    if (Path != null && Pathfinder.GoalDistance(this.myFight.Map, target.CellId, Path.EndCell) <= MaxDistance)
                    {
                        var Action = this.myFight.TryMove(this.myFighter, Path);

                        if (Action != null)
                        {
                            System.Threading.Thread.Sleep(100 + (Path.MovementTime));

                            this.myFight.StopAction(this.myFighter);

                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool MoveTo(Fighter target, int MinDistance, int MaxDistance)
        {
            var BaseDistance = Pathfinder.GoalDistance(this.myFight.Map, this.myFighter.CellId, target.CellId);
            if (MaxDistance < MinDistance)
            {
                MaxDistance = MinDistance;
            }
            if (MaxDistance > BaseDistance)
            {
                MaxDistance = BaseDistance;
            }
            var BaseDistanceDeplacement = BaseDistance - this.myFighter.MP;
            if (BaseDistanceDeplacement <= 1) BaseDistanceDeplacement = 1;
            if (BaseDistanceDeplacement <= MaxDistance && BaseDistanceDeplacement >= MinDistance)
            {
                var BestCell = -1;
                var BestDistance = MaxDistance;

                foreach (var Cell in Pathfinding.GetCircleZone(target.CellId, MaxDistance, this.myFight.Map))
                {
                    FightCell fCell = myFight.GetCell(Cell);
                    if (fCell != null && fCell.CanWalk())
                    {
                        var Distance = Pathfinder.GoalDistance(this.myFight.Map, target.CellId, Cell);
                        if (Distance < BestDistance && Distance > MinDistance)
                        {
                            BestCell = Cell;
                            BestDistance = Distance;
                        }
                    }
                }

                if (BestCell != -1 && BestCell != this.myFighter.CellId)
                {
                    var Path = new Pathmaker().Pathing(this.myFight.Map, this.myFighter.CellId, BestCell, true, this.myFighter.MP, true, this.myFight);
                    Path = Pathfinder.IsValidPath(this.myFight, this.myFighter, this.myFighter.CellId, Path.ToString());
                    var FinalDistance = Pathfinder.GoalDistance(this.myFight.Map, target.CellId, Path.EndCell);
                    if (Path != null && (FinalDistance <= MaxDistance && FinalDistance >= MinDistance))
                    {
                        var Action = this.myFight.TryMove(this.myFighter, Path);

                        if (Action != null)
                        {
                            System.Threading.Thread.Sleep(100 + (Path.MovementTime));

                            this.myFight.StopAction(this.myFighter);

                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool moveToEnnemy()
        {
            var BestDistance = 64;
            Fighter BestEnnemy = null;
            foreach (var Ennemy in this.myFight.GetEnnemyTeam(this.myFighter.Team).GetAliveFighters())
            {
                var Distance = Pathfinder.GoalDistance(this.myFight.Map, this.myFighter.CellId, Ennemy.CellId);

                if (Distance < BestDistance)
                {
                    BestDistance = Distance;
                    BestEnnemy = Ennemy;
                }
            }
            if (BestEnnemy != null)
            {
                return MoveTo(BestEnnemy, 64);
            }
            else return false;
        }

        public bool moveFar(int MaxDistance = 64)
        {
            var BestDistance = MaxDistance;
            var BestEnnemyCell = -1;

            foreach (var Ennemy in this.myFight.GetEnnemyTeam(this.myFighter.Team).GetAliveFighters())
            {
                var Distance = Pathfinder.GoalDistance(this.myFight.Map, this.myFighter.CellId, Ennemy.CellId);

                if (Distance < BestDistance)
                {
                    BestDistance = Distance;
                    BestEnnemyCell = Ennemy.CellId;
                }
            }

            if (BestEnnemyCell != -1)
            {
                var BestCell = -1;
                BestDistance = 0;

                foreach (var Cell in Pathfinding.GetCircleZone(myFighter.CellId, myFighter.MP, this.myFight.Map))
                {
                    FightCell fCell = myFight.GetCell(Cell);
                    if (fCell != null && fCell.CanWalk())
                    {
                        var Distance = Pathfinder.GoalDistance(this.myFight.Map, BestEnnemyCell, Cell);
                        if (Distance > BestDistance)
                        {
                            bool Ok = true;
                            foreach (var AroundCell in Pathfinding.GetCircleZone(myFighter.CellId, Distance, this.myFight.Map))
                            {
                                FightCell fAroundCell = myFight.GetCell(AroundCell);
                                if (fAroundCell != null && fAroundCell.HasEnnemy(myFighter.Team) != null)
                                {
                                    if (Pathfinder.GoalDistance(this.myFight.Map, Cell, AroundCell) < Distance)
                                    {
                                        Ok = false;
                                        break;
                                    }
                                }
                            }
                            if (Ok)
                            {
                                BestCell = Cell;
                                BestDistance = Distance;
                            }
                        }
                    }
                }

                if (BestCell != -1 && BestCell != this.myFighter.CellId)
                {
                    var Path = new Pathmaker().Pathing(this.myFight.Map, this.myFighter.CellId, BestCell, true, this.myFighter.MP, true, this.myFight);
                    Path = Pathfinder.IsValidPath(this.myFight, this.myFighter, this.myFighter.CellId, Path.ToString());
                    if (Path != null)
                    {
                        var Action = this.myFight.TryMove(this.myFighter, Path);

                        if (Action != null)
                        {
                            System.Threading.Thread.Sleep(100 + (Path.MovementTime));

                            this.myFight.StopAction(this.myFighter);
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
