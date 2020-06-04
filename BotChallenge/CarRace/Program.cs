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
            // determine which classes race against each other
            Type[] bots = new Type[]
            {
                typeof(Human),
                //typeof(HumanWASD),
                typeof(EmptyBot),
            };

            RunManagerTournament runManager = new RunManagerTournament(typeof(EnvCarRace), bots, bots.Length, 1, visible, OutputMode.ResultOfEveryIteration, new System.Random().Next());

            runManager.Loop();

            if (visible)
                Console.ReadLine();
        }
    }
}
