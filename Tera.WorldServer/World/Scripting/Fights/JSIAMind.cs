using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.JS;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Scripting.Fights
{
    public class JSIAMind : JSClass
    {
        public JSIAMind(string Namespace, string FilePath, string Name)
            : base(Namespace, FilePath, Name)
        {
            this.Load();
            this.Compile();
        }

        public void Play(AIProcessor IA)
        {
            this.Invoke("Play", IA);
        }


    }
}
