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
    public class ItemCommand : AdminCommand
    {
        public override string Prefix
        {
            get
            {
                return "item";
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
                return "Genere un objet";
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
                int itemID = 0;
                try
                {
                    itemID = parameters.GetIntParameter(0);
                }
                catch (FormatException e)
                {
                    client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
                    return;
                }
                var itemTemplate = ItemTemplateTable.GetTemplate(itemID);
                var quantity = 1;
                var style = false;
                if (itemTemplate != null)
                {
                    if (parameters.Lenght > 1)
                    {
                        try
                        {
                            quantity = parameters.GetIntParameter(1);
                        }
                        catch (FormatException e)
                        {
                            client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
                            return;
                        }
                    }
                    if (parameters.Lenght > 2)
                    {
                        var styleStr = parameters.GetParameter(2).ToLower();
                        if (styleStr == "max")
                        {
                            style = true;
                        }
                    }
                    var item = InventoryItemTable.TryCreateItem(itemID, client.Character, 1, -1, null, style);
                    client.Send(new ConsoleMessage("L'objet <b>'" + itemTemplate.Name + "'</b> a correctement ete generer !", ConsoleColorEnum.GREEN));
                }
                else
                {
                    client.Send(new ConsoleMessage("Impossible de trouver l'objet n°" + itemID, ConsoleColorEnum.RED));
                }
            }
            else
            {
                client.Send(new ConsoleMessage("Parametres invalide !", ConsoleColorEnum.RED));
            }
        }
    }
}
