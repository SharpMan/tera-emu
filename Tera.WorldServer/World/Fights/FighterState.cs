using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Fights.Effects;
using Tera.WorldServer.World.Packets;

namespace Tera.WorldServer.World.Fights
{
    public enum FighterStateEnum
    {
        STATE_NEUTRE = 0,
        STATE_SAOUL = 1,
        STATE_CHERCHEUR_AMES = 2,
        STATE_PORTEUR = 3,
        STATE_PEUREUX = 4,
        STATE_DESORIENTE = 5,
        STATE_ENRACINER = 6,
        STATE_PESANTEUR = 7,
        STATE_PORTE = 8,
        STATE_MOTIVATION_SYLVESTRE = 9,
        STATE_APPRIVOISEMENT = 10,
        STATE_CHEVAUCHANT = 11,

        STATE_PAS_SAGE = 12,
        STATE_VRAIMENT_PAS_SAGE = 13,
        STATE_ENNEIGE = 14,
        STATE_EVEILLE = 15,
        STATE_FRAGILISE = 16,
        STATE_SEPARE = 17,
        STATE_GELE = 18,
        STATE_FISSURE = 19,

        STATE_AFFAIBLI = 42,
        STATE_ALTRUISME = 50,
        STATE_INVISIBLE = 600,
        
        STATE_REFLECT_SPELL = -1,
        STATE_MAXIMIZE_EFFECTS = -2,
        STATE_MINIMIZE_EFFECTS = -3,
    }

    public sealed class FighterState
    {
        private Fighter myFighter;
        private Dictionary<FighterStateEnum, BuffEffect> myStates = new Dictionary<FighterStateEnum, BuffEffect>();

        public FighterState(Fighter Fighter)
        {
            this.myFighter = Fighter;
        }

        public bool CanState(FighterStateEnum State)
        {
            switch (State)
            {
                case FighterStateEnum.STATE_PORTE:
                case FighterStateEnum.STATE_PORTEUR:
                    return !HasState(FighterStateEnum.STATE_PESANTEUR);
            }

            return !HasState(State);
        }

        public bool HasState(FighterStateEnum State)
        {
            return this.myStates.ContainsKey(State);
        }

        public BuffEffect GetBuffByState(FighterStateEnum fse)
        {
            return myStates[fse];
        }

        public void AddState(BuffEffect Buff)
        {
            switch (Buff.CastInfos.EffectType)
            {
                case EffectEnum.Invisible:
                    Buff.Target.Fight.SendToFight(new GameActionMessage((int)EffectEnum.Invisible, this.myFighter.ActorId, this.myFighter.ActorId + "," + Buff.Duration));
                    this.myStates.Add(FighterStateEnum.STATE_INVISIBLE, Buff);
                    return;

                case EffectEnum.ReflectSpell:
                    this.myStates.Add(FighterStateEnum.STATE_REFLECT_SPELL, Buff);
                    return;

                case EffectEnum.MaximizeEffects:
                    this.myStates.Add(FighterStateEnum.STATE_MAXIMIZE_EFFECTS, Buff);
                    return;

                case EffectEnum.MinimizeEffects:
                    this.myStates.Add(FighterStateEnum.STATE_MINIMIZE_EFFECTS, Buff);
                    return;

                default:
                    Buff.Target.Fight.SendToFight(new GameActionMessage((int)EffectEnum.AddState, this.myFighter.ActorId, this.myFighter.ActorId + "," + Buff.CastInfos.Value3 + ",1"));
                    break;
            }
            //TODO System.ArgumentException: An item with the same key has already been added.
            this.myStates.Add((FighterStateEnum)Buff.CastInfos.Value3, Buff);
        }

        public void DelState(BuffEffect Buff)
        {
            switch (Buff.CastInfos.EffectType)
            {
                case EffectEnum.Invisible:
                    Buff.Target.Fight.SendToFight(new GameActionMessage((int)EffectEnum.Invisible, this.myFighter.ActorId, this.myFighter.ActorId.ToString()));
                    Buff.Target.Fight.SendToFight(new GameActionMessage((int)GameActionTypeEnum.MAP_TELEPORT, this.myFighter.ActorId, this.myFighter.ActorId + "," + this.myFighter.CellId));
                    this.myStates.Remove(FighterStateEnum.STATE_INVISIBLE);
                    return;
                case EffectEnum.ReflectSpell:
                    this.myStates.Remove(FighterStateEnum.STATE_REFLECT_SPELL);
                    return;

                case EffectEnum.MaximizeEffects:
                    this.myStates.Remove(FighterStateEnum.STATE_MAXIMIZE_EFFECTS);
                    return;

                case EffectEnum.MinimizeEffects:
                    this.myStates.Remove(FighterStateEnum.STATE_MINIMIZE_EFFECTS);
                    return;

                default:
                    Buff.Target.Fight.SendToFight(new GameActionMessage((int)EffectEnum.AddState, this.myFighter.ActorId, this.myFighter.ActorId + "," + Buff.CastInfos.Value3 + ",0"));
                    break;
            }

            this.myStates.Remove((FighterStateEnum)Buff.CastInfos.Value3);
        }

        public void RemoveState(FighterStateEnum State)
        {
            if (this.HasState(State))
                this.myStates[State].RemoveEffect();
        }

        public BuffEffect FindState(FighterStateEnum State)
        {
            if (this.HasState(State))
                return this.myStates[State];
            return null;
        }

        /*public void Debuff()
        {
            foreach (var State in this.myStates.Values)
                State.RemoveEffect();

            this.myStates.Clear();
        }*/
    }
}
