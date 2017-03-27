using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Actions;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Maps
{
    public class DofusCell
    {
        public int Id;
        public Dictionary<int, Player> _persos;
        public Boolean Walkable = true;
        public Boolean LoS = true;
        public int groundSlope = 1;
        public int groundLevel = 7;
        public short Map;
        private List<CellAction> myActions = new List<CellAction>();
        private Dictionary<long, IGameActor> myActors = new Dictionary<long, IGameActor>();
        public IObject Object;

        public void AddAction(CellAction Action)
        {
            this.myActions.Add(Action);
        }

        public void AddActor(IGameActor Actor)
        {
            lock (this.myActors)
                this.myActors.Add(Actor.ActorId, Actor);

            // on affecte la cell
            Actor.CellId = this.Id;

            if (Actor.ActorType == GameActorTypeEnum.TYPE_CHARACTER)
                foreach (var Action in this.myActions)
                    Action.Apply(Actor as Player);
        }

        public void DelActor(IGameActor Actor)
        {
            lock (this.myActors)
                this.myActors.Remove(Actor.ActorId);
        }

        public void startAction(Player perso, MiniGameAction GA)
        {
            if (GA == null)
            {
                return;
            }
            int actionID = -1;
            short CcellID = -1;
            try
            {
                actionID = int.Parse(GA._args.Split(';')[1]);
                CcellID = short.Parse(GA._args.Split(';')[0]);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            if (actionID == -1)
            {
                return;
            }
            //Verif JobAction ?
            switch ((MiniGameActionEnum)actionID)
            {
                case MiniGameActionEnum.SAVE_POS:
                    String str = this.Map + "," + this.Id;
                    perso.SavePos = str;
                    perso.Client.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 6));
                    break;
                case MiniGameActionEnum.USE_ZAAP:
                    perso.openZaapMenu();
                    perso.GetClient().miniActions.Remove(GA._id);
                    break;
                case MiniGameActionEnum.USE_ZAAPI:
                    perso.openZaapiMenu();
                    perso.GetClient().miniActions.Remove(GA._id);
                    break;
                case MiniGameActionEnum.PUISER:
                    if (!Object.isInteractive())
                    {
                        perso.GetClient().Send(new BasicNoOperationMessage());
                        return;
                    }
                    if (Object.getState() != (int)IObjectEnum.STATE_FULL)
                    {
                        perso.GetClient().Send(new BasicNoOperationMessage());
                        return;//Si le puits est vide
                    }
                    Object.setState((int)IObjectEnum.STATE_EMPTYING);
                    Object.setInteractive(false);
                    perso.myMap.SendToMap(new MapGameActionMessage(GA._id, 501, perso.ID + "", this.Id + "," + Object.getUseDuration() + "," + Object.getUnknowValue()));
                    perso.myMap.SendToMap(new GameMapObjectMessage(perso.myMap));
                    break;
                case MiniGameActionEnum.RETURN_TO_INCARNAM:
                    if (perso.Level > 15)
                    {
                        perso.Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 127));
                        perso.GetClient().miniActions.Remove(GA._id);
                        return;
                    }
                    //TODO Map By Perso
                    perso.Teleport(MapTable.Get(10298), 314);
                    perso.GetClient().miniActions.Remove(GA._id);
                    break;
                case MiniGameActionEnum.ACCESS_TO_MOUNTPARK:
                    if(Object.getState() != (int) IObjectEnum.STATE_EMPTY);
                    if(perso.Deshonor >= 5)
                    {
                        perso.Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 83));
                        perso.GetClient().miniActions.Remove(GA._id);
                        return;
                    }
                    perso.inMountPark = perso.myMap.mountPark;
                    perso.isAaway = true;
                    perso.Send(new GameActionEnvironementMessage(16, perso.GetClient().Account.parseDragoList()));
                    break;
                case MiniGameActionEnum.BUY_MOUNTPARK:
                    MountPark MP = perso.myMap.mountPark;
                    if (MP.get_owner() == -1)//Public
					{
                        perso.Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 96));
						return;
					}
					if (MP.get_price() == 0)//Non en vente
					{
                        perso.Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 97));
						return;
					}
					if (!perso.HasGuild())//Pas de guilde
					{
                        perso.Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 135));
						return;
					}
					if (perso.getCharacterGuild().Grade != 1)//Non meneur
					{
                        perso.Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 98));
						return;
					}
                    perso.Send(new GameRideMessage("D" + MP.get_price() + "|" + MP.get_price()));
                    break;
                case MiniGameActionEnum.SELL_MOUNTPARK:
                case MiniGameActionEnum.SET_MOUNTPARK_PRICE:
                    MountPark MP1 = perso.myMap.mountPark;
                    if (MP1 == null) return;
                    if (MP1.get_owner() == -1)//Public
                    {
                        perso.Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 94));
                        return;
                    }
                    if (MP1.get_owner() != perso.ID)
                    {
                        perso.Client.Send(new TextInformationMessage(TextInformationTypeEnum.ERREUR, 95));
                        return;
                    }
                    perso.Send(new GameRideMessage("D" + MP1.get_price() + "|" + MP1.get_price()));
                    break;
                default:
                    Logger.Error("Case.startAction non definie pour l'actionID = " + actionID);
                    break;
            }
        }

        public Boolean isWalkable(Boolean useObject)
        {
            if (Object != null && useObject)
            {
                return Walkable && Object.isWalkable();
            }
            return Walkable;
        }

        public Boolean canDoAction(int id)
        {
            if (Object == null)
            {
                return false;
            }
            switch (id)
            {
                //Moudre et egrenner - Paysan
                case 122:
                case 47:
                    return Object.getID() == 7007;
                //Faucher Blé
                case 45:
                    switch (Object.getID())
                    {
                        case 7511://Blé
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Faucher Orge
                case 53:
                    switch (Object.getID())
                    {
                        case 7515://Orge
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;

                //Faucher Avoine
                case 57:
                    switch (Object.getID())
                    {
                        case 7517://Avoine
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Faucher Houblon
                case 46:
                    switch (Object.getID())
                    {
                        case 7512://Houblon
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Faucher Lin
                case 50:
                case 68:
                    switch (Object.getID())
                    {
                        case 7513://Lin
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Faucher Riz
                case 159:
                    switch (Object.getID())
                    {
                        case 7550://Riz
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Faucher Seigle
                case 52:
                    switch (Object.getID())
                    {
                        case 7516://Seigle
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Faucher Malt
                case 58:
                    switch (Object.getID())
                    {
                        case 7518://Malt
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Faucher Chanvre - Cueillir Chanvre
                case 69:
                case 54:
                    switch (Object.getID())
                    {
                        case 7514://Chanvre
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Scier - Bucheron
                case 101:
                    return Object.getID() == 7003;
                //Couper Frêne
                case 6:
                    switch (Object.getID())
                    {
                        case 7500://Frêne
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Châtaignier
                case 39:
                    switch (Object.getID())
                    {
                        case 7501://Châtaignier
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Noyer
                case 40:
                    switch (Object.getID())
                    {
                        case 7502://Noyer
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Chêne
                case 10:
                    switch (Object.getID())
                    {
                        case 7503://Chêne
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Oliviolet
                case 141:
                    switch (Object.getID())
                    {
                        case 7542://Oliviolet
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Bombu
                case 139:
                    switch (Object.getID())
                    {
                        case 7541://Bombu
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Erable
                case 37:
                    switch (Object.getID())
                    {
                        case 7504://Erable
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Bambou
                case 154:
                    switch (Object.getID())
                    {
                        case 7553://Bambou
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper If
                case 33:
                    switch (Object.getID())
                    {
                        case 7505://If
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Merisier
                case 41:
                    switch (Object.getID())
                    {
                        case 7506://Merisier
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Ebène
                case 34:
                    switch (Object.getID())
                    {
                        case 7507://Ebène
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Kalyptus
                case 174:
                    switch (Object.getID())
                    {
                        case 7557://Kalyptus
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Charme
                case 38:
                    switch (Object.getID())
                    {
                        case 7508://Charme
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Orme
                case 35:
                    switch (Object.getID())
                    {
                        case 7509://Orme
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Bambou Sombre
                case 155:
                    switch (Object.getID())
                    {
                        case 7554://Bambou Sombre
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Couper Bambou Sacré
                case 158:
                    switch (Object.getID())
                    {
                        case 7552://Bambou Sacré
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Puiser
                case 102:
                    switch (Object.getID())
                    {
                        case 7519://Puits
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Polir
                case 48:
                    return Object.getID() == 7005;//7510
                //Moule/Fondre - Mineur
                case 32:
                    return Object.getID() == 7002;
                //Miner Fer
                case 24:
                    switch (Object.getID())
                    {
                        case 7520://Miner
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Miner Cuivre
                case 25:
                    switch (Object.getID())
                    {
                        case 7522://Miner
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Miner Bronze
                case 26:
                    switch (Object.getID())
                    {
                        case 7523://Miner
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Miner Kobalte
                case 28:
                    switch (Object.getID())
                    {
                        case 7525://Miner
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Miner Manga
                case 56:
                    switch (Object.getID())
                    {
                        case 7524://Miner
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Miner Sili
                case 162:
                    switch (Object.getID())
                    {
                        case 7556://Miner
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Miner Etain
                case 55:
                    switch (Object.getID())
                    {
                        case 7521://Miner
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Miner Argent
                case 29:
                    switch (Object.getID())
                    {
                        case 7526://Miner
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Miner Bauxite
                case 31:
                    switch (Object.getID())
                    {
                        case 7528://Miner
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Miner Or
                case 30:
                    switch (Object.getID())
                    {
                        case 7527://Miner
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Miner Dolomite
                case 161:
                    switch (Object.getID())
                    {
                        case 7555://Miner
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Fabriquer potion - Alchimiste
                case 23:
                    return Object.getID() == 7019;
                //Cueillir Trèfle
                case 71:
                    switch (Object.getID())
                    {
                        case 7533://Trèfle
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Cueillir Menthe
                case 72:
                    switch (Object.getID())
                    {
                        case 7534://Menthe
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Cueillir Orchidée
                case 73:
                    switch (Object.getID())
                    {
                        case 7535:// Orchidée
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Cueillir Edelweiss
                case 74:
                    switch (Object.getID())
                    {
                        case 7536://Edelweiss
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Cueillir Graine de Pandouille
                case 160:
                    switch (Object.getID())
                    {
                        case 7551://Graine de Pandouille
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Vider - Pêcheur
                case 133:
                    return Object.getID() == 7024;
                //Pêcher Petits poissons de mer
                case 128:
                    switch (Object.getID())
                    {
                        case 7530://Petits poissons de mer
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Pêcher Petits poissons de rivière
                case 124:
                    switch (Object.getID())
                    {
                        case 7529://Petits poissons de rivière
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Pêcher Pichon
                case 136:
                    switch (Object.getID())
                    {
                        case 7544://Pichon
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Pêcher Ombre Etrange
                case 140:
                    switch (Object.getID())
                    {
                        case 7543://Ombre Etrange
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Pêcher Poissons de rivière
                case 125:
                    switch (Object.getID())
                    {
                        case 7532://Poissons de rivière
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Pêcher Poissons de mer
                case 129:
                    switch (Object.getID())
                    {
                        case 7531://Poissons de mer
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Pêcher Gros poissons de rivière
                case 126:
                    switch (Object.getID())
                    {
                        case 7537://Gros poissons de rivière
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Pêcher Gros poissons de mers
                case 130:
                    switch (Object.getID())
                    {
                        case 7538://Gros poissons de mers
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Pêcher Poissons géants de rivière
                case 127:
                    switch (Object.getID())
                    {
                        case 7539://Poissons géants de rivière
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Pêcher Poissons géants de mer
                case 131:
                    switch (Object.getID())
                    {
                        case 7540://Poissons géants de mer
                            return Object.getState() == (int)IObjectEnum.STATE_FULL;
                    }
                    return false;
                //Boulanger
                case 109://Pain
                case 27://Bonbon
                    return Object.getID() == 7001;
                //Poissonier
                case 135://Faire un poisson (mangeable)
                    return Object.getID() == 7022;
                //Chasseur
                case 134:
                    return Object.getID() == 7023;
                //Boucher
                case 132:
                    return Object.getID() == 7025;
                case 157:
                    return (Object.getID() == 7030 || Object.getID() == 7031);
                case 44://Sauvegarder le Zaap
                case 114://Utiliser le Zaap
                    switch (Object.getID())
                    {
                        //Zaaps
                        case 7000:
                        case 7026:
                        case 7029:
                        case 4287:
                            return true;
                    }
                    return false;

                case 175://Accéder
                case 176://Acheter
                case 177://Vendre
                case 178://Modifier le prix de vente
                    switch (Object.getID())
                    {
                        //Enclos
                        case 6763:
                        case 6766:
                        case 6767:
                        case 6772:
                            return true;
                    }
                    return false;

                //Se rendre à incarnam
                case 183:
                    switch (Object.getID())
                    {
                        case 1845:
                        case 1853:
                        case 1854:
                        case 1855:
                        case 1856:
                        case 1857:
                        case 1858:
                        case 1859:
                        case 1860:
                        case 1861:
                        case 1862:
                        case 2319:
                            return true;
                    }
                    return false;

                //Enclume magique
                case 1:
                case 113:
                case 115:
                case 116:
                case 117:
                case 118:
                case 119:
                case 120:
                    return Object.getID() == 7020;

                //Enclume
                case 19:
                case 143:
                case 145:
                case 144:
                case 142:
                case 146:
                case 67:
                case 21:
                case 65:
                case 66:
                case 20:
                case 18:
                    return Object.getID() == 7012;

                //Costume Mage
                case 167:
                case 165:
                case 166:
                    return Object.getID() == 7036;

                //Coordo Mage
                case 164:
                case 163:
                    return Object.getID() == 7037;

                //Joai Mage
                case 168:
                case 169:
                    return Object.getID() == 7038;

                //Bricoleur
                case 171:
                case 182:
                    return Object.getID() == 7039;

                //Forgeur Bouclier
                case 156:
                    return Object.getID() == 7027;

                //Coordonier
                case 13:
                case 14:
                    return Object.getID() == 7011;

                //Tailleur (Dos)
                case 123:
                case 64:
                    return Object.getID() == 7015;


                //Sculteur
                case 17:
                case 16:
                case 147:
                case 148:
                case 149:
                case 15:
                    return Object.getID() == 7013;

                //Tailleur (Haut)
                case 63:
                    return (Object.getID() == 7014 || Object.getID() == 7016);
                //Atelier : Créer Amu // Anneau
                case 11:
                case 12:
                    return (Object.getID() >= 7008 && Object.getID() <= 7010);
                //Maison
                case 81://Vérouiller
                case 84://Acheter
                case 97://Entrer
                case 98://Vendre
                case 108://Modifier le prix de vente
                    return (Object.getID() >= 6700 && Object.getID() <= 6776);
                //Coffre	
                case 104://Ouvrir
                case 105://Code
                    return (Object.getID() == 7350 || Object.getID() == 7351 || Object.getID() == 7353);
                //Action ID non trouvé
                default:
                    Logger.Error("MapActionID non existant dans Case.canDoAction: " + id);
                    return false;
            }
        }
    }
}
