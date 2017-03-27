using SingularSys.Jep;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Tera.Libs;
using Tera.Libs.Enumerations;
using Tera.WorldServer.Database.Models;

namespace Tera.WorldServer.World.Controllers
{
    public class ConditionParser
    {
        public static Boolean validConditions(Player perso, String req)
        {
            if ((req == null) || (req.Equals("")))
            {
                return true;
            }
            if (req.Contains("BI"))
            {
                return false;
            }
            JepInstance jep = new JepInstance();
            if (req.Contains("PO"))
            {
                req = havePO(req, perso);
            }
            req = req.Replace("&", "&&").Replace("=", "==").Replace("|", "||").Replace("!", "!=");
            try
            {
                jep.AddVariable("CI", perso.myStats.GetEffect(EffectEnum.AddIntelligence).Total);
                jep.AddVariable("CV", perso.myStats.GetEffect(EffectEnum.AddVitalite).Total);
                jep.AddVariable("CA", perso.myStats.GetEffect(EffectEnum.AddAgilite).Total);
                jep.AddVariable("CW", perso.myStats.GetEffect(EffectEnum.AddSagesse).Total);
                jep.AddVariable("CC", perso.myStats.GetEffect(EffectEnum.AddChance).Total);
                jep.AddVariable("CS", perso.myStats.GetEffect(EffectEnum.AddForce).Total);

                jep.AddVariable("Ci", perso.myStats.GetEffect(126).Base);
                jep.AddVariable("Cs", perso.myStats.GetEffect(118).Base);
                jep.AddVariable("Cv", perso.myStats.GetEffect(125).Base);
                jep.AddVariable("Ca", perso.myStats.GetEffect(119).Base);
                jep.AddVariable("Cw", perso.myStats.GetEffect(124).Base);
                jep.AddVariable("Cc", perso.myStats.GetEffect(123).Base);

                jep.AddVariable("Ps", perso.Alignement);
                jep.AddVariable("Pa", perso.AlignementLevel);
                jep.AddVariable("PP", perso.getGrade());
                jep.AddVariable("PL", perso.Level);
                jep.AddVariable("PK", perso.Kamas);
                jep.AddVariable("PG", perso.Classe);
                jep.AddVariable("PS", perso.Sexe);
                jep.AddVariable("PZ", true);

                jep.AddVariable("MiS", perso.ID);

                jep.Parse(req);
                Object result = jep.Evaluate();
                Boolean ok = false;
                if (result != null)
                {
                    ok = Boolean.Parse(result.ToString());
                }
                return ok;
            }
            catch (JepException e)
            {
                Logger.Error("An error occurred: " + e.ToString());
            }
            return true;
        }

        public static String havePO(String cond, Player perso)
        {
            String[] cut = Regex.Split(cond.Replace("[ ()]", ""), "[|&]");

            List<int> value = new List<int>(cut.Length);

            foreach (String cur in cut)
            {
                if (!cur.Contains("PO"))
                {
                    continue;
                }
                if (perso.InventoryCache.hasItemGuid(int.Parse(Regex.Split(cur, "[=]")[1])))
                {
                    value.Add(Convert.ToInt32(Regex.Split(cur, "[=]")[1]));
                }
                else
                {
                    value.Add(Convert.ToInt32(-1));
                }
            }
            foreach (int i in value)
            {
                Regex regex = new Regex("PO");
                cond = regex.Replace(cond, i + "", 1);
                //cond = cond.replaceFirst("PO", curValue + "");
            }

            return cond;
        }
    }
}
