using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameEffectInformationsMessage : PacketBase
    {
        public EffectEnum Effect;
        public long ActorId;
        public string Value1, Value2, Value3, Chance, Duration, SpellId;

        public GameEffectInformationsMessage(EffectEnum Effect, long ActorId, string Value1, string Value2, string Value3, string Chance, string Duration, string SpellId)
        {
            this.Effect = Effect;
            this.ActorId = ActorId;
            this.Value1 = Value1;
            this.Value2 = Value2;
            this.Value3 = Value3;
            this.Chance = Chance;
            this.Duration = Duration;
            this.SpellId = SpellId;
        }

        public override string Compile()
        {
            return "GIE" + (int)this.Effect + ";" + ActorId + ";" + Value1 + ";" + Value2 + ";" + Value3 + ";" + Chance + ";" + Duration + ";" + SpellId;
        }
    }
}
