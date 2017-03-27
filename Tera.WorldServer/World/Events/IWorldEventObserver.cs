using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;

namespace Tera.WorldServer.World.Events
{
    public interface IWorldEventObserver
    {
        void Register(WorldClient Client);
        void UnRegister(WorldClient Client);
    }
}
