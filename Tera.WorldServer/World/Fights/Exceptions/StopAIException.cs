using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.WorldServer.World.Fights.Exceptions
{
    public class StopAIException : FighterException
    {
        public StopAIException(string message, Fight fight, Fighter fighter)
            : base(message, fight, fighter)
        {

        }

        public override void finalAction()
        {
        }
    }
}
