using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.GameActions
{
    public sealed class GameCreateGuild : GameAction
    {
        public WorldClient Client
        {
            get;
            private set;
        }

        public GameCreateGuild(WorldClient Client)
            : base(GameActionTypeEnum.GUILD_CREATE, Client.GetCharacter())
        {
            this.Client = Client;
        }

        public override void Execute()
        {
            this.Client.Send(new GuildNewCreationMessage());

            base.Execute();
        }

        public override void Abort(params object[] Args)
        {
            this.Client.Send(new GuildCreationLeaveMessage());

            base.Abort(Args);
            base.EndExecute();
        }

        public override void EndExecute()
        {
            this.Client.Send(new GuildCreationLeaveMessage());

            base.EndExecute();
        }

        public override bool CanSubAction(GameActionTypeEnum ActionType)
        {
            return false;
        }
    }
}
