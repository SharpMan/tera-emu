using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Utils;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.Database.Tables
{
    public static class StaticMountTable
    {
        private static Dictionary<int, ItemTemplateModel> ObjTemplateByMount = new Dictionary<int, ItemTemplateModel>();
        private static Dictionary<int, List<Couple<int, Double>>> StatsByMount = new Dictionary<int, List<Couple<int, Double>>>();


        public static void Load()
        {
            ObjTemplateByMount.Clear();
            StatsByMount.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM static_mounts");
            while (reader.Read())
            {
                ObjTemplateByMount.Add(reader.GetInt32("id"), ItemTemplateTable.GetTemplate(reader.GetInt32("scrollID")));
                try
                {
                    List<Couple<int, Double>> _stats = new List<Couple<int, Double>>();
                    foreach (String stat in Regex.Split(reader.GetString("stats"), "\\|"))
                    {
                        String[] infos = stat.Split('=');
                        Couple<int, Double> c = new Couple<int, Double>(int.Parse(infos[0]), (infos.Length > 1 ? double.Parse(infos[1], CultureInfo.InvariantCulture) : 0));
                        _stats.Add(c);
                    }
                    StatsByMount.Add(reader.GetInt32("id"), _stats);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
            reader.Close();

            Logger.Info("Loaded @'" + ObjTemplateByMount.Count + "'@ Mount");
        }

        public static GenericStats getMountStats(int color, int lvl)
        {
            GenericStats stats = new GenericStats();
            List<Couple<int, Double>> statsNbr = StatsByMount[color];
            if (statsNbr != null)
            {
                foreach (Couple<int, Double> stat in statsNbr)
                {
                    //if (0 > (int)(lvl / stat.second)) continue;
                    stats.AddItem((EffectEnum)stat.first, (int)(lvl / stat.second));
                }
            }
            return stats;
        }

        public static ItemTemplateModel getMountScroll(int mountID)
        {
            if (ObjTemplateByMount.ContainsKey(mountID))
            {
                return ObjTemplateByMount[mountID];
            }
            return null;
        }

        public static int getMountColorByParchoTemplate(int tID)
        {
            for (int a = 1; a < 100; a++)
            {
                if (getMountScroll(a) != null)
                {
                    if (getMountScroll(a).ID == tID)
                    {
                        return a;
                    }
                }
            }
            return -1;
        }

    }
}
