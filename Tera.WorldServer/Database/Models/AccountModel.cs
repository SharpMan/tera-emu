using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.Database.Models
{
    public class AccountModel
    {
        public int ID { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String Pseudo { get; set; }
        public String Question { get; set; }
        public String Reponse { get; set; }
        public int Level { get; set; }
        public DateTime LastConnectionDate { get; set; }
        public String LastIP { get; set; }
        public Dictionary<long, Player> Characters = new Dictionary<long, Player>();
        public Player curPlayer { get; set; }

        public AccountDataModel Data;

        public void loadData()
        {
            Data = AccountDataTable.Get(ID);
            if (Data == null)
            {
                Data = new AccountDataModel()
                {
                    Guid = this.ID,
                    Bank = "",
                    BankKamas = 0,
                    Stables = "",
                    Friends = "",
                    Ennemys = "",
                    showFriendConnection = false,
                };
                AccountDataTable.Add(Data);
            }
            else
            {
                Data.Initialize();
            }
        }

        public bool HasCharacter(long characterId)
        {
            return this.Characters.ContainsKey(characterId);
        }

        public void OnDisconnect()
        {
            AccountTable.UpdateLogged(ID, false);
            foreach (Player character in Characters.Values)
            {
                CharacterTable.DelCharacter(character);
            }
            if (Data != null)
            {
                Data.Save();
            }
            this.curPlayer = null;
        }

        public String parseDragoList()
        {
            if (Data.Mounts.Count == 0)
            {
                return "~";
            }
            StringBuilder packet = new StringBuilder();
            foreach (Mount DD in Data.Mounts.Values)
            {
                if (packet.Length > 0)
                {
                    packet.Append(";");
                }
                packet.Append(DD.parse());
            }
            return packet.ToString();
        }

    }
}
