using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.GameRequests;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.GameActions
{
    public sealed class GameRequest : GameAction
    {
        public GameBaseRequest Request
        {
            get;
            private set;
        }

        public GameRequest(IGameActor Actor, GameBaseRequest Request)
            : base(GameActionTypeEnum.BASIC_REQUEST, Actor)
        {
            this.Request = Request;
        }

        public override void Abort(params object[] Args)
        {
            this.Request.Declin();

            base.Abort(Args);
        }

        public override bool CanSubAction(GameActionTypeEnum ActionType)
        {
            return this.Request.CanSubAction(ActionType);
        }
    }
}
