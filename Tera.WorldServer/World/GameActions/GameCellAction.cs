using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Actions;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.GameActions
{
    public sealed class GameCellAction : GameAction
    {
        public WorldClient Client
        {
            get;
            private set;
        }

        public String Packet
        {
            get;
            private set;
        }

        public GameCellAction(WorldClient Client, String Packet): base(GameActionTypeEnum.CELL_ACTION, Client.GetCharacter())
        {
            this.Client = Client;
            this.Packet = Packet;
        }

        public override void Execute()
        {
            int nextGameActionID = 0;
            if (Client.miniActions.Count > 0)
            {
                nextGameActionID = Client.miniActions.Count + 1;
            }
            MiniGameAction GA = new MiniGameAction(nextGameActionID, (int)GameActionTypeEnum.CELL_ACTION, Packet);
            int cellID = -1;
            int actionID = -1;
            try
            {
                cellID = int.Parse(Packet.Substring(5).Split(';')[0]);
                actionID = int.Parse(Packet.Substring(5).Split(';')[1]);
            }
            catch (Exception e)
            {
            }
            if (cellID == -1 || actionID == -1 || Client.Character == null || Client.Character.myMap == null || Client.Character.myMap.getCell(cellID) == null)
            {
                return;
            }
            GA._args = cellID + ";" + actionID;
            Client.miniActions.Add(nextGameActionID, GA);
            if (!Client.Character.myMap.getCell(cellID).canDoAction(actionID))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            Client.Character.myMap.getCell(cellID).startAction(Client.Character, GA);
            base.Execute();
        }

        public override void EndExecute()
        {
            //this.Client.miniActions.Clear();
            base.EndExecute();
        }

        public override bool CanSubAction(GameActionTypeEnum ActionType)
        {
            return true;
        }
    }
}
