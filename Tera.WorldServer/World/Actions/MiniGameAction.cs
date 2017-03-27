using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Actions
{
    public class MiniGameAction
    {
        public int _id;
        public int _actionID;
        public String _packet;
        public String _args;

        public MiniGameAction(int aId, int aActionId, String aPacket)
        {
            _id = aId;
            _actionID = aActionId;
            _packet = aPacket;
        }
    }
}
