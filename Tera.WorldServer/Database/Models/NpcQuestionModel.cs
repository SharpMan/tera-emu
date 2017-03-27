using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.Database.Models
{
    public class NpcQuestion
    {
        public int ID { get; set; }
        public String Reponses { get; set; }
        public String Args { get; set; }
        public String Conditions { get; set; }
        public int FalseQuestion { get; set; }

        public String parseArguments(String args, Player perso)
        {
            String arg = args;
            arg = arg.Replace("[name]", perso.getStringVar("name"));
            arg = arg.Replace("[bankCost]", perso.getStringVar("bankCost"));

            return arg;
        }

    }
}
