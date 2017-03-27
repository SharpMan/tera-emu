using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Tera.WorldServer.World.Scripting.Commands
{
    public class CommandParameters : IEnumerable
    {
        private string[] _parameters;

        public string Prefix { get; set; }

        public string this[int index] {
            get{
                return GetParameter(index);
            }
        }

        public CommandParameters(string[] parameters, bool deletePrefix = true)
        {
            if (parameters.Length > 0)
            {
                this.Prefix = parameters[0].ToLower();
                this._parameters = parameters;
            }
            else
            {
                this.Prefix = null;
                this._parameters = new string[0];
            }
        }

        public void ChangeParametersAfter(int index)
        {
            this._parameters = this.GetParametersAfter(index);
        }

        public CommandParameters(string parameters, bool deletePrefix = true)
            : this(parameters.Split(' '), deletePrefix)
        {
        }

        public string GetFullPameters
        {
            get
            {
                return string.Join(" ", this._parameters);
            }
        }

        public string GetParameter(int index)
        {
            try
            {
                return this._parameters[index];
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public int GetIntParameter(int index)
        {
            return int.Parse(this._parameters[index]);
        }

        public short getShortParamater(int index)
        {
            return short.Parse(this._parameters[index]);
        }

        public bool GetBoolParameter(int index)
        {
            return bool.Parse(this._parameters[index]);
        }
        
        public string[] GetParametersAfter(int index)
        {
            var newParams = new string[_parameters.Length - index];
            System.Array.ConstrainedCopy(_parameters, index, newParams, 0, _parameters.Length - index);
            return newParams;
        }
        
        public string GetParametersAfterJoin(int index)
        {
            return string.Join(" ", GetParametersAfter(index));
        }

        public int Lenght
        {
            get
            {
                return this._parameters.Length;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }
    }
}
