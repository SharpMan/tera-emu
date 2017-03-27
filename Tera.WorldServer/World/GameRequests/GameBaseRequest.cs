using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;

namespace Tera.WorldServer.World.GameRequests
{
    public abstract class GameBaseRequest
    {
        
        public abstract bool CanSubAction(GameActionTypeEnum Action);

        public WorldClient Requester
        {
            get;
            private set;
        }

        
        public WorldClient Requested
        {
            get;
            private set;
        }

        public bool IsFinish
        {
            get;
            private set;
        }

      
        public GameBaseRequest(WorldClient Requester, WorldClient Requested)
        {
            this.Requester = Requester;
            this.Requested = Requested;
        }

        public virtual bool Accept()
        {
            if (this.IsFinish)
                return false;

            this.IsFinish = true;

            return true;
        }

        public virtual bool Declin()
        {
            if (this.IsFinish)
                return false;

            this.IsFinish = true;

            return true;
        }
    }
}
