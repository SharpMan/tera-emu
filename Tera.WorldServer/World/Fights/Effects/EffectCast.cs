using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Spells;

namespace Tera.WorldServer.World.Fights.Effects
{
    public sealed class EffectCast
    {
        private static Random EFFECT_RANDOM = new Random();

        public static bool IsDamageEffect(EffectEnum EffectType)
        {
            switch (EffectType)
            {
                case EffectEnum.VolTerre:
                case EffectEnum.VolFeu:
                case EffectEnum.VolEau:
                case EffectEnum.VolAir:
                case EffectEnum.VolNeutre:
                case EffectEnum.DamageTerre:
                case EffectEnum.DamageNeutre:
                case EffectEnum.DamageFeu:
                case EffectEnum.DamageEau:
                case EffectEnum.DamageAir:
                    return true;
            }

            return false;
        }

        public EffectEnum EffectType
        {
            get;
            set;
        }

        public EffectEnum SubEffect
        {
            get;
            set;
        }

        public int SpellId
        {
            get;
            set;
        }

        public int CellId
        {
            get;
            set;
        }

        public bool IsReflect
        {
            get;
            set;
        }

        public bool IsPoison
        {
            get;
            set;
        }

        public bool IsCAC
        {
            get;
            set;
        }

        public bool IsTrap
        {
            get;
            set;
        }

        public int GenerateJet(Fighter target)
        {
            if(this.Value2 == -1)return this.Value1;
            else if (target.States.HasState(FighterStateEnum.STATE_MAXIMIZE_EFFECTS)) return this.Value2;
            else if (target.States.HasState(FighterStateEnum.STATE_MINIMIZE_EFFECTS)) return this.Value1;
            else return (this.Value2 == -1 ? this.Value1 : EffectCast.EFFECT_RANDOM.Next(this.Value1, this.Value2));
        }

        public int Value1
        {
            get;
            set;
        }

        public int Value2
        {
            get;
            set;
        }

        public int Value3
        {
            get;
            set;
        }

        public int FakeValue
        {
            get;
            set;
        }

        public int DamageValue
        {
            get;
            set;
        }

        public int Chance
        {
            get;
            set;
        }

        public int Duration
        {
            get;
            set;
        }

        public SpellLevel SpellLevel
        {
            get;
            set;
        }
        
        public Fighter Caster
        {
            get;
            set;
        }

        public List<Fighter> Targets
        {
            get;
            set;
        }

        public EffectCast(EffectEnum EffectType, int SpellId, int CellId, int Value1, int Value2, int Value3, int Chance, int Duration, Fighter Caster, List<Fighter> Targets, bool IsCAC = false, EffectEnum SubEffect = EffectEnum.None, int DamageValue = 0,SpellLevel sl  = null)
        {
            this.EffectType = EffectType;
            this.SpellId = SpellId;
            this.CellId = CellId;
            this.Value1 = Value1;
            this.Value2 = Value2;
            this.Value3 = Value3;
            this.Chance = Chance;
            this.Duration = Duration;
            this.Caster = Caster;
            this.Targets = Targets;
            this.SubEffect = SubEffect;
            this.DamageValue = DamageValue;
            this.IsCAC = IsCAC;
            this.SpellLevel = sl;
        }
    }
}
