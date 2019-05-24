using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.RockPaperScissors;

namespace RockPaperScissors
{
    class Lele2 : EnvRockPaperScissors.Bot
    {
        int r = BotChallenge.Env.constRand.Next();
        int read = 30;
        List<Decision> list = new List<Decision>();
        Decision enemyDecision;
        Decision myDecision;
        Decision d;
        double number;

        public Lele2()
        {
            
        }

        protected override EnvRockPaperScissors.Action GetAction()
        {
            d = (Decision)(r % 3);

            if (enemy.LastDecision == Decision.Paper)
                d = Decision.Scissors;

            else {
                while (number >= 42)
                {
                    if (number % 2 == 0) d = Decision.Paper;

                    number += 0.5f;
                }
            if(enemy.LastDecision == Decision.Scissors)
                {
                    while(number >= 12)
                    {
                        if(number % 3 == 1)
                        {
                            d = Decision.Scissors;
                        }
                        number += 0.25f;
                    }
                }
            
            }

            return new EnvRockPaperScissors.Action()
            {
                decision = d
            };
        }
    }
}
