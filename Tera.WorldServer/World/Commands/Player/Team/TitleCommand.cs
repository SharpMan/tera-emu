using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Player.Team
{
    public sealed class TitleCommand : PlayerCommand
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
                return 0;
            }
        }

        public override string Description
        {
            get
            {
                return "Modifier le titre";
            }
        }

        public override bool NeedLoaded
        {
            get
            {
                return true;
            }
        }

        public override void Execute(WorldClient Client, CommandParameters parameters)
        {
            if (Settings.AppSettings.GetBoolElement("NpcShop.ConsultPoint"))
            {
                try
                {
                    if (parameters.Lenght < 1)
                    {
                        Client.Send(new ChatGameMessage("<b>Titres : </b>\n"
                                                + ".titre koli - Donne ton titre avec ton classement en Kolizeum\n"
                                                + ".titre pvp - Donne ton titre avec ton classement en PvP / Quitter\n"
                                                + ".titre pvm - Donne ton titre avec ton classement en Pvm\n"
                                                + ".notitle - Enleve ton titre", "046380"));
                        return;
                    }
                    if (Program.currentTimeMillis() - Client.myChatRestrictions[ChatChannelEnum.CHANNEL_POINT] < 4000)
                    {
                        return;
                    }
                    switch (parameters.GetParameter(0))
                    {
                        case "koli":
                            Client.GetCharacter().Title = 11;
                            break;
                        case "pvp":
                            Client.GetCharacter().Title = 10;
                            break;
                        case "pvm":
                            Client.GetCharacter().Title = 12;
                            break;
                        default:
                            Client.Send(new ChatGameMessage("Titre Invalide consultez la liste en tapant .titre", "3882AC"));
                            return;
                    }
                    Client.Character.RefreshOnMap();
                    Client.Send(new ChatGameMessage("Votre titre a été modifié", "3882AC"));
                    Client.myChatRestrictions[ChatChannelEnum.CHANNEL_POINT] = Program.currentTimeMillis();
                    return;
                }
                catch (Exception e) { Logger.Error(e); return; }
            }
            else
            {
                Client.Send(new ChatGameMessage("Commande desactivée", "046380"));
            }

        }
    }
}
