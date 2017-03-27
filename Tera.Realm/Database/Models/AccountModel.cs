using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Realm.Database.Models
{
    public class AccountModel 
    {
        public int ID;
        public string Username;
        public string Password;
        public string Pseudo;
        public string Email;
        public string SecretQuestion;
        public string SecretAnswer;
        public string LastIP;
        public int Level;
        public long Banned;
        public int Logged;
        public DateTime LastConnectionDate { get; set; }
        public Dictionary<long, int> Characters;

        public bool isConnected() 
        {
            return this.Logged == 1; 
        }
    }
}
