using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Player.Team
{
    public sealed class FmCommand : PlayerCommand
    {
        public override string Prefix
        {
            get
            {
                return "fm";
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
                return "Ajouter un element dans l'equipement";
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
            if (Settings.AppSettings.GetBoolElement("NpcShop.Fm"))
            {
                if ((Program.currentTimeMillis() - Client.lastCheckPoint) < 3000)
                    return;
                if (parameters.Lenght < 2)
                {
                    Client.Send(new ChatGameMessage("Syntaxe invalide : .fm coiffe/cape pa/po/pm", "682B2B"));
                    return;
                }
                ItemSlotEnum Slot = ItemSlotEnum.SLOT_ITEMBAR_13;
                if (parameters.GetParameter(0) == "coiffe")
                {
                    Slot = ItemSlotEnum.SLOT_COIFFE;
                }
                else if (parameters.GetParameter(0) == "cape")
                {
                    Slot = ItemSlotEnum.SLOT_CAPE;
                }
                else
                {
                    Client.Send(new ChatGameMessage("Liste disponnible : coiffe/cape", "682B2B"));
                    return;
                }
                var Effect = EffectEnum.AddPods;
                switch (parameters.GetParameter(1))
                {
                    case "pa":
                        Effect = EffectEnum.AddPA;
                        break;
                    case "pm":
                        Effect = EffectEnum.AddPM;
                        break;
                    case "po":
                        Effect = EffectEnum.AddPO;
                        break;
                    default:
                        Client.Send(new ChatGameMessage("Liste disponnible : pa/pm/po", "682B2B"));
                        return;

                }
                var Points = AccountTable.getPoints(Client.Account);
                if (Settings.AppSettings.GetIntElement("NpcShop.FmCost") > Points)
                {
                    Client.Send(new ChatGameMessage("Il vous manque " + (Settings.AppSettings.GetIntElement("NpcShop.FmCost") - Points) + " points pour utiliser ce privilege", "682B2B"));
                    return;
                }
                var Object = Client.Character.InventoryCache.GetItemInSlot(Slot);
                if (Object == null)
                {
                    Client.Send(new ChatGameMessage("Vous ne portez pas de " + parameters.GetParameter(0), "682B2B"));
                    return;
                }
                if (Client.GetFight() != null)
                {
                    Client.Send(new ChatGameMessage("Impossible en combat", "682B2B"));
                    return;
                }
                if (Object.GetStats().GetEffectFM(Effect).Total != 0)
                {
                    Client.Send(new ChatGameMessage("Votre " + parameters.GetParameter(0) + " donne déjà un " + parameters.GetParameter(1), "682B2B"));
                    return;
                }
                if (Effect != EffectEnum.AddPO)
                {
                    if (Object.GetStats().GetEffectFM(EffectEnum.AddPM).Total != 0 || Object.GetStats().GetEffectFM(EffectEnum.AddPA).Total != 0)
                    {
                        Client.Send(new ChatGameMessage("Votre " + parameters.GetParameter(0) + " donne déjà un pa ou un pm", "682B2B"));
                        return;
                    }
                }
                AccountTable.SubstractPoints(Client.Account.ID, Settings.AppSettings.GetIntElement("NpcShop.FmCost"));
                Client.Character.GetStats().UnMerge(Object.GetStats());
                Object.GetStats().AddItem(Effect, 1);
                Client.Character.GetStats().Merge(Object.GetStats());

                Client.Send(new ObjectRemoveMessage(Object.ID));
                Client.Send(new ObjectAddInventoryMessage(Object));
                Client.Send(new FmMoveMessage("KO+" + Object.ID + "|1|" + Object.TemplateID + "|" + Object.GetStats().ToItemStats().Replace(";", "#")));
                Client.Send(new FmCMessage("K;" + Object.TemplateID));
                InventoryItemTable.Update(Object);
                Client.Send(new ChatGameMessage("Votre " + Object.Template.Name + " donne désormais +1" + parameters.GetParameter(1).ToUpper() + " en plus de ses jets habituels !", "FF0000"));
                Client.Send(new AccountStatsMessage(Client.Character));
                Client.lastCheckPoint = Program.currentTimeMillis();
                return;
            }
            else
            {
                Client.Send(new ChatGameMessage("Fromagerie desactivée", "046380"));
            }
        }
    }
}
