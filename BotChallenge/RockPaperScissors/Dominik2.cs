using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.RockPaperScissors;
using BotChallenge;

namespace RockPaperScissors
{
    class Dominik2 : EnvRockPaperScissors.Bot
    {
        int r = Env.constRand.Next(3);
        public Dominik2(){

        }

        // let's go!
        int sum_Rock, sum_Paper, sum_Scissors, turn = 0;
        protected override EnvRockPaperScissors.Action GetAction()
        {
            turn++;

            Decision d = new Decision();
            Decision lastDec = this.enemy.LastDecision;
            if (turn > 1) {
                if (lastDec == Decision.Paper) { sum_Paper++; }
                else if (lastDec == Decision.Rock) { sum_Rock++; }
                else { sum_Scissors++; }
            }

            // Decision choice
            if (turn % 2 == 0)
            {
                d = lastDec;
            }
            else
            {
                if (sum_Paper > sum_Scissors)
                {
                    if (sum_Rock > sum_Scissors) { d = Decision.Rock; sum_Rock += r+1; }
                    else if (sum_Rock < sum_Scissors) { d = Decision.Paper; sum_Paper += r+1; }
                    else d = Decision.Scissors; sum_Rock += r+1; // 1
                }
                else if (sum_Paper < sum_Scissors)
                {
                    if (sum_Rock < sum_Paper) { d = Decision.Paper; sum_Paper += r+1; }
                    else if (sum_Rock > sum_Paper) { d = Decision.Scissors; sum_Scissors+=r+1; }
                    else d = Decision.Rock; sum_Rock += r+1; // 2
                }
                else
                {
                    if (sum_Paper < sum_Rock) { d = Decision.Paper; sum_Paper += r+1; } // 3
                    else if (sum_Paper > sum_Rock) { d = Decision.Paper; sum_Paper += r+1; }
                    else d = (Decision) r;

                }
            }

            return new EnvRockPaperScissors.Action()
            {
                decision = d
            };
        }
    }
}
