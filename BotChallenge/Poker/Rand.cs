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
            if (EnvPoker.constRand.Next(16) == 0)
                return -1;
            else if (EnvPoker.constRand.Next(3) == 0)
                return EnvPoker.constRand.Next(GetMoney());
            else
                return 0;
        }
    }
}
