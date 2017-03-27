using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GuildGeneralInfos : PacketBase
    {
        private Guild guild;
        private Player player;

        public GuildGeneralInfos(Guild guild , Player player)
        {
            this.guild = guild;
            this.player = player;
        }

        public override string Compile()
        {
            long xpMin = ExpFloorTable.GetFloorByLevel(player.GetGuild().Level).Guild;
            long xpMax;
            if (ExpFloorTable.GetFloorByLevel(player.GetGuild().Level + 1) == null)
            {
                xpMax = -1L;
            }
            else
            {
                xpMax = ExpFloorTable.GetFloorByLevel(player.GetGuild().Level + 1).Guild;
            }
            return "gIG" + (guild.CharactersGuildCache.Count <= 9 ? 0 : 1) + "|" + guild.Level + "|" + xpMin + "|" + xpMax + "|" + guild.Experience + "|" + xpMax;
        }

    }
}
