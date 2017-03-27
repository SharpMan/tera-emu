using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.Database.Models
{
    public class Mount
    {
        public int ID;
        public int Color;
        public int Sexe;
        public int Amour;
        public int Endurance;
        public int Level;
        public long Exp;
        public String Name;
        public int Fatigue;
        public int Energy;
        public int Reproduction;
        public int Maturite;
        public int Serenite;
        private GenericStats myStats;
        public String Ancestres = ",,,,,,,,,,,,,";
        public List<InventoryItemModel> Items = new List<InventoryItemModel>();
        public List<int> Ability = new List<int>(2);
        public bool myIntialized = false;
        public String itemList;

        public Mount(int color)
        {
            ID = DatabaseCache.nextMountId++;
            Color = color;
            Level = 1;
            Exp = 0;
            Name = "SansNom";
            Fatigue = 0;
            Energy = getMaxEnergie();
            Reproduction = 0;
            Maturite = getMaxMatu();
            Serenite = 0;
            myStats = StaticMountTable.getMountStats(Color, Level);
            Ancestres = ",,,,,,,,,,,,,";
            MountTable.Add(this);
            itemList = "";
        }

        public Mount(int id, int color, int sexe, int amour, int endurance,
            int level, long exp, String nom, int fatigue,
            int energie, int reprod, int maturite, int serenite, String items, String anc)
        {
            ID = id;
            Color = color;
            Sexe = sexe;
            Amour = amour;
            Endurance = endurance;
            Level = level;
            Exp = exp;
            Name = nom;
            Fatigue = fatigue;
            Energy = energie;
            Reproduction = reprod;
            Maturite = maturite;
            Serenite = serenite;
            Ancestres = anc;
           
        }

        public Mount()
        {
            // TODO: Complete member initialization
        }

        public void Intialize()
        {
            if (myIntialized)
                return;

            myStats = StaticMountTable.getMountStats(Color, Level);
            foreach (String str in itemList.Split(';'))
            {
                try
                {
                    if (String.IsNullOrEmpty(str))
                    {
                        continue;
                    }
                    long idd;
                    if (!long.TryParse(str, out idd))
                    {
                        continue;
                    }
                    InventoryItemModel obj = InventoryItemTable.getItem(idd);
                    if (obj != null)
                    {
                        Items.Add(obj);
                    }
                    else
                    {
                        obj = InventoryItemTable.Load(idd);
                        if (obj != null)
                        {
                            Items.Add(obj);
                        }
                    }
                }
                catch (Exception e) { continue; }
            }

            myIntialized = true;
        }

        public int get_color(String a = null)
        {
            return Color;
        }

       
        public GenericStats GetStats()
        {
            return myStats;
        }


        public String parse()
        {
            StringBuilder Base = new StringBuilder();

            Base.Append(ID).Append(":");
            Base.Append(Color).Append(":");
            Base.Append(Ancestres).Append(":");
            Base.Append(",").Append(":");//FIXME capacit�s
            Base.Append(Name).Append(":");
            Base.Append(Sexe).Append(":");
            Base.Append(parseXpString()).Append(":");
            Base.Append(Level).Append(":");
            Base.Append("1").Append(":");//FIXME
            Base.Append(getTotalPod()).Append(":");
            Base.Append("0").Append(":");//FIXME podActuel?
            Base.Append(Endurance).Append(",10000:");
            Base.Append(Maturite).Append(",").Append(getMaxMatu()).Append(":");
            Base.Append(Energy).Append(",").Append(getMaxEnergie()).Append(":");
            Base.Append(Serenite).Append(",-10000,10000:");
            Base.Append(Amour).Append(",10000:");
            Base.Append("-1").Append(":");//FIXME
            Base.Append("0").Append(":");//FIXME
            Base.Append(parseStats()).Append(":");//FIXME
            Base.Append(Fatigue).Append(",240:");//FIXME
            Base.Append(Reproduction).Append(",20:");//FIXME

            return Base.ToString();
        }

        private String parseStats()
        {
            this.Intialize();
            String stats = "";
            foreach (KeyValuePair<EffectEnum, GenericEffect> entry in myStats.GetEffects())
            {
                if (entry.Value.Items <= 0) continue;
                if (stats.Length > 0) stats += ",";
                stats += entry.Key.ToString("X") + "#" + entry.Value.Items.ToString("X") + "#0#0";
            }
            return stats;
        }

        public int getMaxEnergie()
        {
            int energie = 10000;
            return energie;
        }

        public int getMaxMatu()
        {
            int matu = 1000;
            return matu;
        }

        public int getTotalPod()
        {
            //int pod = 1000;
            int ability = 0;
            if (Ability.Contains(2))
                ability = 20 * Level;
            return 10 * Level + (100 * MountTable.getGeneration(Color) + ability);
            //return pod;
        }

        public int getActualPods()
        {
            int pods = 0;
            foreach (InventoryItemModel obj in Items)
            {
                if (obj == null)
                    continue;
                pods += obj.Template.Pods * obj.Quantity;
            }
            return pods;
        }

        private String parseXpString()
        {
            return Exp + "," + ExpFloorTable.GetFloorByLevel(Level).Mount + "," + ExpFloorTable.GetFloorByLevel(Level + 1).Mount;
        }

        public Boolean isMountable()
        {
            if (Energy < 10
            || Maturite < getMaxMatu()
            || Fatigue == 240) return false;
            return true;
        }

        public String getItemsId()
        {
            String str = "";
            foreach (InventoryItemModel obj in Items)
            {
                str += obj.ID + ";";
            }
            return str;
        }


        public void addXp(long amount)
        {
            Exp += amount;

            while (Exp >= ExpFloorTable.GetFloorByLevel(Level + 1).Mount && Level < 100)
                levelUp();

        }

        public void levelUp()
        {
            Level++;
            myStats = StaticMountTable.getMountStats(Color, Level);
        }


        public void SerializeAsItemList(StringBuilder SerializedString)
        {
            foreach (InventoryItemModel obj in Items)
            {
                SerializedString.Append("O" + obj.ToString());
            }
        }
    }
}
