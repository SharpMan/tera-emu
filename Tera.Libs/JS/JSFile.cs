using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace Tera.Libs.JS
{
    public abstract class JSFile
    {
        protected string SourceCode
        {
            get;
            set;
        }
        protected string FilePath
        {
            get;
            set;
        }
        protected string ClassName
        {
            get;
            set;
        }
        protected string ClassCanonicalName
        {
            get;
            set;
        }
        public override string ToString()
        {
            return ClassName;
        }
        public override int GetHashCode()
        {
            return ClassName.GetHashCode();
        }
        public string GetName()
        {
            return ClassCanonicalName;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public abstract void Compile();
        [MethodImpl(MethodImplOptions.Synchronized)]
        public abstract void Load();
    } 
}
