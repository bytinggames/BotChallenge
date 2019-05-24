using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.RockPaperScissors;

namespace RockPaperScissors
{
    class Rand : EnvRockPaperScissors.Bot
    {
        protected override EnvRockPaperScissors.Action GetAction()
        {
            return new EnvRockPaperScissors.Action()
            {
                decision = (Decision)Env.constRand.Next(3)
            };
        }
    }
}
