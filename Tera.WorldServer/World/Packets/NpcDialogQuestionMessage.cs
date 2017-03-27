using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public class NpcDialogQuestionMessage : PacketBase
    {
        private NpcQuestion Question;
        private Player Character;

        public NpcDialogQuestionMessage(NpcQuestion npc,Player charc)
        {
            this.Question = npc;
            this.Character = charc;
        }

        public override string Compile()
        {
            var Packet = new StringBuilder("DQ");
            Packet.Append(this.Question.ID);
            if(!this.Question.Args.Equals(""))
            {
                Packet.Append(";").Append(this.Question.parseArguments(this.Question.Args, this.Character));
            }
            Packet.Append('|').Append(this.Question.Reponses);
            return Packet.ToString();
        }
    }
}
