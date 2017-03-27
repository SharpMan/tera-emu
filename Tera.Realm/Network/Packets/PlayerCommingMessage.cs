using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Realm.Database.Models;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;

namespace Tera.Realm.Network.Packets
{
    public class PlayerCommingMessage : TeraPacket
    {
        public PlayerCommingMessage(AccountModel account, String ticket) : base(PacketHeaderEnum.PlayerCommingMessage)
        {
            Writer.Write(ticket);
            Writer.Write(account.ID);
            Writer.Write(account.Username);
            Writer.Write(account.Password);
            Writer.Write(account.Pseudo);
            Writer.Write(account.SecretQuestion);
            Writer.Write(account.SecretAnswer);
            Writer.Write(account.Level);
            Writer.Write(account.LastIP);
            Writer.Write(account.LastConnectionDate.Ticks);
        }
    }
}
