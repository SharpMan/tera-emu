using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Spells;

namespace Tera.WorldServer.Database.Models
{
    public class SpellModel
    {
        public List<SpellLevel> Levels = new List<SpellLevel>();

        public int ID { get; set; }
        public String Name { get; set; }
        public int SpriteID { get; set; }
        public String SpriteInfos { get; set; }
        public String Level1 { get; set; }
        public String Level2 { get; set; }
        public String Level3 { get; set; }
        public String Level4 { get; set; }
        public String Level5 { get; set; }
        public String Level6 { get; set; }
        public String EffectTargets { get; set; }
        public List<int> effectTargets = new List<int>();
        public List<int> CCeffectTargets = new List<int>();

        public void Initialize()
        {
            if (this.EffectTargets != String.Empty)
            {
                String nET = EffectTargets.Split(':')[0];
                String ccET = "";
                if (EffectTargets.Split(':').Length > 1)
                {
                    ccET = EffectTargets.Split(':')[1];
                }
                foreach (String num in nET.Split(';'))
                {
                    try
                    {
                        effectTargets.Add(int.Parse(num));
                    }
                    catch (Exception e)
                    {
                        effectTargets.Add(0);
                        continue;
                    };
                }
                foreach (String num in ccET.Split(';'))
                {
                    try
                    {
                        CCeffectTargets.Add(int.Parse(num));
                    }
                    catch (Exception e)
                    {
                        CCeffectTargets.Add(0);
                        continue;
                    };
                }
            }
            this.Levels.Add(new SpellLevel(1, this.Level1, this));
            this.Levels.Add(new SpellLevel(2, this.Level2, this));
            this.Levels.Add(new SpellLevel(3, this.Level3, this));
            this.Levels.Add(new SpellLevel(4, this.Level4, this));
            this.Levels.Add(new SpellLevel(5, this.Level5, this));
            this.Levels.Add(new SpellLevel(6, this.Level6, this));
        }

        public SpellLevel GetLevel(int level)
        {
            return this.Levels.FirstOrDefault(x => x.Level == level);
        }


    }
}
