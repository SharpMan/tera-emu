using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Tera.Libs.IO
{
    public class IniSettings
    {
        public Dictionary<string, Dictionary<string, string>> Elements = new Dictionary<string, Dictionary<string, string>>();
        public string Path = "";

        public IniSettings(string path)
        {
            this.Path = path;
        }

        public bool GetBoolElement(string group, string key)
        {
            return bool.Parse(this.GetGroup(group)[key]);
        }

        public Dictionary<string, string> GetGroup(string group)
        {
            return this.Elements[group];
        }

        public int GetIntElement(string group, string key)
        {
            return int.Parse(this.GetGroup(group)[key]);
        }

        public string GetStringElement(string group, string key)
        {
            return this.GetGroup(group)[key];
        }

        public void ReadSettings()
        {
            this.Elements.Clear();
            Dictionary<string, string> dictionary = null;
            using( StreamReader reader = new StreamReader(this.Path)){
                while (!reader.EndOfStream)
                {
                    string str = reader.ReadLine();
                    if ((str != "") && !str.StartsWith("#"))
                    {
                        if (str.StartsWith("["))
                        {
                            dictionary = new Dictionary<string, string>();
                            this.Elements.Add(str.Replace("[", "").Replace("]", ""), dictionary);
                        }
                        else if (dictionary != null)
                        {
                            string[] strArray = str.Trim().Split(new char[] { '=' });
                            string key = strArray[0].Trim();
                            string str3 = strArray[1].Trim();
                            dictionary.Add(key, str3);
                        }
                    }
                }
            }
        }
    }
}
