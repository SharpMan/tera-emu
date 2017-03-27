using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.JS;
using System.Runtime.CompilerServices;
using System.IO;

namespace Tera.WorldServer.World.Scripting.Commands
{
    public class JSAdminCommandPackage : JSPackage<JSAdminCommand>
    {
        public JSAdminCommandPackage() : base("commands.admin_commands", "scripts/commands/admin/") { }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadAll()
        {
            string[] filePaths = Directory.GetFiles(@Path, "admin_command_*.tera.js");
            lock (memory)
            {
                memory.Clear();
            }
            foreach (string filePath in filePaths)
            {
                string commandName = System.IO.Path.GetFileName(filePath).Replace("admin_command_", "").Replace(".tera.js", "");
                JSAdminCommand current = new JSAdminCommand(this.NameSpace, filePath, "AdminCommand::" + commandName);
                Attach(current);
            }
        }

        public JSAdminCommand Get(string Command)
        {
            return base.Get("AdminCommand::" + Command.ToLower());
        }

        public void Load(string Command)
        {
            Command = Command.ToLower();
            string filePath = Path + "admin_command_" + Command + ".tera.js";
            if(File.Exists(filePath)){
                if (memory.ContainsKey("AdminCommand::" + Command))
                {
                    lock(memory){
                        memory.Remove("AdminCommand::" + Command);
                    }
                }
                JSAdminCommand current = new JSAdminCommand(this.NameSpace, filePath, "AdminCommand::" + Command);
                Attach(current);
            }
        }
    }
}

