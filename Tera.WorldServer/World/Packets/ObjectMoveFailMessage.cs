using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public enum ObjectMoveFailReasonEnum
    {
        REASON_ALREADY_EQUIPED = 'A',
        REASON_LEVEL_REQUIRED = 'L',
    }

    public sealed class ObjectMoveFailMessage : PacketBase
    {
        public ObjectMoveFailReasonEnum Reason;

        public ObjectMoveFailMessage(ObjectMoveFailReasonEnum Reason)
        {
            this.Reason = Reason;
        }

        public override string Compile()
        {
            return "OAE" + (char)this.Reason;
        }
    }
}
