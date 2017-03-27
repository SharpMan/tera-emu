using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;

namespace Tera.WorldServer.Database
{
    public static class DatabaseCache
    {
        public static int nextPlayerGuid;
        public static long nextItemGuid;
        public static int nextGuildId;
        public static int nextMountId;
        public static long nextSpeakingId;
        private static object mySyncSave = new object();
        private static Queue<Action> mySaveQueue = new Queue<Action>();

        public static void Initialize()
        {
            nextPlayerGuid = CharacterTable.getNextGuid();
            nextPlayerGuid++;
            nextItemGuid = InventoryItemTable.getNextGuid();
            nextItemGuid++;
            nextGuildId = GuildTable.getNextGuid();
            nextGuildId++;
            nextMountId = MountTable.getNextGuid();
            nextMountId++;
            nextSpeakingId = SpeakingTable.getNextGuid();
            nextSpeakingId++;
            SpellTable.Load();
            ExpFloorTable.Load();
            MonsterTable.Load();
            ItemTemplateTable.Load();
            ItemTemplateTable.LoadItemActions();
            ItemSetTable.Load();
            DropTable.Load();
            TitleTable.Load();
            IObjectTemplateTable.Load();
            AreaTable.Load();
            AreaSubTable.Load();
            MapTable.Load();
            MapTable.LoadActions();
            CellActionTable.Load();
            MobGroupFixTable.Load();
            BreedTable.Load();
            NpcTemplateTable.Load();
            NpcTemplateTable.LoadPlaces();
            NpcQuestionTable.Load();
            NpcReplyTable.Load();
            ShopNpcTable.Initialize();
            GuildTable.Load();
            CharactersGuildTable.Load();
            TaxCollectorTable.Load();
            PrismeTable.Load();
            BidHouseTable.Load();
            BidHouseTable.LoadItems();
            MountParkTable.Load();
            StaticMountTable.Load();
            MountTable.Load();//TODO Dynamic load of Character mount
            ZaapTable.Load();
            ZaapiTable.Load();

            var Timer = new System.Timers.Timer(1000 * 60 * 25);
            Timer.Elapsed += delegate(object sender, ElapsedEventArgs e)
            {
                Save();
            };
            Timer.Start();
        }

        /// <summary>
        /// Sauvegarde les changements effectués.
        /// </summary>
        public static bool Save()
        {
            try
            {
                lock (mySyncSave)
                {
                    long StartTime = Environment.TickCount;
                    Logger.Info("World saving ...");

                    // Execution des requetes
                    lock (mySaveQueue)
                        while (mySaveQueue.Count != 0)
                            mySaveQueue.Dequeue()();

                    lock (GuildTable.Cache)
                        foreach (var Guild in GuildTable.Cache.Values)
                        {
                            Guild.SaveChanges();
                            GuildTable.Update(Guild);
                        }

                    lock (CharacterTable.myCharacterById)
                        foreach (var Character in CharacterTable.myCharacterById.Values.Where(x => x.myInitialized))
                            CharacterTable.Update(Character);

                    lock (SpeakingTable.Cache)
                        foreach (var Speaking in SpeakingTable.Cache.Values)
                            SpeakingTable.Add(Speaking);

                    lock (AreaTable.Cache)
                        foreach (var area in AreaTable.Cache.Values)
                            AreaTable.Update(area);

                    lock (AreaSubTable.Cache)
                        foreach(var subarea in AreaSubTable.Cache.Values)
                            AreaSubTable.Save(subarea);

                    lock (BidHouseTable.Cache)
                    {
                        var BHI = new List<BidHouseItem>();
                        foreach (var BH in BidHouseTable.Cache.Values)
                        {
                            BHI.AddRange(BH.getAllEntry());
                        }
                        BidHouseTable.Update(BHI);
                    }

                    Logger.Info("World saved in "+(Environment.TickCount - StartTime)+"ms");

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("DatabaseEntities::Save() "+ ex.ToString());

                return false;
            }
        }
    }
}
