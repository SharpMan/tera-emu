using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GuildBasicInformationMessage : PacketBase
    {
        public Guild Guild;

        public GuildBasicInformationMessage(Guild Guild)
        {
            this.Guild = Guild;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("gIB");

            Packet.Append(this.Guild.PerceptorMaxCount).Append('|');
            Packet.Append(this.Guild.TaxCollectorsCache.Count).Append('|');
            Packet.Append(100 * this.Guild.Level).Append('|');
            Packet.Append(this.Guild.Level).Append('|');
            Packet.Append(this.Guild.BaseStats.GetTotal(EffectEnum.AddPods)).Append('|');
            Packet.Append(this.Guild.BaseStats.GetTotal(EffectEnum.AddProspection)).Append('|');
            Packet.Append(this.Guild.BaseStats.GetTotal(EffectEnum.AddSagesse)).Append('|');
            Packet.Append(this.Guild.PerceptorMaxCount).Append('|');
            Packet.Append(this.Guild.Capital).Append('|');
            Packet.Append(1000 + (10 * this.Guild.Level)).Append('|');
            Packet.Append(this.Guild.compileSpell());

            return Packet.ToString();
        }
    }
}
