using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Actions;

namespace Tera.WorldServer.Database.Tables
{
    public class NpcReplyTable
    {
        public static Dictionary<int, NpcReply> Cache = new Dictionary<int, NpcReply>();

        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM npc_reponses_actions");
            while (reader.Read())
            {
                int id = reader.GetInt32("ID");
                int type = reader.GetInt32("type");
                String args = reader.GetString("args");
                if (!Cache.ContainsKey(id))
                {
                    Cache.Add(id, new NpcReply(id));
                }
                Cache[id].addAction(new ActionModel(type, args, ""));
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ NPC Reply");

        }

        public static NpcReply get(int id)
        {
            if (!Cache.ContainsKey(id))
            {
                return null;
            }
            else
            {
                return Cache[id];
            }
        }
    }
}
