using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public class GameDataCellSetTrap : PacketBase
    {
        private int cellId;

        public GameDataCellSetTrap(int CellId)
        {
            this.cellId = CellId;
        }

        public override string Compile()
        {
            return "GDC|" + cellId + ";Haaaaaaaaz3005;";
        }
    }
}
