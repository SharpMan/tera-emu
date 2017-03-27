using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.GameActions
{
    public sealed class GameFight : GameAction
    {
        public Fight Fight
        {
            get;
            private set;
        }

        public GameFight(Fighter Fighter, Fight Fight)
            : base(GameActionTypeEnum.FIGHT, Fighter)
        {
            this.Fight = Fight;
        }

        public override void Abort(params object[] Args)
        {
            this.Fight.LeaveFight(Actor as Fighter); // TODO : Fight.Disconnect();

            base.Abort(Args);
        }

        public override bool CanSubAction(GameActionTypeEnum ActionType)
        {
            switch (ActionType)
            {
                case GameActionTypeEnum.MAP_MOVEMENT:
                case GameActionTypeEnum.FIGHT_LAUNCHSPELL:
                case GameActionTypeEnum.FIGHT_USEWEAPON:
                    return true;
            }

            return false;
        }
    }
}
