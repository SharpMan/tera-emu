using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterFlagMessage : PacketBase
    {

        public Player Character;

        public CharacterFlagMessage(Player p)
        {
            this.Character = p;
        }

        public override string Compile()
        {
            return "IC" + Character.myMap.X + "|" + Character.myMap.Y;
        }
    }
}
