using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public class ExchangeBankListMessage : PacketBase
    {
        public Player Character;

        public ExchangeBankListMessage(Player character)
        {
            this.Character = character;
        }

        public override string Compile()
        {
            StringBuilder sb = new StringBuilder("EL");
            foreach (InventoryItemModel item in Character.Client.Account.Data.bankItems.Values)
            {
                sb.Append("O" + item.ToString() + ";");
            }
            if (Character.Client.Account.Data.BankKamas != 0L)
            {
                sb.Append("G" + Character.Client.Account.Data.BankKamas);
            }
            return sb.ToString();
        }

    }
}
