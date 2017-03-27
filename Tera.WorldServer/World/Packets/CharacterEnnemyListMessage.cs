using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterEnnemyListMessage : PacketBase
    {
        public Player Character;

        public CharacterEnnemyListMessage(Player perso)
        {
            this.Character = perso;
        }

        public override string Compile()
        {
            StringBuilder sb = new StringBuilder("iL");
            foreach (KeyValuePair<int, String> KeyPair in Character.GetClient().Account.Data.EnnemyList)
            {
                sb.Append("|").Append(KeyPair.Value);
                var socket = WorldServer.Network.WorldServer.Clients.FirstOrDefault(x => x.Account != null && x.Account.ID == KeyPair.Key);
                if (socket != null && socket.Account != null && socket.Character != null)
                {
                    sb.Append(";").Append("?;")/* Chno had zab ?*/.Append(socket.Character.Name).Append(";");
                    if (socket.Account.Data.EnnemyList.ContainsKey(Character.GetClient().Account.ID))
                    {
                        sb.Append(socket.Character.Level).Append(";");
                        sb.Append(socket.Character.Alignement).Append(";");
                    }
                    else
                    {
                        sb.Append("?;");
                        sb.Append("-1;");
                    }
                    sb.Append(socket.Character.Classe).Append(";");
                    sb.Append(socket.Character.Sexe).Append(";");
                    sb.Append(socket.Character.Look);
                }
                
            }
            
            return sb.ToString();
        }
    }
}
