using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Tera.Libs.Utils
{
    public class TypesManager
    {
        public static Type[] GetTypes(Type t)
        {
            return Assembly.GetAssembly(t).GetTypes();
        }

        public static bool IsNumeric(string data)
        {
            try
            {
                int.Parse(data);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
