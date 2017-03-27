using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.Database.Models
{
    public class IObjectTemplate
    {
        public int ID { get; set; }
        public int RespawnTime { get; set; }
        public int Duration { get; set; }
        public int Unk { get; set; }
        public Boolean Walakable { get; set; }
    }
}
