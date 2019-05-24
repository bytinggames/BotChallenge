using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge.Snake;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using BotChallenge;

namespace Snake
{
    class Niklas2 : EnvSnake.Bot
    {
        protected int goForFood()
        {
            if(this.Food.Count == 0)
            {
                return -1;
            }

            int[,] enemyDistance = new int[this.enemies.Length, this.Food.Count];
            int[] myDistance = new int[this.Food.Count];
            int[] goForIt = new int[this.Food.Count];
            
            for(int i = 0; i < this.Food.Count; i++)
            {
                myDistance[i] = Math.Abs(this.HeadPosX - this.Food[i].X) + Math.Abs(this.HeadPosY - this.Food[i].Y);
                goForIt[i] = 1;
            }

            for (int i = 0; i < this.enemies.Length; i++)
            {
                for (int j = 0; j < this.Food.Count; j++)
                {
                    enemyDistance[i, j] = Math.Abs(this.enemies[i].HeadPosX - this.Food[j].X) + Math.Abs(this.enemies[i].HeadPosY - this.Food[j].Y);

                    if (myDistance[j] >= enemyDistance[i, j])
                    {
                        goForIt[j] = 0;
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

            if (this.HeadPosX == 0)
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

            if (this.HeadPosY == 0)
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
                else
                {
                    return (int)SnakeDirection.Down;
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
                else
                {
                    return (int)SnakeDirection.Down;
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

        protected int whereTo()
        {
            int foodResult = goForFood();
            if(foodResult >= 0)
            {
                if(Math.Abs(this.HeadPosX - this.Food[foodResult].X) >= Math.Abs(this.HeadPosY - this.Food[foodResult].Y))
                {
                    if(Math.Abs(this.HeadPosX - this.Food[foodResult].X) < Math.Abs((this.HeadPosX - 1) - this.Food[foodResult].X))
                    {
                        if (testRight())
                        {
                            return (int)SnakeDirection.Right;
                        }
                        else if (testUp())
                        {
                            return (int)SnakeDirection.Up;
                        }
                        else
                        {
                            return (int)SnakeDirection.Down;
                        }
                    }
                    else
                    {
                        if (testLeft())
                        {
                            return (int)SnakeDirection.Left;
                        }
                        else if (testUp())
                        {
                            return (int)SnakeDirection.Up;
                        }
                        else
                        {
                            return (int)SnakeDirection.Down;
                        }
                    }
                }
                else
                {
                    if (Math.Abs(this.HeadPosY - this.Food[foodResult].Y) < Math.Abs((this.HeadPosY - 1) - this.Food[foodResult].Y))
                    {
                        if (testDown())
                        {
                            return (int)SnakeDirection.Down;
                        }
                        else if (testLeft())
                        {
                            return (int)SnakeDirection.Left;
                        }
                        else
                        {
                            return (int)SnakeDirection.Right;
                        }
                    }
                    else
                    {
                        if (testUp())
                        {
                            return (int)SnakeDirection.Up;
                        }
                        else if (testLeft())
                        {
                            return (int)SnakeDirection.Left;
                        }
                        else
                        {
                            return (int)SnakeDirection.Right;
                        }
                    }
                }
            }
            else
            {
                return 5;
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

            direction = whereTo();

            if(direction == 5)
            {
                direction = whereToGo();
            }

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
