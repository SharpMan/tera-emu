﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class CharacterForgetSpellInterface : PacketBase
    {
        public char sign;

        public CharacterForgetSpellInterface(char a)
        {
            this.sign = a;
        }

        public override string Compile()
        {
            return "SF" + sign;
        }

    }
}
