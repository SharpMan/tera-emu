/**
	Commande FM :
		@author: alleos13
		@date: 09/09/2013
**/

///Imports statics
var Settings = Tera.WorldServer.Utils.Settings;
var Program = Tera.WorldServer.Program;
var ItemSlotEnum = Tera.Libs.Enumerations.ItemSlotEnum;
var EffectEnum = Tera.Libs.Enumerations.EffectEnum;
var Logger = Tera.Libs.Logger;
var ObjectRemoveMessage = Tera.WorldServer.World.Packets.ObjectRemoveMessage;
var ObjectAddInventoryMessage = Tera.WorldServer.World.Packets.ObjectAddInventoryMessage;
var FmMoveMessage = Tera.WorldServer.World.Packets.FmMoveMessage;
var FmCMessage = Tera.WorldServer.World.Packets.FmCMessage;
var AccountStatsMessage = Tera.WorldServer.World.Packets.AccountStatsMessage;
var InventoryItemTable = Tera.WorldServer.Database.Tables.InventoryItemTable;
var AccountTable =  Tera.WorldServer.Database.Tables.AccountTable

///Constructor
function construct(){
	setAccessLevel(0);//Set AccessLevel
	addAuthorizedSubCommand("cac");
	addAuthorizedSubCommand("coiffe");
	addAuthorizedSubCommand("cape");
}

function coiffe(Client, CommandParams)
{
				if ((currentTimeMillis() - Client.lastCheckPoint) < 3000)return;
                if (CommandParams.Lenght < 1)
                {
                    SendErrorMessage(Client, "Syntaxe invalide : .fm coiffe pa/po/pm");
                    return;
                }
                var Slot = ItemSlotEnum.SLOT_COIFFE;
                var Effect = EffectEnum.AddPods;
                switch (CommandParams[0].ToLower())
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
                        SendErrorMessage(Client, "Liste disponnible : pa/pm/po");
                        return;
                }
                var Points = AccountTable.getPoints(Client.Account);
                if (Settings.AppSettings.GetIntElement("NpcShop.FmCost") > Points)
                {
                    SendErrorMessage(Client, "Il vous manque " + (Settings.AppSettings.GetIntElement("NpcShop.FmCost") - Points) + " points pour utiliser ce privilege");
                    return;
                }
                var Object = Client.Character.InventoryCache.GetItemInSlot(Slot);
                if (Object == null)
                {
                    SendErrorMessage(Client, "Vous ne portez pas de cape");
                    return;
                }
                if (Client.GetFight() != null)
                {
                    SendErrorMessage(Client, "Impossible en combat");
                    return;
                }
                if (Object.GetStats().GetEffectFM(Effect).Total != 0)
                {
                    SendErrorMessage(Client, "Votre cape donne déjà un " + CommandParams[0]);
                    return;
                }
                if (Effect != EffectEnum.AddPO)
                {
                    if (Object.GetStats().GetEffectFM(EffectEnum.AddPM).Total != 0 || Object.GetStats().GetEffectFM(EffectEnum.AddPA).Total != 0)
                    {
                        SendErrorMessage(Client, "Votre cape donne déjà un pa ou un pm");
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
                SendInformationMessage(Client, "Votre " + Object.Template.Name + " donne désormais +1" + CommandParams[0].ToUpper() + " en plus de ses jets habituels !");
                Client.Send(new AccountStatsMessage(Client.Character));
                Client.lastCheckPoint = currentTimeMillis();
}

function cape(Client, CommandParams)
{
				if ((currentTimeMillis() - Client.lastCheckPoint) < 3000)return;
                if (CommandParams.Lenght < 1)
                {
                    SendErrorMessage(Client, "Syntaxe invalide : .fm cape pa/po/pm");
                    return;
                }
                var Slot = ItemSlotEnum.SLOT_CAPE;
                var Effect = EffectEnum.AddPods;
                switch (CommandParams[0].ToLower())
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
                        SendErrorMessage(Client, "Liste disponnible : pa/pm/po");
                        return;
                }
                var Points = AccountTable.getPoints(Client.Account);
                if (Settings.AppSettings.GetIntElement("NpcShop.FmCost") > Points)
                {
                    SendErrorMessage(Client, "Il vous manque " + (Settings.AppSettings.GetIntElement("NpcShop.FmCost") - Points) + " points pour utiliser ce privilege");
                    return;
                }
                var Object = Client.Character.InventoryCache.GetItemInSlot(Slot);
                if (Object == null)
                {
                    SendErrorMessage(Client, "Vous ne portez pas de coiffe");
                    return;
                }
                if (Client.GetFight() != null)
                {
                    SendErrorMessage(Client, "Impossible en combat");
                    return;
                }
                if (Object.GetStats().GetEffectFM(Effect).Total != 0)
                {
                    SendErrorMessage(Client, "Votre coiffe donne déjà un " + CommandParams[0]);
                    return;
                }
                if (Effect != EffectEnum.AddPO)
                {
                    if (Object.GetStats().GetEffectFM(EffectEnum.AddPM).Total != 0 || Object.GetStats().GetEffectFM(EffectEnum.AddPA).Total != 0)
                    {
                        SendErrorMessage(Client, "Votre coiffe donne déjà un pa ou un pm");
                        return;
                    }
                }
                AccountTable.SubstractPoints(Client.Account.ID, Settings.AppSettings.GetIntElement("NpcShop.FmCost"));
                Client.Character.GetStats().UnMerge(Object.GetStats());
                Object.GetStats().AddItem(Effect, parseInt(1));
                Client.Character.GetStats().Merge(Object.GetStats());

                Client.Send(new ObjectRemoveMessage(Object.ID));
                Client.Send(new ObjectAddInventoryMessage(Object));
                Client.Send(new FmMoveMessage("KO+" + Object.ID + "|1|" + Object.TemplateID + "|" + Object.GetStats().ToItemStats().Replace(";", "#")));
				SendInformationMessage(Client, "1");
                Client.Send(new FmCMessage("K;" + Object.TemplateID));
				SendInformationMessage(Client, "2");
                InventoryItemTable.Update(Object);
				SendInformationMessage(Client, "3");
                SendInformationMessage(Client, "Votre " + Object.Template.Name + " donne désormais +1" + CommandParams[0].ToUpper() + " en plus de ses jets habituels !");
				SendInformationMessage(Client, "4");
                Client.Send(new AccountStatsMessage(Client.Character));
                Client.lastCheckPoint = currentTimeMillis();
}

function cac(Client, CommandParams)
{
		if ((currentTimeMillis() - Client.lastCheckPoint) < 3000)return;
        var Object = Client.Character.InventoryCache.GetItemInSlot(ItemSlotEnum.SLOT_ARME);
        if (Client.GetFight() != null)
        {
            SendErrorMessage(Client, "Impossible en combat!");
            return;
        }
		if (Object == null)
        {
            SendErrorMessage(Client, "Vous ne portez pas d'arme!");
            return;
        }
        var answer = CommandParams[0];
		if(answer == null){
			SendErrorMessage(Client, "Action impossible : vous n'avez pas spécifié l'élément (air, feu, terre, eau) qui remplacera les dégats/vols de vies neutres.");
			return;
		}
		SendInformationMessage(Client, "answer="+answer.ToLower());
		answer = answer.ToLower();
		if (!Object.GetStats().GetWeaponEffects().ContainsKey(EffectEnum.VolNeutre) 
			&& !Object.GetStats().GetWeaponEffects().ContainsKey(EffectEnum.DamageNeutre))
        {
            SendErrorMessage(Client, "Action impossible : votre arme n'a pas de dégats neutres");
            return;
        }
		if (!answer.Equals("air") && !answer.Equals("terre") && !answer.Equals("feu") && !answer.Equals("eau"))
        {
            SendErrorMessage(Client, "Action impossible : l'élément " + answer + " n'existe pas ! (dispo : air, feu, terre, eau)");
            return;
        }
		//try{
			var effectsList = Object.GetStats().WeaponEffectsValues();
			for(effect in effectsList)
			{
                if (effect.Key == EffectEnum.DamageNeutre)
                {
                    if (answer.Equals("air"))
                    {
                        effect.Value.EffectType = EffectEnum.DamageAir;
                    }
                    else if (answer.Equals("feu"))
                    {
                        effect.Value.EffectType = EffectEnum.DamageFeu;
                    }
                    else if (answer.Equals("terre"))
                    {
                        effect.Value.EffectType = EffectEnum.DamageTerre;
                    }
                    else if (answer.Equals("eau"))
					{
                        effect.Value.EffectType = EffectEnum.DamageEau;
                    }
                    Object.GetStats().RemoveWeaponEffects(effect.Key);
                    Object.GetStats().AdWeaponEffects(effect.Value.EffectType, effect.Value);
                } 
				else if (effect.Key == EffectEnum.VolNeutre)
                {
                    if (answer.Equals("air"))
                    {
                        effect.Value.EffectType = EffectEnum.VolAir;
                    }
                    else if (answer.Equals("feu"))
                    {
                        effect.Value.EffectType = EffectEnum.VolFeu;
                    }
                    else if (answer.Equals("terre"))
                    {
                        effect.Value.EffectType = EffectEnum.VolTerre;
                    }
                    else if (answer.Equals("eau"))
                    {
                        effect.Value.EffectType = EffectEnum.VolEau;
                    }
                    Object.GetStats().RemoveWeaponEffects(effect.Key);
                    Object.GetStats().AdWeaponEffects(effect.Value.EffectType, effect.Value);
                }
			}
			Client.Send(new ObjectRemoveMessage(Object.ID));
			Client.Send(new ObjectAddInventoryMessage(Object));
			Client.Send(new FmMoveMessage("KO+" + Object.ID + "|1|" + Object.TemplateID + "|" + Object.GetStats().ToItemStats().Replace(";", "#")));
			Client.Send(new FmCMessage("K;" + Object.TemplateID));
			InventoryItemTable.Update(Object);
			SendInformationMessage(Client, "Votre " + Object.Template.Name + " a été FM avec succès en" + answer + " !");
			Client.Send(new AccountStatsMessage(Client.Character));
			Client.lastCheckPoint = currentTimeMillis();
}