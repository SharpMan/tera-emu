using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Packets
{
    public class GameDataCellReset : PacketBase
    {
        private int cellId;

        public GameDataCellReset(int CellId)
        {
            this.cellId = CellId;
        }

        public override string Compile()
        {
            return "GDC|" + cellId;
        }
    }
}
