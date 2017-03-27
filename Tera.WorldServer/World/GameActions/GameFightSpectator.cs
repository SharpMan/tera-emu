using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.GameActions
{
    public sealed class GameFightSpectator : GameAction
    {
        public WorldClient Client
        {
            get;
            set;
        }

        public Fight Fight
        {
            get;
            set;
        }

        public GameFightSpectator(WorldClient Client, Fight Fight)
            : base(GameActionTypeEnum.FIGHT, Client.GetCharacter())
        {
            this.Client = Client;
            this.Fight = Fight;
        }

        public override void Abort(params object[] Args)
        {
            Fight.LeaveSpectator(Client);

            base.Abort();
        }

        public override bool CanSubAction(GameActionTypeEnum ActionType)
        {
            return false;
        }
    }
}
