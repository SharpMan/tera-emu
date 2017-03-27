using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tera.WorldServer.World.Fights
{
    public sealed class FightTimer
    {
        public bool IsAlive
        {
            get;
            set;
        }

        public bool IsEnd
        {
            get;
            set;
        }

        private List<Action<Object>> myEndCallbacks = new List<Action<Object>>();
        private Timer myTimer;

        public FightTimer()
        {
            this.IsAlive = false;
            this.IsEnd = true;
        }

        public void RegisterCallback(Action<Object> Method)
        {
            this.myEndCallbacks.Add(Method);
        }

        public void Start(int WaitTime, int DueTime = -1, Object Object = null)
        {
            if (this.IsEnd)
            {
                this.IsAlive = true;
                if (this.myTimer == null)
                {
                    this.myTimer = new Timer(new TimerCallback(this.DoActions), Object, WaitTime, DueTime);
                }
                else
                    this.myTimer.Change(WaitTime, DueTime);
            }
        }

        public void Stop()
        {
            this.myTimer.Dispose();

            this.IsAlive = false;
        }

        private void DoActions(Object State)
        {
            if (this.IsAlive)
            {
                foreach (var Method in this.myEndCallbacks)
                    Method.Invoke(State);
            }
        }
    }
}
