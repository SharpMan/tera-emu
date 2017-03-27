using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.Database.Models
{
    public class BreedModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int StartLife { get; set; }
        public int StartAP { get; set; }
        public int StartMP { get; set; }
        public int StartInitiative { get; set; }
        public int StartProspection { get; set; }
        public string WeaponBonus { get; set; }
        public string Fire { get; set; }
        public string Water { get; set; }
        public string Agility { get; set; }
        public string Strenght { get; set; }
        public string Life { get; set; }
        public string Wisdom { get; set; }

        private Dictionary<StatsTypeEnum, List<BreedFloor>> floors = new Dictionary<StatsTypeEnum, List<BreedFloor>>();

        public void Load()
        {
            this.floors.Clear();
            this.floors.Add(StatsTypeEnum.LIFE, new List<BreedFloor>());
            this.floors.Add(StatsTypeEnum.WISDOM, new List<BreedFloor>());
            this.floors.Add(StatsTypeEnum.STRENGHT, new List<BreedFloor>());
            this.floors.Add(StatsTypeEnum.FIRE, new List<BreedFloor>());
            this.floors.Add(StatsTypeEnum.WATER, new List<BreedFloor>());
            this.floors.Add(StatsTypeEnum.AGILITY, new List<BreedFloor>());

            loadCaract(StatsTypeEnum.LIFE, Life);
            loadCaract(StatsTypeEnum.STRENGHT, Strenght);
            loadCaract(StatsTypeEnum.WISDOM, Wisdom);
            loadCaract(StatsTypeEnum.FIRE, Fire);
            loadCaract(StatsTypeEnum.WATER, Water);
            loadCaract(StatsTypeEnum.AGILITY, Agility);
        }

        private void loadCaract(StatsTypeEnum sType, string caract)
        {
            foreach (var floor in caract.Split('|'))
            {
                if (floor != "")
                {
                    var data = floor.Split(':');

                    var floorData = data[0].Split(',');
                    var costData = data[1].Split('-');

                    if (floorData.Length > 1)
                    {
                        this.floors[sType].Add(new BreedFloor
                            (int.Parse(floorData[0]), int.Parse(floorData[1]), int.Parse(costData[0]), int.Parse(costData[1])));
                    }
                    else
                    {
                        this.floors[sType].Add(new BreedFloor
                            (int.Parse(floorData[0]), int.MaxValue, int.Parse(costData[0]), int.Parse(costData[1])));
                    }
                }
            }
        }

        public BreedFloor GetFloor(StatsTypeEnum sType, int floor)
        {
            BreedFloor cost = null;
            foreach (var f in this.floors[sType])
            {
                if (f.From <= floor)
                {
                    cost = f;
                }
            }
            return cost;
        }

        public int GetCost(StatsTypeEnum sType, int floor)
        {
            int cost = 0;
            foreach (var f in this.floors[sType])
            {
                if (f.From <= floor)
                {
                    cost = f.Cost;
                }
            }
            return cost;
        }
    }
}
