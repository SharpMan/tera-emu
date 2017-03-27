using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Fights
{
    public sealed class MonsterFighter : VirtualFighter
    {
        public MonsterGroup MonsterGroup
        {
            get;
            set;
        }

        public MonsterLevel Grade
        {
            get;
            set;
        }

        public MonsterFighter(Fight Fight, MonsterLevel Monster, int MonsterGuid, MonsterGroup MonsterGroup = null, Fighter Invocator = null)
            : base(Fight, GameActorTypeEnum.TYPE_MONSTER, Invocator)
        {
            this.MonsterGroup = MonsterGroup;
            this.Grade = Monster;

            base.InitFighter(this.Grade.Stats, MonsterGuid, this.Grade.Monster.Look);
            if (Invocator != null)
            {
                base.InvocatorGiven(Invocator);
                base.InvokID = Invocator.InvokTotal++;
            }
        }

        public override int MapCell
        {
            get
            {
                return this.MonsterGroup.CellId;
            }
        }

        public override int Level
        {
            get
            {
                return this.Grade.Level;
            }
        }

        public override bool TurnReady
        {
            get { return true; }
            set { }
        }

        public override string Name
        {
            get
            {
                return this.Grade.Monster.ID.ToString();
            }
        }

        StringBuilder mySerializedGMapInformation;
        public override void SerializeAsGameMapInformations(StringBuilder Packet)
        {
            Packet.Append(this.Cell.Id).Append(';');
            if (mySerializedGMapInformation == null)
            {
                mySerializedGMapInformation = new StringBuilder("1").Append(';'); // Direction
                mySerializedGMapInformation.Append('0').Append(';');
                mySerializedGMapInformation.Append(this.ActorId).Append(';');
                mySerializedGMapInformation.Append(this.Name).Append(';');
                mySerializedGMapInformation.Append("-2;");
                mySerializedGMapInformation.Append(this.Skin).Append('^').Append(this.Grade.Size).Append(';');
                mySerializedGMapInformation.Append(this.Grade.Grade).Append(';');
                mySerializedGMapInformation.Append(this.Grade.Monster.Colors.Replace(",", ";"));
                mySerializedGMapInformation.Append(";0,0,0,0;");
            }
            Packet.Append(mySerializedGMapInformation);
            Packet.Append(this.Life).Append(';');
            Packet.Append(this.AP).Append(';');
            Packet.Append(this.MP).Append(';');
            //Packet.Append("0;0;0;0;0;0;0;");
            Packet.Append(this.Team.Id);
        }

        public override List<Spells.SpellLevel> getSpells()
        {
            return Grade.Spells.GetSpells();
        }
    }
}
