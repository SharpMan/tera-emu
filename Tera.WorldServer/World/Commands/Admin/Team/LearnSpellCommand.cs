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
    public class LearnSpellCommand : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "learnspell";
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
                return "Apprendre un sort";
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
                var spellid = parameters.GetIntParameter(0);
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
                if (!SpellTable.Cache.ContainsKey(spellid))
                {
                    client.Send(new ConsoleMessage("Impossible de trouver le sort n°" + spellid, ConsoleColorEnum.RED));
                    return;
                }
                target.GetSpellBook().AddSpell(spellid,6,25,client);
                target.Send(new SpellsListMessage(target));
                client.Send(new ConsoleMessage("Le sort a été appris", ConsoleColorEnum.GREEN));
            }
            else
            {
                client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
            }
        }
    }
}
