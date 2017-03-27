using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Character
{
    public sealed class RandomJet
    {
        private static Random RANDOM_JET_GEN = new Random();

        public RandomJet(int effectId, int effectMinJet, int effectMaxJet)
        {
            this.EffectId = effectId;
            this.Min = effectMinJet;
            this.Max = effectMaxJet;
        }

        public int Min, Max;
        public int EffectId;

        public int GetRandomJet()
        {
            if (Min > Max) return Min;

            return RandomJet.RANDOM_JET_GEN.Next(this.Min, this.Max);
        }
    }
}
