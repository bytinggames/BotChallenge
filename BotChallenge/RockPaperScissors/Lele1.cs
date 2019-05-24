using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.RockPaperScissors;

namespace RockPaperScissors
{
    class Lele1 : EnvRockPaperScissors.Bot
    {
        int r = BotChallenge.Env.constRand.Next();
        int read = 30;
        List<Decision> list = new List<Decision>();
        Decision enemyDecision;
        Decision myDecision;
        Decision d; 

        public Lele1()
        {
            
        }

        protected override EnvRockPaperScissors.Action GetAction()
        {

            for (int i = 30;  i < read; i++)
            {
                if (enemyDecision == Decision.Paper) d = Decision.Paper;
                    list.Add(enemyDecision); 
            }

            return new EnvRockPaperScissors.Action()
            {
                decision = d
            };
        }
    }
}
