using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Spells;
using Tera.WorldServer.World.Maps;
using Tera.WorldServer.World.Packets;
using Tera.Libs.Network;
using Tera.WorldServer.World.Fights.Effects;
using Tera.Libs.Helper;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights.FightObjects
{
    public class FightTrapLayer : FightGroundLayer
    {
        public override short Color
        {
            get
            {
                switch (MasterSpellId)
                {
                    case 65:
                        return 7;
                    case 69:
                        return 10;
                    case 71:
                    case 2068:
                        return 9;
                    case 73:
                        return 12;
                    case 77:
                    case 2071:
                        return 11;
                    case 79:
                    case 2072:
                        return 8;
                    case 80:
                        return 13;
                }
                return 7;
            }
        }

        private static readonly char[] dirs = { 'b', 'd', 'f', 'h' };

        public FightTrapLayer(Fighter Caster, SpellLevel CastSpell, FightCell CastCell, int MasterSpellId, int MasterSpellSpriteId, String Zone)
            : base(FightObjectType.OBJECT_TRAP, Caster, CastSpell, CastCell, MasterSpellId, MasterSpellSpriteId, Zone, -1)
        {
        }

        private bool[] ShowTeams = { false, false };

        public override void Show(Fighter Scout = null)
        {
            if (Scout != null)
            {
                if (!ShowTeams[Scout.Team.Id])
                {
                    PacketBase p1 = new FightAddGroundLayerMessage(this);
                    PacketBase p2 = new GameDataCellSetTrap(this.CellId);
                    Scout.Team.GetFighters().ForEach(x => x.Send(p1));
                    Scout.Team.GetFighters().ForEach(x => x.Send(p2));
                    ShowTeams[Scout.Team.Id] = true;
                }
            }
        }

        public override void Hide()
        {
            PacketBase p1 = new FightRemoveGroundLayerMessage(this);
            PacketBase p2 = new GameDataCellReset(this.CellId);
            if (ShowTeams[Fight.Team1.Id])
            {
                Fight.Team1.GetFighters().ForEach(x => x.Send(p1));
                Fight.Team1.GetFighters().ForEach(x => x.Send(p2));
                ShowTeams[Fight.Team1.Id] = false;
            }
            if (ShowTeams[Fight.Team2.Id])
            {
                Fight.Team2.GetFighters().ForEach(x => x.Send(p1));
                Fight.Team2.GetFighters().ForEach(x => x.Send(p2));
                ShowTeams[Fight.Team2.Id] = false;
            }
        }

        protected override void DisposeOverridable()
        {
        }

        protected override void onTouchAtBeginTurn(Fighter target)
        {
        }

        protected override void onTouchAtEndTurn(Fighter target)
        {
        }

        protected override void Triggered(Fighter triggerer)
        {
            this.Fight.SendToFight(new FightGameActionMessage(306, triggerer.ActorId.ToString(), 
                MasterSpellId + "," + CellId + ","+MasterSpellSpriteId +",10,0," + Caster.ActorId));
        }

        public override void onWalkOnLayer(Fighter fighter, FightCell newCell)
        {
            this.Triggered(fighter);
            List<FightCell> myCells = new List<FightCell>();
            myCells.AddRange(this.myCells);
            Dispose();

            // Combat encore en cour ?
            if (this.Fight.FightState != Fights.FightState.STATE_ACTIVE)
                return;

            // La cible si elle existe
            var TargetE = this.Fight.HasEnnemyInCell(CellId, Caster.Team);
            long TargetId = TargetE == null ? -1 : TargetE.ActorId;

            CastSpell.Initialize();

            var Effects = CastSpell.Effects;
            if (Effects == null)
                Effects = CastSpell.CriticEffects;
            var Targets = new Dictionary<EffectInfos, List<Fighter>>();
            foreach (var Effect in Effects)
            {
                Targets.Add(Effect, new List<Fighter>());
                foreach (var FightCell in myCells)
                {
                    if (FightCell != null)
                    {
                        if (FightCell.HasGameObject(FightObjectType.OBJECT_FIGHTER) | FightCell.HasGameObject(FightObjectType.OBJECT_STATIC))
                            Targets[Effect].AddRange(FightCell.GetObjects<Fighter>());
                    }
                }
                if (!Targets[Effect].Contains(fighter))
                    Targets[Effect].Add(fighter);
            }

            var ActualChance = 0;
            foreach (var Effect in Effects)
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
                
                // Actualisation des morts
                Targets[Effect].RemoveAll(F => F.Dead);
                var CastInfos = new EffectCast(Effect.EffectType, CastSpell.SpellCache.ID, CellId, Effect.Value1, Effect.Value2, Effect.Value3, Effect.Chance, Effect.Duration, Caster, Targets[Effect], false, EffectEnum.None, 0, Effect.Spell);

                if (EffectBase.TryApplyEffect(CastInfos) == -3)
                    break;
            }
            if (!this.Fight.TryEndFight())
            {
                this.Fight.onApplyGroundLayer(this, Caster, CastSpell, CellId, fighter, Targets);
            }
        }
    }
}
