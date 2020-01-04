using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotChallenge
{
    public enum OutputMode
    {
        None,
        ResultOfTournament,
        ResultOfEveryBattle,
        ResultOfEveryIteration
    }

    public class RunManagerTournament
    {
        int botsPerBattle;
        int iterations;
        Type envType;
        Type[] botTypes;
        bool visible;
        OutputMode outputMode;

        public RunManagerTournament(Type env, Type[] bots, int botsPerBattle, int iterations, bool visible, OutputMode outputMode)
        {
            if (botsPerBattle > bots.Length)
                botsPerBattle = bots.Length;

            this.envType = env;
            this.botTypes = bots;
            this.botsPerBattle = botsPerBattle;
            this.iterations = iterations;
            this.visible = visible;
            this.outputMode = outputMode;
        }

        public void Loop()
        {
            int j;

            float[] totalScores = new float[botTypes.Length];

            int[] currentBots = new int[botsPerBattle];
            for (int i = 0; i < botsPerBattle; i++)
                currentBots[i] = i;

            do
            {
                //int[] indices = new int[botsPerBattle];
                List<Type> currentTypes = new List<Type>();
                List<int> indices = new List<int>();
                for (int i = 0; i < botsPerBattle; i++)
                {
                    j = Env.constRand.Next(i);
                    indices.Insert(j, currentBots[i]);
                    currentTypes.Insert(j, botTypes[currentBots[i]]);
                }


                
                RunManager runManager = new RunManager(envType, currentTypes.ToArray(), iterations, visible, outputMode);
                float[] scores = runManager.Loop();

                if (outputMode >= OutputMode.ResultOfEveryBattle)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n_________________________________________________\nBattle Result:");
                    for (int i = 0; i < scores.Length; i++)
                    {
                        string name = currentTypes[i].Name;
                        Console.WriteLine(name + ": " + scores[i]);
                    }
                }

                for (int i = 0; i < scores.Length; i++)
                {
                    int index = indices[i];

                    for (j = 0; j < scores.Length; j++)
                    {
                        if (i != j && scores[i] > scores[j])
                        {
                            totalScores[index]++;
                        }
                    }
                }

                if (!visible)
                {
                    Console.WriteLine("<Enter>");
                    Console.ReadLine();
                }
                int max = botTypes.Length;
                for (j = botsPerBattle - 1; j >= 0; j--)
                {
                    currentBots[j]++;
                    if (currentBots[j] >= max)
                        max--;
                    else
                        break;
                }

                if (j == -1)
                    break;

                for (j++; j < botsPerBattle; j++)
                {
                    currentBots[j] = currentBots[j - 1] + 1;
                }

            } while (true);

            if (outputMode >= OutputMode.ResultOfEveryBattle)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n_________________________________________________________\nTournament Result:");
                for (int i = 0; i < totalScores.Length; i++)
                {
                    string name = botTypes[i].Name;
                    Console.WriteLine(name + ": " + totalScores[i]);
                }
            }
        }
    }
}
