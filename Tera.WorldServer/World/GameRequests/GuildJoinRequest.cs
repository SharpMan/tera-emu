using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.WorldServer.Network;

namespace Tera.WorldServer.World.GameRequests
{
    public sealed class GuildJoinRequest : GameBaseRequest
    {
        public GuildJoinRequest(WorldClient Inviter, WorldClient Invited)
            : base(Inviter, Invited)
        {
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Accept()
        {
            if (!base.Accept())
                return false;

            return true;
        }

        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Declin()
        {
            if (!base.Declin())
                return false;

            return true;
        }

        
        public override bool CanSubAction(GameActionTypeEnum Action)
        {
            return false;
        }
    }
}
