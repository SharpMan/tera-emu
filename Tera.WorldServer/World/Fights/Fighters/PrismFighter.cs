using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Fights.Fighters
{
    public sealed class PrismFighter : VirtualFighter
    {
        public Prisme Prisme
        {
            get;
            set;
        }


        public PrismFighter(Fight Fight, Prisme Monster, int MonsterGuid) : base(Fight, GameActorTypeEnum.TYPE_PRISM)
        {
            this.Prisme = Monster;
            Prisme.Initialize();
            Prisme.inFight = (byte)0;
            Prisme.CurrentFight = Fight;
            var oldStat = this.Prisme.myFightStats;
            base.InitFighter(oldStat, MonsterGuid, Prisme.Look);
            base.JoinFight();
        }

        public override void Send(PacketBase Packet)
        {
        }


        public override void JoinFight()
        {
        }

        public override int MapCell
        {
            get
            {
                return this.Prisme.CellId;
            }
        }

        public override int Level
        {
            get
            {
                return this.Prisme.Level;
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
                return Prisme.Name.ToString();
            }
        }

        public override void SerializeAsGameMapInformations(StringBuilder Packet)
        {
            Packet.Append(this.Cell.Id).Append(';');
            Packet.Append('1').Append(';'); // Direction
            Packet.Append('0').Append(';');
            Packet.Append(this.ActorId).Append(';');
            Packet.Append(this.Name).Append(';');
            Packet.Append((int)GameActorTypeEnum.TYPE_PRISM).Append(";");
            Packet.Append(this.Skin).Append('^').Append(this.SkinSize).Append(';');
            Packet.Append(this.Prisme.Level).Append(';');
            Packet.Append(this.Life).Append(';');
            Packet.Append(this.AP).Append(';');
            Packet.Append(this.MP).Append(';');
            Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentNeutre)).Append(';');
            Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentTerre)).Append(';');
            Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentFeu)).Append(';');
            Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentEau)).Append(';');
            Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentAir)).Append(';');
            Packet.Append(this.Stats.GetTotal(EffectEnum.AddEsquivePA)).Append(';'); // TODO Total equive PA
            Packet.Append(this.Stats.GetTotal(EffectEnum.AddEsquivePM)).Append(';'); // TODO Total equive PM
            Packet.Append(this.Team.Id);
        }

        public override List<Spells.SpellLevel> getSpells()
        {
            return Prisme.mySpells.GetSpells();
        }
    }
}
