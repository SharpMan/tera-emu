using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tera.Libs;

namespace Tera.WorldServer.Utils
{
    public class Settings
    {
        public static Settings AppSettings { get; set; }
        public static int Server { get; set; }

        public static void Initialize()
        {
            AppSettings = new Settings("Settings.ini");
            AppSettings.ReadSettings();
            Logger.canDebug = AppSettings.GetBoolElement("Logging.Debug");
            Server = Settings.AppSettings.GetIntElement("World.ID");
        }

        public string Path = ("");
        public Dictionary<string, Dictionary<string, string>> Elements = new Dictionary<string, Dictionary<string, string>>();

        public Settings(string path)
        {
            this.Path = path;
        }

        public Dictionary<string, string> GetGroup(string group)
        {
            return this.Elements[group];
        }

        public string GetStringElement(string e)
        {
            return FastElement(e);
        }

        public int GetIntElement(string e)
        {
            return int.Parse(FastElement(e));
        }

        public short GetShortElement(string e)
        {
            return short.Parse(FastElement(e));
        }

        public bool GetBoolElement(string e)
        {
            return bool.Parse(FastElement(e));
        }

        public string FastElement(string element)
        {
            var g = element.Split('.')[0];
            var k = element.Split('.')[1];
            return GetGroup(g)[k];
        }

        public void Save()
        {
            StreamWriter writer = new StreamWriter(this.Path);
            foreach (var group in this.Elements)
            {
                writer.WriteLine("[" + group.Key + "]");
                foreach (var value in group.Value)
                {
                    writer.WriteLine(value.Key + " = " + value.Value);
                }
            }
            writer.Close();
        }

        public void ReadSettings()
        {
            this.Elements.Clear();
            Dictionary<string, string> currentGroup = null;
            StreamReader reader = new StreamReader(this.Path);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line != "" && !line.StartsWith(";"))
                {
                    if (line.StartsWith("["))
                    {
                        currentGroup = new Dictionary<string, string>();
                        this.Elements.Add(line.Replace("[", "").Replace("]", ""), currentGroup);
                    }
                    else if (currentGroup != null)
                    {
                        string[] data = line.Trim().Split('=');
                        string key = data[0].Trim();
                        string value = data[1].Trim();
                        currentGroup.Add(key, value);
                    }
                }
            }
            reader.Close();
        }
    }
}
