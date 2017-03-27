using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameActionMessage : PacketBase
    {
        public int ActionType;
        public long ActorId;
        public string Args;

        public GameActionMessage(int ActionType, long ActorId, string Args = "")
        {
            this.ActionType = ActionType;
            this.ActorId = ActorId;
            this.Args = Args;
        }

        public override string Compile()
        {
            return "GA" + (ActionType == (int)GameActionTypeEnum.MAP_MOVEMENT || ActionType == (int)GameActionTypeEnum.FIGHT_LAUNCHSPELL || ActionType == (int)GameActionTypeEnum.CHANGE_MAP ? "0" : "") + ";" + (int)ActionType + ';' + ActorId + ';' + Args;
        }
    }
}
