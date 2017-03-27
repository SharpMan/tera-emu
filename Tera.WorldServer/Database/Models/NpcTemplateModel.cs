using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.Database.Models
{
    public class NpcTemplateModel
    {
        public long ID { get; set; }
        public int BonusValue { get; set; }
        public int SkinID { get; set; }
        public int ScaleX { get; set; }
        public int ScaleY { get; set; }
        public int Sexe { get; set; }
        public int Color1 { get; set; }
        public int Color2 { get; set; }
        public int Color3 { get; set; }
        public String Accessories { get; set; }
        public int ExtraClip { get; set; }
        public int CustomArtWork { get; set; }
        public int InitQuestion { get; set; }
        public String Ventes { get; set; }
    }
}
