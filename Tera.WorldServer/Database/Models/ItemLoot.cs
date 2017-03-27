using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.Database.Models
{
    public class ItemLoot
    {
        public int TemplateId
        {
            get;
            private set;
        }

        public GenericStats ItemStats
        {
            get;
            private set;
        }

        public int Quantity
        {
            get;
            private set;
        }

        public ItemLoot(int TemplateId, GenericStats Stats, int Quantity)
        {
            this.TemplateId = TemplateId;
            this.ItemStats = Stats;
            this.Quantity = Quantity;
        }
    }
}
