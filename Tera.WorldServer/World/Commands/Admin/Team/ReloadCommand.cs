using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Team
{
    public class ReloadCommand : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "reload";
            }
        }

        public override int AccessLevel
        {
            get
            {
                return 4;
            }
        }

        public override string Description
        {
            get
            {
                return "recharger un parametre";
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
            if (parameters.Lenght > 0)
            {
                switch (parameters.GetParameter(0))
                {
                    case "script":
                        Scripting.ScriptKernel.Load();
                        break;
                    case "config":
                        Tera.WorldServer.Utils.Settings.Initialize();
                        break;
                }
                client.Send(new ConsoleMessage(parameters.GetParameter(0)+" reloaded !", ConsoleColorEnum.GREEN));
            }
            else
            {
                client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
            }
        }
    }
}
