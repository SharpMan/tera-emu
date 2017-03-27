using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeBidHouseItemsList : PacketBase
    {
        public int Categorie;
        public String Models;

        public ExchangeBidHouseItemsList(int c , string a)
        {
            this.Categorie = c;
            this.Models = a;
        }

        public override string Compile()
        {
            return "EHL" + Categorie + "|" + Models;
        }
    }
}
