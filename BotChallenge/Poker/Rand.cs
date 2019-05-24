using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.Poker;

namespace Poker
{
    class Rand : EnvPoker.Bot
    {
        protected override int GetAction(int addMinimum)
        {
            if (EnvPoker.constRand.Next(4) == 0)
                return -1;
            else if (EnvPoker.constRand.Next(2) == 0)
                return 0;
            else
                return EnvPoker.constRand.Next(GetMoney());
        }
    }
}
