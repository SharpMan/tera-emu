using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Helper
{
    public sealed class CachedString
    {
        private Func<String> myCallback = null;
        private string myCachedString = null;
        private bool myNeedRefresh = false;

        public CachedString(Func<String> Callback)
        {
            this.myCallback = Callback;
            this.myCachedString = Callback();
        }

        public void NeedToBeRefresh()
        {
            lock (this.myCallback)
                this.myNeedRefresh = true;
        }

        public override string ToString()
        {
            lock (this.myCallback)
                if (this.myNeedRefresh)
                {
                    this.myCachedString = this.myCallback();
                    this.myNeedRefresh = false;
                }

            return base.ToString();
        }
    }
}
