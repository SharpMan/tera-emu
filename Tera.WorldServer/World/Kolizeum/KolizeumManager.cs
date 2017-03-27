using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tera.Libs;
using Tera.Libs.Network;
using Tera.Libs.Utils;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.Network;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Character;
using Tera.WorldServer.World.Character.Sorter;
using Tera.WorldServer.World.Fights;
using Tera.WorldServer.World.Fights.Types;
using Tera.WorldServer.World.GameActions;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Kolizeum
{
    public class KolizeumManager
    {
        public Timer KoliCheckTimer { get; set; }
        private List<Player> Wating; //TODO ConcurrentQueue Gestion Kolizeum Event
        private List<Party> WatingGroups;
        private static readonly int TEAM_SIZE = Settings.AppSettings.GetIntElement("Kolizeum.MemberPerTeam");
        private static PlayersPartySorter SORTER_GROUP = new PlayersPartySorter(true);
        private static PersonnageLevelSorter SORTER = new PersonnageLevelSorter(true);

        public KolizeumManager()
        {
            //GetTypes des commande instants les contructeurs de classes ça fait tripple Kolizeum
        }

        public void Start()
        {
            try
            {
                this.Wating = new List<Player>();
                this.WatingGroups = new List<Party>();
                this.KoliCheckTimer = new Timer(new TimerCallback(MakeTeams), null, Settings.AppSettings.GetIntElement("Kolizeum.CheckInterval"), Settings.AppSettings.GetIntElement("Kolizeum.CheckInterval"));
                Logger.Info("Kolizeum task started !");
            }
            catch (Exception e)
            {
                Logger.Error("Can't start kolizeum : " + e.ToString());
            }
        }

        public void MakeTeams(object obj)
        {
            try
            {
                ClearOfflineWaiters();
                ClearOfflineWaitersOnGroups();
                int waitingSize = Wating.Count;
                WatingGroups.ForEach(x => waitingSize += x.GetPersosNumber());
                if (waitingSize >= TEAM_SIZE * 2)
                {
                    tryToStartFight();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Can't execute kolizeum task : " + ex.ToString());
            }

        }

        private void tryToStartFight()
        {
            List<Player> team1 = new List<Player>();
            List<Player> team2 = new List<Player>();
            int lvlTeam1 = 0;
            int lvlTeam2 = 0;

            List<Party> Group_sortedList = new List<Party>();
            Group_sortedList.AddRange(WatingGroups);
            Group_sortedList.Sort(SORTER_GROUP);

            foreach (var Group in Group_sortedList)
            {
                if (!Group.inKolizeum)
                {
                    unregisterGroupEvent(Group);
                    continue;
                }
                int GroupSize = Group.GetPersosNumber();
                int GroupLevel = Group.GetGroupLevel();
                if (lvlTeam1 > lvlTeam2 && team1.Count + GroupSize <= 3)
                {
                    lvlTeam2 += GroupLevel;
                    team2.AddRange(Group.Players);
                }
                else if (team1.Count + GroupSize <= TEAM_SIZE)
                {
                    lvlTeam1 += GroupLevel;
                    team1.AddRange(Group.Players);
                }
                else if (team2.Count + GroupSize <= TEAM_SIZE)
                {
                    lvlTeam2 += GroupLevel;
                    team2.AddRange(Group.Players);
                }
                else
                {
                    break;//all teams are ready
                }
                Group.Send(new ChatGameMessage("Votre groupe a été associé à une équipe de combat, en attente d'autres joueurs...", "38A7C8"));
            }
            Group_sortedList.Clear();
            Group_sortedList = null;
            if (Wating.Count != 0)
            {
                List<Player> sortedList = new List<Player>();
                Player p;
                sortedList.AddRange(Wating);
                sortedList.Sort(SORTER);
                p = Wating[0];

                while (team1.Count != TEAM_SIZE || team2.Count != TEAM_SIZE || p != null)
                {
                    if (lvlTeam1 > lvlTeam2 && team2.Count < 3)
                    {
                        lvlTeam2 += p.Level;
                        team2.Add(p);
                        sortedList.Remove(p);
                    }
                    else if (team1.Count < TEAM_SIZE)
                    {
                        lvlTeam1 += p.Level;
                        team1.Add(p);
                        sortedList.Remove(p);
                    }
                    else if (team2.Count < TEAM_SIZE)
                    {
                        lvlTeam2 += p.Level;
                        team2.Add(p);
                        sortedList.Remove(p);
                    }
                    else
                    {
                        break;//all teams are ready
                    }

                    int teamDiff = Math.Abs(lvlTeam1 - lvlTeam2);
                    p = null;
                    IEnumerator<Player> enumerator = sortedList.GetEnumerator();
                    bool hasNext = enumerator.MoveNext();
                    while (hasNext)
                    {
                        if (p == null)
                        {
                            p = enumerator.Current;
                        }
                        else
                        {
                            hasNext = enumerator.MoveNext();
                            if (!hasNext || enumerator.Current.Level > teamDiff)
                            {
                                break;
                            }
                        }
                        hasNext = enumerator.MoveNext();
                    }
                    enumerator.Dispose();

                    //P is Initialized => recheck
                }
                p = null;
                sortedList.Clear();
                sortedList = null;

            }
            if (team1.Count != TEAM_SIZE || team2.Count != TEAM_SIZE)
            {
                return;
            }
            startFight(team1, team2);
        }

        private void startFight(List<Player> team1, List<Player> team2)
        {
            var Map = MapTable.GetRandomMap();
            foreach (var Player in team1)
            {
                if (Player.Client.IsGameAction(GameActionTypeEnum.GROUP) && (Player.Client.GetGameAction(GameActionTypeEnum.GROUP) as GameParty).Party.inKolizeum)
                {
                    unregisterGroupEvent((Player.Client.GetGameAction(GameActionTypeEnum.GROUP) as GameParty).Party);
                    (Player.Client.GetGameAction(GameActionTypeEnum.GROUP) as GameParty).Party.Send(new ChatGameMessage("D'autres joueurs ont été trouvés, lancement du combat...", "38A7C8"));
                }
                Player.Client.EndGameAction(GameActionTypeEnum.KOLIZEUM);
                Player.OldPosition = new Couple<Map, int>(Player.myMap, Player.CellId);
                Player.Teleport(Map, 0);
            }
            foreach (var Player in team2)
            {
                if (Player.Client.IsGameAction(GameActionTypeEnum.GROUP) && (Player.Client.GetGameAction(GameActionTypeEnum.GROUP) as GameParty).Party.inKolizeum)
                {
                    unregisterGroupEvent((Player.Client.GetGameAction(GameActionTypeEnum.GROUP) as GameParty).Party);
                    (Player.Client.GetGameAction(GameActionTypeEnum.GROUP) as GameParty).Party.Send(new ChatGameMessage("D'autres joueurs ont été trouvés, lancement du combat...", "38A7C8"));
                }
                Player.Client.EndGameAction(GameActionTypeEnum.KOLIZEUM);
                Player.OldPosition = new Couple<Map, int>(Player.myMap, Player.CellId);
                Player.Teleport(Map, 0);
            }
            //var delayer = new Delayer<KolizeumManager>(GetType().GetMethod("startFightTask"), new object[] { Map,team1,team2 }, this, 3000);
            //delayer.Start();
            new StartFightTask(Map, team1, team2);
        }

        private void ClearOfflineWaiters()
        {
            lock (Wating)
            {
                Wating.RemoveAll(Player => Player == null || Player.Client == null);
                foreach (var Player in Wating)
                {
                    if (!Player.IsOnline())
                    {
                        Wating.Remove(Player);
                    }
                    if (Player.Client.IsGameAction(GameActionTypeEnum.FIGHT))
                    {
                        Player.Send(new ChatGameMessage("Vous avez été désinscrit du Kolizeum car vous êtes en combat", "38A7C8"));
                        Player.Client.EndGameAction(GameActionTypeEnum.KOLIZEUM);
                    }
                    if (Player.Client.IsGameAction(GameActionTypeEnum.GROUP) && (Player.Client.GetGameAction(GameActionTypeEnum.GROUP) as GameParty).Party.inKolizeum)
                    {
                        Player.Send(new ChatGameMessage("Vous avez été désinscrit du Kolizeum car vous êtes déjà dans un Groupe inscrit en Kolizéum.", "38A7C8"));
                        Player.Client.EndGameAction(GameActionTypeEnum.KOLIZEUM);
                    }
                    if (Player.Client.IsGameAction(GameActionTypeEnum.EXCHANGE))
                    {
                        Player.Send(new ChatGameMessage("Vous avez été désinscrit du Kolizeum car vous êtes en échange", "38A7C8"));
                        Player.Client.EndGameAction(GameActionTypeEnum.KOLIZEUM);
                    }
                }
            }
        }

        private void ClearOfflineWaitersOnGroups()
        {
            lock (WatingGroups)
            {
                WatingGroups.RemoveAll(Party => Party == null || !Party.inKolizeum);
                foreach (var Group in WatingGroups)
                {
                    if (Group.GetPersosNumber() > TEAM_SIZE)
                    {
                        Group.Send(new ChatGameMessage("Votre groupe a été désinscrit du Kolizéum car il contient plus de <b>" + TEAM_SIZE + "</b> joueurs.", "38A7C8"));
                        unregisterGroupEvent(Group);
                        continue;
                    }

                    foreach (var Player in Group.Players)
                    {
                        if (Player == null)
                            continue;
                        if (!Player.IsOnline())
                        {
                            Group.Send(new ChatGameMessage("Le personnage <b>" + Player.Name + "</b> a désinscrit votre groupe du Kolizeum car il est hors ligne.", "38A7C8"));
                            unregisterGroupEvent(Group);
                            break;
                        }
                        if (Player.Client.IsGameAction(GameActionTypeEnum.FIGHT))
                        {
                            Group.Send(new ChatGameMessage("Le personnage <b>" + Player.Name + "</b> a désinscrit votre groupe du Kolizeum car car il est en combat.", "38A7C8"));
                            unregisterGroupEvent(Group);
                            break;
                        }
                        if (Player.Client.IsGameAction(GameActionTypeEnum.EXCHANGE))
                        {
                            Group.Send(new ChatGameMessage("Le personnage <b>" + Player.Name + "</b> a désinscrit votre groupe du Kolizeum car il est en échange.", "38A7C8"));
                            unregisterGroupEvent(Group);
                            break;
                        }
                    }
                }
            }
        }

        public void unregisterGroupEvent(Party group)
        {
            WatingGroups.Remove(group);
            group.inKolizeum = false;
        }

        public void UnregisterPlayer(Player p)
        {
            Wating.Remove(p);
        }

        public void RegisterPlayer(Player p)
        {
            Wating.Add(p);
        }

        public Boolean registerGroup(Player executor, Party group)
        {
            if (group == null || executor == null)
            {
                return false;
            }
            if (!group.isChief(executor.ID))
            {
                executor.Send(new ChatGameMessage("Seul le chef <b>" + group.Chief.Name + "</b> peut inscrire le groupe au Kolizeum.", "38A7C8"));
                return false;
            }
            if (group.inKolizeum)
            {
                executor.Send(new ChatGameMessage("Votre groupe est déjà inscrit à un Kolizeum.", "38A7C8"));
                return false;
            }
            if (group.GetPersosNumber() > TEAM_SIZE)
            {
                executor.Send(new ChatGameMessage("Votre groupe ne peut pas être inscrit car il contient plus de " + TEAM_SIZE + " joueurs.", "38A7C8"));
                return false;
            }
            WatingGroups.Add(group);
            group.inKolizeum = true;
            group.Send(new ChatGameMessage("Votre groupe a été inscrit au Kolizeum.", "38A7C8"));

            return true;
        }




    }
}
