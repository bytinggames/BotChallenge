/* please copy this class and rename it to Program.cs, then in the new file, write one / in front of this first line to enable the Program class
// like so: //* please copy this class....


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
        static readonly bool visible = true;

        static void Main(string[] args)
        {
            // determine which classes race against each other
            Type[] bots = new Type[]
            {
                typeof(Human),
                //typeof(HumanWASD),
                //typeof(EmptyBot),
            };

            RunManagerTournament runManager = new RunManagerTournament(
                typeof(EnvCarRace)
                , bots          // participating bots
                , bots.Length   // amount of bots per battle
                , 1             // iterations per battle
                , visible       // if the simulation is drawn to screen (60fps, otherwise as fast as possible)
                , OutputMode.ResultOfEveryIteration
                , new System.Random().Next() // seed (currently randomized)
                );

            runManager.Loop();

            Console.ReadLine();
        }
    }
}


//*/