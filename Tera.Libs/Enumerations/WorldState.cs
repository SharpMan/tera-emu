using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tera.Libs.Enumerations
{
    public enum WorldState
    {
        STATE_NON_AUTHENTIFIED = 0,
        STATE_AUTHENTIFIED = 1,
        STATE_CHARACTER_SELECTION = 2,
        STATE_GAME_CREATE = 3,
        STATE_GAME_INFORMATION = 4,
        STATE_IN_GAME = 5,
    }
}
