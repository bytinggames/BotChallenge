using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.Snake;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Snake
{
    class Niklas1 : EnvSnake.Bot
    {
        protected int goForFood()
        {
            int[,] enemyDistance = new int[this.enemies.Length, this.Food.Count];
            int[] myDistance = new int[this.Food.Count];
            int[] goForIt = new int[this.Food.Count];
            
            for(int i = 0; i < this.Food.Count; i++)
            {
                myDistance[i] = (this.HeadPosX - this.Food[i].X) - (this.HeadPosY - this.Food[i].Y);
                goForIt[i] = 1;
            }

            for (int i = 0; i < this.Food.Count; i++)
            {
                for (int j = 0; j < this.enemies.Length; j++)
                {
                    enemyDistance[i, j] = (this.enemies[j].HeadPosX - this.Food[i].X) + (this.enemies[j].HeadPosY - this.Food[i].Y);

                    if (myDistance[i] >= enemyDistance[i, j])
                    {
                        goForIt[i] = 0;
                    }
                }
            }

            for(int i = 0; i < this.Food.Count; i++)
            {
                if(goForIt[i] == 1)
                {
                    return i;
                }
            }

            return -1;
        }

        protected bool testRight()
        {
            int isPossible = 1;

            if (this.HeadPosX + 1 == this.GridWidth)
            {
                isPossible = 0;
            }
            else
            {
                for (int i = 0; i < this.enemies.Length; i++)
                {
                    for (int j = 0; j < this.enemies[i].BodyParts.Count; j++)
                    {
                        if (this.HeadPosX + 1 == this.enemies[i].BodyParts[j].X && this.HeadPosY == this.enemies[i].BodyParts[j].Y)
                        {
                            isPossible = 0;
                        }
                    }
                }
            }

            if (isPossible >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool testLeft()
        {
            int isPossible = 1;

            if (this.HeadPosX - 1 == 0)
            {
                isPossible = 0;
            }
            else
            {
                for (int i = 0; i < this.enemies.Length; i++)
                {
                    for (int j = 0; j < this.enemies[i].BodyParts.Count; j++)
                    {
                        if (this.HeadPosX - 1 == this.enemies[i].BodyParts[j].X && this.HeadPosY == this.enemies[i].BodyParts[j].Y)
                        {
                            isPossible = 0;
                        }
                    }
                }
            }

            if (isPossible >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool testUp()
        {
            int isPossible = 1;

            if (this.HeadPosY - 1 == 0)
            {
                isPossible = 0;
            }
            else
            {
                for (int i = 0; i < this.enemies.Length; i++)
                {
                    for (int j = 0; j < this.enemies[i].BodyParts.Count; j++)
                    {
                        if (this.HeadPosY - 1 == this.enemies[i].BodyParts[j].Y && this.HeadPosX == this.enemies[i].BodyParts[j].X)
                        {
                            isPossible = 0;
                        }
                    }
                }
            }

            if (isPossible >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool testDown()
        {
            int isPossible = 1;

            if (this.HeadPosY + 1 == this.GridHeight)
            {
                isPossible = 0;
            }
            else
            {
                for (int i = 0; i < this.enemies.Length; i++)
                {
                    for (int j = 0; j < this.enemies[i].BodyParts.Count; j++)
                    {
                        if (this.HeadPosY + 1 == this.enemies[i].BodyParts[j].Y && this.HeadPosX == this.enemies[i].BodyParts[j].X)
                        {
                            isPossible = 0;
                        }
                    }
                }
            }

            if (isPossible >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected int whereToGo()
        {
            if(this.Direction == SnakeDirection.Right)
            {
                if(testRight())
                {
                    return (int)SnakeDirection.Right;
                }
                else if(testUp())
                {
                    return (int)SnakeDirection.Up;
                }
                else if (testDown())
                {
                    return (int)SnakeDirection.Down;
                }
                else
                {
                    return (int)SnakeDirection.Right;
                }
            }
            else if(this.Direction == SnakeDirection.Left)
            {
                if (testLeft())
                {
                    return (int)SnakeDirection.Left;
                }
                else if (testUp())
                {
                    return (int)SnakeDirection.Up;
                }
                else if (testDown())
                {
                    return (int)SnakeDirection.Down;
                }
                else
                {
                    return (int)SnakeDirection.Right;
                }
            }
            else if (this.Direction == SnakeDirection.Up)
            {
                if (testUp())
                {
                    return (int)SnakeDirection.Up;
                }
                else if (testLeft())
                {
                    return (int)SnakeDirection.Left;
                }
                else if (testRight())
                {
                    return (int)SnakeDirection.Right;
                }
                else
                {
                    return (int)SnakeDirection.Right;
                }
            }
            else if (this.Direction == SnakeDirection.Down)
            {
                if (testDown())
                {
                    return (int)SnakeDirection.Down;
                }
                else if (testLeft())
                {
                    return (int)SnakeDirection.Left;
                }
                else if (testRight())
                {
                    return (int)SnakeDirection.Right;
                }
                else
                {
                    return (int)SnakeDirection.Right;
                }
            }
            else
            {
                return (int)SnakeDirection.Right;
            }
        }

        protected override EnvSnake.Action GetAction()
        {
            int direction;
            
            if(!this.Alive)
            {
                return new EnvSnake.Action()
                {
                    movementDirection = SnakeDirection.Up
                };
            }

            direction = whereToGo();

           switch(direction)
            {
                case 0:
                    return new EnvSnake.Action()
                    {
                        movementDirection = SnakeDirection.Right
                    };
                case 1:
                    return new EnvSnake.Action()
                    {
                        movementDirection = SnakeDirection.Down
                    };
                case 2:
                    return new EnvSnake.Action()
                    {
                        movementDirection = SnakeDirection.Left
                    };
                case 3:
                    return new EnvSnake.Action()
                    {
                        movementDirection = SnakeDirection.Up
                    };
            }


            return new EnvSnake.Action()
            {
                movementDirection = SnakeDirection.Up
            };
        }

        protected override Color GetColor()
        {
            return Color.Gold;
        }
    }
}
