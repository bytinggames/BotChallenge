using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.RockPaperScissors;
using BotChallenge;

namespace RockPaperScissors
{
    class Niklas1 : EnvRockPaperScissors.Bot
    {
        int r = Env.constRand.Next();
        int[] lastEnemyTurn = new int[3];
        int turn = 0;

        public Niklas1()
        {

        }

        protected int StandartChoice()
        {
            short Scissors = 0, Rock = 0, Paper = 0;

            for (int i = 0; i < 3; i++)
            {
                if (lastEnemyTurn[i] == (int)Decision.Scissors)
                {
                    Scissors++;
                }
                else if (lastEnemyTurn[i] == (int)Decision.Rock)
                {
                    Rock++;
                }
                else
                {
                    Paper++;
                }
            }

            if (Scissors >= Rock && Scissors >= Paper)
            {
                return (Rock >= Paper) ? (int)Decision.Paper : (int)Decision.Rock;
            }
            else if (Rock >= Scissors && Rock >= Paper)
            {
                return (Scissors >= Paper) ? (int)Decision.Paper : (int)Decision.Scissors;
            }
            else if (Paper >= Scissors && Paper >= Rock)
            {
                return (Scissors >= Rock) ? (int)Decision.Scissors : (int)Decision.Rock;
            }
            else
            {
                return (int)Decision.Rock;
            }
        }

        protected override EnvRockPaperScissors.Action GetAction()
        {
            if (turn <= 5)
            {
                turn++;
                if (this.enemy.LastDecision == Decision.Rock)
                {
                    return new EnvRockPaperScissors.Action()
                    {
                        decision = Decision.Scissors
                    };
                }
                else if (this.enemy.LastDecision == Decision.Paper)
                {
                    return new EnvRockPaperScissors.Action()
                    {
                        decision = Decision.Rock
                    };
                }
                else
                {
                    return new EnvRockPaperScissors.Action()
                    {
                        decision = Decision.Paper
                    };
                }
            }
            else
            {
                int[] bufferarray = new int[3];
                for(int i = 0; i < 2; i++)
                {
                    bufferarray[i] = lastEnemyTurn[i + 1];
                }
                bufferarray[2] = (int)this.enemy.LastDecision;

                switch(StandartChoice())
                {
                    case (int)Decision.Scissors:
                        return new EnvRockPaperScissors.Action()
                        {
                            decision = Decision.Scissors
                        };
                    case (int)Decision.Paper:
                        return new EnvRockPaperScissors.Action()
                        {
                            decision = Decision.Paper
                        };
                    case (int)Decision.Rock:
                        return new EnvRockPaperScissors.Action()
                        {
                            decision = Decision.Rock
                        };
                }   
            }


            return new EnvRockPaperScissors.Action()
            {
                decision = Decision.Rock
            };
        }
    }
}
