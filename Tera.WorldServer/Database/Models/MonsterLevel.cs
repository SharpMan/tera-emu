using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.Libs.Enumerations;
using Tera.WorldServer.World.Character;

namespace Tera.WorldServer.Database.Models
{
    public sealed class MonsterLevel
    {
        public Monster Monster { get; set; }
        public int Grade { get; set; }
        public int Level { get; set; }
        public int Life { get; set; }
        public int MaxLife { get; set; }
        public int Initiative { get; set; }
        public long BaseXP { get; set; }
        public GenericStats Stats { get; set; }
        public SpellBook Spells { get; set; }

        public int Size { get; set; }

        public MonsterLevel(Monster Monster, int Grade, int Level, int Life, int MaxLife, int Initiative, int AP, int MP, long BaseXP, string Resistances, string Stats, string Spells)
        {
            this.Monster = Monster;
            this.Grade = Grade;
            this.Size = 100 + (Grade - 1) * 4;
            this.Level = Level;
            this.Life = Life;
            this.MaxLife = MaxLife;
            this.Initiative = Initiative;
            this.BaseXP = BaseXP;

            var Resistance = Resistances.Split(';');
            var StatsArray = Stats.Split(',');

            try
            {
                var RN = int.Parse(Resistance[0]);
                var RT = int.Parse(Resistance[1]);
                var RF = int.Parse(Resistance[2]);
                var RE = int.Parse(Resistance[3]);
                var RA = int.Parse(Resistance[4]);
                var AF = int.Parse(Resistance[5]);
                var MF = int.Parse(Resistance[6]);
                var force = int.Parse(StatsArray[0]);
                var sagesse = int.Parse(StatsArray[1]);
                var intell = int.Parse(StatsArray[2]);
                var chance = int.Parse(StatsArray[3]);
                var agilite = int.Parse(StatsArray[4]);

                this.Stats = new GenericStats();

                // PA/PM
                this.Stats.AddBase(EffectEnum.AddPA, AP);
                this.Stats.AddBase(EffectEnum.AddPM, MP);

                // RESIST
                this.Stats.AddBase(EffectEnum.AddReduceDamagePourcentNeutre, RN);
                this.Stats.AddBase(EffectEnum.AddReduceDamagePourcentTerre, RT);
                this.Stats.AddBase(EffectEnum.AddReduceDamagePourcentFeu, RF);
                this.Stats.AddBase(EffectEnum.AddReduceDamagePourcentEau, RE);
                this.Stats.AddBase(EffectEnum.AddReduceDamagePourcentAir, RA);
                this.Stats.AddBase(EffectEnum.AddEsquivePA, AF);
                this.Stats.AddBase(EffectEnum.AddEsquivePM, MF);

                // STATS
                this.Stats.AddBase(EffectEnum.AddVitalite, Life);
                this.Stats.AddBase(EffectEnum.AddSagesse, sagesse);
                this.Stats.AddBase(EffectEnum.AddForce, force);
                this.Stats.AddBase(EffectEnum.AddIntelligence, intell);
                this.Stats.AddBase(EffectEnum.AddAgilite, agilite);
                this.Stats.AddBase(EffectEnum.AddChance, chance);
                this.Stats.AddBase(EffectEnum.AddInitiative, Initiative);
            }
            catch (Exception ex)
            {

            }

            try
            {
                this.Spells = new SpellBook();

                var SpellInfos = Spells.Split(';');

                foreach (var Info in SpellInfos.Where(x => x != string.Empty))
                {
                    var Data = Info.Split('@');

                    if (Data.Length >= 2)
                    {
                        var SpellId = int.Parse(Data[0]);
                        var SpellLevel = int.Parse(Data[1]);

                        this.Spells.AddSpell(SpellId, SpellLevel);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
