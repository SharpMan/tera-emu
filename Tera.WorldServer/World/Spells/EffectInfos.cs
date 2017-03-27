using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Fights.Effects;

namespace Tera.WorldServer.World.Spells
{
    public class EffectInfos
    {
        private static Random RANDOM = new Random();

        public SpellLevel Spell
        {
            get;
            set;
        }

        public EffectEnum EffectType
        {
            get;
            set;
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

        public string RangeType
        {
            get;
            set;
        }

        public int RandomJet
        {
            get
            {
                if (this.Value2 == -1 || this.Value2 < this.Value1)
                    return this.Value1;
                return EffectInfos.RANDOM.Next(this.Value1, this.Value2);
            }
        }



        public EffectInfos(SpellLevel Spell, EffectEnum Type, int v1, int v2, int v3, int Duration, int Chance, string RangeType)
        {
            this.Spell = Spell;
            this.EffectType = Type;
            this.Value1 = v1;
            this.Value2 = v2;
            this.Value3 = v3;
            this.Duration = Duration;
            this.Chance = Chance;
            this.RangeType = RangeType;
        }

        public EffectInfos(EffectCast cast)
        {
            this.Spell = null;
            this.EffectType = cast.EffectType;
            this.Value1 = cast.Value1;
            this.Value2 = cast.Value2;
            this.Value3 = cast.Value3;
            this.Duration = cast.Duration;
            this.Chance = cast.Chance;
            this.RangeType = "Pa";
        }
    }
}
