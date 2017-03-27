using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.JS;
using Tera.WorldServer.World.Scripting.Maps;
using Tera.WorldServer.World.Scripting.Fights;
using Tera.WorldServer.World.Scripting.Commands;

namespace Tera.WorldServer.World.Scripting
{
    public static class JSKernel
    {
        public static JSCellActionsPackage MapCellActions = new JSCellActionsPackage();
        public static JSIAMindPackage FightIAMinds = new JSIAMindPackage();
        public static JSAdminCommandPackage CommandsAdmin = new JSAdminCommandPackage();
        public static JSPlayerCommandPackage CommandsPlayer = new JSPlayerCommandPackage();

        public static void Load()
        {
            MapCellActions.LoadAll();
            FightIAMinds.LoadAll();
            CommandsAdmin.LoadAll();
            CommandsPlayer.LoadAll();
        }
    }
}
