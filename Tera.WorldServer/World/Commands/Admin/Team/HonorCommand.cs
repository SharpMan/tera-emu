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
    public sealed class HonorCommand : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "honor";
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
                return "Ajouter des points d'honneurs";
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
                int honor;
                try
                {
                    honor = parameters.GetIntParameter(0);
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

                target.AddHonor(honor);
                target.RefreshOnMap();

                client.Send(new ConsoleMessage("Les points d'honneurs ont été ajouté", ConsoleColorEnum.GREEN));
            }
            else
            {
                client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
            }
        }
    }
}
