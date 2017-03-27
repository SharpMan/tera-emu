using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GuildGeneralBoostInformations : PacketBase
    {
        public Guild Guild;

        public GuildGeneralBoostInformations(Guild Guild)
        {
            this.Guild = Guild;
        }

        public override string Compile()
        {
            return "gIB" + Guild.PerceptorMaxCount + "|" + TaxCollectorTable.Cache.Values.Where(x => x.GuildID == Guild.ID).Count() + "|" + 100 * Guild.Level + "|" + Guild.Level + "|" + Guild.FightStats.GetEffect(158).Total + "|" + Guild.FightStats.GetEffect(176).Total + "|" + Guild.FightStats.GetEffect(124).Total + "|" + Guild.PerceptorMaxCount + "|" + Guild.Capital + "|" + (1000 + 10 * Guild.Level) + "|" + Guild.compileSpell();
        }

    }
}
