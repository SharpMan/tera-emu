using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tera.Libs;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.Fights.Types;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Kolizeum
{
    public class StartFightTask
    {
        public Timer Timer;

        private Map map;
        private Player p1;
        private Player p2;
        private List<Player> team1;
        private List<Player> team2;
        public Boolean Started = false;

        public StartFightTask(Map map, List<Player> team1, List<Player> team2)
        {

            this.map = map;
            this.p1 = team1[0];
            this.p2 = team2[0];
            this.team1 = team1;
            this.team2 = team2;
            this.Timer = new Timer(new TimerCallback(Run), null, 3000, 3000);
        }

        private void Run(object state)
        {
            if (Started)
            {
                this.cancel();
                return;
            }
            Started = true;
            team1.Remove(p1);
            team2.Remove(p2);
            p1.Send(new BasicNoOperationMessage());
            p2.Send(new BasicNoOperationMessage());
            var Fight = new KolizeumFight(map, p1.Client, p2.Client);
            map.AddFight(Fight);
            foreach (var Player in team1)
            {
                try
                {
                    Player.Send(new BasicNoOperationMessage());
                    Logger.Error("Ini Player " + Player.Name);
                    var Fighter = new CharacterFighter(Fight, Player.GetClient());
                    var FightAction = new GameFight(Fighter, Fight);
                    Player.Client.AddGameAction(FightAction);
                    Fight.JoinFightTeam(Fighter, Fight.GetTeam1(), false, -1, false);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
            foreach (var Player in team2)
            {
                try
                {
                    Player.Send(new BasicNoOperationMessage());
                    Logger.Error("Ini Player " + Player.Name);
                    //if (Prism.CurrentFight.CanJoin(Team, Client.GetCharacter()))
                    var Fighter = new CharacterFighter(Fight, Player.GetClient());
                    var FightAction = new GameFight(Fighter, Fight);
                    Player.Client.AddGameAction(FightAction);
                    Fight.JoinFightTeam(Fighter, Fight.GetTeam2(), false, -1, false);
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            
            }
        }

        public void cancel()
        {
            try
            {
                this.Timer.Dispose();
                this.map = null;
                this.p1 = null;
                this.p2 = null;
                this.team1 = null;
                this.team2 = null;
                this.Timer = null;
            }
            catch (Exception e)
            {
            }
        }

    }
}
