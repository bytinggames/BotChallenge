using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.Poker;

namespace Poker
{
    class Program
    {
        const bool VISIBLE = true;

        static void Main(string[] args)
        {
            Type[] bots = new Type[]
            {
                typeof(Human),
                typeof(Human),
                typeof(Human),
            };

            RunManagerTournament runManager = new RunManagerTournament(typeof(EnvPoker), bots, bots.Length, 3, VISIBLE, OutputMode.ResultOfEveryIteration);

            runManager.Loop();

            while (true)
                Console.ReadLine();
        }
    }
}
