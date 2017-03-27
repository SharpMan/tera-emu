using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Team
{
    public sealed class LevelCommand : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "level";
            }
        }

        public override int AccessLevel
        {
            get
            {
                return 3;
            }
        }

        public override string Description
        {
            get
            {
                return "Modifier le niveau";
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
                int level;
                try
                {
                    level = parameters.GetIntParameter(0);
                }
                catch (FormatException e)
                {
                    client.Send(new ConsoleMessage("Le parametre 1 n'est pas un nombre décimal", ConsoleColorEnum.RED));
                    return;
                }
                var target = client.Character;
                if (parameters.Lenght > 1)
                {
                    if (CharacterTable.Contains(parameters.GetParameter(1)))
                    {
                        target = CharacterTable.GetCharacter(parameters.GetParameter(1));
                    }
                }
                if (!target.IsOnline())
                {
                    client.Send(new ConsoleMessage("La Cible est deconnecté", ConsoleColorEnum.RED));
                    return;
                }
                while (target.Level < level)
                {
                    target.LevelUP();
                }
                target.Send(new AccountStatsMessage(target));
                target.Send(new CharacterNewLevelMessage(target.Level));
                target.Send(new SpellsListMessage(target));
                client.Send(new ConsoleMessage("Le niveau a été modifié", ConsoleColorEnum.GREEN));
            }
            else
            {
                client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
            }
        }
    }
}
