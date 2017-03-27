using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Controllers;
using Tera.WorldServer.World.Exchanges;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Actions
{
    public class ActionModel
    {
        public int ID;
        public String args;
        public String cond;

        public ActionModel(int id, String args, String cond)
        {
            this.ID = id;
            this.args = args;
            this.cond = cond;
        }

        public void apply(Player perso, Player target, int itemID, int cellID)
        {
            if (perso == null)
            {
                return;
            }
            /* if Is Fighting return basicdataNoOperationMessage ? */
            if ((!cond.Equals("")) && (!cond.Equals("-1")) && (!ConditionParser.validConditions(perso, cond)))
            {
                perso.Send(new TextInformationMessage(TextInformationTypeEnum.UNK, 119));
                return;
            }
            switch ((ActionTypeEnum)ID)
            {
                case ActionTypeEnum.Teleport:
                    PlayerAction.ApplyTeleportion(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.GiveKamas:
                    PlayerAction.GiveKamas(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.PlacePrisme:
                    PlayerAction.PlacePrisme(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.GiveItem:
                    PlayerAction.GiveItem(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.WarpToSavePos:
                    PlayerAction.WarpToSavePos(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.AddStat:
                    PlayerAction.AddStats(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.LearnSpell:
                    PlayerAction.LearnSpell(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.EatOrDrinkItem:
                    PlayerAction.EatOrDrinkItem(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.ModifAlignement:
                    PlayerAction.ModifAlignement(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.ResetStats:
                    PlayerAction.ResetStats(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.OpenPanelToForgetSpell:
                    PlayerAction.OpenPanelToForgetSpell(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.EnterToDonjon:
                    PlayerAction.EnterToDonjon(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.AddHonor:
                    PlayerAction.AddHonor(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.AddSpellPoints:
                    PlayerAction.AddSpellPoints(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.GiveEnergy:
                    PlayerAction.GiveEnergy(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.AddXP:
                    PlayerAction.AddXP(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.SetLook:
                    PlayerAction.SetLook(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.UnLook:
                    PlayerAction.UnLook(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.CreateGuild:
                    if (perso.Client.CanGameAction(GameActionTypeEnum.GUILD_CREATE))
                    {
                        perso.Client.AddGameAction(new GameCreateGuild(perso.Client));
                    }
                    break;
                case ActionTypeEnum.GiveQuestion:
                    NpcAction.GiveQuestion(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.OpenBank:
                    PlayerAction.OpenBank(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.ToogleOnMount:
                    PlayerAction.ToogleOnMount(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.SendMessage:
                    PlayerAction.SendMessange(this, perso, target, itemID, cellID);
                    break;
                case ActionTypeEnum.SetPDVPER:
                    PlayerAction.SetPDVPER(this, perso, target, itemID, cellID);
                    break;

                case ActionTypeEnum.SetTitle:
                    PlayerAction.SetTitle(this, perso, target, itemID, cellID);
                    break;
                default:
                    Logger.Error("Action ID=" + ID + " non implantee");
                    break;
            }
        }

    }
}
