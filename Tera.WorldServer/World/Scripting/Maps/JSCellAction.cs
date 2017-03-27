using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.JS;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Scripting.Maps
{
    public class JSCellAction : JSClass
    {
        public JSCellAction(string Namespace, string FilePath, string Name)
            : base(Namespace, FilePath, Name)
        {
            this.Load();
            this.Compile();
        }

        public object constructCache(int mapId, int cellId, string arguments)
        {
            return this.Invoke("constructCache", mapId, cellId, arguments);
        }

        public bool canApply(object cellActionProperties, Player character)
        {
            return (bool)this.Invoke("canApply", cellActionProperties, character);
        }

        public void apply(object cellActionProperties, Player character)
        {
            this.Invoke("apply", cellActionProperties, character);
        }
        
    }
}
