using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Actions
{
    public static class NpcAction
    {
        public static void GiveQuestion(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            if (action.args.Equals("DV"))
            {
                perso.GetClient().EndGameAction(GameActionTypeEnum.DIALOG_CREATE);
            }
            else
            {
                int qID = -1;
                try
                {
                    qID = int.Parse(action.args);
                }
                catch (Exception localNumberFormatException)
                {
                }
                NpcQuestion quest = NpcQuestionTable.getNPCQuestion(qID);
                if (quest == null)
                {
                    perso.GetClient().EndGameAction(GameActionTypeEnum.DIALOG_CREATE);
                    return;
                }
                perso.Send(new NpcDialogQuestionMessage(quest, perso));
            }
        }
    }
}
