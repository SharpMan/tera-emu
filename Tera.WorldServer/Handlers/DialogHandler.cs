using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public class DialogHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'C':
                    DialogHandler.DialogStart(Client, Packet);
                    break;

                case 'R':
                    DialogHandler.DialogReponse(Client, Packet);
                    break;
                case 'V':
                    DialogHandler.DialogEnd(Client, Packet);
                    break;
            }
        }

        private static void DialogStart(WorldClient Client, String Packet)
        {
            try
            {
                int npcID = int.Parse(Packet.Substring(2).Split((char)0x0A)[0]);
                Npc npc = Client.Character.myMap.getNPC(npcID);
                if (npc == null)
                {
                    return;
                }
                Client.Send(new NpcDialogStartMessage(npcID));
                int qID = npc.get_template().InitQuestion;
                if (!NpcQuestionTable.Cache.ContainsKey(qID))
                {
                    Client.Send(new NpcEndDialogMessage());
                }
                NpcQuestion quest = NpcQuestionTable.Cache[qID];
                Client.AddGameAction(new GameDialog(Client, npcID, npc, quest));
            }
            catch (Exception e)
            {
            }
        }

        private static void DialogReponse(WorldClient Client, String Packet)
        {
            if (!Client.IsGameAction(GameActionTypeEnum.DIALOG_CREATE))
            {
                return;
            }
            String[] infos = Regex.Split(Packet.Substring(2),"\\|");
            Client.AbortGameAction(GameActionTypeEnum.DIALOG_CREATE, infos); 
        }

        private static void DialogEnd(WorldClient Client, String Packet)
        {
            Client.EndGameAction(GameActionTypeEnum.DIALOG_CREATE);
        }
    }
}
