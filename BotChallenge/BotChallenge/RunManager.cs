using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotChallenge
{
    public class RunManager
    {
        int iterations;
        Type envType;
        Type[] botTypes;
        bool Visible;
        OutputMode outputMode;

        public RunManager(Type env, Type[] bots,int iterations,bool Visible, OutputMode outputMode)
        {
            this.envType = env;
            this.botTypes = bots;
            this.iterations = iterations;
            this.Visible = Visible;
            this.outputMode = outputMode;
        }

        public float[] Loop()
        {
            if (outputMode >= OutputMode.ResultOfEveryBattle)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine("__________________________________________________________________________________________________");
                Console.Write("\n Battle: ");
                for (int i = 0; i < botTypes.Length; i++)
                    Console.Write(botTypes[i].Name + " " + (i < botTypes.Length - 1 ? " vs " : ""));

                //Console.ReadLine();
            }


            float[] globalScores = new float[botTypes.Length];
            for (int i = 0; i < iterations; i++)
            {
                if (outputMode >= OutputMode.ResultOfEveryIteration)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine();
                    Console.WriteLine("________________________");
                    Console.WriteLine("Iteration " + (i + 1) + " / " + iterations);
                }

                Env env = (Env)Activator.CreateInstance(this.envType, new object[] { botTypes });
                env.Visible = Visible;

                float[] scores = env.Loop();
                for (int j = 0; j < scores.Length; j++)
                {
                    for (int k = 0; k < scores.Length; k++)
                    {
                        if (k != j)
                        {
                            if (scores[j] > scores[k])
                                globalScores[j]++; //score for every enemy, which has less points than you
                        }
                    }
                    //globalScores[j] += scores[j];
                }

                if (outputMode >= OutputMode.ResultOfEveryIteration)
                {
                    Console.WriteLine();
                    Console.Write("Iteration Results: ");
                    for (int j = 0; j < scores.Length; j++)
                    {
                        Console.Write(scores[j] + " ");
                    }
                    Console.WriteLine();

                    Console.WriteLine("<Enter>");
                    Console.ReadLine();
                }
            }

            return globalScores;
        }
    }

    //TODO:
    //good console ui (iteration visibility)
}
