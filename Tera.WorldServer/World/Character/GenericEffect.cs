using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Character
{
    public sealed class GenericEffect
    {
        public GenericEffect(EffectEnum EffectId, int Base = 0, int Items = 0, int Dons = 0, int Boosts = 0)
        {
            this.EffectType = EffectId;
            this.Base = Base;
            this.Items = Items;
            this.Dons = Dons;
            this.Boosts = Boosts;
        }

        public EffectEnum EffectType;
        public int Base;
        public int Items;
        public int Dons;
        public int Boosts;

        public void Merge(GenericEffect Effect)
        {
            this.Base += Effect.Base;
            this.Items += Effect.Items;
            this.Dons += Effect.Dons;
            this.Boosts += Effect.Boosts;
        }

        public void UnMerge(GenericEffect Effect)
        {
            this.Base -= Effect.Base;
            this.Items -= Effect.Items;
            this.Dons -= Effect.Dons;
            this.Boosts -= Effect.Boosts;
        }

        public int Total
        {
            get
            {
                return this.Base + this.Items + this.Dons + this.Boosts;
            }
        }

        public override string ToString()
        {
            return this.Base + "," + this.Items + "," + this.Dons + "," + this.Boosts;
        }

        public string ToItemString()
        {
            var SerializedStats = new StringBuilder();

            /*if (ItemTemplateModel.IsWeaponEffect(this.EffectType))
            {
                // TODO JET DES WEAPONS 1dx+x
                SerializedStats.Append(((int)this.EffectType).ToString("x"));
                SerializedStats.Append("#" + this.Items.ToString("x") + "#0#0#");
                SerializedStats.Append("0d0+0");
           }
            else
            {*/
            SerializedStats.Append(((int)this.EffectType).ToString("x"));
            SerializedStats.Append("#" + this.Items.ToString("x") + "#0#0#");
            SerializedStats.Append("0d0+0");
            //        }

            return SerializedStats.ToString();
        }
    }
}
