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
            };

            RunManagerTournament runManager = new RunManagerTournament(typeof(EnvCarRace), bots, bots.Length, 1, visible, OutputMode.ResultOfEveryIteration, new System.Random().Next());

            runManager.Loop();

            //if (!visible)
            Console.ReadLine();
        }
    }
}
