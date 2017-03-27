using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Exceptions
{
    public abstract class FighterException : Exception
    {
        protected Fight fight;
        protected Fighter fighter;

        public FighterException(string message, Fight fight, Fighter fighter) : base(message)
        {
            this.fight = fight;
            this.fighter = fighter;
        }

        public abstract void finalAction();
    }
}
