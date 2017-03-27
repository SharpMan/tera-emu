using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class ExchangeItemListMessagePerAccount : PacketBase
    {
        public AccountDataModel Account;
        public int BidHouseID;

        public ExchangeItemListMessagePerAccount(AccountDataModel adm, int bhi)
        {
            this.Account = adm;
            this.BidHouseID = bhi;
        }

        public override string Compile()
        {
            StringBuilder Packet = new StringBuilder("EL");
            var ListBidHouseItems = this.Account.getBidHouseItemDeposed(this.BidHouseID);
            Boolean isFirst = true;
            foreach (var BHI in ListBidHouseItems)
            {
                if (BHI == null)
                    continue;
                if (!isFirst)
                    Packet.Append("|");
                BHI.SerializeAsDisplayEquipment(Packet);
                isFirst = false;
            }
            return Packet.ToString();
        }
    }
}
