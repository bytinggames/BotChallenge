using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotChallenge.RockPaperScissors
{
    public enum Decision
    {
        Rock = 0,
        Paper = 1,
        Scissors = 2
    }

    public class EnvRockPaperScissors : Env
    {
        public abstract class Bot
        {
            internal int wins;
            internal Decision lastDecision;

            internal void Initialize(Bot enemy)
            {
                this.enemy = enemy;
            }

            protected Bot enemy;

            internal Action GetInternalAction()
            {
                return GetAction();
            }
            protected abstract Action GetAction();


            public int Wins { get { return wins; } }
            public Decision LastDecision { get { return lastDecision; } }

            internal string GetName() { return GetType().Name; }
        }

        public struct Action
        {
            public Decision decision;
        }
        
        int turns = 100;
        Bot[] bots;

        public EnvRockPaperScissors(Type[] botTypes)
        {
            this.bots = GetBots<Bot>(botTypes).Take(2).ToArray();

            bots[0].Initialize(bots[1]);
            bots[1].Initialize(bots[0]);
        }

        internal override float[] Loop()
        {
            Action[] actions = new Action[bots.Length];

            Console.WriteLine(bots[0].GetName() + " VS. " + bots[1].GetName());

            for (int i = 0; i < turns; i++)
            {
                for (int j = 0; j < bots.Length; j++)
                {
                    actions[j] = bots[j].GetInternalAction();
                    actions[j].decision = (Decision)((int)actions[j].decision % 3);
                    Console.Write(actions[j].decision + " ");
                }
                Console.WriteLine();

                ExecuteActions(actions);

            }
            Console.WriteLine();
            Console.WriteLine(bots[0].GetType().Name + ": " + bots[0].Wins);
            Console.WriteLine(bots[1].GetType().Name + ": " + bots[1].Wins);
            if (bots[0].Wins > bots[1].Wins) return new float[] { 1, 0 };
            else if (bots[0].Wins < bots[1].Wins) return new float[] { 0, 1 };
            else return new float[2];            
        }

        private void ExecuteActions(Action[] actions)
        {
            int outcome = -1;    // -1 = tie; 0 = bot0 wins; 1 = bot1;

            // Tie
            if ((actions[0].decision == Decision.Rock && actions[1].decision == Decision.Rock) ||
                (actions[0].decision == Decision.Paper && actions[1].decision == Decision.Paper)||
                (actions[0].decision == Decision.Scissors && actions[1].decision == Decision.Scissors))
            {
                outcome = -1;
            }
            // Bot0 Wins
            else if ((actions[0].decision == Decision.Rock && actions[1].decision == Decision.Scissors) ||
                (actions[0].decision == Decision.Paper && actions[1].decision == Decision.Rock) ||
                (actions[0].decision == Decision.Scissors && actions[1].decision == Decision.Paper))
            {
                outcome = 0;
                bots[0].wins++;
            }
            // Bot1 Wins
            else if ((actions[0].decision == Decision.Scissors && actions[1].decision == Decision.Rock) ||
                (actions[0].decision == Decision.Rock && actions[1].decision == Decision.Paper) ||
                (actions[0].decision == Decision.Paper && actions[1].decision == Decision.Scissors))
            {
                outcome = 1;
                bots[1].wins++;
            }
            bots[0].lastDecision = actions[0].decision;
            bots[1].lastDecision = actions[1].decision;

            if (Visible)
            {
                Console.Write(DecisionToString(actions[0].decision) + " "+ DecisionToString(actions[1].decision) + " - ");
                if (outcome == -1) Console.WriteLine("Tie");
                else if (outcome == 0) Console.WriteLine(bots[0].GetName() + " Wins");
                else Console.WriteLine(bots[1].GetName() + " Wins");
            }
        }

        private string DecisionToString(Decision d)
        {
            if (d == Decision.Paper)
                return "Paper";
            else if (d == Decision.Rock)
                return "Rock";
            else
                return "Scissors";
        }
    }
}
