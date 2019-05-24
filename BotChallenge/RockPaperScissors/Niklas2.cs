using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.RockPaperScissors;
using BotChallenge;

namespace RockPaperScissors
{
    class Niklas2 : EnvRockPaperScissors.Bot
    {
        const int size = 4;
        int turn = 0;
        int r = Env.constRand.Next();
        int[] lastEnemyTurn = new int[size];

        public Niklas2()
        {

        }

        protected int StandartChoice()
        {
            short Scissors = 0, Rock = 0, Paper = 0;

            for (int i = 0; i < size; i++)
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
                return (Scissors >= Paper) ? (int)Decision.Scissors : (int)Decision.Paper;
            }
            else if (Paper >= Scissors && Paper >= Rock)
            {
                return (Scissors >= Rock) ? (int)Decision.Rock : (int)Decision.Scissors;
            }
            else
            {
                return (int)Decision.Rock;
            }
        }

        protected int StartChoice()
        {
            lastEnemyTurn[turn] = (int)this.enemy.LastDecision;
            turn++;

            if (this.enemy.LastDecision == Decision.Rock)
            {
                return (int)Decision.Paper;
            }
            else if (this.enemy.LastDecision == Decision.Paper)
            {
                return (int)Decision.Scissors;
            }
            else
            {
                return (int)Decision.Rock;
            }
        }

        protected void ShiftArray()
        {
            int[] bufferarray = new int[size];
            for (int i = 0; i < (size - 1); i++)
            {
                bufferarray[i] = lastEnemyTurn[i + 1];
            }
            bufferarray[size - 1] = (int)this.enemy.LastDecision;

            for (int i = 0; i < size; i++)
            {
                lastEnemyTurn[i] = bufferarray[i];
            }
        }

        protected override EnvRockPaperScissors.Action GetAction()
        {
            if (turn < size)
            {
                switch (StartChoice())
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
            else
            {
                ShiftArray();

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