using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.World.Spells;

namespace Tera.WorldServer.World.Fights
{
    public sealed class FighterSpell
    {
        private Dictionary<int, List<SpellTarget>> myTargets = new Dictionary<int, List<SpellTarget>>();
        public Dictionary<int, SpellTurnNumber> myTurnNumber = new Dictionary<int, SpellTurnNumber>();

        public bool CanLaunchSpell(SpellLevel Spell, long TargetId)
        {
            if (Spell.TurnNumber > 0)
            {
                if (this.myTurnNumber.ContainsKey(Spell.SpellCache.ID))
                {
                    if (this.myTurnNumber[Spell.SpellCache.ID] != null)
                    {
                        if (this.myTurnNumber[Spell.SpellCache.ID].TurnNumber > 0)
                            return false;
                    }
                }
            }
            if (Spell.MaxPerTurn == 0 && Spell.MaxPerPlayer == 0)
                return true;

            if (Spell.MaxPerTurn > 0)
            {
                if (this.myTargets.ContainsKey(Spell.SpellCache.ID))
                {
                    if (this.myTargets[Spell.SpellCache.ID].Count >= Spell.MaxPerTurn)
                        return false;
                }
            }

            if (Spell.MaxPerPlayer > 0)
            {
                if (this.myTargets.ContainsKey(Spell.SpellCache.ID))
                {
                    if (this.myTargets[Spell.SpellCache.ID].Count(x => x.TargetId == TargetId) >= Spell.MaxPerPlayer)
                        return false;
                }
            }

            return true;
        }

        public void Actualise(SpellLevel Spell, long TargetId)
        {
            if (Spell.TurnNumber > 0)
            {
                if (!this.myTurnNumber.ContainsKey(Spell.SpellCache.ID))
                {
                    this.myTurnNumber.Add(Spell.SpellCache.ID, new SpellTurnNumber(Spell.TurnNumber));
                }
                else
                {
                    this.myTurnNumber[Spell.SpellCache.ID].TurnNumber = Spell.TurnNumber;
                }
            }

            if (Spell.MaxPerTurn == 0 && Spell.MaxPerPlayer == 0)
                return;

            if (Spell.MaxPerTurn > 0)
            {
                if (!this.myTargets.ContainsKey(Spell.SpellCache.ID))
                {
                    this.myTargets.Add(Spell.SpellCache.ID, new List<SpellTarget>());
                }
                else
                {
                    this.myTargets[Spell.SpellCache.ID].Add(new SpellTarget(TargetId));
                }
            }

            if (Spell.MaxPerPlayer > 0)
            {
                if (this.myTargets.ContainsKey(Spell.SpellCache.ID))
                {
                    this.myTargets[Spell.SpellCache.ID].Add(new SpellTarget(TargetId));
                }
                else
                {
                    this.myTargets.Add(Spell.SpellCache.ID,new List<SpellTarget> { new SpellTarget(TargetId) });
                }
            }
        }

        public void EndTurn()
        {
            foreach (var Targets in myTargets.Values)
                Targets.Clear();

            foreach (var TurnNumber in this.myTurnNumber.Values)
                TurnNumber.Decrement();

        }
    }

    public sealed class SpellTurnNumber
    {
        public int TurnNumber
        {
            get;
            set;
        }

        public SpellTurnNumber(int TurnNumber)
        {
            this.TurnNumber = TurnNumber;
        }

        public void Decrement()
        {
            this.TurnNumber--;
        }
    }

    public sealed class SpellTarget
    {
        public long TargetId
        {
            get;
            set;
        }

        public SpellTarget(long TargetId)
        {
            this.TargetId = TargetId;
        }
    }
}
