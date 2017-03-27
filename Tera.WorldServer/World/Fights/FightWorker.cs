using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;

namespace Tera.WorldServer.World.Fights
{
    public sealed class FightWorker
    {
        private List<Fighter> myFightersTurn = new List<Fighter>();
        private Fighter myCurrentFighter;

        public IEnumerable<Fighter> Fighters
        {
            get { return this.myFightersTurn; }
        }

        public void InitTurns(IEnumerable<Fighter> Fighters)
        {
            this.myFightersTurn.Clear();
            var Team1 = Fighters.Where(Fighter => Fighter.Team.Id == 0 && Fighter.Invocator == null).ToList();
            var Team2 = Fighters.Where(Fighter => Fighter.Team.Id == 1 && Fighter.Invocator == null).ToList();

            Team1 = Team1.OrderByDescending(Fighter => Fighter.Initiative).ToList();
            Team2 = Team2.OrderByDescending(Fighter => Fighter.Initiative).ToList();

            foreach (var Fighter in Team1)
            {
                var FIndex = Team1.IndexOf(Fighter);

                if (Team2.Count - 1 >= FIndex)
                {
                    var OppositeFighter = Team2[FIndex];

                    if (OppositeFighter.Initiative > Fighter.Initiative)
                    {
                        this.addFighter(Fighters,OppositeFighter);
                        this.addFighter(Fighters,Fighter);
                    }
                    else
                    {
                        this.addFighter(Fighters, Fighter);
                        this.addFighter(Fighters, OppositeFighter);
                    }
                }
                else
                {
                    this.addFighter(Fighters, Fighter);
                }
            }

            foreach (var Fighter in Team2)
            {
                if (!this.myFightersTurn.Contains(Fighter))
                    this.addFighter(Fighters, Fighter);
            }
        }

        public Fighter GetNextFighter()
        {
            do
            {
                if (this.myCurrentFighter == null || this.myCurrentFighter == this.myFightersTurn.LastOrDefault())
                {
                    this.myCurrentFighter = this.myFightersTurn[0];
                }
                else
                    this.myCurrentFighter = this.myFightersTurn[this.myFightersTurn.IndexOf(this.myCurrentFighter) + 1];
            }
            while (!this.myCurrentFighter.CanBeginTurn());

            return this.myCurrentFighter;
        }

        public void RemakeTurns(IEnumerable<Fighter> Fighters)
        {
            var myFightersTurn = new List<Fighter>();
            foreach (var fighter in this.myCurrentFighter.Fight.Fighters.Where(Fighter => Fighter.Invocator == null))
            {
                addFighter(Fighters, fighter,myFightersTurn);
            }
            this.myFightersTurn = myFightersTurn;
        }

        private void addFighter(IEnumerable<Fighter> Fighters, Fighter fighter,List<Fighter> fl)
        {
            fl.Add(fighter);
            var invok = Fighters.Where(x => x.Invocator != null && x.Invocator.ActorId == fighter.ActorId);
            //invok.Concat(Fighters.Where(x => x.IsDouble && x.Name.Equals(fighter.Name)).ToList());
            if (invok.Count(x => !x.Dead) > 0)
            {
                invok = invok.Where(x => !x.Dead);
                invok.OrderByDescending(Fighter => -Fighter.InvokID).ToList();

                foreach (var Fighter in invok)
                {
                    if (!fl.Contains(Fighter))
                        this.addFighter(Fighters, Fighter,fl);
                }
            }
        }

        private void addFighter(IEnumerable<Fighter> Fighters, Fighter fighter)
        {
            this.myFightersTurn.Add(fighter);
            var invok = Fighters.Where(x => x.Invocator != null && x.Invocator.ActorId == fighter.ActorId);
            //invok.Concat(Fighters.Where(x => x.IsDouble && x.Name.Equals(fighter.Name)).ToList());
            if (invok.Count(x => !x.Dead) > 0)
            {
                invok = invok.Where(x => !x.Dead);
                invok.OrderByDescending(Fighter => -Fighter.InvokID).ToList();

                foreach (var Fighter in invok)
                {
                    if (!this.myFightersTurn.Contains(Fighter))
                        this.addFighter(Fighters, Fighter);
                }
            }
        }

        public void Dispose()
        {
            this.myFightersTurn.Clear();

            this.myCurrentFighter = null;
            this.myFightersTurn = null;
        }
    }
}
