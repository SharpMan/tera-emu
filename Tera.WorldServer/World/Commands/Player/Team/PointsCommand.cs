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
    public sealed class PointsCommand : PlayerCommand
    {
        public override string Prefix
        {
            get
            {
                return "points";
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
                return "Consulter les points";
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
                    if (Program.currentTimeMillis() - Client.myChatRestrictions[ChatChannelEnum.CHANNEL_POINT] < 4000)
                    {
                        return;
                    }
                    var CachedP = Client.Character.Points;
                    Client.Send(new ChatGameMessage("Vous possedez " + CachedP + " Points", "3882AC"));
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
