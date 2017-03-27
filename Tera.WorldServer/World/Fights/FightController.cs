using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Network;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights
{
    public sealed class FightController
    {
        public int NextFightId
        {
            get
            {
                if (this.myFights.Count == 0)
                    return 1;
                return this.myFights.Max(x => x.FightId) + 1;
            }
        }
        private List<Fight> myFights = new List<Fight>();

        public int FightCount
        {
            get { return this.myFights.Count; }
        }

        public List<Fight> Fights
        {
            get { return this.myFights; }
        }

        public Fight GetFight(int FightId)
        {
            lock (this.myFights)
                foreach (var Fight in this.myFights)
                    if (Fight.FightId == FightId)
                        return Fight;

            return null;
        }

        public void SendFightInfos(WorldClient Client)
        {
            lock (this.myFights)
                foreach (var Fight in this.myFights)
                    if (Fight.FightState == FightState.STATE_PLACE)
                    {
                        Fight.SendFightFlagInfos(Client);
                    }

            Client.Send(new MapFightCountMessage(this.myFights.Count));
        }

        public void AddFight(Fight Fight)
        {
            lock (this.myFights)
                this.myFights.Add(Fight);
        }

        public void RemoveFight(Fight Fight)
        {
            lock (this.myFights)
                this.myFights.Remove(Fight);
        }
    }
}
