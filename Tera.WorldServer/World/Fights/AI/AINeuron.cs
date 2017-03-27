using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tera.WorldServer.World.Spells;

namespace Tera.WorldServer.World.Fights
{
    public class AINeuron : IDisposable
    {
        public List<Fighter> myEnnemies = new List<Fighter>();
        public bool myAttacked = false;
        public List<int> myReachableCells = new List<int>();
        public SpellLevel myBestSpell;
        public int myBestMoveCell, myBestCastCell, myBestScore = 0;
        public bool myFirstTargetIsMe;
        public Dictionary<int, int> myScoreInvocations = new Dictionary<int, int>();
        
        public void Dispose()
        {
            if (myEnnemies != null)
            {
                myEnnemies.Clear();
                myEnnemies = null;
            }
            myAttacked = false;
            if (myReachableCells != null)
            {
                myReachableCells.Clear();
                myReachableCells = null;
            }
            if (myScoreInvocations != null)
            {
                myScoreInvocations.Clear();
                myScoreInvocations = null;
            }
            myBestSpell = null;
            myFirstTargetIsMe = false;
            myBestMoveCell = myBestCastCell = myBestScore = 0;
        }
    }
}
