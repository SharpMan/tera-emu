using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Actions;

namespace Tera.WorldServer.Database.Models
{
    public class NpcReply
    {
        public int ID;
        private List<ActionModel> Actions = new List<ActionModel>();

        public NpcReply(int i)
        {
            this.ID = i;
        }

        public void addAction(ActionModel act)
        {
            List<ActionModel> c = new List<ActionModel>();
            Actions.ForEach(x => c.Add(x));
            foreach (ActionModel a in c)
            {
                if (a.ID != act.ID)
                {
                    continue;
                }
                Actions.Remove(a);
            }
            Actions.Add(act);
        }

        public void apply(Player perso)
        {
            foreach (ActionModel act in Actions)
            {
                act.apply(perso, null, -1, this.ID);
            }
        }

        public Boolean isAnotherDialog()
        {
            foreach (ActionModel curAct in Actions)
            {
                if (curAct.ID == 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
