using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Database.Tables;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Controllers;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Scripting.Maps;
using Tera.WorldServer.World.Scripting;

namespace Tera.WorldServer.World.Maps
{
    public sealed partial class CellAction
    {
        public int MapID;
        public int CellID;
        public int ActionID;
        public int EventID;
        public String Arguments;
        public String Conditions;

        private JSCellAction action;
        private object ActionCache;

        public void Apply(Player perso)
        {
            if (perso == null)
            {
                return;
            }
            /* if Is Fighting return basicdataNoOperationMessage ? */
            if ((!Conditions.Equals("")) && (!Conditions.Equals("-1")) && (!ConditionParser.validConditions(perso, Conditions)))
            {
                perso.Send(new TextInformationMessage(TextInformationTypeEnum.UNK, 119));
                return;
            }
            JSCellAction action = JSKernel.MapCellActions.Get(ActionID);
            if (action != null)
            {
                if (ActionCache == null)
                {
                    ActionCache = action.constructCache(MapID, CellID, Arguments);
                }
                if(action.canApply(ActionCache, perso)){
                    action.apply(ActionCache, perso);
                }
            }
        }
    }
}
