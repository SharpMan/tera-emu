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
    public class TitleComamnd : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "title";
            }
        }

        public override int AccessLevel
        {
            get
            {
                return 2;
            }
        }

        public override string Description
        {
            get
            {
                return "Changer de titre";
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
            if (parameters.Lenght > 1)
            {
                try
                {
                    var targeti = parameters.GetParameter(0);
                    var packet = parameters.getShortParamater(1);

                    var target = CharacterTable.GetCharacter(targeti);

                    if (target == null || !target.IsOnline())
                    {
                        client.Send(new ConsoleMessage("Cible invalide !", ConsoleColorEnum.RED));
                        return;
                    }

                    target.Title = packet;

                    target.RefreshOnMap();

                    client.Send(new ConsoleMessage("Le titre a été modifié avec succès !", ConsoleColorEnum.GREEN));
                }
                catch (FormatException e)
                {
                    client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
                }
            }
            else
            {
                client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
            }
        }
    }
}
