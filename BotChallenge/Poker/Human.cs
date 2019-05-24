using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.Poker;

namespace Poker
{
    class Human : EnvPoker.Bot
    {
        protected override int GetAction(int addMinimum)
        {
            //env.GetMoneyPot();
            Console.Write(GetName() + ":");
            int action = -1;
            int.TryParse(Console.ReadLine(), out action);
            return action;
        }
    }
}
