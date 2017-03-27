using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public class NpcQuestionTable
    {
        public static Dictionary<int, NpcQuestion> Cache = new Dictionary<int, NpcQuestion>();

        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM npc_questions");
            while (reader.Read())
            {
                var npc = new NpcQuestion()
                {
                    ID = reader.GetInt32("ID"),
                    Reponses = reader.GetString("responses"),
                    Args = reader.GetString("params"),
                    Conditions = reader.GetString("cond"),
                    FalseQuestion = reader.GetInt32("ifFalse"),
                };
                Cache.Add(npc.ID, npc);
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ NPC Questions");

        }

        public static NpcQuestion getNPCQuestion(int guid)
        {
            if (!Cache.ContainsKey(guid))
            {
                return null;
            }
            return Cache[guid];
        }
    }
}
