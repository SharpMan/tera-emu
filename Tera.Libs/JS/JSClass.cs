using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jint;
using System.Reflection;
using System.Linq.Expressions;
using Jint.Native;
using Tera.Libs.Network;

namespace Tera.Libs.JS
{
    public abstract class JSClass : JSFile
    {
        private JintEngine executor;

        public JSClass(string Namespace, string File)
        {
            this.ClassName = Namespace + "." + this.GetType().Name;
            this.ClassCanonicalName = this.GetType().Name;
            this.FilePath = File;
        }

        public JSClass(string Namespace, string File, string Name)
        {
            this.ClassName = Namespace + "." + Name;
            this.ClassCanonicalName = Name;
            this.FilePath = File;
        }
       
        private Delegate delegateOf(MethodInfo method)
        {
            var tArgs = new List<Type>();
            foreach (var param in method.GetParameters())
                tArgs.Add(param.ParameterType);
            tArgs.Add(method.ReturnType);
            return Delegate.CreateDelegate(Expression.GetDelegateType(tArgs.ToArray()), this, method);
        }

        public override void Compile()
        {
            this.executor = null;
            this.executor = new JintEngine();
            foreach (MethodInfo method in this.GetType().GetMethods())
            {
                if (method.GetCustomAttributes(typeof(JSFunction), true) != null)
                {
                    executor.SetFunction(method.Name, delegateOf(method));
                }
            }
            executor.AllowClr = true;
            executor.DisableSecurity();
            executor.Run(SourceCode, false);
            
        }

        public override void Load()
        {
            this.SourceCode = System.IO.File.ReadAllText(FilePath, System.Text.Encoding.Default);
        }
        
        protected void __construct(params object[] args)
        {
            Invoke("construct", args);
        }

        protected object Invoke(string name, params object[] args)
        {
            try
            {
                return executor.CallFunction(name, args);
            }
            catch (NullReferenceException e)
            {
                return null;
            }
            catch (InvalidCastException e)
            {
                return null;
            }
        }
    }
}
