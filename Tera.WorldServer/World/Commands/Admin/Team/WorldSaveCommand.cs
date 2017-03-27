using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Network;

namespace Tera.WorldServer.World.Commands.Team
{
    public class WorldSaveCommand : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "worldsave";
            }
        }

        public override int AccessLevel
        {
            get
            {
                return 1;
            }
        }

        public override string Description
        {
            get
            {
                return "Sauvegarder le serveur";
            }
        }

        public override bool NeedLoaded
        {
            get
            {
                return true;
            }
        }

        public override void Execute(WorldClient client, CommandParameters parameters)
        { 
            Database.DatabaseCache.Save();
        }
    
    }
}
