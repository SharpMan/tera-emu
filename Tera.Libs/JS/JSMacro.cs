using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint;
using Jint.Expressions;

namespace Tera.Libs.JS
{
    public class JSMacro : JSFile
    {
        private Program compiledCode; 

        public JSMacro(string Namespace, string File)
        {
            this.ClassName = Namespace + "." + this.GetType().Name;
            this.ClassCanonicalName = this.GetType().Name;
            this.FilePath = File;
        }
        
        public object execute(params KeyValuePair<string, object>[] parameters)
        {
            JintEngine executor = new JintEngine();
            foreach (KeyValuePair<String, object> KeyVal in parameters)
            {
                executor.SetParameter(KeyVal.Key, KeyVal.Value);
            }
            executor.AllowClr = true;
            executor.DisableSecurity();
            return executor.Run(@compiledCode, false);
        }

        public override void Compile()
        {
            this.compiledCode = JintEngine.Compile(SourceCode, false);
        }

        public override void Load()
        {
            this.SourceCode = System.IO.File.ReadAllText(FilePath);
        }
    }
}
