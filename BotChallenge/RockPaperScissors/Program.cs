using BotChallenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.RockPaperScissors;

namespace RockPaperScissors
{
    class Program
    {
        const bool visible = false;

        static void Main(string[] args)
        {
            Type[] bots = new Type[]
            {
                typeof(Julian1),
                typeof(Lele1),
                typeof(Dominik1),
                typeof(Niklas1),
                typeof(BummBaBumm1),
                typeof(Julian2),
                typeof(Lele2),
                typeof(Dominik2),
                typeof(Niklas2),
                typeof(BummBaBumm2),
            };

            RunManagerTournament runManager = new RunManagerTournament(typeof(EnvRockPaperScissors), bots, 2, 3, visible, OutputMode.ResultOfEveryIteration);

            runManager.Loop();

            while (true)
                Console.ReadLine();
        }
    }
}
