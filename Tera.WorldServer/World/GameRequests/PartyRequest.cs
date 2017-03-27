using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.GameRequests
{
    public sealed class PartyRequest : GameBaseRequest
    {
        public PartyRequest(WorldClient Client, WorldClient Target)
            : base(Client, Target)
        {

        }

        public override bool Accept()
        {
            return base.Accept();
        }

        public override bool Declin()
        {
            if (!base.Declin())
                return false;

            this.Requester.Send(new PartyRefuseMessage());
            this.Requested.Send(new BasicNoOperationMessage());

            this.Requester.SetBaseRequest(null);
            this.Requested.SetBaseRequest(null);

            return true;
        }

        public override bool CanSubAction(GameActionTypeEnum Action)
        {
            return false;
        }
    }
}
