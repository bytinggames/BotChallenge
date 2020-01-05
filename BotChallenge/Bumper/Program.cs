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
                typeof(Julian2_2),
                typeof(Dominik1),
                typeof(Morpheus2),
                typeof(Julian1_2),
                typeof(Dominik2),
                typeof(Morpheus2_2),
            };

            RunManagerTournament runManager = new RunManagerTournament(typeof(EnvBumper), bots, bots.Length, 3, visible, OutputMode.ResultOfEveryIteration);

            runManager.Loop();

            //if (!visible)
            Console.ReadLine();
        }
    }
}
