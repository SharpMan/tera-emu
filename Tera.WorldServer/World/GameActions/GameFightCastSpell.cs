using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.GameActions
{
    public sealed class GameFightCastSpell : GameAction
    {
        public GameFightCastSpell(Fighter Fighter)
            : base(GameActionTypeEnum.FIGHT_LAUNCHSPELL, Fighter)
        {
        }

        public override bool CanSubAction(GameActionTypeEnum ActionType)
        {
            return false;
        }
    }
}
