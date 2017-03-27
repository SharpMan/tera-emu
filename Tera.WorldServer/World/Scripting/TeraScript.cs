using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Actions;

namespace Tera.WorldServer.World.Scripting
{
    public class TeraScript
    {
        public String Name;
        private List<ActionModel> Actions = new List<ActionModel>();

        public TeraScript(String Name)
        {
            this.Name = Name;
        }

        public void addAction(ActionModel act)
        {
            /*List<ActionModel> c = new List<ActionModel>();
            Actions.ForEach(x => c.Add(x));
            foreach (ActionModel a in c)
            {
                if (a.ID != act.ID)
                {
                    continue;
                }
                Actions.Remove(a);
            }*/
            Actions.Add(act);
        }

        public void apply(Player perso)
        {
            foreach (ActionModel act in Actions)
            {
                act.apply(perso, null, -1, perso.CellId);
            }
        }


    }
}
