using BotChallenge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.Snake;

namespace Snake
{
    class Program
    {
        const bool visible = true;

        static void Main(string[] args)
        {
            //List<Type> bots = new List<Type>();
            //for (int i = 0; i < 10; i++)
            //{
            //    bots.Add(typeof(Julian1));
            //}

            Type[] bots = new Type[]
            {
                typeof(Der_Schredder1),
                typeof(Dominik2_1),
                typeof(Dominik3),
                typeof(Winner2),
                typeof(Niklas2),
                typeof(Julian2),
            };

            RunManagerTournament runManager = new RunManagerTournament(typeof(EnvSnake), bots, bots.Length, 10, true, OutputMode.ResultOfEveryIteration);


            //RunManager runManager = new RunManager(typeof(EnvSnake), bots.ToArray(), 1, visible, OutputMode.ResultOfEveryIteration);

            runManager.Loop();

            //if (!visible)
            while (true)
                Console.ReadLine();
        }

        static void Main2(string[] args)
        {
            Type[] bots = new Type[]
            {
                typeof(Der_Schredder1),
                typeof(Dominik2),
                typeof(Julian2),
                typeof(Niklas1),
                typeof(Winner2),
            };

            RunManagerTournament runManager = new RunManagerTournament(typeof(EnvSnake), bots.ToArray(), 2, 3, true, OutputMode.ResultOfEveryIteration);
            
            runManager.Loop();

            //if (!visible)
            while (true)
                Console.ReadLine();
        }
    }
}
