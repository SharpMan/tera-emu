using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Spells;
using System.Runtime.CompilerServices;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;
using Tera.Libs.Enumerations;
using Tera.Libs.Helper;

namespace Tera.WorldServer.World.Fights.FightObjects
{
    public abstract class FightGroundLayer : IFightObject, FightListener, IDisposable
    {
        public Fighter Caster
        {
            get;
            protected set;
        }

        public short Size{
            get;
            protected set;
        }

        protected static FightObjectType[] VIEW_TYPES = { FightObjectType.OBJECT_BLYPHE, FightObjectType.OBJECT_GLYPHE };

        public short Duration{
            get;
            protected set;
        }

        public abstract short Color
        {
            get;
        }

        public SpellLevel CastSpell
        {
            get;
            protected set;
        }

        private FightObjectType myObjectType;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public abstract void Show(Fighter Scout = null);
        [MethodImpl(MethodImplOptions.Synchronized)]
        public abstract void Hide();

        protected int MasterSpellId;
        protected int MasterSpellSpriteId;

        protected List<FightCell> myCells = new List<FightCell>();

        public FightGroundLayer(FightObjectType myObjectType, Fighter Caster, SpellLevel CastSpell, FightCell CastCell, int MasterSpellId, int MasterSpellSpriteId, String Zone, short Duration = -1)
        {
            this.myObjectType = myObjectType;
            this.myCell = CastCell;
            this.Caster = Caster;
            this.CastSpell = CastSpell;
            this.Size = (short)StringHelper.HashToInt(Zone[1]);
            this.Duration = Duration;
            this.MasterSpellId = MasterSpellId;
            this.MasterSpellSpriteId = MasterSpellSpriteId;
            
            //CastCell.AddObject(this);
            Fight.addLayer(this);
            Fight.RegisterFightListener(this);

            foreach (int cellId in CellZone.GetCells(this.Fight.Map, CellId, CellId, Zone))
            {
                var Cell = this.Fight.GetCell(cellId);
                if (Cell != null)
                {
                    myCells.Add(Cell);
                    Cell.AddObject(this);
                }
            }
            
            Show(Caster);
        }

        public Fight Fight { get { return Caster.Fight; } }

        public FightObjectType ObjectType { get { return this.myObjectType; } }

        public FightCell myCell
        {
            get;
            private set;
        }
        public int CellId
        {
            get { return this.myCell.Id; }
            set { }
        }

        public bool CanWalk()
        {
            return true;
        }

        public virtual bool CanStack()
        {
            return true;
        }
         
        public void onLeaveFight(Fighter Fighter)
        {
        }

        public void onEndFight(FightTeam Winners, FightTeam Loosers)
        {
            Dispose();
        }

        public void onStartFight()
        {
        }

        public void onLaunchWeapon(Fighter Launcher, Database.Models.InventoryItemModel Weapon, int TargetCellId, Fighter TargetFighter, Dictionary<Character.WeaponEffect, List<Fighter>> TargetEffects, bool IsCC, bool IsEchec, bool isPunch)
        {
        }

        public void onLaunchSpell(Fighter Launcher, SpellLevel Spell, int TargetCellId, Fighter TargetFighter, Dictionary<EffectInfos, List<Fighter>> TargetEffects, bool IsCC, bool IsEchec)
        {
        }

        public void onActorMoved(Fighter Fighter, Maps.MovementPath Path, FightCell NewCell)
        {
            if (NewCell.HasIFightObject(this))
            {
                onWalkOnLayer(Fighter, NewCell);
            }
        }

        public void onBeginTurn(Fighter newFighter)
        {
            if (newFighter.Cell.HasIFightObject(this))
            {
                onTouchAtBeginTurn(newFighter);
            }
        }
        
        public void onMiddleTurn(Fighter fighter)
        {
        }

        bool firstEndTurn = true;
        public void onEndTurn(Fighter endFighter, bool Finish = false)
        {
            if (Finish)
            {
                return;
            }
            if (Duration != -1 && endFighter == Caster && !firstEndTurn)
            {
                if (Duration > 0)
                {
                    Duration--;
                }
                if (Duration == 0)
                {
                    Dispose();
                    return;
                }
            }
            firstEndTurn = false;
            if (endFighter.Cell.HasIFightObject(this))
            {
                onTouchAtEndTurn(endFighter);
            }
        }

        public void onDie(Fighter fighterDead, Fighter Caster)
        {
            if (fighterDead == this.Caster)
            {
                Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            Fight.unRegisterFightListener(this);
            myCell.RemoveObject(this);
            myCells.ForEach(x => x.RemoveObject(this));
            myCells.Clear();
            Fight.removeLayer(this);
            Hide();
            DisposeOverridable();
        }

        protected abstract void DisposeOverridable();

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected abstract void onTouchAtBeginTurn(Fighter target);
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected abstract void onTouchAtEndTurn(Fighter target);
        [MethodImpl(MethodImplOptions.Synchronized)]
        public abstract void onWalkOnLayer(Fighter fighter, FightCell newCell);

        protected virtual void Triggered(Fighter triggerer)
        {
            this.Fight.SendToFight(new FightGameActionMessage(307, triggerer.ActorId.ToString(), MasterSpellId + "," + CellId + ",0,1,1," + Caster.ActorId));
        }

        public void onApplyGroundLayer(FightGroundLayer layer, Fighter Caster, SpellLevel Spell, int TargetCellId, Fighter TargetFighter, Dictionary<EffectInfos, List<Fighter>> TargetEffects)
        {
        }

        public bool EqualsView(FightGroundLayer o)
        {
            if (o == this)
            {
                return true;
            }
            else if (o is FightBlypheLayer)
            {
                return (o as FightBlypheLayer).Color == this.Color && (o as FightBlypheLayer).Size == this.Size && (o as FightBlypheLayer).CellId == this.CellId;
            }
            else if (o is FightGlypheLayer)
            {
                return (o as FightGlypheLayer).Color == this.Color && (o as FightGlypheLayer).Size == this.Size && (o as FightGlypheLayer).CellId == this.CellId;
            }
            else return false;
        }
    }
}
