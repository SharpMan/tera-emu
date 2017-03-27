using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.Database.Models;
using Tera.WorldServer.World.Fights.FightObjects;
using Tera.WorldServer.World.Fights.Effects;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights.Fighters
{
    public class StaticMonster : StaticFighter
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

        public StaticMonster(Fight Fight, MonsterLevel Monster, int MonsterGuid, MonsterGroup MonsterGroup = null, Fighter Invocator = null)
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
            if (mySerializedGMapInformation == null)
            {
                mySerializedGMapInformation = new StringBuilder().Append(this.Cell.Id).Append(';');
                mySerializedGMapInformation.Append("1").Append(';'); // Direction
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

        public override void Send(Libs.Network.PacketBase Packet)
        {
        }

        public override void JoinFight()
        {
        }

        private bool firstTurn = true;
        public int BeginTurn()
        {
            int result = this.Buffs.BeginTurn();
            if (firstTurn && Grade.Monster.StaticFighterBeginEffects != null)
            {
                try
                {
                    var ActualChance = 0;
                    foreach (var Effect in Grade.Monster.StaticFighterBeginEffects)
                    {
                        if (Effect.Chance > 0)
                        {
                            if (Fight.RANDOM.Next(1, 100) > (Effect.Chance + ActualChance))
                            {
                                ActualChance += Effect.Chance;
                                continue;
                            }
                            ActualChance -= 100;
                        }
                        List<Fighter> target = new List<Fighter>(){this};
                        var CastInfos = new EffectCast(Effect.EffectType, -1, CellId, Effect.Value1, Effect.Value2, Effect.Value3, Effect.Chance, Effect.Duration, this, target, false, EffectEnum.None, 0, Effect.Spell);

                        if (EffectBase.TryApplyEffect(CastInfos) == -3)
                            break;
                    }
                }
                catch (Exception e)
                {
                }
                finally
                {
                    firstTurn = false;
                }
                
            }

            return result;
        }
    }
}
