using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Actions;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.Database.Models
{
    public class ItemTemplateModel
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string StatsTemplate { get; set; }
        public int Pods { get; set; }
        public int ItemSetID { get; set; }
        public int Price { get; set; }
        public string Criterions { get; set; }
        public string WeaponInfos { get; set; }
        public int PACost, POmin, POmax, TauxCC, TauxEC, BonusCC;
        public Boolean isTwoHanded;
        public long AvgPrice { get; set; }
        public long Sold { get; set; }
        private List<ActionModel> onUseActions = new List<ActionModel>();

        public static bool IsWeaponEffect(EffectEnum EffectType)
        {
            switch (EffectType)
            {
                case EffectEnum.VolTerre:
                case EffectEnum.VolFeu:
                case EffectEnum.VolEau:
                case EffectEnum.VolAir:
                case EffectEnum.VolNeutre:
                case EffectEnum.DamageTerre:
                case EffectEnum.DamageNeutre:
                case EffectEnum.DamageFeu:
                case EffectEnum.DamageEau:
                case EffectEnum.DamageAir:
                case EffectEnum.SubPAEsquivable:
                    return true;
            }

            return false;
        }

        public bool isIntialize = false;

        public void Initialize()
        {
            if (isIntialize || WeaponInfos.Split(';').Length < 4) return;

            try
            {
                String[] infos = WeaponInfos.Split(';');
                PACost = int.Parse(infos[0]);
                POmin = int.Parse(infos[1]);
                POmax = int.Parse(infos[2]);
                TauxCC = int.Parse(infos[3]);
                TauxEC = int.Parse(infos[4]);
                BonusCC = int.Parse(infos[5]);
                isTwoHanded = infos[6].Equals("1");
            }
            catch (Exception e) { };

            isIntialize = true;
        }



        private Dictionary<int, RandomJet> myRandomStats;

        public ItemTypeEnum ItemType
        {
            get
            {
                return (ItemTypeEnum)this.Type;
            }
        }

        public GenericStats GenerateStats(bool Max = false)
        {
            var GeneratedStats = new GenericStats();
            
            if (StatsTemplate == string.Empty) // aucun stats
                return GeneratedStats;

            // On initialise si c'est pas deja fait
            if (this.myRandomStats == null)
            {
                this.myRandomStats = new Dictionary<int, RandomJet>();

                foreach (var Effect in this.StatsTemplate.Split(','))
                {
                    var effectDatas = Effect.Split('#');
                    var effectId = int.Parse(effectDatas[0], System.Globalization.NumberStyles.HexNumber);
                    var effectMinJet = int.Parse(effectDatas[1], System.Globalization.NumberStyles.HexNumber);
                    var effectMaxJet = int.Parse(effectDatas[2], System.Globalization.NumberStyles.HexNumber);
                    if (IsWeaponEffect((EffectEnum)effectId))
                    {
                        int Min = int.Parse(effectDatas[1], System.Globalization.NumberStyles.HexNumber);
                        int Maxx = int.Parse(effectDatas[2], System.Globalization.NumberStyles.HexNumber);

                        string Args = Min + ";" + Maxx + ";-1;-1;0;" + effectDatas[4];
                        int Value = 0;
                        if (effectDatas.Length > 4)
                        {
                            Value = int.Parse(effectDatas[4].Split('+')[1]);
                        }
                        try
                        {
                            GeneratedStats.AddWeaponEffect((EffectEnum)effectId, Min, Maxx, Args, Value);
                        }
                        catch (System.ArgumentException e1)
                        {
                            Logger.Error("Item " + ID + " has Double WeaponEffectID " + (EffectEnum)effectId);
                            continue;
                        }
                        continue;
                    }
                    this.myRandomStats.Add(effectId, new RandomJet(effectId, effectMinJet, effectMaxJet));
                }
            }

            // On recupere des jets au hasard
            foreach (var Effect in this.myRandomStats.Values)
            {
                //if (IsWeaponEffect((EffectEnum)Effect.EffectId))
                //  continue;

                GeneratedStats.AddItem((EffectEnum)Effect.EffectId, Max ? (Effect.Max == 0 ? Effect.Min : Effect.Max) : Effect.GetRandomJet());
            }

            return GeneratedStats;
        }

        public static bool CanPlaceInSlot(ItemTypeEnum Type, ItemSlotEnum Slot)
        {
            switch (Type)
            {
                case ItemTypeEnum.ITEM_TYPE_AMULETTE:
                    if (Slot == ItemSlotEnum.SLOT_AMULETTE)
                        return true;
                    break;

                case ItemTypeEnum.ITEM_TYPE_ARC:
                case ItemTypeEnum.ITEM_TYPE_BAGUETTE:
                case ItemTypeEnum.ITEM_TYPE_BATON:
                case ItemTypeEnum.ITEM_TYPE_DAGUES:
                case ItemTypeEnum.ITEM_TYPE_EPEE:
                case ItemTypeEnum.ITEM_TYPE_MARTEAU:
                case ItemTypeEnum.ITEM_TYPE_PELLE:
                case ItemTypeEnum.ITEM_TYPE_HACHE:
                case ItemTypeEnum.ITEM_TYPE_OUTIL:
                case ItemTypeEnum.ITEM_TYPE_PIOCHE:
                case ItemTypeEnum.ITEM_TYPE_FAUX:
                case ItemTypeEnum.ITEM_TYPE_PIERRE_AME:
                    if (Slot == ItemSlotEnum.SLOT_ARME)
                        return true;
                    break;

                case ItemTypeEnum.ITEM_TYPE_ANNEAU:
                    if (Slot == ItemSlotEnum.SLOT_ANNEAU_D || Slot == ItemSlotEnum.SLOT_ANNEAU_G)
                        return true;
                    break;

                case ItemTypeEnum.ITEM_TYPE_CEINTURE:
                    if (Slot == ItemSlotEnum.SLOT_CEINTURE)
                        return true;
                    break;

                case ItemTypeEnum.ITEM_TYPE_BOTTES:
                    if (Slot == ItemSlotEnum.SLOT_BOTTES)
                        return true;
                    break;

                case ItemTypeEnum.ITEM_TYPE_COIFFE:
                    if (Slot == ItemSlotEnum.SLOT_COIFFE)
                        return true;
                    break;

                case ItemTypeEnum.ITEM_TYPE_CAPE:
                    if (Slot == ItemSlotEnum.SLOT_CAPE)
                        return true;
                    break;

                case ItemTypeEnum.ITEM_TYPE_FAMILIER:
                    if (Slot == ItemSlotEnum.SLOT_FAMILIER)
                        return true;
                    break;

                case ItemTypeEnum.ITEM_TYPE_DOFUS:
                    if (Slot == ItemSlotEnum.SLOT_DOFUS_1
                     || Slot == ItemSlotEnum.SLOT_DOFUS_2
                     || Slot == ItemSlotEnum.SLOT_DOFUS_3
                     || Slot == ItemSlotEnum.SLOT_DOFUS_4
                     || Slot == ItemSlotEnum.SLOT_DOFUS_5
                     || Slot == ItemSlotEnum.SLOT_DOFUS_6)
                        return true;
                    break;

                case ItemTypeEnum.ITEM_TYPE_BOUCLIER:
                    if (Slot == ItemSlotEnum.SLOT_BOUCLIER)
                        return true;
                    break;
            }

            return false;
        }

        public void addAction(ActionModel A)
        {
            onUseActions.Add(A);
        }

        public void applyAction(Player perso, Player target, int objID, short cellid)
        {
            foreach (ActionModel a in onUseActions)
            {
                a.apply(perso, target, objID, cellid);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void newSold(int amount, long price)
        {
            long oldSold = Sold;
			Sold += amount;
            AvgPrice = (int)((AvgPrice * oldSold + price) / Sold);
        }
    }
}
