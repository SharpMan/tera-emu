using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    class DropTable
    {
        public static void Load()
        {
            int a = 0;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM drops");
            while (reader.Read())
            {
                var mob = MonsterTable.GetMonster(reader.GetInt32("mob"));
                if (mob == null)
                {
                    continue;
                }
                var drop = new Drop()
                {
                    MonsterID = reader.GetInt32("mob"),
                    TemplateId = reader.GetInt32("item"),
                    Seuil = reader.GetInt32("seuil"),
                    Max = reader.GetInt32("max"),
                    Taux = reader.GetDecimal("taux"),
                };
                mob.addDrop(drop);
                a++;
            }
            reader.Close();

            Logger.Info("Loaded @'" + a + "'@ Drops");

        }
    }
}
