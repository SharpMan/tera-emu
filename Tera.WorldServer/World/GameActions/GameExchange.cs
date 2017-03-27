using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Exchanges;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.GameActions
{
    public sealed class GameExchange : GameAction
    {
        public Exchange Exchange
        {
            get;
            set;
        }

        public GameExchange(IGameActor Actor, Exchange Exchange)
            : base(GameActionTypeEnum.EXCHANGE, Actor)
        {
            this.Exchange = Exchange;
        }

        public override void Execute()
        {
            base.Execute();
        }

        public override void Abort(params object[] Args)
        {
            if (!Exchange.ExchangeFinish)
            {
                Exchange.CloseExchange();
            }

            base.EndExecute();
        }

        public override void EndExecute()
        {
            if (!Exchange.ExchangeFinish)
            {
                Exchange.CloseExchange(true);
            }

            base.EndExecute();
        }

        public override bool CanSubAction(GameActionTypeEnum ActionType)
        {
            return false;
        }
    }
}
