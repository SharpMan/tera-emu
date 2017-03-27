using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tera.Libs.VirtualMemory
{
    public class GarbageCollector : IDisposable
    {
        private Timer timer;

        public GarbageCollector(long passageDelayMinutes)
        {
            this.timer = new Timer(new TimerCallback(GarbageCollectorPassage), null, passageDelayMinutes * 1000 * 60, passageDelayMinutes * 1000 * 60);
        }

        private void GarbageCollectorPassage(object obj)
        {
             GC.Collect();
        }

        public void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
            }
            timer = null;
        }
    }
}
