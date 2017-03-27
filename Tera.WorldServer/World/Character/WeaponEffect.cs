using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Character
{
    public sealed class WeaponEffect
    {
        public WeaponEffect(int Min, int Max, string Args, EffectEnum EffectType, int Value)
        {
            this.Min = Min;
            this.Max = Max;
            this.Args = Args;
            this.EffectType = EffectType;
            this.Value = Value;
        }

        public int Min;
        public int Max;
        public string Args;
        public EffectEnum EffectType;
        public int Value { get; set; }

        private int getMinWithoutValue
        {
            get
            {
                return Min - Value < 0 ? 0 : Min - Value;
            }
        }

        internal String ToItemString()
        {
            var SerializedStats = new StringBuilder();

            SerializedStats.Append(((int)EffectType).ToString("x").Replace("000000", "").ToLower());
            SerializedStats.Append("#");
            SerializedStats.Append((Min == 0 ? "" : Min.ToString("x")).ToLower());
            SerializedStats.Append("#");
            SerializedStats.Append(Max.ToString("x").ToLower());
            SerializedStats.Append("#0#");
            SerializedStats.Append(this.getMinWithoutValue.ToString("x").ToLower());
            SerializedStats.Append("d");
            SerializedStats.Append(Value.ToString());
            SerializedStats.Append("+");
            SerializedStats.Append(Value.ToString());


            return SerializedStats.ToString();
        }
    }
}
