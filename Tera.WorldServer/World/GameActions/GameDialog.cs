using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Controllers;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.GameActions
{
    public class GameDialog : GameAction
    {

        public int NpcID;
        public WorldClient Client;
        public NpcQuestion Question;
        public Npc NPC;


        public GameDialog(WorldClient Client, int NpcID, Npc npc, NpcQuestion nQuestion) : base(GameActionTypeEnum.DIALOG_CREATE, Client.Character)
        {
            this.NpcID = NpcID;
            this.Client = Client;
            this.Question = nQuestion;
            this.NPC = npc;
        }

        public override void Execute()
        {
            if (!ConditionParser.validConditions(this.Client.Character, this.Question.Conditions))
            {
                this.Client.Send(new NpcDialogQuestionMessage(NpcQuestionTable.Cache[this.Question.FalseQuestion], this.Client.Character));
            }
            this.Client.Send(new NpcDialogQuestionMessage(this.Question,this.Client.Character));

            base.Execute();
        }


        

        public override void Abort(params object[] Args)
        {
            try
            {
                String[] infos = Array.ConvertAll(Args, p => (p ?? String.Empty).ToString());
                int qID = int.Parse(infos[0]);
                int rID = int.Parse(infos[1]);
                if (this.Client.Character.myMap.getNPC(NpcID) != null)
                {
                    this.EndExecute();
                }
                NpcReply rep = NpcReplyTable.get(rID);
                if (rep == null)
                {
                    this.EndExecute();
                }
                if (rep.isAnotherDialog())
                {
                    this.EndExecute();
                }
                rep.apply(this.Client.Character);
            }
            catch (Exception e)
            {
                this.EndExecute();
            }

            base.Abort(Args);
        }

        public override void EndExecute()
        {
            this.Client.Send(new NpcEndDialogMessage());

            base.EndExecute();
        }

        public override bool CanSubAction(GameActionTypeEnum ActionType)
        {
            return false;
        }
    }
}
