using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.Libs.Network;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.Utils;
using Tera.WorldServer.World.Character;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights
{
    public abstract class Fighter : IFightObject, IGameActor
    {
        private static Random Random = new Random();

        public abstract void Send(PacketBase Packet);
        public abstract void JoinFight();
        public abstract int MapCell
        {
            get;
        }

        #region IFightObject

        public virtual FightObjectType ObjectType
        {
            get
            {
                return FightObjectType.OBJECT_FIGHTER;
            }
        }

        public bool CanWalk()
        {
            return false;
        }

        public bool CanStack()
        {
            return false;
        }

        #endregion

        #region IGameActor


        public int Orientation
        {
            get;
            set;
        }

        public long ActorId
        {
            get;
            set;
        }

        public GameActorTypeEnum ActorType
        {
            get;
            set;
        }

        public int CellId
        {
            get { return this.Cell.Id; }
            set { }
        }

        public abstract void SerializeAsGameMapInformations(StringBuilder SerializedString);

        #endregion

        #region Propeties

        public Fight Fight
        {
            get;
            set;
        }

        public int ReflectDamage
        {
            get
            {
                return ((1 + (this.Stats.GetTotal(EffectEnum.AddSagesse) / 100)) * this.Stats.GetTotal(EffectEnum.AddRenvoiDamage)) + this.Stats.GetTotal(EffectEnum.AddRenvoiDamageItem);
            }
        }

        public int MaxLife
        {
            get
            {
                return this.Stats.GetTotal(EffectEnum.AddVitalite) + this.Stats.GetTotal(EffectEnum.AddVie) + (this.ActorType == GameActorTypeEnum.TYPE_CHARACTER ? (Level * 5) + 50 : 0);
            }
        }


        public InventoryItemModel Weapon { get; set; }


        public int InvokTotal, InvokID = 0;


        public int Initiative
        {
            get
            {
                if (this is CharacterFighter)
                {
                    return (this as CharacterFighter).Character.Initiative;
                }
                return (int)Math.Floor((double)(this.MaxLife / 4 + this.Stats.GetTotal(EffectEnum.AddInitiative)) * (double)(this.Life / this.MaxLife));
            }
        }

        public int Prospection
        {
            get
            {
                return (int)Math.Floor((double)(this.Stats.GetTotal(EffectEnum.AddChance) / 10)) + this.Stats.GetTotal(EffectEnum.AddProspection);
            }
        }

        public int CurrentLife
        {
            get;
            set;
        }

        public int Life
        {
            get
            {
                return this.CurrentLife + this.Stats.GetTotal(EffectEnum.AddVitalite) + this.Stats.GetTotal(EffectEnum.AddVie);
            }
            set
            {
                this.CurrentLife = value - this.Stats.GetTotal(EffectEnum.AddVitalite) + this.Stats.GetTotal(EffectEnum.AddVie);
            }
        }

        public abstract int Level
        {
            get;
        }

        public bool Dead
        {
            get
            {
                return this.Life <= 0;
            }
        }

        public virtual int UsedAP
        {
            get;
            set;
        }

        public virtual int UsedMP
        {
            get;
            set;
        }



        public virtual int MaxAP
        {
            get
            {
                return this.Stats.GetTotal(EffectEnum.AddPA);
            }
        }

        public virtual int MaxMP
        {
            get
            {
                return this.Stats.GetTotal(EffectEnum.AddPM);
            }
        }

        public virtual int AP
        {
            get
            {
                if (this is CharacterFighter)
                {
                    return ((this.Stats.GetTotal(EffectEnum.AddPA) > Settings.AppSettings.GetIntElement("Limit.Pa")) ? Settings.AppSettings.GetIntElement("Limit.Pa") : this.Stats.GetTotal(EffectEnum.AddPA)) - this.UsedAP;
                }
                return this.Stats.GetTotal(EffectEnum.AddPA) - this.UsedAP;
            }
        }

        public virtual int MP
        {
            get
            {
                if (this is CharacterFighter)
                {
                    return ((this.Stats.GetTotal(EffectEnum.AddPM) > Settings.AppSettings.GetIntElement("Limit.Pm")) ? Settings.AppSettings.GetIntElement("Limit.Pm") : this.Stats.GetTotal(EffectEnum.AddPM)) - this.UsedMP;
                }
                return this.Stats.GetTotal(EffectEnum.AddPM) - this.UsedMP;
            }
        }

        public int APDodge
        {
            get
            {
                return (int)Math.Floor((double)this.Stats.GetTotal(EffectEnum.AddSagesse) / 4) + this.Stats.GetTotal(EffectEnum.AddEsquivePA);
            }
        }

        public int MPDodge
        {
            get
            {
                return (int)Math.Floor((double)this.Stats.GetTotal(EffectEnum.AddSagesse) / 4) + this.Stats.GetTotal(EffectEnum.AddEsquivePM);
            }
        }

        public int Skin
        {
            get;
            set;
        }

        public int SkinSize
        {
            get;
            set;
        }

        public GenericStats Stats
        {
            get;
            set;
        }

        public FightTeam Team
        {
            get;
            set;
        }

        private FightCell myCell;

        public FightCell Cell
        {
            get { return this.myCell; }
        }

        public abstract bool TurnReady
        {
            get;
            set;
        }

        public abstract string Name
        {
            get;
        }

        public bool CanBeginTurn()
        {
            if (this.Dead)
                return false; // Mort    

            return true;
        }

        #endregion

        public FighterBuff Buffs
        {
            get;
            set;
        }

        public FighterState States
        {
            get;
            set;
        }

        public FighterSpell SpellsController
        {
            get;
            set;
        }

        public Fighter Invocator
        {
            get;
            set;
        }

        public bool Left
        {
            get;
            set;
        }

        public Fighter(Fight Fight, GameActorTypeEnum ActorType, Fighter Invocator = null)
        {
            this.ActorType = ActorType;
            this.Fight = Fight;
            this.Left = false;
            this.Buffs = new FighterBuff();
            this.States = new FighterState(this);
            this.SpellsController = new FighterSpell();
            this.Invocator = Invocator;
        }

        public int SetCell(FightCell Cell)
        {
            if (this.myCell != null)
                this.myCell.RemoveObject(this); // On vire le fighter de la cell:

            this.myCell = Cell;

            if (this.myCell != null)
            {
                var AddResult = this.myCell.AddObject(this);

                if (AddResult == -3 || AddResult == -2)
                    return AddResult;

                var BuffResult = this.Buffs.EndMove();

                if (BuffResult == -3 || BuffResult == -2)
                    return BuffResult;
            }

            return this.TryDie(this.ActorId);
        }


        protected void InitFighter(GenericStats StatsToMerge, long ActorId, int Skin, bool setLifeMax = true)
        {
            this.Stats = new GenericStats();
            this.Stats.Merge(StatsToMerge);

            this.ActorId = ActorId;
            this.Skin = Skin;
            this.SkinSize = 100;
            if (setLifeMax)
                this.Life = MaxLife;
        }

        protected void InvocatorGiven(Fighter caster)
        {
            int coef = 1 + caster.Level / 100;
            Life = (MaxLife * coef);
            this.Stats.AddBase(EffectEnum.AddVitalite, (Life - MaxLife));
            int force = this.Stats.GetTotal(EffectEnum.AddForce) * coef;
            int intel = this.Stats.GetTotal(EffectEnum.AddIntelligence) * coef;
            int agili = this.Stats.GetTotal(EffectEnum.AddAgilite) * coef;
            int sages = this.Stats.GetTotal(EffectEnum.AddSagesse) * coef;
            int chanc = this.Stats.GetTotal(EffectEnum.AddChance) * coef;

            this.Stats.AddBase(EffectEnum.AddForce, (force - this.Stats.GetTotal(EffectEnum.AddForce)));
            this.Stats.AddBase(EffectEnum.AddIntelligence, (intel - this.Stats.GetTotal(EffectEnum.AddIntelligence)));
            this.Stats.AddBase(EffectEnum.AddAgilite, (agili - this.Stats.GetTotal(EffectEnum.AddAgilite)));
            this.Stats.AddBase(EffectEnum.AddSagesse, (sages - this.Stats.GetTotal(EffectEnum.AddSagesse)));
            this.Stats.AddBase(EffectEnum.AddChance, (chanc - this.Stats.GetTotal(EffectEnum.AddChance)));
        }

        public virtual void LeaveFight()
        {
            if (this.Fight.FightState == FightState.STATE_PLACE)
            {
                // On le vire de l'equipe
                Team.FighterLeave(this);
            }

            this.Left = true;

            // On le vire de la cell
            Cell.RemoveObject(this);
        }

        public bool haveSendKillingMessage = false;

        public int TryDie(long CasterId, bool force = false)
        {
            if (force)
                this.Life = 0;

            if (this.Life <= 0)
            {
                if (!haveSendKillingMessage)//TODO mat affichich quand le double meurt alors que l'invoqueur a abondonné
                {
                    this.Fight.SendToFight(new GameActionMessage((int)GameActionTypeEnum.FIGHT_KILLFIGHTER, CasterId, this.ActorId.ToString()));
                    haveSendKillingMessage = true;
                }

                if (this.InvokTotal > 0)
                {
                    this.Team.GetAliveFighters().Where(x => x.Invocator != null && x.Invocator.ActorId == this.ActorId).ToList().ForEach(Fighter => Fighter.TryDie(this.ActorId, true));
                }

                System.Threading.Thread.Sleep(500);

                this.Fight.onFighterDie(this, this.Fight.GetFighter(CasterId));

                if (this.Fight.TryEndFight())
                    return -3;

                if (this.Fight.CurrentFighter == this)
                    this.Fight.FightLoopState = FightLoopState.STATE_END_TURN;

                /*foreach (var item in this.Fight.getGlyphe().Where(x => x.Caster.ActorId == this.ActorId))
                {
                    this.Fight.SendToFight(new FightGlypheMessage("-", item.CellId, item.Size, 4));
                    this.Fight.SendToFight(new FightGlypheCanceledMessage(item.CellId));
                    this.Fight.removeGlyphe(item);
                }

                foreach (var item in this.Fight.getTraps().Where(x => x.Caster.ActorId == this.ActorId))
                {
                    item.desappear();
                    this.Fight.removeTrap(item);
                }*/
                return -2;
            }

            return -1;
        }

        public virtual int BeginTurn()
        {
            return this.Buffs.BeginTurn();
        }

        public virtual void MiddleTurn()
        {
            this.UsedAP = 0;
            this.UsedMP = 0;
        }

        public virtual int EndTurn()
        {
            this.SpellsController.EndTurn();
            //FightGlyphe.EndTurn(this);
            return this.Buffs.EndTurn();
        }

        public virtual void EndFight()
        {
            /*if (this.ActorType != GameActorTypeEnum.TYPE_CHARACTER)
                return;
            if (this.Fight.GetWinners().GetFighters().Contains(this))
            {
                (this as CharacterFighter).Character.myMap.applyEndFightAction((int)this.Fight.FightType, (this as CharacterFighter).Character);
            }*/
        }

        public void CalculDamages(EffectEnum Effect, ref int Jet)
        {
            switch (Effect)
            {
                case EffectEnum.DamageTerre:
                case EffectEnum.VolTerre:
                case EffectEnum.DamageNeutre:
                case EffectEnum.VolNeutre:
                    Jet = (int)Math.Floor((double)Jet * (100 + this.Stats.GetTotal(EffectEnum.AddForce) + this.Stats.GetTotal(EffectEnum.AddDamagePercent)) / 100 +
                                                  this.Stats.GetTotal(EffectEnum.AddDamagePhysic) + this.Stats.GetTotal(EffectEnum.AddDamage));
                    break;

                case EffectEnum.DamageFeu:
                case EffectEnum.VolFeu:
                    Jet = (int)Math.Floor((double)Jet * (100 + this.Stats.GetTotal(EffectEnum.AddIntelligence) + this.Stats.GetTotal(EffectEnum.AddDamagePercent)) / 100 +
                                           this.Stats.GetTotal(EffectEnum.AddDamageMagic) + this.Stats.GetTotal(EffectEnum.AddDamage));
                    break;

                case EffectEnum.DamageAir:
                case EffectEnum.VolAir:
                    Jet = (int)Math.Floor((double)Jet * (100 + this.Stats.GetTotal(EffectEnum.AddAgilite) + this.Stats.GetTotal(EffectEnum.AddDamagePercent)) / 100 +
                                           this.Stats.GetTotal(EffectEnum.AddDamageMagic) + this.Stats.GetTotal(EffectEnum.AddDamage));
                    break;

                case EffectEnum.DamageEau:
                case EffectEnum.VolEau:
                    Jet = (int)Math.Floor((double)Jet * (100 + this.Stats.GetTotal(EffectEnum.AddChance) + this.Stats.GetTotal(EffectEnum.AddDamagePercent)) / 100 +
                                           this.Stats.GetTotal(EffectEnum.AddDamageMagic) + this.Stats.GetTotal(EffectEnum.AddDamage));
                    break;
            }
        }

        public void CalculReduceDamages(EffectEnum Effect, ref int Damages)
        {
            switch (Effect)
            {
                case EffectEnum.DamageNeutre:
                case EffectEnum.VolNeutre:
                    Damages = Damages * (100 - this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentNeutre) - this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentPvPNeutre)) / 100
                                             - this.Stats.GetTotal(EffectEnum.AddReduceDamageNeutre) - this.Stats.GetTotal(EffectEnum.AddReduceDamagePvPNeutre) - this.Stats.GetTotal(EffectEnum.AddReduceDamagePhysic);
                    break;

                case EffectEnum.DamageTerre:
                case EffectEnum.VolTerre:
                    Damages = Damages * (100 - this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentTerre) - this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentPvPTerre)) / 100
                                             - this.Stats.GetTotal(EffectEnum.AddReduceDamageTerre) - this.Stats.GetTotal(EffectEnum.AddReduceDamagePvPTerre) - this.Stats.GetTotal(EffectEnum.AddReduceDamagePhysic);
                    break;

                case EffectEnum.DamageFeu:
                case EffectEnum.VolFeu:
                    Damages = Damages * (100 - this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentFeu) - this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentPvPFeu)) / 100
                                             - this.Stats.GetTotal(EffectEnum.AddReduceDamageFeu) - this.Stats.GetTotal(EffectEnum.AddReduceDamagePvPFeu) - this.Stats.GetTotal(EffectEnum.AddReduceDamageMagic);
                    break;

                case EffectEnum.DamageAir:
                case EffectEnum.VolAir:
                    Damages = Damages * (100 - this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentAir) - this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentPvPAir)) / 100
                                             - this.Stats.GetTotal(EffectEnum.AddReduceDamageAir) - this.Stats.GetTotal(EffectEnum.AddReduceDamagePvPAir) - this.Stats.GetTotal(EffectEnum.AddReduceDamageMagic);
                    break;

                case EffectEnum.DamageEau:
                case EffectEnum.VolEau:
                    Damages = Damages * (100 - this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentEau) - this.Stats.GetTotal(EffectEnum.AddReduceDamagePourcentPvPEau)) / 100
                                             - this.Stats.GetTotal(EffectEnum.AddReduceDamageEau) - this.Stats.GetTotal(EffectEnum.AddReduceDamagePvPEau) - this.Stats.GetTotal(EffectEnum.AddReduceDamageMagic);
                    break;
            }
        }

        /// <summary>
        /// Calcul des soins
        /// </summary>
        /// <param name="Heal"></param>
        public void CalculHeal(ref int Heal)
        {
            Heal = (int)Math.Floor((double)Heal * (100 + this.Stats.GetTotal(EffectEnum.AddIntelligence)) / 100 + this.Stats.GetTotal(EffectEnum.AddSoins));
        }

        /// <summary>
        /// Calcul de l'armure
        /// </summary>
        /// <param name="DamageEffect"></param>
        public int CalculArmor(EffectEnum DamageEffect)
        {
            switch (DamageEffect)
            {
                case EffectEnum.DamageTerre:
                case EffectEnum.VolTerre:
                case EffectEnum.DamageNeutre:
                case EffectEnum.DamageLifeNeutre:
                case EffectEnum.VolNeutre:
                    return (this.Stats.GetTotal(EffectEnum.AddArmorTerre) * Math.Max(1 + this.Stats.GetTotal(EffectEnum.AddForce) / 100, 1 + this.Stats.GetTotal(EffectEnum.AddForce) / 200 + this.Stats.GetTotal(EffectEnum.AddIntelligence) / 200)) +
                           (this.Stats.GetTotal(EffectEnum.AddArmor) * Math.Max(1 + this.Stats.GetTotal(EffectEnum.AddForce) / 100, 1 + this.Stats.GetTotal(EffectEnum.AddForce) / 200 + this.Stats.GetTotal(EffectEnum.AddIntelligence) / 200));

                case EffectEnum.DamageFeu:
                case EffectEnum.DamageLifeFeu:
                case EffectEnum.VolFeu:
                    return (this.Stats.GetTotal(EffectEnum.AddArmorFeu) * Math.Max(1 + this.Stats.GetTotal(EffectEnum.AddIntelligence) / 100, 1 + this.Stats.GetTotal(EffectEnum.AddIntelligence) / 200 + this.Stats.GetTotal(EffectEnum.AddIntelligence) / 200)) +
                           (this.Stats.GetTotal(EffectEnum.AddArmor) * Math.Max(1 + this.Stats.GetTotal(EffectEnum.AddIntelligence) / 100, 1 + this.Stats.GetTotal(EffectEnum.AddIntelligence) / 200 + this.Stats.GetTotal(EffectEnum.AddIntelligence) / 200));

                case EffectEnum.DamageAir:
                case EffectEnum.DamageLifeAir:
                case EffectEnum.VolAir:
                    return (this.Stats.GetTotal(EffectEnum.AddArmorAir) * Math.Max(1 + this.Stats.GetTotal(EffectEnum.AddAgilite) / 100, 1 + this.Stats.GetTotal(EffectEnum.AddAgilite) / 200 + this.Stats.GetTotal(EffectEnum.AddIntelligence) / 200)) +
                           (this.Stats.GetTotal(EffectEnum.AddArmor) * Math.Max(1 + this.Stats.GetTotal(EffectEnum.AddAgilite) / 100, 1 + this.Stats.GetTotal(EffectEnum.AddAgilite) / 200 + this.Stats.GetTotal(EffectEnum.AddIntelligence) / 200));

                case EffectEnum.DamageEau:
                case EffectEnum.DamageLifeEau:
                case EffectEnum.VolEau:
                    return (this.Stats.GetTotal(EffectEnum.AddArmorEau) * Math.Max(1 + this.Stats.GetTotal(EffectEnum.AddChance) / 100, 1 + this.Stats.GetTotal(EffectEnum.AddChance) / 200 + this.Stats.GetTotal(EffectEnum.AddIntelligence) / 200)) +
                           (this.Stats.GetTotal(EffectEnum.AddArmor) * Math.Max(1 + this.Stats.GetTotal(EffectEnum.AddChance) / 100, 1 + this.Stats.GetTotal(EffectEnum.AddChance) / 200 + this.Stats.GetTotal(EffectEnum.AddIntelligence) / 200));

            }

            return 0;
        }

        /// <summary>
        /// Calcul des PA/PM perdus
        /// </summary>
        /// <param name="Caster"></param>
        /// <param name="LostPoint"></param>
        /// <param name="MP"></param>
        /// <returns></returns>
        public int CalculDodgeAPMP(Fighter Caster, int LostPoint, bool MP = false)
        {
            var RealLostPoint = 0;

            if (!MP)
            {
                var DodgeAPCaster = Caster.APDodge + 1;
                var DodgeAPTarget = this.APDodge + 1;

                for (int i = 0; i < LostPoint; i++)
                {
                    var ActualAP = this.AP - RealLostPoint;
                    var PercentLastAP = ActualAP / this.AP;
                    var Chance = 0.5 * (DodgeAPCaster / DodgeAPTarget) * PercentLastAP;
                    var PercentChance = Chance * 100;

                    if (PercentChance > 100) PercentChance = 90;
                    if (PercentChance < 10) PercentChance = 10;

                    if (Random.Next(0, 99) < PercentChance) RealLostPoint++;
                }
            }
            else
            {
                var DodgeMPCaster = Caster.MPDodge;
                var DodgeMPTarget = this.MPDodge;

                for (int i = 0; i < LostPoint; i++)
                {
                    var ActualMP = this.MP - RealLostPoint;
                    var PercentLastMP = ActualMP / this.MP;
                    var Chance = 0.5 * (DodgeMPCaster / DodgeMPTarget) * PercentLastMP;
                    var PercentChance = Chance * 100;

                    if (PercentChance > 100) PercentChance = 90;
                    if (PercentChance < 10) PercentChance = 10;

                    if (Random.Next(0, 99) < PercentChance) RealLostPoint++;
                }
            }

            return RealLostPoint;
        }




        public int LifePercentage
        {
            get
            {
                float percentage = ((float)CurrentLife / (float)this.MaxLife);
                return (int)(percentage * 100);
            }
        }

        public int NextCell { get; set; }
        public int NextDir { get; set; }
    }
}
