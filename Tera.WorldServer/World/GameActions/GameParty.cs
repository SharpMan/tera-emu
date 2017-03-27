using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.World.GameActions
{
    public sealed class GameParty : GameAction
    {
        public Party Party
        {
            get;
            private set;
        }

        public Player p
        {
            get;
            private set;

        }

        public GameParty(Player p1 , Party p)
            : base(GameActionTypeEnum.GROUP, p1)
        {
            Party = p;
            this.p = p1;
            Party.Players.Add(p1);
            Party.Register(p1.Client);
        }

        public override void Abort(params object[] Args)
        {
            Party.Leave(this.p);
            base.Abort(Args);
        }

        public override bool CanSubAction(GameActionTypeEnum ActionType)
        {
            if (ActionType == GameActionTypeEnum.GROUP)
                return false;
            return true;
        }
    }
}
