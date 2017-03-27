using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Realm.Database;
using Tera.Realm.Database.Models;
using Tera.Realm.Database.Tables;
using Tera.Libs.Helper;

namespace Tera.Realm.Utils
{
    public static class LoginMessageFormatter
    {
        public static String helloWorld(String ticket)
        {
            return "HC" + ticket;
        }

        public static String serversInformationsMessage(bool subscriber,int accountLevel)
        {
            StringBuilder sb = new StringBuilder().Append("AH");
            char subscribr = subscriber ? '1' : '0';
            bool first = true;
            foreach (GameServerModel gs in GameServerTable.Cache)
            {
                if (!first)
                    sb.Append('|');
                else
                    first = false;

                sb.Append(gs.ID).Append(';');
                if (gs.LevelRequired > accountLevel)
                    sb.Append("0");
                            
                else
                    sb.Append((int)gs.State).Append(';');

               sb.Append(GameServerModel.Completion).Append(';')
                 .Append(subscribr);

            }
            return sb.ToString();
        }

        public static String charactersListMessage(long subscribeTimeEnd, Dictionary<long, int> charactersList) {
            StringBuilder sb = new StringBuilder("AxK").Append(subscribeTimeEnd);
            foreach (GameServerModel g in GameServerTable.Cache)
            {
                sb.Append("|").Append(g.ID).Append(",");
                sb.Append(charactersList.Where(x => x.Value == g.ID).ToList().Count);
            }
           /* foreach(KeyValuePair<int, int> entry in charactersList){
                sb.Append('|')
                  .Append(entry.Key).Append(',')
                  .Append(entry.Value);
            }*/
            return sb.ToString();
        }


        public static String badClientVersion(String requiredVersion)
        {
            return "AlEv" + requiredVersion;
        }

        public static String accessDenied()
        {
            return "AlEf";
        }

        public static String banned()
        {
            return "AlEb";
        }

        public static String alreadyConnected()
        {
            return "AlEc";
        }

        public static String nicknameInformationMessage(String nickname)
        {
            return "Ad" + nickname;
        }

        public static String communityInformationMessage(int community)
        {
            return "Ac" + community;
        }

        public static String identificationSuccessMessage(bool hasRights)
        {
            return "AlK" + (hasRights ? "1" : "0");
        }

        public static String accountQuestionInformationMessage(String question)
        {
            return "AQ" + question.Replace(" ", "+");
        }

        public static String selectedHostInformationMessage(String address, int port, String ticket, bool loopback)
        {
            //return "AYK" + (loopback ? "127.0.0.1" : address) + ":" + port + ";" + ticket;
            return "AXK" + AdressHelper.CryptIP(address) + AdressHelper.CryptPort(port) + ticket;
        }



        public static String ServerInSave()
        {
            return "AXEd";
        }

        public static String InaccessedServer()
        {
            return "AXEr";
        }

        public static String serverSelectionErrorMessage()
        {
            return "AYE";
        }

        public static String WrongPassword()
        {
            return "AlEx";
        }

        public static String MessageBoxMaintenance()
        {
            return "M013|";
        }

        public static String ServerHasntDisponnible()
        {
            return "ATE";
        }

        public static String isConnectedToRealm()
        {
            return "AlEd";
        }
    }
}
