using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.Database.Models
{
    public class ItemSetModel
    {
        public int ID { get; set; }
        public String StringItems { get; set; }
        public String StringBonus { get; set; }
        private List<GenericStats> Stats { get; set; }
        public List<ItemTemplateModel> Items { get; set; }
        private bool myInitialized = false;

        public void Initialize()
        {
            if (myInitialized)
                return;

            Items = new List<ItemTemplateModel>();

            foreach (String str in StringItems.Split(','))
            {
                try
                {
                    Items.Add(ItemTemplateTable.GetTemplate(int.Parse(str)));
                }
                catch (NullReferenceException e) { Logger.Error("Item " + str + " Introuvable dans la panoplie " + ID); }
                catch (FormatException e) { Logger.Error("Echoue lors de la convertion de l'item " + str); } 
            }

            Stats = new List<GenericStats>();
            foreach (String str in StringBonus.Split(';'))
            {
                var stats = new GenericStats();
               
                foreach (String str2 in str.Split(','))
                {
                    try
                    {
                        String[] infos = str2.Split(':');
                        if (infos.Length < 2)
                        {
                            continue;
                        }
                        stats.AddItem((EffectEnum)int.Parse(infos[0], NumberStyles.Number, CultureInfo.InvariantCulture), int.Parse(infos[1], NumberStyles.Number, CultureInfo.InvariantCulture));
                    }
                    catch (FormatException e) { Logger.Error(str2.Split(':')[0] + "|" + str2.Split(':')[1] + ""); }
                }
                Stats.Add(stats);
            }
            myInitialized = true;
        }

        public GenericStats getBonusStatByItemCount(int numb)
        {
            try
            {
                return Stats[numb - 2];
            }
            catch (Exception e)
            {
                return new GenericStats();
            }
            
        }

    }
}
