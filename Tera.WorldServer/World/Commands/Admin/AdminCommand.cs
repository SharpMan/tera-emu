using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands
{
    public abstract class AdminCommand
    {
        public virtual string Prefix { get; set; }
        public virtual int AccessLevel { get; set; }
        public virtual string Description { get; set; }

        public virtual bool NeedLoaded { get; set; }

        public bool Locked = false;

        public void PreExecute(WorldClient client, CommandParameters parameters)
        {
            if (!Locked)
            {
                if (client.Account.Level >= AccessLevel)
                {
                    Execute(client, parameters);
                }
                else
                    client.Send(new ConsoleMessage("Votre compte n'est pas autoriser a executer ce type de command client.Account.Level = " + client.Account.Level + " AccessLevel = " + AccessLevel, ConsoleColorEnum.RED));
            }
        }

        public virtual void Execute(WorldClient client, CommandParameters parameters)
        {

        }
    }
}
