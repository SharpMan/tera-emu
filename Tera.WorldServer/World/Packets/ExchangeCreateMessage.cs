using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Exchanges;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeCreateMessage : PacketBase
    {
        public ExchangeTypeEnum ExchangeType;
        public string Args;

        public ExchangeCreateMessage(ExchangeTypeEnum ExchangeType, string Args = "")
        {
            this.ExchangeType = ExchangeType;
            this.Args = Args;
        }

        public override string Compile()
        {
            return "ECK" + (int)this.ExchangeType + (Args != "" ? "|" + Args : "");
        }
    }
}
