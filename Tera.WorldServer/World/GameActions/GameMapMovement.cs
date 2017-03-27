using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.GameActions
{
    public sealed class GameMapMovement : GameAction
    {
        // liste des cell sur lesquelles le joueur va marcher
        private MovementPath myMovementPath;

        // event attente de fin de deplacement, anti speedhack etc
        private ManualResetEventSlim myFinish = new ManualResetEventSlim(false);

        // syncro
        private object sync = new object();

        // mouvement stoppé ?
        private bool myAborted = false;

        // map du deplacement
        private IWorldField myField;

        public GameMapMovement(IWorldField Field, IGameActor Actor, MovementPath MovementPath)
            : base(GameActionTypeEnum.MAP_MOVEMENT, Actor)
        {
            this.myField = Field;
            this.myMovementPath = MovementPath;
        }

        public override void Abort(params object[] Args)
        {
            lock (this.sync)
            {
                // deja fini ?
                if (!this.IsFinish)
                {
                    // deja aborté ?
                    if (!this.myAborted)
                    {
                        // cell de transit ?
                        if (Args.Length > 0)
                        {
                            var StoppedCell = (int)Args[0];

                            // on apell                            
                            base.EndExecute();

                            this.myField.ActorMoved(this.myMovementPath, this.Actor, StoppedCell);
                        }

                        this.myAborted = true;
                    }
                }
            }
        }

        public override void EndExecute()
        {
            lock (this.sync)
            {
                // mouvement fini ?
                if (!this.IsFinish)
                {
                    // mouvement stoppé ?
                    if (!this.myAborted)
                    {
                        // on apell                            
                        base.EndExecute();

                        // on retire de la cell
                        this.myField.ActorMoved(this.myMovementPath, this.Actor, this.myMovementPath.EndCell);
                    }
                }
            }
        }

        public override bool CanSubAction(GameActionTypeEnum ActionType)
        {
            return false;
        }
    }
}
