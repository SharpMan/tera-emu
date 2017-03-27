using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.Database.Models
{
    public class Drop
    {
        public int MonsterID { get; set; }
        public int Seuil { get; set; }
        public int Max { get; set; }
        public Decimal Taux { get; set; }
        public int TemplateId { get; set; }

        public ItemTemplateModel ItemTemplateCache
        {
            get
            {
                return ItemTemplateTable.GetTemplate(this.TemplateId);
            }
        }
    }
}
