using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.Database.Tables
{
    public class CellActionTable
    {
        public static string Collums = "MapID,CellID,ActionID,EventID,ActionsArgs,Conditions";

        public static void Load()
        {
            long time = Environment.TickCount;
            var reader = DatabaseManager.Provider.ExecuteReader("SELECT * FROM scripted_cells");
            int nbr = 0;
            while (reader.Read())
            {
                var cell = new CellAction()
                {
                    MapID = reader.GetInt32("MapID"),
                    CellID = reader.GetInt32("CellID"),
                    ActionID = reader.GetInt32("ActionID"),
                    EventID = reader.GetInt32("EventID"),
                    Arguments = reader.GetString("ActionsArgs"),
                    Conditions = reader.GetString("Conditions"),
                };
                Map mm = MapTable.Cache.FirstOrDefault(x => x.Key == cell.MapID).Value;
                if (mm != null)
                {
                    mm.CellActionsCache.Add(cell);
                    nbr++;
                }

            }
            reader.Close();
            Logger.Info("Loaded @'" + nbr + "'@ CellActions in @" + (Environment.TickCount - time) + "@ ms");
        }

    }
}
