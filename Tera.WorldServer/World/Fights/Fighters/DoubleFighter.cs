using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Maps;
using Tera.Libs.Network;

namespace Tera.WorldServer.World.Fights
{
    public  class DoubleFighter : VirtualFighter
    {   
        #region IGameActor

        public override void SerializeAsGameMapInformations(StringBuilder Packet)
        {
            Packet.Append(this.Cell.Id).Append(';');
            Packet.Append('1').Append(';'); // Direction
            Packet.Append((int)this.ActorType).Append(';');
            Packet.Append(this.ActorId).Append(';');
            Packet.Append(this.Character.Name).Append(';');
            Packet.Append(this.Character.Classe).Append(';');
            Packet.Append(this.Skin).Append('^').Append(this.Character.SkinSize).Append(';');
            Packet.Append(this.Character.Sexe).Append(';');
            Packet.Append(this.Level).Append(';');
            Packet.Append(this.Character.AlignmentPatternInfos).Append(';');
            Packet.Append(this.Character.Color1.ToString("x")).Append(';');
            Packet.Append(this.Character.Color2.ToString("x")).Append(';');
            Packet.Append(this.Character.Color3.ToString("x")).Append(';');
            this.Character.InventoryCache.SerializeAsDisplayEquipment(Packet);
            Packet.Append(';');
            Packet.Append(this.Life).Append(';');
            Packet.Append(this.AP).Append(';');
            Packet.Append(this.MP).Append(';');
            switch (this.Fight.FightType)
            {
                case FightType.TYPE_CHALLENGE:
                case FightType.TYPE_AGGRESSION:
                case FightType.TYPE_KOLIZEUM:
                    Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentNeutre) + this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentPvPNeutre)).Append(';');
                    Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentTerre) + this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentPvPTerre)).Append(';');
                    Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentFeu) + this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentPvPFeu)).Append(';');
                    Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentEau) + this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentPvPEau)).Append(';');
                    Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentAir) + this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentPvPAir)).Append(';');
                    break;

                default:
                    Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentNeutre)).Append(';');
                    Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentTerre)).Append(';');
                    Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentFeu)).Append(';');
                    Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentEau)).Append(';');
                    Packet.Append(this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentAir)).Append(';');
                    break;
            }
            Packet.Append(this.Stats.GetTotal(EffectEnum.AddEsquivePA)).Append(';'); // TODO Total equive PA
            Packet.Append(this.Stats.GetTotal(EffectEnum.AddEsquivePM)).Append(';'); // TODO Total equive PM
            Packet.Append(this.Team.Id).Append(';');
            if (this.Character.isOnMount() && this.Character.Mount != null)
                Packet.Append(Character.Mount.get_color());
            Packet.Append(';');
        }

        #endregion

        #region Fighter

        public bool InvocatorHasLeft = false;

        public override void LeaveFight()
        {
            InvocatorHasLeft = true;
            base.LeaveFight();
            this.EndFight();
            if (Fight.FightType != FightType.TYPE_CHALLENGE)
            {
                this.Character.Life = 1;
            }
        }

        public override int BeginTurn()
        {
            return base.BeginTurn();
        }

        public override void MiddleTurn()
        {
            base.MiddleTurn();
        }

        public override int EndTurn()
        {
            return base.EndTurn();
        }

        public override void EndFight()
        {
            if (this.Fight.FightType == FightType.TYPE_CHALLENGE)
                this.Character.Life = this.Character.MaxLife;
            else
                this.Character.Life = base.Life;

            base.EndFight();
        }

        public override int MapCell
        {
            get
            {
                return this.Character.CellId;
            }
        }

        public override int Level
        {
            get
            {
                return this.Character.Level;
            }
        }

        public override bool TurnReady
        {
            get;
            set;
        }

        public override string Name
        {
            get
            {
                return this.Character.Name;
            }
        }

        #endregion

        public Player Character
        {
            get;
            set;
        }

        public DoubleFighter(Fight Fight, Player Character,Fighter Invocator)
            : base(Fight, GameActorTypeEnum.TYPE_CHARACTER)
        {
            this.Character = Character;
            base.InitFighter(this.Character.GetStats(), this.Character.ID, this.Character.Look, false);
            base.Life = this.Character.Life;
            if (base.Life == 0) base.Life = 1;
            base.Invocator = Invocator;
            base.InvokID = Invocator.InvokTotal++;
        }

        public override List<Spells.SpellLevel> getSpells()
        {
            return this.Character.GetSpellBook().GetSpells();
        }
    }
}
