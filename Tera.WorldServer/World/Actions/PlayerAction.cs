using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Handlers;
using Tera.WorldServer.World.Chats;
using Tera.WorldServer.World.Exchanges;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Actions
{
    public static class PlayerAction
    {
        public static void GiveKamas(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                int count = int.Parse(action.args);
                long curKamas = perso.Kamas;
                long newKamas = curKamas + count;
                if (newKamas < 0L)
                {
                    newKamas = 0L;
                }
                perso.Kamas = newKamas;
                if (!perso.IsOnline())
                {
                    return;
                }
                perso.Send(new AccountStatsMessage(perso));
            }
            catch (Exception e)
            {
                return;
            }
        }
        public static void GiveItem(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                int tID = int.Parse(action.args.Split(',')[0]);
                int count = int.Parse(action.args.Split(',')[1]);
                Boolean send = true;
                if (action.args.Split(',').Length > 2)
                {
                    send = action.args.Split(',')[2].Equals("1");
                }
                if (count > 0)
                {
                    var item = InventoryItemTable.TryCreateItem(itemID, perso, count, -1, null);
                }
                else
                {
                    perso.InventoryCache.removeByTemplateID(tID, -count);
                }
                if (perso.IsOnline())//on envoie le Packet qui indique l'ajout//retrait d'un item
                {
                    //POD
                    if (send)
                    {
                        if (count >= 0)
                        {
                            perso.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 21, +count + "~" + tID));
                        }
                        else if (count < 0)
                        {
                            perso.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 22, +-count + "~" + tID));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
        public static void ToogleOnMount(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            if (perso.Level < 60 || perso.Mount == null || !perso.Mount.isMountable())
            {
                perso.Send(new CharacterRideEventMessage("Er", null));
                return;
            }
            perso.toogleOnMount();
        }

        public static void OpenBank(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            if (perso.Deshonor >= 1)
            {
                perso.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 83));
                return;
            }
            int cost = perso.Client.Account.Data.bankItems.Count;
            if (cost > 0)
            {
                long nKamas = perso.Kamas - cost;
                if (nKamas < 0)
                {
                    perso.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 128, cost.ToString()));
                    return;
                }
                perso.InventoryCache.SubstractKamas(cost);
                perso.Send(new AccountStatsMessage(perso));
                perso.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 20, cost.ToString()));
            }
            perso.Send(new ExchangeCreateMessage(ExchangeTypeEnum.EXCHANGE_STORAGE, ""));
            perso.Send(new ExchangeBankListMessage(perso));
            perso.isAaway = true;
            perso.isInBank = true;
        }


        public static void ApplyTeleportion(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                short newMapID = short.Parse(action.args.Split(',')[0]);
                int newCellID = int.Parse(action.args.Split(',')[1]);
                var NextMap = MapTable.Get(newMapID);
                if (NextMap == null)
                {
                    return;
                }

                perso.Teleport(NextMap, newCellID);
            }
            catch (Exception e)
            {
                return;
            }
        }

        internal static void WarpToSavePos(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                perso.Teleport(MapTable.Cache[short.Parse(perso.SavePos.Split(',')[0])], int.Parse(perso.SavePos.Split(',')[1]));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal static void LearnSpell(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                if (target == null)
                    target = perso;
                int sID = int.Parse(action.args);
                if (!SpellTable.Cache.ContainsKey(sID))
                {
                    return;
                }
                target.GetSpellBook().AddSpell(sID, 1, 25, target.Client);
                target.Send(new SpellsListMessage(target));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal static void AddStats(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                int statID = int.Parse(action.args.Split(',')[0]);
                int number = int.Parse(action.args.Split(',')[1]);

                var EffectType = EffectEnum.None;

                if (target != null)
                    perso = target;

                if (!CharacterHandler.BOOST_ID_TO_STATS.TryGetValue(statID, out EffectType))
                {
                    perso.Send(new BasicNoOperationMessage());
                    return;
                }

                lock (perso.Client.BoostStatsSync)
                {
                    switch (EffectType)
                    {
                        case EffectEnum.AddForce:
                            perso.Strength += number;
                            break;

                        case EffectEnum.AddVitalite:
                            perso.Vitality += number;
                            perso.Life += number; // on boost la life
                            break;

                        case EffectEnum.AddSagesse:
                            perso.Wisdom += number;
                            break;

                        case EffectEnum.AddIntelligence:
                            perso.Intell += number;
                            break;

                        case EffectEnum.AddChance:
                            perso.Chance += number;
                            break;

                        case EffectEnum.AddAgilite:
                            perso.Agility += number;
                            break;
                    }
                }

                perso.GetStats().AddBase(EffectType, number);
                perso.Send(new AccountStatsMessage(perso));

                int messID1 = 0;
                switch (EffectType)
                {
                    case EffectEnum.AddIntelligence:
                        messID1 = 14;
                        break;
                    case EffectEnum.AddVitalite:
                        messID1 = 15;
                        break;
                    default:
                        messID1 = 15;
                        break;
                }
                //CharacterTable.Update(perso);
                if (messID1 > 0)
                {
                    //    perso.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, messID1, number.ToString()));
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal static void EatOrDrinkItem(ActionModel actionModel, Player perso, Player target, int itemID, int cellID)
        {
            InventoryItemModel objPain = InventoryItemTable.getItem(itemID);
            if (objPain == null)
            {
                return;
            }
            if (objPain.GetStats().ContainsEffect(EffectEnum.AddVie))
            {
                if (target != null)
                {
                    if ((target.Life >= target.MaxLife) || (objPain.GetStats().GetEffect(EffectEnum.AddVie).Items <= 0))
                    {

                    }
                    else
                    {
                        target.Life += objPain.GetStats().GetEffect(EffectEnum.AddVie).Items;
                        if (target.Life > target.MaxLife)
                        {
                            target.Life = target.MaxLife;
                        }
                        target.Send(new AccountStatsMessage(target));
                    }
                }
                else
                {
                    if ((perso.Life >= perso.MaxLife) || (objPain.GetStats().GetEffect(EffectEnum.AddVie).Items <= 0))
                    {

                    }
                    else
                    {
                        perso.Life += objPain.GetStats().GetEffect(EffectEnum.AddVie).Items;
                        if (perso.Life > perso.MaxLife)
                        {
                            perso.Life = perso.MaxLife;
                        }
                        perso.Send(new AccountStatsMessage(perso));
                        perso.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 1, objPain.GetStats().GetEffect(EffectEnum.AddVie).Items.ToString()));
                    }
                }
            }
            if (objPain.GetStats().ContainsEffect(EffectEnum.AddEnergy))
            {
                int val = objPain.GetStats().GetEffect(EffectEnum.AddEnergy).Items;
                if (target != null)
                {
                    if (/*target.isDead()) || */(target.Energy >= 10000) || (val <= 0))
                    {

                    }
                    else
                    {
                        target.Energy += val;
                        if (target.Energy > 10000)
                        {
                            target.Energy = 10000;
                        }
                        target.Send(new AccountStatsMessage(target));
                    }
                }
                else
                {
                    if ((perso.Energy >= 10000) || (val <= 0))
                    {
                    }
                    else
                    {
                        perso.Energy += val;
                        if (perso.Energy > 10000)
                        {
                            target.Energy = 10000;
                        }
                        perso.Send(new AccountStatsMessage(perso));
                    }
                }
            }

            if (target == null)
            {
                perso.setEmoteActive(0);
                perso.myMap.SendToMap(new MapEatOrDrinkMessage(perso.ActorId));
            }
            else
            {
                if (target.isSitted()) { return; }
                target.myMap.SendToMap(new MapEatOrDrinkMessage(target.ActorId));
                if (target.Orientation != 2) return;
                target.Orientation = 3;
            }

        }

        internal static void ModifAlignement(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                byte newAlign = byte.Parse(action.args.Split(',')[0]);
                Boolean replace = int.Parse(action.args.Split(',')[1]) == 1;

                if (((perso.Alignement != -1) && (!replace)) || (Program.currentTimeMillis() - perso.lastAlignementUpdate) < 2000)
                {
                    return;
                }
                perso.AlignementReset();

                perso.Alignement = newAlign;
                Network.WorldServer.GetChatController().RegisterClient(perso.Client, perso.AlignmentType);

                perso.Send(new PlayerAlignementUpdateMessage(newAlign));
                perso.Send(new AccountStatsMessage(perso));
                perso.RefreshOnMap();
                perso.lastAlignementUpdate = Program.currentTimeMillis();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }



        internal static void ResetStats(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                perso.GetStats().AddBase((EffectEnum)125, -perso.GetStats().GetBase(((EffectEnum)125)));
                perso.GetStats().AddBase((EffectEnum)124, -perso.GetStats().GetBase(((EffectEnum)124)));
                perso.GetStats().AddBase((EffectEnum)118, -perso.GetStats().GetBase(((EffectEnum)118)));
                perso.GetStats().AddBase((EffectEnum)123, -perso.GetStats().GetBase(((EffectEnum)123)));
                perso.GetStats().AddBase((EffectEnum)119, -perso.GetStats().GetBase(((EffectEnum)119)));
                perso.GetStats().AddBase((EffectEnum)126, -perso.GetStats().GetBase(((EffectEnum)126)));

                perso.Strength = 0;
                perso.Vitality = 0;
                perso.Wisdom = 0;
                perso.Intell = 0;
                perso.Agility = 0;
                perso.Chance = 0;

                perso.CaractPoint = (perso.Level - 1) * 5;
                perso.Send(new AccountStatsMessage(perso));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal static void OpenPanelToForgetSpell(ActionModel actionModel, Player perso, Player target, int itemID, int cellID)
        {
            perso.setisForgetingSpell(true);
            perso.Send(new CharacterForgetSpellInterface('+'));
        }

        internal static void EnterToDonjon(ActionModel Model, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                short newMapID = short.Parse(Model.args.Split(',')[0]);
                int newCellID = int.Parse(Model.args.Split(',')[1]);
                int ItemNeed = int.Parse(Model.args.Split(',')[2]);
                int MapNeed = int.Parse(Model.args.Split(',')[3]);

                if (ItemNeed == 0)
                {
                    perso.Teleport(MapTable.Cache[newMapID], newCellID);
                }
                else
                {
                    if (ItemNeed <= 0)
                    {
                        return;
                    }
                    if (MapNeed == 0)
                    {
                        perso.Teleport(MapTable.Cache[newMapID], newCellID);
                    }
                    else
                    {
                        if (MapNeed <= 0)
                        {
                            return;
                        }
                        if ((perso.InventoryCache.hasItemTemplate(ItemNeed, 1)) && (perso.Map == MapNeed))
                        {
                            perso.Teleport(MapTable.Cache[newMapID], newCellID);
                            perso.InventoryCache.removeByTemplateID(ItemNeed, 1);
                            //TODO POD PACKET
                        }
                        else if (perso.Map != MapNeed)
                        {
                            perso.Send(new ChatGameMessage("Vous n'etes pas sur la bonne map du donjon pour etre teleporter.", "009900"));
                        }
                        else
                        {
                            perso.Send(new ChatGameMessage("Vous ne possedez pas la clef necessaire.", "009900"));
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal static void AddHonor(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                int AddHonor = int.Parse(action.args);
                perso.AddHonor(AddHonor);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal static void AddSpellPoints(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            int pts = int.Parse(action.args);
            if (pts < 1)
            {
                return;
            }
            perso.SpellPoint += pts;
            perso.Send(new AccountStatsMessage(perso));
        }

        internal static void GiveEnergy(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                int Energy = int.Parse(action.args);
                if (Energy < 1)
                {
                    return;
                }

                int EnergyTotal = perso.Energy + Energy;
                if (EnergyTotal > 10000)
                {
                    EnergyTotal = 10000;
                }

                perso.Energy = EnergyTotal;
                perso.Send(new AccountStatsMessage(perso));
            }
            catch (Exception e) { Logger.Error(e); }
        }

        internal static void AddXP(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            long XpAdd = int.Parse(action.args);
            if (XpAdd < 1L)
            {
                return;
            }
            perso.AddExperience(XpAdd);
            perso.Send(new AccountStatsMessage(perso));
        }

        internal static void SetLook(ActionModel action, Player perso, Player target, int itemID, int cellID)
        {
            int morphID = int.Parse(action.args);
            if (morphID < 0)
            {
                return;
            }
            perso.Look = morphID;
            perso.RefreshOnMap();
        }



        internal static void UnLook(ActionModel actionModel, Player perso, Player target, int itemID, int cellID)
        {
            int UnMorphID = perso.Classe * 10 + perso.Sexe;
            perso.Look = UnMorphID;
            perso.RefreshOnMap();
        }

        internal static void SendMessange(ActionModel actionModel, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                String[] Args = actionModel.args.Split('%');
                //Logger.Error("OldMesage "+Args[0]+" \n new Message = "+Regex.Replace(Args[0], "\n", Environment.NewLine));
                perso.Send(new ChatGameMessage(Args[0].Trim(), Args[1].Trim()));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal static void SetPDVPER(ActionModel actionModel, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                if (target != null) perso = target;
                perso.Life = perso.MaxLife;
                perso.Send(new AccountStatsMessage(perso));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal static void PlacePrisme(ActionModel actionModel, Player perso, Player target, int itemID, int cellID)
        {
            try
            {
                if (perso.CellId <= 0)
                    return;
                if (perso.Deshonor >= 1)
                    perso.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 83));
                if (perso.getGrade() < 3)
                    perso.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 155));
                if (perso.Alignement == 0 || perso.Alignement == 3)
                {
                    perso.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 34, "43"));
                    return;
                }
                if (!perso.showWings)
                {
                    perso.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 148));
                    return;
                }
                if (perso.myMap.FightCell == null || Regex.Split(perso.myMap.FightCell, "\\|").Length != 2)
                {
                    perso.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 146));
                    return;
                }
                if (perso.myMap.subArea.Alignement != 0 || !perso.myMap.subArea.CanConquest)
                {
                    perso.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 149));
                    return;
                }
                var Prisme = new Prisme()
                {
                    ActorId = PrismeTable.getPrismeID(),
                    Alignement = perso.Alignement,
                    Level = 1,
                    Mapid = perso.myMap.Id,
                    CellId = perso.CellId,
                    Honor = 0,
                    Area = -1,
                    inFight = -1,
                    Orientation = 1,
                };
                perso.myMap.subArea.setAlignement(perso.Alignement);
                perso.myMap.subArea.Prisme = Prisme.ActorId;

                Network.WorldServer.GetChatController().getAlignementChannel(AlignmentTypeEnum.ALIGNMENT_NEUTRAL).Send(new SubAreaAlignMessage(perso.myMap.subArea.ID + "|" + perso.Alignement + "|1"));
                if (perso.myMap.subArea.area.Alignement == 0)
                    Network.WorldServer.GetChatController().getAlignementChannel(AlignmentTypeEnum.ALIGNMENT_NEUTRAL).Send(new AreaAlignMessage(perso.myMap.subArea.areaID + "|" + perso.Alignement));

                Network.WorldServer.GetChatController().getAlignementChannel(AlignmentTypeEnum.ALIGNMENT_BONTARIAN).Send(new SubAreaAlignMessage(perso.myMap.subArea.ID + "|" + perso.Alignement + "|0"));
                if (perso.myMap.subArea.area.Alignement == 0)
                    Network.WorldServer.GetChatController().getAlignementChannel(AlignmentTypeEnum.ALIGNMENT_BONTARIAN).Send(new AreaAlignMessage(perso.myMap.subArea.areaID + "|" + perso.Alignement));

                Network.WorldServer.GetChatController().getAlignementChannel(AlignmentTypeEnum.ALIGNMENT_BRAKMARIAN).Send(new SubAreaAlignMessage(perso.myMap.subArea.ID + "|" + perso.Alignement + "|0"));
                if (perso.myMap.subArea.area.Alignement == 0)
                    Network.WorldServer.GetChatController().getAlignementChannel(AlignmentTypeEnum.ALIGNMENT_BRAKMARIAN).Send(new AreaAlignMessage(perso.myMap.subArea.areaID + "|" + perso.Alignement));

                if (perso.myMap.subArea.area.Alignement == 0)
                {
                    perso.myMap.subArea.area.Prisme = Prisme.ActorId;
                    perso.myMap.subArea.area.Alignement = perso.Alignement;
                    Prisme.Area = perso.myMap.subArea.area.ID;
                }

                PrismeTable.Add(Prisme);

                perso.myMap.SpawnActor(Prisme);
                PrismeTable.DestroyPrismGepositionCache();

            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        internal static void SetTitle(ActionModel actionModel, Player perso, Player target, int itemID, int cellID)
        {
            short title;
            if (!short.TryParse(actionModel.args, out title))
            {
                return;
            }
            target.Title = title;
            target.RefreshOnMap();
        }
    }
}
