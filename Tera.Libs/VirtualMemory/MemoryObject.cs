using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Tera.Libs.VirtualMemory
{
    [Serializable()]
    public abstract class MemoryObject : ISerializable, IDisposable
    {
        public static MemoryObject operator --(MemoryObject o)
        {
            o.Dispose();
            return o;
        }

        public MemoryObjectState MemoryState
        {
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            protected set;
        }

        
        protected abstract void InitializeOverridable();
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void Initialize()
        {
            if (MemoryState != MemoryObjectState.LOADED)
            {
                return;
            }
            else
            {
                MemoryState = MemoryObjectState.INITIALIZING;
                try
                {
                    InitializeOverridable();
                }
                finally
                {
                    MemoryState = MemoryObjectState.ALIVE;
                }
            }
        }
        protected abstract void DeleteOverridable();

        protected abstract void MemoryLoad(SerializationInfo inData);
        protected abstract void MemoryUnload(SerializationInfo outData);
        
        public MemoryObject(SerializationInfo info, StreamingContext ctxt)
        {
            MemoryState = MemoryObjectState.LOADING;
            MemoryLoad(info);
            MemoryState = MemoryObjectState.LOADED;
        }

        public MemoryObject()
        {
            MemoryState = MemoryObjectState.LOADED;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            MemoryState = MemoryObjectState.UNLOADING;
            try
            {
                MemoryUnload(info);
            }
            finally
            {
                MemoryState = MemoryObjectState.UNLOADED;
            }
        }

        public virtual void Dispose()
        {
            MemoryState = MemoryObjectState.DELETING;
            try
            {
                DeleteOverridable();
            }
            finally
            {
                MemoryState = MemoryObjectState.DELETED;
            }
        }
    }
}
