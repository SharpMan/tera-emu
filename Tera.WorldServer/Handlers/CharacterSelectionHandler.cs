using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.Utils;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Database;
using Tera.Libs.Helper;
using System.Text.RegularExpressions;
using Tera.Libs;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.Handlers
{
    public static class CharacterSelectionHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'A': // Creation
                    CharacterSelectionHandler.ProcessCharacterCreationRequest(Client, Packet);
                    break;

                case 'D': // Delete
                    CharacterSelectionHandler.ProcessCharacterDeletionRequest(Client, Packet);
                    break;

                case 'L': // List
                    CharacterSelectionHandler.ProcessCharactersListRequest(Client);
                    break;

                case 'S': // Selection
                    CharacterSelectionHandler.ProcessCharacterSelectionRequest(Client, Packet);
                    break;
                case 'P':
                    CharacterSelectionHandler.ProcessCharactersGenerateName(Client);
                    break;
                case 'R':
                    //CharacterSelectionHandler.ProcessCharacterResetResquest(Client, Packet);
                    break;
                case 'f':
                    CharacterSelectionHandler.ProcessQueueMessage(Client);
                    break;
            }
        }

        private static void ProcessCharacterCreationRequest(WorldClient Client, string Packet)
        {
            var characterInfos = Packet.Substring(2);

            // fake Packet
            if (!characterInfos.Contains('|'))
            {
                Client.Disconnect();
                return;
            }

            var infos = characterInfos.Split('|');

            // fake Packet
            if (infos.Length != 6)
            {
                Client.Disconnect();
                return;
            }

            String first = (infos[0][0].ToString()).ToUpper();
            var Name = first + infos[0].Substring(1).ToLower();

            var Class = int.Parse(infos[1]);
            var Sex = int.Parse(infos[2]);
            var Color1 = int.Parse(infos[3]);
            var Color2 = int.Parse(infos[4]);
            var Color3 = int.Parse(infos[5]);

            // trop de personnage
            if (Client.Account.Characters.Count > (Settings.AppSettings.GetIntElement("Account.MaxPlayer") - 1))
            {
                Client.Send(new CharacterSlotFullMessage());
                return;
            }

            // fake class
            if (!StringHelper.isValidName(Name) || Class < (int)ClassEnum.CLASS_FECA || Class > (int)ClassEnum.CLASS_PANDAWA)
            {
                Client.Send(new CharacterCreationFailMessage());
                return;
            }

            // fake sex
            if (Sex > 1 || Sex < 0)
            {
                Client.Send(new CharacterCreationFailMessage());
                return;
            }

            // fake color
            if (Color1 < -1 || Color2 < -1 || Color3 < -1)
            {
                Client.Send(new CharacterCreationFailMessage());
                return;
            }

            // pseudo deja pris ?
            if (CharacterTable.Contains(Name) || 4 > Name.Length)
            {
                Client.Send(new CharacterNameAlreadyExistMessage());
                return;
            }

            var character = new Database.Models.Player()
            {
                ID = DatabaseCache.nextPlayerGuid++,
                Owner = Client.Account.ID,
                Name = Name,
                Level = Settings.AppSettings.GetIntElement("World.StartLevel"),
                Color1 = Color1,
                Color2 = Color2,
                Color3 = Color3,
                Look = Class * 10,
                Sexe = Sex,
                Classe = Class,
                EnabledChannels = "i*#$p%!?:@",
                Map = Settings.AppSettings.GetShortElement("World.StartMap"),
                CellId = Settings.AppSettings.GetIntElement("World.StartCell"),
                Restriction = 0,
                Experience = ExpFloorTable.GetFloorByLevel(Settings.AppSettings.GetIntElement("World.StartLevel")).Character,
                Kamas = Settings.AppSettings.GetIntElement("World.KamasStart"),
                CaractPoint = (Settings.AppSettings.GetIntElement("World.StartLevel") - 1) * 5,
                SpellPoint = Settings.AppSettings.GetIntElement("World.StartLevel") - 1,
                LifePer = 100,
                Energy = 10000,
                AP = (Settings.AppSettings.GetIntElement("World.StartLevel") >= 100 ? 7 : 6),
                MP = 3,
                Vitality = 0,
                Wisdom = 0,
                Strength = 0,
                Intell = 0,
                Agility = 0,
                Chance = 0,
                Alignement = 0,
                Honor = 0,
                Deshonor = 0,
                Stuff = "",
                MountID = -1,
                MountXPGive = 0,
                Title = 0,
                SavePos = Settings.AppSettings.GetShortElement("World.StartMap") + "," + Settings.AppSettings.GetIntElement("World.StartCell"),
                Account = Client.Account,
            };

            if (!CharacterTable.Add(character))
            {
                Client.Send(new CharacterCreationFailMessage());
                return;
            }

            Client.Account.Characters.Add(character.ID, character);

            using (CachedBuffer Buffer = new CachedBuffer(Client))
            {
                Buffer.Append(new CharacterCreationSuccessMessage());
                Buffer.Append(new CharactersListMessage(Client.Account.Characters));
            }
        }

        private static void ProcessCharactersGenerateName(WorldClient Client)
        {
            Client.Send(new CharacterRandomName());
        }

        private static void ProcessCharactersListRequest(WorldClient Client)
        {
            Client.Send(new CharactersListMessage(Client.Account.Characters));
        }

        private static void ProcessQueueMessage(WorldClient Client)
        {
            Client.Send(new AccountQueuPacket(1, 1, 1, "" + 1, 1));
        }

        private static void ProcessCharacterSelectionRequest(WorldClient Client, string Packet)
        {
            var Account = Client.Account;

            // minimum = header (2) + 4 chiffre
            if (Packet.Length < 3)
            {
                Client.Send(new CharacterSelectionFailMessage());
                return;
            }

            long characterId = -1;
            // fake Packet
            if (!long.TryParse(Packet.Substring(2), out characterId))
            {
                Client.Send(new CharacterSelectionFailMessage());
                return;
            }

            // inexistant
            if (!Account.Characters.ContainsKey(characterId))
            {
                Client.Send(new CharacterSelectionFailMessage());
                return;
            }

            var Character = Client.Account.Characters[characterId];
            Client.Account.curPlayer = Character;
            Character.InventoryCache = new CharacterInventory(Character);

            // on affecte le personnage et change le status du client, en attente de gc
            Client.SetCharacter(Character);
            WorldServer.Network.WorldServer.GetChatController().RegisterClient(Client);
            Client.SetState(WorldState.STATE_GAME_CREATE);

            // Declanche le stockage avant envoi
            Character.BeginCachedBuffer();
            
            Character.Send(new CharacterSelectedInformationsMessage(Character));
            if (Character.getCharacterGuild() != null)
            {
                Character.getCharacterGuild().lastConnection = Client.Account.LastConnectionDate;
                Character.getCharacterGuild().SendGuildSettingsInfos();
            }
            if (Character.Mount != null)
                Character.Send(new CharacterRideEventMessage("+", Character.Mount));
            Character.InventoryCache.RefreshSet();
            Character.Send(new SpellsListMessage(Character));
            Character.Send(new BasicReferenceTimeMessage());
            Character.Send(new SubAreaListMessage());
            Character.Send(new MountXpMessage());
            Character.Send(new SpecialisationSetMessage(Character.Alignement));
            Character.Send(new ChatChannelEnabledMessage(Character.GetEnabledChatChannels()));
            Character.Send(new CharacterRightMessage());
            Character.Send(new CharacterSeeFriendConnection(Client.Account.Data.showFriendConnection));
            Character.Send(new InventoryWeightMessage(0, 2000)); // TODO PODS

            // last connection / ip
            Character.Send(new TextInformationMessage(TextInformationTypeEnum.UNK, 189));
            Character.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 152, Account.LastConnectionDate.Year.ToString(),
                                                                                        Account.LastConnectionDate.Month.ToString(),
                                                                                        Account.LastConnectionDate.Day.ToString(),
                                                                                        Account.LastConnectionDate.Hour.ToString(),
                                                                                        Account.LastConnectionDate.Minute.ToString(),
                                                                                        Account.LastIP
                                                     )
                         );
            // current ip
            Character.Send(new TextInformationMessage(TextInformationTypeEnum.INFO, 153, Client.getIP()));

            if (!Settings.AppSettings.GetStringElement("World.onLogged").Trim().Equals(""))
            {
                Character.Send(new ChatGameMessage(Settings.AppSettings.GetStringElement("World.onLogged"), "FF0000"));
            }

            // Declenche l'envoi des données
            Character.EndCachedBuffer();

            // on affecte
            Account.LastConnectionDate = DateTime.Now;
            Account.LastIP = Client.getIP();
            AccountTable.Update(Account);
        }

        private static void ProcessCharacterDeletionRequest(WorldClient Client, string Packet)
        {
            if (!Packet.Contains('|') || Client.Character != null || Client.Account == null)
            {
                Client.Send(new CharacterDeletionFailMessage());
                return;
            }

            String[] split = Regex.Split(Packet.Substring(2), "\\|");

            if (split.Length < 2)
            {
                Client.Send(new CharacterDeletionFailMessage());
                return;
            }

            long characterId = long.Parse(split[0]);
            String reponse = split.Length > 1 ? split[1] : "";



            if (!Client.Account.HasCharacter(characterId) || !reponse.ToLower().Equals(Client.Account.Reponse.ToLower()))
            {
                Client.Send(new CharacterDeletionFailMessage());
                return;
            }

            if (!CharacterTable.Delete(Client.Account.Characters[characterId]))
            {
                Client.Send(new CharacterDeletionFailMessage());
                return;
            }

            Client.Account.Characters.Remove(characterId);
            Client.Send(new CharactersListMessage(Client.Account.Characters));
        }
    }
}
