using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GuildSettingInformationMessage : PacketBase
    {
        public Guild Guild;
        public string Rights;

        public GuildSettingInformationMessage(Guild Guild, string Rights)
        {
            this.Guild = Guild;
            this.Rights = Rights;
        }

        public override string Compile()
        {
            return "gS" + this.Guild.Name + "|" + this.Guild.Emblem.Replace(',', '|') + "|" + this.Rights;
        }
    }
}
