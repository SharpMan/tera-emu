using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Helper;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.World.Character
{
    public class GenericStats
    {
        private Dictionary<EffectEnum, GenericEffect> myStats = new Dictionary<EffectEnum, GenericEffect>();
        private Dictionary<EffectEnum, WeaponEffect> myWeaponStats = new Dictionary<EffectEnum, WeaponEffect>();
        private Dictionary<EffectEnum, string> mySpecialEffects = new Dictionary<EffectEnum, string>();

        public List<KeyValuePair<EffectEnum, WeaponEffect>> WeaponEffectsValues()
        {
            return myWeaponStats.ToList();
        }

        private InventoryItemModel Item = null;
        
        private static Dictionary<EffectEnum, List<EffectEnum>> OPPOSITE_STATS = new Dictionary<EffectEnum, List<EffectEnum>>()
        {
            /* Statistiques de base */            
            {EffectEnum.AddInitiative, new List<EffectEnum>() { EffectEnum.SubInitiative }},
            {EffectEnum.AddPA, new List<EffectEnum>() { EffectEnum.SubPA,  EffectEnum.SubPAEsquivable }},
            {EffectEnum.AddPM, new List<EffectEnum>() { EffectEnum.SubPM, EffectEnum.SubPMEsquivable }},
            {EffectEnum.AddPO, new List<EffectEnum>() { EffectEnum.SubPO }},
            {EffectEnum.AddSoins, new List<EffectEnum>() { EffectEnum.SubSoins }},
            {EffectEnum.AddProspection, new List<EffectEnum>() { EffectEnum.SubProspection }},
            {EffectEnum.AddPods, new List<EffectEnum>() { EffectEnum.SubPods }},
            {EffectEnum.AddVitalite, new List<EffectEnum>() { EffectEnum.SubVitalite }},
            {EffectEnum.AddSagesse, new List<EffectEnum>() { EffectEnum.SubSagesse }},
            {EffectEnum.AddForce, new List<EffectEnum>() { EffectEnum.SubForce }},
            {EffectEnum.AddIntelligence, new List<EffectEnum>() { EffectEnum.SubIntelligence }},
            {EffectEnum.AddAgilite, new List<EffectEnum>() { EffectEnum.SubAgilite }},
            {EffectEnum.AddChance, new List<EffectEnum>() { EffectEnum.SubChance }},

            /* Statistiques avancés */
            {EffectEnum.AddDamage, new List<EffectEnum>() { EffectEnum.SubDamage }},
            {EffectEnum.AddDamagePercent, new List<EffectEnum>() { EffectEnum.SubDamagePercent }},
            {EffectEnum.AddDamageCritic, new List<EffectEnum>() { EffectEnum.SubDamageCritic }},
            {EffectEnum.AddDamageMagic, new List<EffectEnum>() { EffectEnum.SubDamageMagic }},
            {EffectEnum.AddDamagePhysic, new List<EffectEnum>() { EffectEnum.SubDamagePhysic }},
            {EffectEnum.AddEsquivePA, new List<EffectEnum>() { EffectEnum.SubEsquivePA }},
            {EffectEnum.AddEsquivePM, new List<EffectEnum>() { EffectEnum.SubEsquivePM }},   

            /* Resistances / Malus */
            {EffectEnum.AddReduceDamageAir, new List<EffectEnum>() { EffectEnum.SubReduceDamageAir }},
            {EffectEnum.AddReduceDamageEau, new List<EffectEnum>() { EffectEnum.SubReduceDamageEau }},
            {EffectEnum.AddReduceDamageFeu, new List<EffectEnum>() { EffectEnum.SubReduceDamageFeu }},
            {EffectEnum.AddReduceDamageNeutre, new List<EffectEnum>() { EffectEnum.SubReduceDamageNeutre }},
            {EffectEnum.AddReduceDamageTerre, new List<EffectEnum>() { EffectEnum.SubReduceDamageTerre }},

            {EffectEnum.AddReduceDamagePourcentAir, new List<EffectEnum>() { EffectEnum.SubReduceDamagePourcentAir }},
            {EffectEnum.AddReduceDamagePourcentEau, new List<EffectEnum>() { EffectEnum.SubReduceDamagePourcentEau }},
            {EffectEnum.AddReduceDamagePourcentFeu, new List<EffectEnum>() { EffectEnum.SubReduceDamagePourcentFeu }},
            {EffectEnum.AddReduceDamagePourcentNeutre, new List<EffectEnum>() { EffectEnum.SubReduceDamagePourcentNeutre }},
            {EffectEnum.AddReduceDamagePourcentTerre, new List<EffectEnum>() { EffectEnum.SubReduceDamagePourcentTerre }},   

            {EffectEnum.AddReduceDamagePourcentPvPAir, new List<EffectEnum>() { EffectEnum.SubReduceDamagePourcentPvPAir }},
            {EffectEnum.AddReduceDamagePourcentPvPEau, new List<EffectEnum>() { EffectEnum.SubReduceDamagePourcentPvPEau }},
            {EffectEnum.AddReduceDamagePourcentPvPFeu, new List<EffectEnum>() { EffectEnum.SubReduceDamagePourcentPvPFeu }},
            {EffectEnum.AddReduceDamagePourcentPvPNeutre, new List<EffectEnum>() { EffectEnum.SubReduceDamagePourcentPvpNeutre }},
            {EffectEnum.AddReduceDamagePourcentPvPTerre, new List<EffectEnum>() { EffectEnum.SubReduceDamagePourcentPvPTerre }},                     
        };

        public GenericStats()
        {
        }

        public GenericStats(InventoryItemModel Item)
        {
            this.Item = Item;
        }

        public GenericStats(Player Character)
        {
            this.myStats.Add(EffectEnum.AddPA, new GenericEffect(EffectEnum.AddPA, Base: Character.AP));
            this.myStats.Add(EffectEnum.AddPM, new GenericEffect(EffectEnum.AddPM, Base: Character.MP));
            this.myStats.Add(EffectEnum.AddProspection, new GenericEffect(EffectEnum.AddProspection, Base: ((ClassEnum)Character.Classe == ClassEnum.CLASS_ENUTROF ? 120 : 100)));
            this.myStats.Add(EffectEnum.AddPods, new GenericEffect(EffectEnum.AddPods, Base: 1000));
            this.myStats.Add(EffectEnum.AddInvocationMax, new GenericEffect(EffectEnum.AddInvocationMax, Base: 1));
            this.myStats.Add(EffectEnum.AddInitiative, new GenericEffect(EffectEnum.AddInitiative, Base: 100));

            this.myStats.Add(EffectEnum.AddVitalite, new GenericEffect(EffectEnum.AddVitalite, Base: Character.Vitality));
            this.myStats.Add(EffectEnum.AddSagesse, new GenericEffect(EffectEnum.AddSagesse, Base: Character.Wisdom));
            this.myStats.Add(EffectEnum.AddForce, new GenericEffect(EffectEnum.AddForce, Base: Character.Strength));
            this.myStats.Add(EffectEnum.AddIntelligence, new GenericEffect(EffectEnum.AddIntelligence, Base: Character.Intell));
            this.myStats.Add(EffectEnum.AddAgilite, new GenericEffect(EffectEnum.AddAgilite, Base: Character.Agility));
            this.myStats.Add(EffectEnum.AddChance, new GenericEffect(EffectEnum.AddChance, Base: Character.Chance));
        }

        public Dictionary<EffectEnum, GenericEffect> GetEffects()
        {
            return this.myStats;
        }

        public string ToItemStats(Boolean forsave = false)
        {
            if (Item != null && Item.Template.Type == 113)
            {
                var SpeakingItem = Item.SpeakingItem;
                if (SpeakingItem != null)
                {
                    return SpeakingItem.convertirAString();
                }
                else
                {
                    return "3cc#0#0#" + IntHelper.toHexString(1) + "," + "3cb#0#0#1," + "3cd#0#0#" + Item.GetObjiType() + "," + "3ca#0#0#0," + "3ce#0#0#0";
                }
            }
            if (!forsave && Item != null && Item.SpeakingItem != null)
            {
                var NormalStat = new StringBuilder(string.Join(",", this.myWeaponStats.Select(x => x.Value.ToItemString())) + (this.myStats.Count > 0 ? "," : "") + string.Join(",", this.myStats.Select(x => x.Value.ToItemString())) + (this.mySpecialEffects.Count > 0 ? "," : "") + string.Join(",", this.mySpecialEffects.Select(x => SpecialEffectToString(x))));
                if (NormalStat.Length > 0)
                    NormalStat.Append(",");
                NormalStat.Append(Item.SpeakingItem.convertirAString());
                return NormalStat.ToString();
            }
            return string.Join(",", this.myWeaponStats.Select(x => x.Value.ToItemString())) + (this.myStats.Count > 0 ? "," : "") + string.Join(",", this.myStats.Select(x => x.Value.ToItemString()))    + (this.mySpecialEffects.Count > 0 ? "," : "") + string.Join(",", this.mySpecialEffects.Select(x => SpecialEffectToString(x)));
        }

        public String SpecialEffectToString(KeyValuePair<EffectEnum, String> pair)
        {
            if ((int)pair.Key == 623)
            {
                return ((int)pair.Key).ToString("X") + "#0#0#" + pair.Value;
            }
            else
            {
                return ((int)pair.Key).ToString("X") + "#0#0#0#" + pair.Value;
            }
        }

        public bool HasEffect(EffectEnum EffectType)
        {
            return this.myStats.ContainsKey(EffectType);
        }

        public bool ContainsEffect(EffectEnum EffectType)
        {
            return this.myStats.ContainsKey(EffectType);
        }

        public GenericEffect GetEffect(EffectEnum EffectType)
        {
            if (!this.myStats.ContainsKey(EffectType))
                this.myStats.Add(EffectType, new GenericEffect(EffectType));

            return this.myStats[EffectType];
        }

        public GenericEffect GetEffect(int EffectType)
        {
            return GetEffect((EffectEnum)EffectType);
        }

        public Dictionary<EffectEnum, WeaponEffect> GetWeaponEffects()
        {
            return this.myWeaponStats;
        }

        public void RemoveWeaponEffects(EffectEnum a)
        {
           this.myWeaponStats.Remove(a); ;
        }

        public void AdWeaponEffects(EffectEnum a,WeaponEffect b)
        {
            this.myWeaponStats.Add(a,b); ;
        }

        public void Merge(GenericStats Stats)
        {
            foreach (var Effect in Stats.GetEffects())
            {
                if (!this.myStats.ContainsKey(Effect.Key))
                    this.myStats.Add(Effect.Key, new GenericEffect(Effect.Key));

                this.myStats[Effect.Key].Merge(Effect.Value);
            }
        }

        public void UnMerge(GenericStats Stats)
        {
            foreach (var Effect in Stats.GetEffects())
            {
                if (!this.myStats.ContainsKey(Effect.Key))
                    this.myStats.Add(Effect.Key, new GenericEffect(Effect.Key));

                this.myStats[Effect.Key].UnMerge(Effect.Value);
            }
        }

        public int GetTotal(EffectEnum EffectType)
        {
            int Total = 0;

            // existant ?
            if (myStats.ContainsKey(EffectType))
            {
                Total += myStats[EffectType].Total;
            }

            switch (EffectType)
            {
                case EffectEnum.AddEsquivePA:
                case EffectEnum.AddEsquivePM:
                    Total += GetTotal(EffectEnum.AddSagesse) / 4;
                    break;
                case EffectEnum.AddPA:
                    Total += GetTotal(EffectEnum.AddPABis);
                    break;
                case EffectEnum.AddPM:
                    Total += GetTotal(EffectEnum.BonusPM);
                    break;
                case EffectEnum.AddRenvoiDamage:
                    Total += GetTotal(EffectEnum.AddRenvoiDamageItem);
                    break;
            }

            // malus ?
            if (OPPOSITE_STATS.ContainsKey(EffectType))
            {
                foreach (EffectEnum OppositeEffect in OPPOSITE_STATS[EffectType])
                {
                    if (this.myStats.ContainsKey(OppositeEffect))
                    {
                        Total -= this.myStats[OppositeEffect].Total;
                    }
                }
            }

            return Total;
        }

        public static int GetRequiredStatsPoint(ClassEnum classID, int statID, int val)
        {
            switch (statID)
            {
                case 11://Vita
                    return 1;
                case 12://Sage
                    return 3;
                case 10://Force
                    switch (classID)
                    {
                        case ClassEnum.CLASS_SACRIEUR:
                            return 3;

                        case ClassEnum.CLASS_FECA:
                            if (val < 50)
                                return 2;
                            if (val < 150)
                                return 3;
                            if (val < 250)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_XELOR:
                            if (val < 50)
                                return 2;
                            if (val < 150)
                                return 3;
                            if (val < 250)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_SRAM:
                            if (val < 100)
                                return 1;
                            if (val < 200)
                                return 2;
                            if (val < 300)
                                return 3;
                            if (val < 400)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_OSAMODAS:
                            if (val < 50)
                                return 2;
                            if (val < 150)
                                return 3;
                            if (val < 250)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_ENIRIPSA:
                            if (val < 50)
                                return 2;
                            if (val < 150)
                                return 3;
                            if (val < 250)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_PANDAWA:
                            if (val < 50)
                                return 1;
                            if (val < 200)
                                return 2;
                            return 3;

                        case ClassEnum.CLASS_SADIDA:
                            if (val < 50)
                                return 1;
                            if (val < 250)
                                return 2;
                            if (val < 300)
                                return 3;
                            if (val < 400)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_CRA:
                            if (val < 50)
                                return 1;
                            if (val < 150)
                                return 2;
                            if (val < 250)
                                return 3;
                            if (val < 350)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_ENUTROF:
                            if (val < 50)
                                return 1;
                            if (val < 150)
                                return 2;
                            if (val < 250)
                                return 3;
                            if (val < 350)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_ECAFLIP:
                            if (val < 100)
                                return 1;
                            if (val < 200)
                                return 2;
                            if (val < 300)
                                return 3;
                            if (val < 400)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_IOP:
                            if (val < 100)
                                return 1;
                            if (val < 200)
                                return 2;
                            if (val < 300)
                                return 3;
                            if (val < 400)
                                return 4;
                            return 5;

                    }
                    break;
                case 13://Chance
                    switch (classID)
                    {
                        case ClassEnum.CLASS_FECA:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_XELOR:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_SACRIEUR:
                            return 3;

                        case ClassEnum.CLASS_SRAM:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_SADIDA:
                            if (val < 100)
                                return 1;
                            if (val < 200)
                                return 2;
                            if (val < 300)
                                return 3;
                            if (val < 400)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_PANDAWA:
                            if (val < 50)
                                return 1;
                            if (val < 200)
                                return 2;
                            return 3;

                        case ClassEnum.CLASS_IOP:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_ENUTROF:
                            if (val < 100)
                                return 1;
                            if (val < 150)
                                return 2;
                            if (val < 230)
                                return 3;
                            if (val < 330)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_OSAMODAS:
                            if (val < 100)
                                return 1;
                            if (val < 200)
                                return 2;
                            if (val < 300)
                                return 3;
                            if (val < 400)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_ECAFLIP:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_ENIRIPSA:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_CRA:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;
                    }
                    break;
                case 14://Agilit�
                    switch (classID)
                    {
                        case ClassEnum.CLASS_FECA:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_XELOR:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_SACRIEUR:
                            return 3;

                        case ClassEnum.CLASS_SRAM:
                            if (val < 100)
                                return 1;
                            if (val < 200)
                                return 2;
                            if (val < 300)
                                return 3;
                            if (val < 400)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_SADIDA:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_PANDAWA:
                            if (val < 50)
                                return 1;
                            if (val < 200)
                                return 2;
                            return 3;

                        case ClassEnum.CLASS_ENIRIPSA:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_IOP:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_ENUTROF:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_ECAFLIP:
                            if (val < 50)
                                return 1;
                            if (val < 100)
                                return 2;
                            if (val < 150)
                                return 3;
                            if (val < 200)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_CRA:
                            if (val < 50)
                                return 1;
                            if (val < 100)
                                return 2;
                            if (val < 150)
                                return 3;
                            if (val < 200)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_OSAMODAS:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;
                    }
                    break;
                case 15://Intelligence
                    switch (classID)
                    {
                        case ClassEnum.CLASS_XELOR:
                            if (val < 100)
                                return 1;
                            if (val < 200)
                                return 2;
                            if (val < 300)
                                return 3;
                            if (val < 400)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_FECA:
                            if (val < 100)
                                return 1;
                            if (val < 200)
                                return 2;
                            if (val < 300)
                                return 3;
                            if (val < 400)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_SACRIEUR:
                            return 3;

                        case ClassEnum.CLASS_SRAM:
                            if (val < 50)
                                return 2;
                            if (val < 150)
                                return 3;
                            if (val < 250)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_SADIDA:
                            if (val < 100)
                                return 1;
                            if (val < 200)
                                return 2;
                            if (val < 300)
                                return 3;
                            if (val < 400)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_ENUTROF:
                            if (val < 20)
                                return 1;
                            if (val < 60)
                                return 2;
                            if (val < 100)
                                return 3;
                            if (val < 140)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_PANDAWA:
                            if (val < 50)
                                return 1;
                            if (val < 200)
                                return 2;
                            return 3;

                        case ClassEnum.CLASS_IOP:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_ENIRIPSA:
                            if (val < 100)
                                return 1;
                            if (val < 200)
                                return 2;
                            if (val < 300)
                                return 3;
                            if (val < 400)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_CRA:
                            if (val < 50)
                                return 1;
                            if (val < 150)
                                return 2;
                            if (val < 250)
                                return 3;
                            if (val < 350)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_OSAMODAS:
                            if (val < 100)
                                return 1;
                            if (val < 200)
                                return 2;
                            if (val < 300)
                                return 3;
                            if (val < 400)
                                return 4;
                            return 5;

                        case ClassEnum.CLASS_ECAFLIP:
                            if (val < 20)
                                return 1;
                            if (val < 40)
                                return 2;
                            if (val < 60)
                                return 3;
                            if (val < 80)
                                return 4;
                            return 5;
                    }
                    break;
            }

            return 5;
        }



        public void AddBase(EffectEnum EffectType, int Value)
        {
            if (!this.myStats.ContainsKey(EffectType))
                this.myStats.Add(EffectType, new GenericEffect(EffectType, Base: Value));
            else
                this.myStats[EffectType].Base += Value;
        }

        public void AddBoost(EffectEnum EffectType, int Value)
        {
            if (!this.myStats.ContainsKey(EffectType))
                this.myStats.Add(EffectType, new GenericEffect(EffectType, Boosts: Value));
            else
                this.myStats[EffectType].Boosts += Value;
        }

        public int GetBoost(EffectEnum EffectType)
        {
            if (!this.myStats.ContainsKey(EffectType))
                this.myStats.Add(EffectType, new GenericEffect(EffectType, 0));
            return this.myStats[EffectType].Boosts;
        }

        public int GetBase(EffectEnum EffectType)
        {
            if (!this.myStats.ContainsKey(EffectType))
                this.myStats.Add(EffectType, new GenericEffect(EffectType, 0));
            return this.myStats[EffectType].Base;
        }


        public int GetItem(EffectEnum EffectType)
        {
            if (!this.myStats.ContainsKey(EffectType))
                this.myStats.Add(EffectType, new GenericEffect(EffectType, Items : 0));
            return this.myStats[EffectType].Items;
        }

        public void AddItem(EffectEnum EffectType, int Value)
        {
            if (!this.myStats.ContainsKey(EffectType))
                this.myStats.Add(EffectType, new GenericEffect(EffectType, Items: Value));
            else
                this.myStats[EffectType].Items += Value;
        }

        public void AddSpecialEffect(EffectEnum EffectType, string Args)
        {
            this.mySpecialEffects.Add(EffectType, Args);
        }


        public String GetSpecialEffect(EffectEnum EffectType)
        {
            if (mySpecialEffects.ContainsKey(EffectType))
                return mySpecialEffects[EffectType];

            return null;
        }

        public void AddWeaponEffect(EffectEnum EffectType, int MinJet, int MaxJet, string Args,int Value)
        {
            this.myWeaponStats.Add(EffectType, new WeaponEffect(MinJet, MaxJet, Args, EffectType,Value));
        }

        public WeaponEffect GetWeaponEffet(EffectEnum EffectType)
        {
            if (myWeaponStats.ContainsKey(EffectType))
                return myWeaponStats[EffectType];

            return null;
        }

        public GenericEffect GetEffectFM(EffectEnum EffectType)
        {
            if (!this.myStats.ContainsKey(EffectType))
                return new GenericEffect(EffectType);

            return this.myStats[EffectType];
        }
    }
}
