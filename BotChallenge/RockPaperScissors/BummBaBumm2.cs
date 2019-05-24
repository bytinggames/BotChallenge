using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.RockPaperScissors;
using BotChallenge;

namespace RockPaperScissors
{
    class BummBaBumm2 : EnvRockPaperScissors.Bot
    {
        int r = Env.constRand.Next();
        int zufall;
        int meine;
        int seine;
        int i = 0;

        // Setup
        public BummBaBumm2()
        {
            zufall = r % 3;
            seine = zufall;
            meine = 2;
        }

        // Loop
        protected override EnvRockPaperScissors.Action GetAction()
        {
            if(i <5)
            {
                meine = 1;
            }
            else
                meine = ((seine+1) * (meine+1) * zufall++) % 3;

            seine = enemy.Wins;
            i++;


            return new EnvRockPaperScissors.Action()
            {
                decision = (Decision)meine
            };
        }
    }
}
