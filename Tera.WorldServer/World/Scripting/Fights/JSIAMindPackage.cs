using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;
using Tera.Libs.JS;
using Tera.Libs;

namespace Tera.WorldServer.World.Scripting.Fights
{
    public class JSIAMindPackage : JSPackage<JSIAMind>
    {
        public JSIAMindPackage() : base("fights.ia_minds", "scripts/fights/ia_minds/") { }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadAll()
        {
            string[] filePaths = Directory.GetFiles(@Path, "ia_mind_*.tera.js");
            lock (memory)
            {
                memory.Clear();
            }
            foreach (string filePath in filePaths)
            {
                string MindID = System.IO.Path.GetFileName(filePath).Replace("ia_mind_", "").Replace(".tera.js", "");
                JSIAMind current = new JSIAMind(this.NameSpace, filePath, "IAMind[" + MindID + "]");
                Attach(current);
                //Logger.Info("- IAMind[" + MindID + "] loaded.");
            }
        }

        public JSIAMind Get(int MindID)
        {
            if (MindID != 0)
            {
                return Get("IAMind[" + 1 + "]");
            }
            else
            {
                return Get("IAMind[" + 0 + "]");
            }
        }

        public void Load(int MindID)
        {
            string filePath = Path + "ia_mind_" + MindID + ".tera.js";
            if(File.Exists(filePath)){
                if (memory.ContainsKey("IAMind[" + MindID + "]"))
                {
                    lock(memory){
                        memory.Remove("IAMind[" + MindID + "]");
                    }
                }
                JSIAMind current = new JSIAMind(this.NameSpace, filePath, "IAMind[" + MindID + "]");
                Attach(current);
            }
        }
    }
}
