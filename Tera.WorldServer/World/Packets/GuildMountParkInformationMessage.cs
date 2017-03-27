using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GuildMountParkInformationMessage : PacketBase
    {
        public Guild guild;

        public GuildMountParkInformationMessage(Guild guild)
        {
            this.guild = guild;
        }

        public override string Compile()
        {
            StringBuilder sb = new StringBuilder("gIF");
            foreach (MountPark data in MountParkTable.Cache.Where(x => x.get_guild() != null && x.get_guild().ID == guild.ID))
            {
                sb.Append("|").Append(data.get_map().Id).Append(";").Append(data.get_size()).Append(";").Append(data.get_size());
            }
            return sb.ToString();
        }
    }
}
