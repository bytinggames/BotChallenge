using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.CarRace;

namespace CarRace
{
    class Program
    {
        const bool visible = true;

        static void Main(string[] args)
        {
            Type[] bots = new Type[]
            {
                typeof(Human),
                typeof(Random),
            };

            RunManagerTournament runManager = new RunManagerTournament(typeof(EnvCarRace), bots, bots.Length, 3, visible, OutputMode.ResultOfEveryIteration);

            runManager.Loop();

            //if (!visible)
            Console.ReadLine();
        }
    }
}
