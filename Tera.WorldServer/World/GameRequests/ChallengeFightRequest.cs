using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.GameRequests
{
    public sealed class ChallengeFightRequest : GameBaseRequest
    {
        /// <summary>
        /// Constructeur de base
        /// </summary>
        /// <param name="C1"></param>
        /// <param name="C2"></param>
        public ChallengeFightRequest(WorldClient C1, WorldClient C2)
            : base(C1, C2)
        {
        }

        /// <summary>
        /// Acceptation du duel
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Declin()
        {
            if (!base.Declin())
                return false;

            var Message = new GameActionMessage((int)GameActionTypeEnum.CHALLENGE_DENY, this.Requested.GetCharacter().ActorId, this.Requester.GetCharacter().ActorId.ToString());

            this.Requester.Send(Message);
            this.Requested.Send(Message);

            return true;
        }

        /// <summary>
        /// Refus du duel
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public override bool Accept()
        {
            if (!base.Accept())
                return false;

            var Message = new GameActionMessage((int)GameActionTypeEnum.CHALLENGE_ACCEPT, this.Requested.GetCharacter().ActorId, this.Requester.GetCharacter().ActorId.ToString());

            this.Requester.Send(Message);
            this.Requested.Send(Message);

            return true;
        }

        public override bool CanSubAction(GameActionTypeEnum Action)
        {
            if (Action == GameActionTypeEnum.CHALLENGE_DENY || Action == GameActionTypeEnum.CHALLENGE_ACCEPT)
                return true;

            return false;
        }
    }
}
