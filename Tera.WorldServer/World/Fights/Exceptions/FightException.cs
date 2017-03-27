using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Exceptions
{
    public abstract class FightException : Exception
    {
        protected Fight fight;
        
        public FightException(String message, Fight fight) : base(message)
        {
            this.fight = fight;
        }

        public abstract void finalAction();
    }
}
