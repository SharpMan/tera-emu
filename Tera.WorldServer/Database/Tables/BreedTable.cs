﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;

namespace Tera.WorldServer.Database.Tables
{
    public static class BreedTable
    {
        public static Dictionary<int, Models.BreedModel> Cache = new Dictionary<int, Models.BreedModel>();

        public static void Load()
        {
            Cache.Clear();
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM breeds_data");
            while (reader.Read())
            {
                var breed = new Models.BreedModel()
                {
                    ID = reader.GetInt32("Race"),
                    Name = reader.GetString("Name"),
                    StartLife = reader.GetInt32("StartLife"),
                    StartAP = reader.GetInt32("StartPA"),
                    StartMP = reader.GetInt32("StartPM"),
                    StartInitiative = reader.GetInt32("StartInitiative"),
                    StartProspection = reader.GetInt32("StartProspection"),
                    WeaponBonus = reader.GetString("WeaponBonus"),
                    Fire = reader.GetString("Intelligence"),
                    Water = reader.GetString("Chance"),
                    Agility = reader.GetString("Agilite"),
                    Strenght = reader.GetString("Force"),
                    Life = reader.GetString("Vitalite"),
                    Wisdom = reader.GetString("Sagesse"),
                };
                breed.Load();
                Cache.Add(breed.ID, breed);
            }
            reader.Close();

           Logger.Info("Loaded @'" + Cache.Count + "'@ breeds");
        }



        public static Database.Models.BreedModel GetBreed(int id)
        {
            lock (Database.Tables.BreedTable.Cache)
            {
                return Database.Tables.BreedTable.Cache[id];
            }
        }
    }
}
