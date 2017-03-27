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
    public class InventoryItemModel
    {
        public long ID { get; set; }
        public int TemplateID { get; set; }
        public int Position { get; set; }
        public int Quantity { get; set; }
        public string Effects { get; set; }
        public long SpeakingID { get; set; }

        private GenericStats myStats;
        public Dictionary<EffectEnum, string> mySpecialEffects = new Dictionary<EffectEnum, string>();

        public Speaking SpeakingItem
        {
            get
            {
                if (SpeakingID == 0)
                    return null;
                var item = SpeakingTable.GetSpeakingItem(SpeakingID);
                if (item == null)
                    SpeakingID = 0;
                return item;
            }
        }

        public ItemTemplateModel Template
        {
            get
            {
                return ItemTemplateTable.GetTemplate(this.TemplateID);
            }
        }

        public bool IsStuffed { get { return this.Position != -1; } }

        public GenericStats GetStats()
        {
            if (this.myStats == null)
            {
                this.ParseStats();
            }

            return myStats;
        }

        public void ClearStats()
        {
            this.myStats = new GenericStats();
        }

        private static int HexToInt(string s)
        {
            return int.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }



        private void ParseStats()
        {
            this.myStats = new GenericStats(this);

            string[] StatsArray = this.Effects.Split(',');

            foreach (string StatsRow in StatsArray)
            {
                try
                {
                    string[] Stats = StatsRow.Split('#');
                    EffectEnum StatsId = (EffectEnum)HexToInt(Stats[0]);

                    if ((int)StatsId == 997 || (int)StatsId == 996)
                    {
                        this.myStats.AddSpecialEffect(StatsId, Stats[4]);

                        continue;
                    }

                    //Si stats avec Texte (Signature, apartenance, etc)
                    if (StatsId == EffectEnum.LivingType ||  (!Stats[3].Equals("") && !Stats[3].Equals("0")))
                    {
                        this.myStats.AddSpecialEffect(StatsId, Stats[3]);
                        continue;
                    }

                    string Jet = Stats[4];

                    if (ItemTemplateModel.IsWeaponEffect(StatsId))
                    {
                        int Min = int.Parse(Stats[1], System.Globalization.NumberStyles.HexNumber);
                        int Max = int.Parse(Stats[2], System.Globalization.NumberStyles.HexNumber);

                        string Args = Min + ";" + Max + ";-1;-1;0;" + Jet;
                        int Value = 0;
                        if (Stats.Length > 4)
                        {
                            Value = int.Parse(Stats[4].Split('+')[1]);
                        }

                        this.myStats.AddWeaponEffect(StatsId, Min, Max, Args, Value);
                    }
                    else
                    {
                        int value = HexToInt(Stats[1]);
                        this.myStats.AddItem(StatsId, value);
                    }
                }
                catch (Exception e)
                {
                }
            }
        }

        public int GetObjiType()
        {
            if (this.GetStats().GetSpecialEffect(EffectEnum.LivingType) == null)
                return (int)ItemTypeEnum.ITEM_TYPE_OBJET_VIVANT;
            else
            {
                return int.Parse(this.GetStats().GetSpecialEffect(EffectEnum.LivingType));
            }
        }

        public override string ToString()
        {
            return ID.ToString("x") + '~' +
                   TemplateID.ToString("x") + '~' +
                   Quantity.ToString("x") + '~' +
                   (Slot != ItemSlotEnum.SLOT_INVENTAIRE ? ((int)Slot).ToString("x") : "") + '~' +
                   GetStats().ToItemStats() + ';';
        }

        public ItemSlotEnum Slot
        {
            get
            {
                return (ItemSlotEnum)Position;
            }
        }
    }
}
