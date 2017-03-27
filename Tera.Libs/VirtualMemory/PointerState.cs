using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.VirtualMemory
{
    public enum PointerState
    {
        UNASSIGNED,
        ASSIGNED,
        OBJECT_DELETED,
        OBJECT_UNLOADED
    }
}
