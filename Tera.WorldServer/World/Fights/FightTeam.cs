using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights
{
    public enum ToggleTypeEnum
    {
        TYPE_NEW_PLAYER = 'N',
        TYPE_HELP = 'H',
        TYPE_PARTY = 'P',
        TYPE_SPECTATOR = 'S',
    }

    public enum TeamTypeEnum
    {
        TEAM_TYPE_PLAYER_ALIGNED = 0,
        TEAM_TYPE_MONSTER = 1,
        TEAM_TYPE_PLAYER = 2,
        TEAM_TYPE_TAXCOLLECTOR = 3,
    }

    public sealed class FightTeam
    {
        private Dictionary<ToggleTypeEnum, bool> myToggleLocks = new Dictionary<ToggleTypeEnum, bool>()
        {
            { ToggleTypeEnum.TYPE_NEW_PLAYER, false },
            { ToggleTypeEnum.TYPE_HELP, false },
            { ToggleTypeEnum.TYPE_PARTY, false },
            { ToggleTypeEnum.TYPE_SPECTATOR, false },
        };

        private List<Fighter> myFighters = new List<Fighter>(8);
        public int Id;

        public FightTeam(int Id)
        {
            this.Id = Id;
        }

        public long LeaderId
        {
            get;
            set;
        }

        public List<Fighter> GetFighters()
        {
            return this.myFighters;
        }

        public List<Fighter> GetAliveFighters()
        {
            return this.myFighters.Where(x => !x.Dead).ToList();
        }

        public Fighter Leader
        {
            get;
            set;
        }

        public void SetLeader(Fighter Fighter)
        {
            this.Leader = Fighter;
            this.LeaderId = Fighter.ActorId;
        }

        public bool CanJoin(Player Character)
        {
            if (this.myFighters.Count >= 8)
                return false;

            if (this.IsToggle(ToggleTypeEnum.TYPE_NEW_PLAYER))
            {
                Character.Client.Send(new GameActionMessage((int)GameActionTypeEnum.FIGHT_JOIN, Character.ActorId, "f"));
            }

            return !this.IsToggle(ToggleTypeEnum.TYPE_NEW_PLAYER);
        }

        public void Toggle(ToggleTypeEnum ToggleType, bool Value)
        {
            lock (this.myToggleLocks)
                this.myToggleLocks[ToggleType] = Value;
        }

        public bool IsToggle(ToggleTypeEnum ToggleType)
        {
            lock (this.myToggleLocks)
                return this.myToggleLocks[ToggleType];
        }

        public void FighterJoin(Fighter Fighter)
        {
            Fighter.Team = this;

            this.myFighters.Add(Fighter);
        }

        public void FighterLeave(Fighter Fighter)
        {
            this.myFighters.Remove(Fighter);
        }

        public bool HasFighterAlive()
        {
            return this.myFighters.Any(x => !x.Dead);
        }

        public void EndFight()
        {
            this.myFighters.RemoveAll(x => x.Invocator != null); // On delete les invocations
            this.myFighters.RemoveAll(x => x is DoubleFighter);  // On delete les doubles
        }

        public void Dispose()
        {
            this.myFighters.Clear();

            this.myFighters = null;
            this.Leader = null;
        }

        public bool IsFriendly(Fighter fighter)
        {
            return fighter.Team.Id == this.Id;
        }
    }
}
