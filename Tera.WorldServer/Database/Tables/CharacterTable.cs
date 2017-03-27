using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using MySql.Data.MySqlClient;
using Tera.WorldServer.Utils;
using Tera.Libs;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.Database.Tables
{
    public class CharacterTable
    {
        public static string Collums = "guid,owner,name,level,color1,color2,color3,look,sexe,classe,map,cell,restriction,experience,kamas,capital,spellboost,lifeper,energy,ap,mp,vitality,wisdom,strength,intell,agility,chance,alignement,honor,deshonor,stuff,spells,savepos,zaaps,mount,mountxpgive,wornitem,seeAlign,title,cote";
        public static Dictionary<long, Player> myCharacterById = new Dictionary<long, Player>();
        private static Dictionary<string, Player> myCharacterByName = new Dictionary<string, Player>();

        public static Dictionary<long, Player> FindAll(int owner)
        {
            Dictionary<long, Player> CharacterList = new Dictionary<long, Player>();
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.RealmProvider.getConnection(),
                    CommandText = "SELECT " + Collums + ",canaux from characters WHERE owner=@owner AND server_id=@s",
                };

                Command.Prepare();
                Command.Parameters.AddWithValue("@owner", owner);
                Command.Parameters.AddWithValue("@s", Settings.Server);

                var reader = DatabaseManager.RealmProvider.ExecuteCommand(Command);

                while (reader.Read())
                {
                    var character = new Models.Player()
                    {
                        ID = reader.GetInt64("guid"),
                        Owner = reader.GetInt32("owner"),
                        Name = reader.GetString("name"),
                        Level = reader.GetInt32("level"),
                        Color1 = reader.GetInt32("color1"),
                        Color2 = reader.GetInt32("color2"),
                        Color3 = reader.GetInt32("color3"),
                        Look = reader.GetInt32("look"),
                        Sexe = reader.GetInt32("sexe"),
                        Classe = reader.GetInt32("classe"),
                        EnabledChannels = reader.GetString("canaux"),
                        Map = reader.GetInt16("map"),
                        CellId = reader.GetInt32("cell"),
                        Restriction = reader.GetInt32("restriction"),
                        Experience = reader.GetInt64("experience"),
                        Kamas = reader.GetInt64("kamas"),
                        CaractPoint = reader.GetInt32("capital"),
                        SpellPoint = reader.GetInt32("spellboost"),
                        LifePer = reader.GetInt32("lifeper"),
                        Energy = reader.GetInt32("energy"),
                        AP = reader.GetInt32("ap"),
                        MP = reader.GetInt32("mp"),
                        Vitality = reader.GetInt32("vitality"),
                        Wisdom = reader.GetInt32("wisdom"),
                        Strength = reader.GetInt32("strength"),
                        Intell = reader.GetInt32("intell"),
                        Agility = reader.GetInt32("agility"),
                        Chance = reader.GetInt32("chance"),
                        Alignement = reader.GetInt32("alignement"),
                        Honor = reader.GetInt32("honor"),
                        Deshonor = reader.GetInt32("deshonor"),
                        Stuff = reader.GetString("stuff"),
                        SpellString = reader.GetString("spells"),
                        SavePos = reader.GetString("savepos"),
                        ZaapString = reader.GetString("zaaps"),
                        MountID = reader.GetInt32("mount"),
                        MountXPGive = reader.GetInt32("mountxpgive"),
                        WornItem = reader.GetString("wornitem"),
                        Title = reader.GetInt16("title"),
                        Cote = reader.GetInt32("cote"),
                        showWings = reader.GetInt32("seeAlign") == 1 ? true : false
                    };
                    CharacterList.Add(character.ID, character);
                    int guildId = CharactersGuildTable.playerIsOnGuild(character.ID);
                    if (guildId >= 0)
                    {
                        character.setCharacterGuild(GuildTable.GetGuild(guildId).GetMember(character.ID));
                    }
                    AddCharacter(character);
                }
                reader.Close();
            }
            catch (System.InvalidOperationException e1)
            {
                DatabaseManager.RealmProvider.Restart();
                return FindAll(owner);
            }
            return CharacterList;
        }

        public static Boolean Contains(String name)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.RealmProvider.getConnection(),
                    CommandText = "SELECT name from characters WHERE name LIKE @name",
                };

                Command.Prepare();
                Command.Parameters.AddWithValue("@name", name);

                var reader = DatabaseManager.RealmProvider.ExecuteCommand(Command);

                if (reader.Read())
                {
                    if (string.Equals(reader.GetString("name"), name, StringComparison.OrdinalIgnoreCase))
                    {
                        reader.Close();
                        return true;
                    }
                }
                reader.Close();
            }
            catch (System.InvalidOperationException e1)
            {
                DatabaseManager.RealmProvider.Restart();
                return Contains(name);
            }
            return false;
        }

        public static Boolean Add(Models.Player character)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.RealmProvider.getConnection(),
                    CommandText = "INSERT INTO characters (" + Collums + ",server_id) VALUES           (@guid,@owner,@name,@level,@color1,@color2,@color3,@look,@sexe,@class,@map,@CellId,@res,@experience,@kamas,@capital,@spellboost,@lifeper,@energy,@ap,@mp,@vitality,@wisdom,@strength,@intell,@agility,@chance,@align,@h,@ds,@stuff,@spells,@pos,@zaap,@mount,@mxp,@witem,@hw,@t,@server)",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", character.ID);
                Command.Parameters.AddWithValue("@server", Settings.Server);
                Command.Parameters.AddWithValue("@owner", character.Owner);
                Command.Parameters.AddWithValue("@name", character.Name);
                Command.Parameters.AddWithValue("@level", character.Level);
                Command.Parameters.AddWithValue("@color1", character.Color1);
                Command.Parameters.AddWithValue("@color2", character.Color2);
                Command.Parameters.AddWithValue("@color3", character.Color3);
                Command.Parameters.AddWithValue("@look", character.Look);
                Command.Parameters.AddWithValue("@sexe", character.Sexe);
                Command.Parameters.AddWithValue("@class", character.Classe);
                Command.Parameters.AddWithValue("@map", character.Map);
                Command.Parameters.AddWithValue("@CellId", character.CellId);
                Command.Parameters.AddWithValue("@res", character.Restriction);
                Command.Parameters.AddWithValue("@experience", character.Experience);
                Command.Parameters.AddWithValue("@kamas", character.Kamas);
                Command.Parameters.AddWithValue("@capital", character.CaractPoint);
                Command.Parameters.AddWithValue("@spellboost", character.SpellPoint);
                Command.Parameters.AddWithValue("@energy", character.Energy);
                Command.Parameters.AddWithValue("@lifeper", character.LifePer);
                Command.Parameters.AddWithValue("@ap", character.AP);
                Command.Parameters.AddWithValue("@mp", character.MP);
                Command.Parameters.AddWithValue("@vitality", character.Vitality);
                Command.Parameters.AddWithValue("@wisdom", character.Wisdom);
                Command.Parameters.AddWithValue("@strength", character.Strength);
                Command.Parameters.AddWithValue("@intell", character.Intell);
                Command.Parameters.AddWithValue("@agility", character.Agility);
                Command.Parameters.AddWithValue("@chance", character.Chance);
                Command.Parameters.AddWithValue("@align", character.Alignement);
                Command.Parameters.AddWithValue("@h", character.Honor);
                Command.Parameters.AddWithValue("@ds", character.Deshonor);
                Command.Parameters.AddWithValue("@stuff", character.Stuff);
                Command.Parameters.AddWithValue("@pos", character.SavePos);
                Command.Parameters.AddWithValue("@zaap", character.parseZaaps());
                Command.Parameters.AddWithValue("@mount", character.MountID);
                Command.Parameters.AddWithValue("@mxp", character.MountXPGive);
                Command.Parameters.AddWithValue("@witem", ",,,,");
                Command.Parameters.AddWithValue("@t", character.Title);
                Command.Parameters.AddWithValue("@cote", character.Cote);
                Command.Parameters.AddWithValue("@hw", 0);
                if (character.GetSpellBook() != null)
                {
                    Command.Parameters.AddWithValue("@spells", character.GetSpellBook().ToDatabase());
                }
                else
                    Command.Parameters.AddWithValue("@spells", "");

                return DatabaseManager.RealmProvider.ExecuteQuery(Command);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return false;
            }
        }

        public static void Update(Player character)
        {
            try
            {
                MySqlCommand Command = new MySqlCommand()
                {
                    Connection = DatabaseManager.RealmProvider.getConnection(),
                    CommandText = "UPDATE characters SET owner = @owner,name = @name,level = @level,color1 = @color1,color2 = @color2,color3 = @color3,look = @look,sexe = @sexe,classe = @class,map = @map,cell = @CellId,restriction = @res,experience = @experience,kamas = @kamas,capital = @capital,spellboost = @spellboost,lifeper = @lifeper,energy = @energy,ap = @ap,mp = @mp,vitality = @vitality,wisdom = @wisdom,strength = @strength,intell = @intell,agility = @agility,chance = @chance,alignement = @align,honor = @h,deshonor = @ds, stuff= @stuff, spells = @spells, savepos = @spos, zaaps = @za, mount = @mount, mountxpgive = @mxp, wornitem = @witem, seeAlign = @hw, title = @ti, cote = @cote WHERE guid = @guid",
                };
                Command.Prepare();
                Command.Parameters.AddWithValue("@guid", character.ID);
                Command.Parameters.AddWithValue("@owner", character.Owner);
                Command.Parameters.AddWithValue("@name", character.Name);
                Command.Parameters.AddWithValue("@level", character.Level);
                Command.Parameters.AddWithValue("@color1", character.Color1);
                Command.Parameters.AddWithValue("@color2", character.Color2);
                Command.Parameters.AddWithValue("@color3", character.Color3);
                Command.Parameters.AddWithValue("@look", character.Look);
                Command.Parameters.AddWithValue("@sexe", character.Sexe);
                Command.Parameters.AddWithValue("@class", character.Classe);
                Command.Parameters.AddWithValue("@map", character.Map);
                Command.Parameters.AddWithValue("@CellId", character.CellId);
                Command.Parameters.AddWithValue("@res", character.Restriction);
                Command.Parameters.AddWithValue("@experience", character.Experience);
                Command.Parameters.AddWithValue("@kamas", character.Kamas);
                Command.Parameters.AddWithValue("@capital", character.CaractPoint);
                Command.Parameters.AddWithValue("@spellboost", character.SpellPoint);
                Command.Parameters.AddWithValue("@energy", character.Energy);
                Command.Parameters.AddWithValue("@lifeper", character.GetPDVper());
                Command.Parameters.AddWithValue("@ap", character.AP);
                Command.Parameters.AddWithValue("@mp", character.MP);
                Command.Parameters.AddWithValue("@vitality", character.Vitality);
                Command.Parameters.AddWithValue("@wisdom", character.Wisdom);
                Command.Parameters.AddWithValue("@strength", character.Strength);
                Command.Parameters.AddWithValue("@intell", character.Strength);
                Command.Parameters.AddWithValue("@agility", character.Agility);
                Command.Parameters.AddWithValue("@chance", character.Chance);
                Command.Parameters.AddWithValue("@align", character.Alignement);
                Command.Parameters.AddWithValue("@h", character.Honor);
                Command.Parameters.AddWithValue("@ds", character.Deshonor);
                Command.Parameters.AddWithValue("@stuff", character.parseItemsToDB());
                Command.Parameters.AddWithValue("@spells", character.GetSpellBook().ToDatabase());
                Command.Parameters.AddWithValue("@spos", character.SavePos);
                Command.Parameters.AddWithValue("@za", character.parseZaaps());
                Command.Parameters.AddWithValue("@mount", character.Mount != null ? character.Mount.ID : -1);
                Command.Parameters.AddWithValue("@mxp", character.MountXPGive);
                Command.Parameters.AddWithValue("@hw", character.showWings ? 1 : 0);
                Command.Parameters.AddWithValue("@ti", character.Title);
                Command.Parameters.AddWithValue("@cote", character.Cote);
                if(character.InventoryCache == null)
                    Command.Parameters.AddWithValue("@witem", ",,,,");
                else
                    Command.Parameters.AddWithValue("@witem", character.InventoryCache.SerializeAsDisplayEquipment());
                DatabaseManager.RealmProvider.ExecuteQuery(Command);
                if (character.InventoryCache != null)
                {
                    InventoryItemTable.Update(character.InventoryCache);
                }
                if (character.getCharacterGuild() != null)
                {
                    CharactersGuildTable.Add(character.getCharacterGuild());
                }
                if (character.Mount != null)
                {
                    MountTable.Update(character.Mount);
                }
            }
            catch (System.InvalidOperationException e1)
            {
                DatabaseManager.RealmProvider.Restart();
                Update(character);
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }

        public static Boolean Delete(Models.Player character)
        {
            try
            {
                DatabaseManager.RealmProvider.ExecuteQuery("DELETE FROM characters WHERE guid = '" + character.ID + "'");
                if (character.getCharacterGuild() != null)
                {
                    character.GetGuild().RemovePlayer(character.getCharacterGuild());
                }
                if (character.InventoryCache != null && character.InventoryCache.ItemsCache.Count > 0)
                {
                    try
                    {
                        MySqlCommand Command = new MySqlCommand()
                        {
                            Connection = DatabaseManager.Provider.getConnection(),
                            CommandText = "DELETE FROM inventory_item WHERE guid IN @items ;",
                        };
                        Command.Prepare();
                        Command.Parameters.AddWithValue("@items", character.parseItemsToDB(','));
                        Command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Logger.Error("Can't execute query : " + e.ToString());
                    }
                }
                if (character.Mount != null)
                {
                    DatabaseManager.RealmProvider.ExecuteQuery("DELETE FROM mounts_data WHERE id = '" + character.Mount.ID + "'");
                    MountTable.Cache.Remove(character.Mount.ID);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Can't execute query : " + e.ToString());
                return false;
            }
            return true;
        }

        public static int getNextGuid()
        {
            var reader = DatabaseManager.RealmProvider.ExecuteReader("SELECT MAX(guid) AS max FROM characters");
            int id = 0;
            if (reader.Read())
            {
                try
                {
                    id = reader.GetInt32("max");
                }
                catch (Exception e) { }
            }
            reader.Close();
            return id;
        }

        public static void AddCharacter(Player character)
        {
            lock (myCharacterById)
                myCharacterById.Add(character.ID, character);

            lock (myCharacterByName)
                myCharacterByName.Add(character.Name.ToLower(), character);
        }

        public static void DelCharacter(Player character)
        {
            lock (myCharacterById)
                myCharacterById.Remove(character.ID);

            lock (myCharacterByName)
                myCharacterByName.Remove(character.Name.ToLower());
        }

        public static Player GetCharacter(long CharacterId)
        {
            Player Character = null;
            lock (myCharacterById)
                myCharacterById.TryGetValue(CharacterId, out Character);
            return Character;
        }

        public static Player GetCharacter(string CharacterName)
        {
            Player Character = null;
            lock (myCharacterByName)
                myCharacterByName.TryGetValue(CharacterName.ToLower(), out Character);
            return Character;
        }
    }
}
