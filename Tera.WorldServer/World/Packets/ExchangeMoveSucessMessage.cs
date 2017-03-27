using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public enum ExchangeMoveType
    {
        TYPE_OBJECT = 'O',
        TYPE_KAMAS = 'K',
    }

    public sealed class ExchangeMoveSucessMessage : PacketBase
    {
        public ExchangeMoveType MoveType;
        public string Args;
        public bool Lower = false;
        public bool Add = false;

        public ExchangeMoveSucessMessage(ExchangeMoveType MoveType, string Args, bool Lower = false, bool Add = false)
        {
            this.MoveType = MoveType;
            this.Args = Args;
            this.Lower = Lower;
            this.Add = Add;
        }

        public override string Compile()
        {
            return "E" + (Lower ? "m" : "M") + "K" + (char)this.MoveType + (Add ? "+" : "-") + Args;
        }
    }
}
