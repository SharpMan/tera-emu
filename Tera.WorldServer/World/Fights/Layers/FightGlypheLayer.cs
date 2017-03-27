using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Packets;
using Tera.WorldServer.World.Spells;
using Tera.WorldServer.World.Fights.Effects;
using Tera.Libs.Enumerations;

namespace Tera.WorldServer.World.Fights.FightObjects
{
    public class FightGlypheLayer : FightGroundLayer
    {
        public override short Color
        {
            get
            {
                switch (MasterSpellId)
                {
                    case 949:
                        return 0;
                    case 10:
                    case 2033:
                        return 4;
                    case 12:
                    case 2034:
                        return 3;
                    case 13:
                    case 2035:
                        return 6;
                    case 15:
                    case 2036:
                        return 5;
                    case 17:
                    case 2037:
                        return 2;
                }

                return 2;
            }
        }

        public FightGlypheLayer(Fighter Caster, SpellLevel CastSpell, FightCell CastCell, int MasterSpellId, short Duration, String Zone)
            : base(FightObjectType.OBJECT_GLYPHE, Caster, CastSpell, CastCell, MasterSpellId, 0, Zone, Duration)
        {
        }

        private bool isShow = false;
        public override void Show(Fighter Scout = null)
        {
            if (isShow)
            {
                return;
            }
            this.Fight.SendToFight(new FightAddGroundLayerMessage(this));
            isShow = true;
        }

        public override void Hide()
        {
            if (!isShow)
            {
                return;
            }
            List<FightGroundLayer> similars = myCell.GetSimilarObjects(this, VIEW_TYPES).OfType<FightGroundLayer>().ToList();
            if (similars.Count > 0)
            {
                foreach (FightGroundLayer layer in similars)
                {
                    if (layer.EqualsView(this) && layer.Duration > this.Duration)
                    {
                        isShow = false;
                        return;
                    }
                }
            }
            this.Fight.SendToFight(new FightRemoveGroundLayerMessage(this));
            isShow = false;
        }

        protected override void DisposeOverridable()
        {
        }
        
        protected override void onTouchAtBeginTurn(Fighter target)
        {
            this.Triggered(target);
            if (Duration == -1)
            {
                Dispose();
            }
            CastSpell.Initialize();
            var Effects = CastSpell.Effects;
            List<Fighter> listTarget = new List<Fighter>(){target };
            var Targets = new Dictionary<EffectInfos, List<Fighter>>();
            Effects.ForEach(x => Targets.Add(x, listTarget));

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
                if (Targets[Effect][0].Dead)
                    continue;

                /* if (this.fight is MonsterFight && Caster is CharacterFighter)
                     this.fight.Challanges.ForEach(x => x.CheckSpell(Caster, Effect, new List<Fighter> { target }, target.CellId));*/

                List<Fighter> item = new List<Fighter>();
                item.Add(Targets[Effect][0]);
                var CastInfos = new EffectCast(Effect.EffectType, CastSpell.SpellCache.ID, CellId, Effect.Value1, Effect.Value2, Effect.Value3, Effect.Chance, Effect.Duration, Caster, Targets[Effect], false, EffectEnum.None, 0, Effect.Spell);
                if (EffectBase.TryApplyEffect(CastInfos) == -3)
                {
                    break;
                }
                   
            }
            if (!this.Fight.TryEndFight())
            {
                this.Fight.onApplyGroundLayer(this, Caster, CastSpell, target.CellId, target, Targets);
            }
        }

        protected override void onTouchAtEndTurn(Fighter target)
        {
        }

        public override void onWalkOnLayer(Fighter fighter, FightCell newCell)
        {
        }
    }
}
