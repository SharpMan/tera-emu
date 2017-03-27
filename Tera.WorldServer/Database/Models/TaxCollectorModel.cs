using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Helper;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Database.Models
{
    public class TaxCollector : IGameActor
    {
        public int GuildID { get; set; }
        public long ActorId { get; set; }
        public int Orientation { get; set; }
        public int CellId { get; set; }
        public short Mapid { get; set; }
        public short N1 { get; set; }
        public short N2 { get; set; }
        public byte inFight = 0;
        public Fight CurrentFight = null;
        private StringBuilder mySerializedPattern;
        public Dictionary<long, InventoryItemModel> Items = new Dictionary<long, InventoryItemModel>();
        public Dictionary<long, InventoryItemModel> LogItems = new Dictionary<long, InventoryItemModel>();
        public long Kamas, XP = 0L;
        public Boolean inExchange = false;
        public int TimeTurn = (int)TurnTimeEnum.TAXCOLLECTOR;
        public long LogXP = 0L;
        private Boolean myIntialized = false;
        public string ItemList { get; set; }


        public Guild Guild
        {
            get
            {
                return GuildTable.GetGuild(GuildID);
            }
        }

        public Map Map
        {
            get
            {
                return MapTable.Get(Mapid);
            }
        }

        public void Intialize()
        {
            if (myIntialized)
                return;

            foreach (String str in ItemList.Split(';'))
            {
                try
                {
                    if (String.IsNullOrEmpty(str))
                    {
                        continue;
                    }
                    long idd;
                    if (!long.TryParse(str, out idd))
                    {
                        continue;
                    }
                    InventoryItemModel obj = InventoryItemTable.getItem(idd);
                    if (obj != null)
                    {
                        Items.Add(obj.ID, obj);
                    }
                    else
                    {
                        obj = InventoryItemTable.Load(idd);
                        if (obj != null)
                        {
                            Items.Add(obj.ID, obj);
                        }
                    }
                }
                catch (Exception e) { continue; }
            }


            myIntialized = true;
        }


        public GameActorTypeEnum ActorType
        {
            get { return GameActorTypeEnum.TYPE_TAX_COLLECTOR; }
        }

        public static void parseAttaque(Player perso, int guildID)
        {
            foreach (TaxCollector perco in TaxCollectorTable.Cache.Values.Where(x => x.inFight > 0 && x.GuildID == guildID))
            {
                perso.Send(new GuildInformationsAttackMessage(parseAttaqueToGuild(perco.ActorId, perco.CurrentFight), false));
            }
        }

        public static void parseDefense(Player perso, int guildID)
        {
            foreach (TaxCollector perco in TaxCollectorTable.Cache.Values.Where(x => x.inFight > 0 && x.GuildID == guildID))
            {
                perso.Send(new GuildInformationsAttackMessage(parseDefenseToGuild(perco.ActorId, perco.CurrentFight), true));
            }
        }


        public static String parseAttaqueToGuild(long guid, Fight fight)
        {
            StringBuilder sb = new StringBuilder("+").Append(guid);

            if (fight != null)
            {
                foreach (Fighter f in fight.Team1.GetFighters())
                {
                    if (f == null || f.ActorType != GameActorTypeEnum.TYPE_CHARACTER)
                    {
                        continue;
                    }
                    sb.Append("|");
                    sb.Append(IntHelper.toString((int)(f as CharacterFighter).Character.ActorId, 36)).Append(";");
                    sb.Append(f.Name).Append(";");
                    sb.Append(f.Level).Append(";");
                    sb.Append("0;");
                }
            }

            return sb.ToString();
        }

        public static String parseDefenseToGuild(long guid, Fight fight)
        {
            StringBuilder sb = new StringBuilder("+").Append(guid);

            if (fight != null)
            {
                foreach (Fighter f in fight.Team2.GetFighters())
                {
                    if (f.ActorType == GameActorTypeEnum.TYPE_CHARACTER)
                    {
                        sb.Append("|");
                        sb.Append(IntHelper.toString((int)(f as CharacterFighter).Character.ActorId, 36)).Append(";");
                        sb.Append((f as CharacterFighter).Character.Name).Append(";");
                        sb.Append((f as CharacterFighter).Character.Look).Append(";");
                        sb.Append((f as CharacterFighter).Character.Level).Append(";");
                        sb.Append(IntHelper.toString((f as CharacterFighter).Character.Color1, 36)).Append(";");
                        sb.Append(IntHelper.toString((f as CharacterFighter).Character.Color2, 36)).Append(";");
                        sb.Append(IntHelper.toString((f as CharacterFighter).Character.Color3, 36)).Append(";");
                        sb.Append("0;");
                    }
                }
            }
            return sb.ToString();
        }

        public static String parsetoGuild(int GuildID)
        {
            StringBuilder sb = new StringBuilder("+");
            Boolean isFirst = true;
            foreach (var perco in TaxCollectorTable.Cache.Values.Where(x => x.GuildID == GuildID))
            {
                if (!isFirst)
                {
                    sb.Append("|");
                }
                sb.Append(perco.ActorId).Append(";").Append(perco.N1).Append(perco.N2).Append(";");
                sb.Append(IntHelper.toString(perco.Map.Id, 36)).Append(",").Append(perco.Map.X).Append(",").Append(perco.Map.Y).Append(";"); //Integer.toString(map.get_id(), 36)
                sb.Append(perco.inFight).Append(";");
                if (perco.inFight == 1)
                {
                    if (perco.Map.GetFight(perco.inFight) == null)
                    {
                        sb.Append("45000;");
                    }
                    else
                    {
                        sb.Append(perco.TimeTurn).Append(";");
                    }
                    sb.Append("45000;");
                    sb.Append("7;");
                    sb.Append("?,?,");
                }
                else
                {
                    sb.Append("0;");
                    sb.Append("45000;");
                    sb.Append("7;");
                    sb.Append("?,?,");
                }
                sb.Append("1,2,3,4,5");

                isFirst = false;

            }
            if (sb.Length == 1)
            {
                return null;
            }
            return sb.ToString();
        }

        public void SerializeAsGameMapInformations(StringBuilder SerializedString)
        {
            if (this.mySerializedPattern == null)
            {
                this.mySerializedPattern = new StringBuilder();
                this.mySerializedPattern.Append(this.CellId).Append(';');
                this.mySerializedPattern.Append(this.Orientation).Append(';');
                this.mySerializedPattern.Append('0').Append(';'); // Unknow
                this.mySerializedPattern.Append(this.ActorId).Append(';');
                this.mySerializedPattern.Append(this.N1).Append(",").Append(N2).Append(';');
                this.mySerializedPattern.Append("-6").Append(';');
                this.mySerializedPattern.Append("6000").Append("^100;");
                this.mySerializedPattern.Append(this.Guild.Level).Append(';');
                this.mySerializedPattern.Append(this.Guild.Name).Append(';');
                this.mySerializedPattern.Append(this.Guild.Emblem);
            }

            SerializedString.Append(this.mySerializedPattern.ToString());
        }

        public String SerializeAsItemList()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var obj in Items.Values)
            {
                sb.Append("O").Append(obj.ToString()).Append(";");
            }
            if (this.Kamas != 0L)
            {
                sb.Append("G").Append(this.Kamas);
            }
            return sb.ToString();
        }

        public String getItemsId()
        {
            StringBuilder str = new StringBuilder();
            foreach (InventoryItemModel obj in Items.Values)
            {
                str.Append(obj.ID).Append(";");
            }
            return str.ToString();
        }

        public String GetLogItems()
        {
            StringBuilder sb = new StringBuilder();
            Boolean isFirst = true;
            foreach (var obj in LogItems.Values)
            {
                if (!isFirst) sb.Append(";");
                sb.Append(obj.ID).Append(",").Append(obj.Quantity);
                isFirst = false;
            }
            return sb.ToString();
        }
    }
}
