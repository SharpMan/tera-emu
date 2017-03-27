using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Maps;

namespace Tera.WorldServer.World.Fights
{
    public sealed class PercepteurFighter : VirtualFighter
    {
        public TaxCollector TaxCollector
        {
            get;
            set;
        }


        public PercepteurFighter(Fight Fight, TaxCollector Monster, int MonsterGuid) : base(Fight, GameActorTypeEnum.TYPE_TAX_COLLECTOR)
        {
            this.TaxCollector = Monster;
            TaxCollector.Guild.Initialize();
            TaxCollector.inFight = (byte)1;
            TaxCollector.CurrentFight = Fight;
            var oldStat = this.TaxCollector.Guild.FightStats;
            oldStat.AddBase(EffectEnum.AddVitalite, Monster.Guild.Level * 100);
            oldStat.AddBase(EffectEnum.AddPA, 6);
            oldStat.AddBase(EffectEnum.AddPM, 3);
            base.InitFighter(oldStat, MonsterGuid, 6000);
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
                return this.TaxCollector.CellId;
            }
        }

        public override int Level
        {
            get
            {
                return this.TaxCollector.Guild.Level;
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
                return this.TaxCollector.N1 + "," + this.TaxCollector.N2;
            }
        }

        public override void SerializeAsGameMapInformations(StringBuilder Packet)
        {
            Packet.Append(this.Cell.Id).Append(';');
            Packet.Append('1').Append(';'); // Direction
            Packet.Append('0').Append(';');
            Packet.Append(this.ActorId).Append(';');
            Packet.Append(this.Name).Append(';');
            Packet.Append((int)GameActorTypeEnum.TYPE_TAX_COLLECTOR).Append(";");
            Packet.Append(this.Skin).Append('^').Append(this.SkinSize).Append(';');
            Packet.Append(this.TaxCollector.Guild.Level).Append(';');
            Packet.Append(this.Life).Append(';');
            Packet.Append(this.AP).Append(';');
            Packet.Append(this.MP).Append(';');
            Packet.Append((int) Math.Floor((double)this.TaxCollector.Guild.Level / 2)).Append(";").Append((int) Math.Floor((double)this.TaxCollector.Guild.Level / 2)).Append(";").Append((int) Math.Floor((double)this.TaxCollector.Guild.Level / 2)).Append(";").Append((int) Math.Floor((double)this.TaxCollector.Guild.Level / 2)).Append(";").Append((int) Math.Floor((double)this.TaxCollector.Guild.Level / 2)).Append(";").Append((int) Math.Floor((double)this.TaxCollector.Guild.Level / 2)).Append(";").Append((int) Math.Floor((double)this.TaxCollector.Guild.Level / 2)).Append(";");//Résistances
            Packet.Append(this.Team.Id);
        }

        public override List<Spells.SpellLevel> getSpells()
        {
            return TaxCollector.Guild.GetSpellBook().GetSpells();
        }
    }
}
