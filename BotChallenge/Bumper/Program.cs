using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.Bumper;

namespace Bumper
{
    class Program
    {
        const bool visible = true;

        static void Main(string[] args)
        {
            Type[] bots = new Type[]
            {
                typeof(Rand),
                typeof(Julian),
            };

            RunManagerTournament runManager = new RunManagerTournament(typeof(EnvBumper), bots, bots.Length, 10, visible, OutputMode.ResultOfEveryIteration);

            runManager.Loop();

            //if (!visible)
            Console.ReadLine();
        }
    }
}
