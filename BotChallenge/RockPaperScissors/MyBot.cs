using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.RockPaperScissors;

namespace RockPaperScissors
{
    class MyBot : EnvRockPaperScissors.Bot
    {
        int r = Env.constRand.Next();

        public MyBot()
        {
            //Start
        }

        protected override EnvRockPaperScissors.Action GetAction()
        {
            //Loop
            int x = Env.constRand.Next(10);
            Decision d = x < 4 ? Decision.Rock : x < 8 ? Decision.Paper : Decision.Scissors;

            return new EnvRockPaperScissors.Action()
            {
                decision = d
            };
        }
    }
}
