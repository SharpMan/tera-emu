using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.JS;
using System.IO;
using System.Runtime.CompilerServices;
using Tera.Libs;

namespace Tera.WorldServer.World.Scripting.Maps
{
    public class JSCellActionsPackage : JSPackage<JSCellAction>
    {
        public JSCellActionsPackage() : base("maps.cell_actions", "scripts/maps/cell_actions/"){}

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadAll()
        {
            string[] filePaths = Directory.GetFiles(@Path, "cell_action_*.tera.js");
            lock (memory)
            {
                memory.Clear();
            }
            foreach (string filePath in filePaths)
            {
                string ActionID = System.IO.Path.GetFileName(filePath).Replace("cell_action_", "").Replace(".tera.js", "");
                JSCellAction current = new JSCellAction(this.NameSpace, filePath, "CellAction[" + ActionID + "]");
                Attach(current);
            }
        }

        public JSCellAction Get(int ActionID)
        {
            return Get("CellAction[" + ActionID + "]");
        }

        public void Load(int ActionID)
        {
            string filePath = Path + "cell_action_"+ActionID+".tera.js";
            if(File.Exists(filePath)){
                if(memory.ContainsKey("CellAction[" + ActionID + "]")){
                    lock(memory){
                        memory.Remove("CellAction[" + ActionID + "]");
                    }
                }
                JSCellAction current = new JSCellAction(this.NameSpace, filePath, "CellAction[" + ActionID + "]");
                Attach(current);
            }
        }
    }
}
