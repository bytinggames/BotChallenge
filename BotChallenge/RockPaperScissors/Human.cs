using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.RockPaperScissors;

namespace RockPaperScissors
{
    class Human : EnvRockPaperScissors.Bot
    {
        protected override EnvRockPaperScissors.Action GetAction()
        {
            //Console.Write("Input (0-2): ");

            int input = Console.ReadKey().KeyChar - 49;

            return new EnvRockPaperScissors.Action()
            {
                decision = input == 0 ? Decision.Rock : input == 1 ? Decision.Paper : Decision.Scissors
            };
        }
    }
}
