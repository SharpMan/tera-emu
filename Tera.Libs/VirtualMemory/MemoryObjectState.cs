using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.VirtualMemory
{
    public enum MemoryObjectState
    {
        UNLOADING,
        UNLOADED,
        DELETING,
        DELETED,
        LOADING,
        LOADED,
        INITIALIZING,
        ALIVE
    }
}
