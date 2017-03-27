using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.JS
{
    public class JSPackage<T> where T: JSFile
    {
        protected Dictionary<string, T> memory = new Dictionary<string, T>();

        public string NameSpace
        {
           get;
           set;
        }

        public string Path
        {
           get;
           set;
        }
        
        public JSPackage(string Name, string path)
        {
            this.NameSpace = "js."+Name;
            this.Path = path;
        }

        public void Attach(T file)
        {
            lock (memory)
            {
                memory.Add(file.GetName(), file);
            }
        }

        public T Get(string fileName)
        {
            T toReturn = null;
            memory.TryGetValue(fileName, out toReturn);
            return toReturn;
        }
    }
}
