using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World
{
    public enum GameActionTypeEnum
    {
        OFF_FIGHT = 0,
        MAP_MOVEMENT = 1,
        MAP_TELEPORT = 4,
        MAP_PUSHBACK = 5,
        CHANGE_MAP = 2,
        CHALLENGE_REQUEST = 900,
        CHALLENGE_ACCEPT = 901,
        CHALLENGE_DENY = 902,
        FIGHT_JOIN = 903,
        FIGHT_AGGRESSION = 906,
        TAXCOLLECTOR_AGRESSION = 909,
        PRISM_ATTACK = 912,
        OPEN_PRISM_MENU = 512,
        EXCHANGE,
        FIGHT,
        GROUP,
        KOLIZEUM,
        FIGHT_HEAL = 100,
        FIGHT_KILLFIGHTER = 103,
        CELL_ACTION = 500,
        FIGHT_TACLE = 104,
        FIGHT_ARMOR = 105,
        FIGHT_LOSTPA = 102,
        FIGHT_LOSTPM = 129,
        FIGHT_DAMAGE = 100,
        FIGHT_LAUNCHSPELL = 300,
        FIGHT_LAUNCHSPELL_CRITIC = 301,
        FIGHT_LAUNCHSPELL_ECHEC = 302,
        FIGHT_USEWEAPON = 303,
        FIGHT_USEWEAPON_ECHEC = 305,
        FIGHT_DODGE_SUBPA = 308,
        FIGHT_DODGE_SUBPM = 309,

        BASIC_REQUEST,
        GUILD_CREATE,
        DIALOG_CREATE,
    }
    public abstract class GameAction
    {
        private List<Action<GameAction>> myEndCallBacks = new List<Action<GameAction>>();

        
        public abstract bool CanSubAction(GameActionTypeEnum ActionType);

        
        public GameActionTypeEnum ActionType
        {
            get;
            private set;
        }

        public IGameActor Actor
        {
            get;
            private set;
        }

        public bool IsFinish
        {
            get;
            private set;
        }

        public GameAction(GameActionTypeEnum ActionType, IGameActor Actor)
        {
            this.Actor = Actor;
            this.ActionType = ActionType;
        }

        public virtual void Execute()
        {
            
        }

        public virtual void Abort(params object[] Args)
        {
            this.IsFinish = true;
        }

        public virtual void EndExecute()
        {
            foreach (var CallBack in this.myEndCallBacks)
                CallBack.Invoke(this);

            this.IsFinish = true;
        }

        public void RegisterEnd(Action<GameAction> CallBack)
        {
            this.myEndCallBacks.Add(CallBack);
        }
    }
}
