using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.GameActions
{
    public sealed class GameKolizeum : GameAction
    {

        public Player p
        {
            get;
            private set;
        }

        public GameKolizeum(Player p1)
            : base(GameActionTypeEnum.KOLIZEUM, p1)
        {
            this.p = p1;
            Network.WorldServer.Kolizeum.RegisterPlayer(p1);
        }

        public override void Abort(params object[] Args)
        {
            Network.WorldServer.Kolizeum.UnregisterPlayer(this.p);
            base.Abort(Args);
        }

        public override void EndExecute()
        {
            Network.WorldServer.Kolizeum.UnregisterPlayer(this.p);
            base.EndExecute();
        }

        public override bool CanSubAction(GameActionTypeEnum ActionType)
        {
            if (ActionType == GameActionTypeEnum.KOLIZEUM)
                return false;
            return true;
        }
    }
}
