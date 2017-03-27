using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.JS;
using System.Runtime.CompilerServices;
using System.IO;

namespace Tera.WorldServer.World.Scripting.Commands
{
    public class JSPlayerCommandPackage : JSPackage<JSPlayerCommand>
    {
        public JSPlayerCommandPackage() : base("commands.player_commands", "scripts/commands/player/") { }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LoadAll()
        {
            string[] filePaths = Directory.GetFiles(@Path, "player_command_[*].tera.js");
            lock (memory)
            {
                memory.Clear();
            }
            foreach (string filePath in filePaths)
            {
                string commandName = System.IO.Path.GetFileName(filePath).Replace("player_command_[", "").Replace("].tera.js", "");
                JSPlayerCommand current = new JSPlayerCommand(this.NameSpace, filePath, "PlayerCommand::" + commandName);
                Attach(current);
            }
        }

        public JSPlayerCommand Get(string Command)
        {
            return base.Get("PlayerCommand::" + Command.ToLower());
        }

        public void Load(string Command)
        {
            Command = Command.ToLower();
            string filePath = Path + "player_command_[" + Command + "].tera.js";
            if(File.Exists(filePath)){
                if (memory.ContainsKey("PlayerCommand::" + Command))
                {
                    lock(memory){
                        memory.Remove("PlayerCommand::" + Command);
                    }
                }
                JSPlayerCommand current = new JSPlayerCommand(this.NameSpace, filePath, "PlayerCommand::" + Command);
                Attach(current);
            }
        }
    }
}
