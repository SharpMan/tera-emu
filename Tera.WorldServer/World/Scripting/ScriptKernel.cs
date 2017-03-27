using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.WorldServer.World.Actions;

namespace Tera.WorldServer.World.Scripting
{
    public static class ScriptKernel
    {
        public static List<TeraScript> Scripts = new List<TeraScript>();

        public static void Load()
        {
            Scripts.Clear();
            foreach (var scriptPath in Directory.GetFiles("Scripts"))
            {
                char spliter = (char)92;
                try
                {
                    var script = new TeraScript(scriptPath.Split(spliter)[1].Split('.')[0]);
                    string[] lines = System.IO.File.ReadAllLines(scriptPath);
                    foreach (String line in lines)
                    {
                        try
                        {
                            String[] Content = line.Split('|');
                            int Type = int.Parse(Content[0]);
                            String Args = Content[1];
                            String Citerion = Content[2];
                            script.addAction(new ActionModel(Type, Args, Citerion));
                        }
                        catch (Exception e)
                        {
                            Logger.Error("TeraScript::" + scriptPath + " Error at " + e.ToString());
                            continue;
                        }
                    }
                    Scripts.Add(script);
                }
                catch (Exception e)
                {
                    Logger.Error("Can't load script @'" + scriptPath + "'@ : " + e.ToString());
                }
            }
            Logger.Info("Loaded @'" + Scripts.Count + "'@ TeraScript");
        }

        public static TeraScript getScript(String name)
        {
            return Scripts.FirstOrDefault(x => x.Name == name);
        }
    }
}
