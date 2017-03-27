using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.Database.Tables
{
    public static class ZaapiTable
    {
        public static Dictionary<AlignmentTypeEnum, List<short>> Cache = new Dictionary<AlignmentTypeEnum, List<short>>();

        public static void Load()
        {
            Cache.Clear();
            int nbr = 0;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM zaapi");
            while (reader.Read())
            {
                if (!Cache.ContainsKey((AlignmentTypeEnum)reader.GetInt32("align")))
                {
                    Cache.Add((AlignmentTypeEnum)(reader.GetInt32("align")), new List<short>());
                }
                Cache[(AlignmentTypeEnum)reader.GetInt32("align")].Add(reader.GetInt16("mapid"));
                nbr++;
            }
            reader.Close();

            Logger.Info("Loaded @'" + nbr + "'@ Zaapis");
        }

    }
}
