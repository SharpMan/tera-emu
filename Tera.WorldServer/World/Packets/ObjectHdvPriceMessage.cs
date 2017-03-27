using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ObjectHdvPriceMessage : PacketBase
    {
        public int TemplateID;

        public ObjectHdvPriceMessage(int t)
        {
            this.TemplateID = t;
        }

        public override string Compile()
        {
            return "EHP" + TemplateID + "|" +ItemTemplateTable.Cache[TemplateID].AvgPrice;
        }
    }
}
