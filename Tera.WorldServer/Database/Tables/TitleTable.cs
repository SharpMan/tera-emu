using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.Database.Tables
{
    public static class TitleTable
    {
        public static Dictionary<short, String> Cache = new Dictionary<short, string>();

        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM titles");
            while (reader.Read())
            {
                Cache.Add(reader.GetInt16("id"), reader.GetString("title"));
            }
            reader.Close();

            Logger.Info("Loaded @'" + Cache.Count + "'@ Titles");
        }


        public static String getTitle(Player character)
        {
            if (!Cache.ContainsKey(character.Title))
                return character.Title.ToString();
            String Title = Cache[character.Title];
            String name_align = "Neutre";
            switch (character.Alignement)
            {
                case 1:
                    name_align = "Bontarien";
                    break;
                case 2:
                    name_align = "Brâkmarien";
                    break;
                case 3:
                    name_align = "Sériane";
                    break;
            }
            Title = '*' + Title;
            Title = Title.Replace("%p%", character.Name);
            if (character.HasGuild()) Title = Title.Replace("%g%", character.GetGuild().Name);
            Title = Title.Replace("%lvl%", "" + character.Level);
            Title = Title.Replace("%gm%", "" + character.Account);
            Title = Title.Replace("%ps%", character.Account.Pseudo);
            Title = Title.Replace("%id%", character.Title.ToString());
            Title = Title.Replace("%vie%", character.Life + "");
            Title = Title.Replace("%gfx%", character.Look.ToString());
            Title = Title.Replace("%align%", name_align);
            //Title = Title.Replace("%pvprank%", character.RankPvP.ToString());
            //Title = Title.Replace("%pvmrank%", character.RankPvM.ToString());
            //Title = Title.Replace("%kolirank%", character.RankKolizeum.ToString());
            return character.Title + Title;
        }

    }
}
