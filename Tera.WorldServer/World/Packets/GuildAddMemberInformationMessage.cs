using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GuildAddMemberInformationMessage : PacketBase
    {
        public IEnumerable<CharacterGuild> Members;

        public GuildAddMemberInformationMessage(IEnumerable<CharacterGuild> Members)
        {
            this.Members = Members;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("gIM+");

            Packet.Append(string.Join("|", this.Members.Select(x => x.SerializeAs_GuildAddMemberInformationMessage())));

            return Packet.ToString();
        }
    }
}
