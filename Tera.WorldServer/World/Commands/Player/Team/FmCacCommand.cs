using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Commands.Player.Team
{
    public sealed class FmCacCommand : PlayerCommand
    {
        public override string Prefix
        {
            get
            {
                return "fmcac";
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
                return "Changer l'element de son Arme";
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
            if (Settings.AppSettings.GetBoolElement("NpcShop.FmCac"))
            {
                if ((Program.currentTimeMillis() - Client.lastCheckPoint) < 3000)
                    return;

                var Object = Client.Character.InventoryCache.GetItemInSlot(ItemSlotEnum.SLOT_ARME);
                if (Object == null)
                {
                    Client.Send(new ChatGameMessage("Vous ne portez pas d'arme", "682B2B"));
                    return;
                }
                if (Client.GetFight() != null)
                {
                    Client.Send(new ChatGameMessage("Impossible en combat", "682B2B"));
                    return;
                }
                String answer;

                try
                {
                    answer = parameters.GetParameter(0);
                }
                catch (Exception e)
                {
                    Client.Send(new ChatGameMessage("Action impossible : vous n'avez pas spécifié l'élément (air, feu, terre, eau) qui remplacera les dégats/vols de vies neutres", "682B2B"));
                    return;
                }
                if (!answer.Equals("air") && !answer.Equals("terre") && !answer.Equals("feu") && !answer.Equals("eau"))
                {
                    Client.Send(new ChatGameMessage("Action impossible : l'élément " + answer + " n'existe pas ! (dispo : air, feu, terre, eau)", "682B2B"));
                    return;
                }

                if (!Object.GetStats().GetWeaponEffects().ContainsKey(EffectEnum.VolNeutre) && !Object.GetStats().GetWeaponEffects().ContainsKey(EffectEnum.DamageNeutre))
                {
                    Client.Send(new ChatGameMessage("Action impossible : votre arme n'a pas de dégats neutre", "682B2B"));
                    return;
                }
                try
                {
                    var b = Object.GetStats().GetWeaponEffects().ToArray();
                    foreach (var i in b)
                    {
                        if (i.Key == EffectEnum.DamageNeutre)
                        {
                            if (answer.Equals("air"))
                            {
                                i.Value.EffectType = EffectEnum.DamageAir;
                            }
                            if (answer.Equals("feu"))
                            {
                                i.Value.EffectType = EffectEnum.DamageFeu;
                            }
                            if (answer.Equals("terre"))
                            {
                                i.Value.EffectType = EffectEnum.DamageTerre;
                            }
                            if (answer.Equals("eau"))
                            {
                                i.Value.EffectType = EffectEnum.DamageEau;
                            }
                            Object.GetStats().RemoveWeaponEffects(i.Key);
                            Object.GetStats().AdWeaponEffects(i.Value.EffectType, i.Value);
                        }

                        if (i.Key == EffectEnum.VolNeutre)
                        {
                            if (answer.Equals("air"))
                            {
                                i.Value.EffectType = EffectEnum.VolAir;
                            }
                            if (answer.Equals("feu"))
                            {
                                i.Value.EffectType = EffectEnum.VolFeu;
                            }
                            if (answer.Equals("terre"))
                            {
                                i.Value.EffectType = EffectEnum.VolTerre;
                            }
                            if (answer.Equals("eau"))
                            {
                                i.Value.EffectType = EffectEnum.VolEau;
                            }
                            Object.GetStats().RemoveWeaponEffects(i.Key);
                            Object.GetStats().AdWeaponEffects(i.Value.EffectType, i.Value);
                        }
                    }

                    Client.Send(new ObjectRemoveMessage(Object.ID));
                    Client.Send(new ObjectAddInventoryMessage(Object));
                    Client.Send(new FmMoveMessage("KO+" + Object.ID + "|1|" + Object.TemplateID + "|" + Object.GetStats().ToItemStats().Replace(";", "#")));
                    Client.Send(new FmCMessage("K;" + Object.TemplateID));
                    InventoryItemTable.Update(Object);
                    Client.Send(new ChatGameMessage("Votre " + Object.Template.Name + " a été FM avec succès en" + answer + " !", "FF0000"));
                    Client.Send(new AccountStatsMessage(Client.Character));
                    Client.lastCheckPoint = Program.currentTimeMillis();
                }
                catch (Exception e) { Logger.Error(e); return; }
            }
            else
            {
                Client.Send(new ChatGameMessage("Fromagerie d'Armes desactivée", "046380"));
            }

        }
    }
}
