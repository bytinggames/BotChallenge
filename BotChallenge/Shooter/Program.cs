using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using Microsoft.Xna.Framework;
using BotChallenge.Shooter;

namespace Shooter
{
    class Program
    {
        const bool visible = true;

        static void Main(string[] args)
        {
            Type[] bots = new Type[]
            {
                typeof(Julian3_2),
                typeof(Zachi2),
            };

            RunManagerTournament runManager = new RunManagerTournament(typeof(EnvShooter), bots, bots.Length, 10, visible, OutputMode.ResultOfEveryIteration);
            
            runManager.Loop();

            //if (!visible)
                Console.ReadLine();
        }
    }
}
