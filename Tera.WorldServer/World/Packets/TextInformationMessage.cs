using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Packets
{
    public sealed class TextInformationMessage : PacketBase
    {
        public TextInformationTypeEnum TextType;
        public int Id;
        public string[] Args;

        public TextInformationMessage(TextInformationTypeEnum Type, int Id, params string[] Args)
        {
            this.TextType = Type;
            this.Id = Id;
            this.Args = Args;
        }

        public override string Compile()
        {
            if (TextType == TextInformationTypeEnum.UNK)
                return "Im" + this.Id + (this.Args.Length > 0 ? ";" : "") + string.Join("~", this.Args);
            else
                return "Im" + (int)this.TextType + this.Id + (this.Args.Length > 0 ? ";" : "") + string.Join("~", this.Args);
        }
    }
}
