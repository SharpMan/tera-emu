using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.World;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.Handlers
{
    public static class WayPointHandler
    {
        public static void ProcessPacket(WorldClient Client, string Packet)
        {
            switch (Packet[1])
            {
                case 'U':
                    WayPointHandler.WayPointUseRequest(Client, Packet);
                    break;
                case 'u':
                    WayPointHandler.WayPointUseZaapiRequest(Client, Packet);
                    break;
                case 'V':
                    WayPointHandler.WayPointQuitRequest(Client, Packet);
                    break;
                case 'p':
                    WayPointHandler.PrismeUseRequest(Client, Packet);
                    break;
                case 'v':
                    WayPointHandler.ZaapiQuitRequest(Client, Packet);
                    break;
            }
        }

        private static void ZaapiQuitRequest(WorldClient Client, string Packet)
        {
            if (!Client.Character.isZaaping)
            {
                Client.Send(new BasicNoOperationMessage());
            }
            Client.Character.isZaaping = false;
            Client.Send(new ZaapiValideMessage());
        }

        private static void WayPointUseZaapiRequest(WorldClient Client, string Packet)
        {
            short id = -1;
            if (!short.TryParse(Packet.Substring(2), out id) || id == -1)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            if (Client.GetFight() != null || !MapTable.Cache.ContainsKey(id))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            if (!Client.Character.isZaaping)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.IsGameAction(GameActionTypeEnum.MAP_MOVEMENT))
            {
                Client.EndGameAction(GameActionTypeEnum.MAP_MOVEMENT);
            }
            if (Client.IsGameAction(GameActionTypeEnum.CELL_ACTION))
            {
                Client.EndGameAction(GameActionTypeEnum.CELL_ACTION);
            }

            if (Client.Character.Deshonor >= 2)
            {
                Client.Send(new TextInformationMessage(Libs.Enumerations.TextInformationTypeEnum.ERREUR, 83));
                return;
            }


            var Map = MapTable.Cache[id];

            short cellid = 100;
            if (Map != null)
            {
                foreach (DofusCell entry in Map.GetCells())
                {
                    IObject obj = ((DofusCell)entry).Object;
                    if (obj == null)
                    {
                        continue;
                    }
                    if ((obj.getID() != 7031) && (obj.getID() != 7030))
                    {
                        continue;
                    }
                    cellid = (short)(((DofusCell)entry).Id + 18);
                }

            }

            if ((Map.subArea.areaID == 7) || (Map.subArea.areaID == 11))
            {
                int price = 20;
                if ((Client.Character.Alignement == 1) || (Client.Character.Alignement == 2))
                {
                    price = 10;
                }
                Client.Character.InventoryCache.SubstractKamas(price);
                Client.Send(new AccountStatsMessage(Client.Character));
                Client.Character.Teleport(Map, cellid);
                Client.Send(new PlayerZaapiValideMessage());
                Client.Character.isZaaping = false;
            }

        }

        private static void PrismeUseRequest(WorldClient Client, string Packet)
        {
            if (Client.Character.Deshonor >= 1)
            {
                Client.Send(new TextInformationMessage(Libs.Enumerations.TextInformationTypeEnum.ERREUR, 83));
                return;
            }
            if (!Client.Character.showWings)
            {
                Client.Send(new TextInformationMessage(Libs.Enumerations.TextInformationTypeEnum.ERREUR, 144));
                return;
            }
            int cellid = 340;
            short mapid = 7411;
            try
            {
                mapid = short.Parse(Packet.Substring(2));
            }
            catch (Exception e) { }
            var Prism = PrismeTable.Cache.Values.First(x => x.Mapid == mapid);
            if (Prism != null)
            {
                mapid = Prism.Mapid;
                cellid = Prism.CellId;
            }
            int prix = ZaapTable.calculZaapCost(Client.Character.myMap, MapTable.Get(mapid));
            if (mapid == Client.Character.Map)
                prix = 0;
            if (Client.Character.Kamas < prix)
            {
                Client.Send(new TextInformationMessage(Libs.Enumerations.TextInformationTypeEnum.ERREUR, 82));
                return;
            }
            Client.Character.InventoryCache.SubstractKamas(prix);
            Client.Character.Send(new AccountStatsMessage(Client.Character));
            Client.Character.Teleport(MapTable.Get(mapid), cellid);
            Client.Send(new PrismeMenuCloseMessage());
            Client.Character.isZaaping = false;
        }

        public static void WayPointQuitRequest(WorldClient Client, string Packet)
        {
            if (!Client.Character.isZaaping)
            {
                Client.Send(new BasicNoOperationMessage());
            }
            Client.Character.isZaaping = false;
            Client.Send(new WayPointValideMessage());
        }

        public static void WayPointUseRequest(WorldClient Client, String Packet)
        {
            short id = -1;
            if (!short.TryParse(Packet.Substring(2), out id) || id == -1)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            if (Client.GetFight() != null)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            if (!Client.Character.isZaaping)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            if (!Client.Character.Zaaps.Contains(id))
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }

            if (Client.IsGameAction(GameActionTypeEnum.MAP_MOVEMENT))
            {
                Client.EndGameAction(GameActionTypeEnum.MAP_MOVEMENT);
            }
            if (Client.IsGameAction(GameActionTypeEnum.CELL_ACTION))
            {
                Client.EndGameAction(GameActionTypeEnum.CELL_ACTION);
            }
            int cost = ZaapTable.calculZaapCost(Client.GetCharacter().myMap, MapTable.Get(id));
            if (Client.Character.Kamas < cost)
            {
                Client.Send(new BasicNoOperationMessage());
                return;
            }
            //int SubAreaID = _curCarte.getSubArea().get_area().get_superArea().get_id();
            int SubAreaID = Client.Character.myMap.subArea.area.superArea.ID;
            if (MapTable.Get(id) == null)
            {
                Logger.Error("La map " + id + " n'est pas implantee, Zaap refuse");
                Client.Send(new WayPointUseMessage());
                return;
            }
            else
            {
                if (!MapTable.Get(id).myInitialized)
                    MapTable.Get(id).Init();
            }
            int cellID = ZaapTable.GetCell(id);
            if (MapTable.Get(id).getCell(cellID) == null)
            {
                Logger.Error("La cellule associee au zaap " + id + " n'est pas implantee, Zaap refuse");
                Client.Send(new WayPointUseMessage());
                return;
            }
            if (!MapTable.Get(id).getCell(cellID).isWalkable(true))
            {
                Logger.Error("La cellule associee au zaap " + id + " n'est pas acessible, Zaap refuse");
                Client.Send(new WayPointUseMessage());
                return;
            }
            if (MapTable.Get(id).subArea.area.superArea.ID != SubAreaID)
            {
                Client.Send(new WayPointUseMessage());
                return;
            }
            Client.Character.myMap.SendToMap(new MapAnimOffFightMessage(Client.Character.ID, 505));
            Client.Character.InventoryCache.SubstractKamas(cost);
            Client.Send(new AccountStatsMessage(Client.Character));
            Client.Send(new WayPointValideMessage());
            Client.Character.Teleport(MapTable.Get(id), cellID);
            Client.Character.myMap.SendToMap(new MapAnimOffFightMessage(Client.Character.ID, 505));
            Client.Character.isZaaping = false;
            Client.DelGameAction(GameActionTypeEnum.CELL_ACTION);
        }
    }
}
