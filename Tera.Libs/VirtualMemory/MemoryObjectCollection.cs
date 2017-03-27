using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Tera.Libs.VirtualMemory
{
    public class MemoryObjectCollection<TKey, TValue> : IDisposable, IEnumerable<KeyValuePair<TKey, Pointer<TValue>>>  where TValue : MemoryObject
    {
        public Pointer<TValue> this[TKey key]
        {
            get{return Get(key);}
            set{Attach(key, value);}
        }

        public static MemoryObjectCollection<TKey, TValue> operator -(MemoryObjectCollection<TKey, TValue> collection, TKey key)
        {
            collection.Detach(key);
            return collection;
        }

        public bool isEmpty()
        {
            return memory == null || memory.Count == 0;
        }

        public Dictionary<TKey, Pointer<TValue>>.KeyCollection Keys
        {
            get
            {
                return memory.Keys;
            }
        }
        
        public Dictionary<TKey, Pointer<TValue>>.ValueCollection Values
        {
            get
            {
                return memory.Values;
            }
        }

        private Dictionary<TKey, Pointer<TValue>> memory;
        private Timer garbageCollector;
        private long lifeTimeObject;

        public MemoryObjectCollection() : this(900, 300) {}
        public MemoryObjectCollection(long lifeTimeObjectSeconds) : this(lifeTimeObjectSeconds, 300) {}
        public MemoryObjectCollection(long lifeTimeObjectSeconds, long timerIntervalSeconds)
        {
            this.lifeTimeObject = lifeTimeObjectSeconds*1000;
            this.memory = new Dictionary<TKey, Pointer<TValue>>();
            this.garbageCollector = new Timer(new TimerCallback(GarbageCollectorPassage), null, timerIntervalSeconds * 1000, timerIntervalSeconds * 1000);
        }

        private void GarbageCollectorPassage(object obj)
        {
            if (memory == null) return;
            lock (memory)
            {
                foreach(Pointer<TValue> pointer in memory.Values){
                    if (pointer.PointerState == PointerState.ASSIGNED && (Environment.TickCount - pointer.lastAccess) >= lifeTimeObject)
                    {
                        pointer.Unload();
                    }
                }
            }
        }

       /* [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteAll()
        {
            lock (memory)
            {
                foreach(TKey key in memory.Keys){
                    Delete(key);
                }
            }
        }
        */
        public Pointer<TValue> Attach(TKey key, TValue value)
        {
            Pointer<TValue> pointer = new Pointer<TValue>();
            pointer += value;
            memory.Add(key, pointer);
            return pointer;
        }

        public Pointer<TValue> Attach(TKey key, Pointer<TValue> pointer)
        {
            if (!memory.ContainsKey(key))
            {
                memory.Add(key, pointer);
            }
            return pointer;
        }

        public void Detach(TKey key)
        {
           lock (memory)memory.Remove(key);
        }

        public Pointer<TValue> Get(TKey key)
        {
            Pointer<TValue> pointer = null;
            memory.TryGetValue(key, out pointer);
            return pointer;
        }

        public void Delete(TKey key)
        {
            Pointer<TValue> pointer = Get(key);
            if (pointer != null)
            {
                Detach(key);
                pointer--;
                pointer = null;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            if (garbageCollector != null) garbageCollector.Dispose();
            garbageCollector = null;
            if (memory != null)
            {
                //DeleteAll();
                memory.Clear();
            }
            lifeTimeObject = 0;
        }

        public IEnumerator<KeyValuePair<TKey, Pointer<TValue>>> GetEnumerator()
        {
            return memory.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return memory.GetEnumerator();
        }
    }
}
