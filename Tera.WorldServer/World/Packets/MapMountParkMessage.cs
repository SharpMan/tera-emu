using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public class MapMountParkMessage : PacketBase
    {
        private MountPark mountPark;

        public MapMountParkMessage(MountPark mountPark)
        {
            this.mountPark = mountPark;
        }

        public override string Compile()
        {
            String packet = "Rp" + mountPark.get_owner() + ";" + mountPark.get_price() + ";" + mountPark.get_size() + ";" + mountPark.getObjectNumb() + ";";

            Guild G = mountPark.get_guild();
            //Si une guilde est definie
            if (G != null)
            {
                packet += G.Name + ";" + G.Emblem;
            }
            else
            {
                packet += ";";
            }
            return packet;
        }

    }
}
