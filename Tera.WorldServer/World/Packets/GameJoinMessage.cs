using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Packets
{
    public sealed class GameJoinMessage : PacketBase
    {
        public short State, CancelBtn, Challenge, Spectator, Time;

        public GameJoinMessage(short State, short CancelBtn, short Challenge, short Spectator, short Time)
        {
            //"GJK"
            //    .append(state)
            //    .append("|")
            //    .append(cancelBtn)
            //    .append("|")
            //    .append(duel)
            //    .append("|")
            //    .append(spec)
            //    .append("|")
            //    .append(time)
            //    .append("|")
            //    .append(type).toString();

            this.State = State;
            this.CancelBtn = CancelBtn;
            this.Challenge = Challenge;
            this.Spectator = Spectator;
            this.Time = Time;
        }

        public override string Compile()
        {
            return "GJK" + this.State.ToString() + "|" + this.CancelBtn.ToString() + "|" + this.Challenge.ToString() + "|" + this.Spectator.ToString() + "|" + this.Time.ToString();
        }
    }
}
