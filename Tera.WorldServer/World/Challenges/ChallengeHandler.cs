using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Fights;

namespace Tera.WorldServer.World.Challenges
{
    public static class ChallengeHandler
    {
        public static Dictionary<int, Type> TemplateList = new Dictionary<int, Type>(){
            { 1, typeof(Zombie) },                                              
            { 2, typeof(Statue) },
            { 3, typeof(DesigneVolontaire) },
            { 4, typeof(Sursis) } ,    
            { 5, typeof(Econome) },
            { 6, typeof(Versatile) },
            { 9, typeof(Barbare) },
            { 21, typeof(Circulez) },
            { 23, typeof(PerduDeVue)},
            { 33, typeof(Survivant) },
            { 36, typeof(Hardi) },
            { 37, typeof(Collant) },
            { 39, typeof(Anachorete) },
            { 41, typeof(Petulant) },
            { 43, typeof(Abnegation) },
        };

        public static Challenge GetRandomChallenge(Fight Fight)
        {
            var c = TemplateList.Select(t => t.Value).ToArray();
            var rnd = Algo.Random(0, c.Count() - 1);
            return (Challenge)Activator.CreateInstance(c[rnd], Fight);
        }

        public static Challenge GetChallenge(int Id, Fight Fight)
        {
            if (TemplateList.ContainsKey(Id))
                return (Challenge)Activator.CreateInstance(TemplateList[Id], Fight);
            else
                return null;
        }


    }
}
