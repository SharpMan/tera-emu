using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.Database.Models
{
    public class ExpFloorModel
    {
        public int ID { get; set; }
        public long Character { get; set; }
        public int Job { get; set; }
        public int Mount { get; set; }
        public int PvP { get; set; }
        public int Living { get; set; }
        public long Guild { get; set; }
    }
}
