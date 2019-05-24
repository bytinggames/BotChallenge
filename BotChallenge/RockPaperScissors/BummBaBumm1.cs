using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.RockPaperScissors;
using BotChallenge;

namespace RockPaperScissors
{
    class BummBaBumm1 : EnvRockPaperScissors.Bot
    {
        int r = Env.constRand.Next();
        int zufall;
        int meine;
        int seine;
        

        // Setup
        public BummBaBumm1()
        {
            zufall = r % 3;
            seine = zufall;
            meine = 2;
        }

        // Loop
        protected override EnvRockPaperScissors.Action GetAction()
        {
            meine = ((seine+1) * (meine+1) * zufall++) % 3;
            seine = enemy.Wins;


            return new EnvRockPaperScissors.Action()
            {
                decision = (Decision)meine
            };
        }
    }
}
