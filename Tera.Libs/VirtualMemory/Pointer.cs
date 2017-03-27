using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tera.Libs.VirtualMemory
{
    public class Pointer<T> : IDisposable where T:MemoryObject
    {

        public static Pointer<T> operator +(Pointer<T> pointer, T o)
        {
            pointer.assign(o);
            return pointer;
        }
        public static Pointer<T> operator --(Pointer<T> p)
        {
            p.Delete();
            return p;
        }

        private volatile T Object;
        public long lastAccess
        {
            get;
            protected set;
        }
        protected string filePath
        {
            get;
            set;
        }
        private Object locker = new Object();
        private BinaryFormatter formatter;
        private BinaryFormatter getFormatter()
        {
            if (formatter == null)
            {
                lock (locker)
                {
                    formatter = new BinaryFormatter();
                }
            }
            return formatter;
        }

        private static bool DirectoriesCreated = false;

        public Pointer(BinaryFormatter formatter) : this()
        {
            this.formatter = formatter;
        }

        public Pointer()
        {
            if (!DirectoriesCreated)
            {
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "tera", "memory_objects"));
                DirectoriesCreated = true;
            }
            this.filePath = Path.Combine(Path.GetTempPath(), "tera", "memory_objects", DateTime.Now.Ticks.ToString() + "_" + Guid.NewGuid().ToString() + ".mo");
            this.PointerState = PointerState.UNASSIGNED;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void assign(T Object)
        {
            if (PointerState == PointerState.ASSIGNED || this.Object!=null)
            {
                return;
            }
            lock (locker)
            {
                this.Object = Object;
            }
            if (this.Object != null)
            {
                this.PointerState = PointerState.ASSIGNED;
                lastAccess = Environment.TickCount;
            }
        }

        public MemoryObjectState MemoryState
        {
            get{
                if (Object == null)
                {
                    if (PointerState == PointerState.OBJECT_UNLOADED)
                    {
                        return MemoryObjectState.UNLOADED;
                    }
                    else
                    {
                        return MemoryObjectState.DELETED;
                    }
                }
                else
                {
                    return Object.MemoryState;
                }
            }
        }

        public PointerState PointerState
        {
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            protected set;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Delete()
        {
            if (PointerState == PointerState.ASSIGNED && Object!=null)
            {
                try
                {
                    Object.Dispose();
                }
                finally
                {
                    PointerState = PointerState.OBJECT_DELETED;
                    lock (locker)
                    {
                        Object = null;
                    }
                    lastAccess = 0;
                }
            }
        }

        public virtual T get()
        {
            if (Object != null)
            {
                lastAccess = Environment.TickCount;
            }
            return Object;
        }

        public virtual T _()
        {
            if (Object != null)
            {
                lastAccess = Environment.TickCount;
            }
            return Object;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual T Load()
        {
            if (PointerState != PointerState.OBJECT_UNLOADED)
            {
                return get();
            }
            try
            {
               using( Stream stream = File.Open(filePath, FileMode.Open)){
                   this.assign((T)this.getFormatter().Deserialize(stream));
               }
               try{
                   File.Delete(filePath);
               }catch(Exception e){}
               return get();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual bool Unload()
        {
            if (PointerState != PointerState.ASSIGNED || Object == null)
            {
                return false;
            }
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Create))
                {
                    this.getFormatter().Serialize(stream, Object);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            finally
            {
                PointerState = PointerState.OBJECT_UNLOADED;
                lock (locker)
                {
                    Object = null;
                }
                lastAccess = 0;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual bool Initialize()
        {
            if (PointerState != PointerState.ASSIGNED || Object == null)
            {
                return false;
            }
            else
            {
                this.get().Initialize();
                return true;
            }
        }


        public virtual void Dispose()
        {
            this.Delete();
        }
    }
}
