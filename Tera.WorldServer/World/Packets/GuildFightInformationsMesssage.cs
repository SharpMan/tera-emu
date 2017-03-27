using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GuildFightInformationsMesssage : PacketBase
    {
        public Guild Guild;

        public GuildFightInformationsMesssage(Guild Guild)
        {
            this.Guild = Guild;
        }

        public override string Compile()
        {
            return "gITM" + TaxCollector.parsetoGuild(Guild.ID);
        }

    }
}
