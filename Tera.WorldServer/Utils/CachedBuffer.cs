using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;
using Tera.Libs.Network;

namespace Tera.WorldServer.Utils
{
    public sealed class CachedBuffer : IDisposable
    {
        private WorldClient myClient;
        private StringBuilder myData = new StringBuilder();

        public CachedBuffer(WorldClient Client)
        {
            this.myClient = Client;
        }

        public void Append(PacketBase message)
        {
            this.myData.Append(message.Build());
        }

        public void Dispose()
        {
            this.myClient.Send(myData.ToString());
        }
    }
}
