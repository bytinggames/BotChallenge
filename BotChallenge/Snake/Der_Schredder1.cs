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
    class Der_Schredder1 : EnvSnake.Bot
    {

        protected override EnvSnake.Action GetAction()
        {
            SnakeDirection richtung = SnakeDirection.Right;

            if (!this.Alive)
            {
                return new EnvSnake.Action()
                {
                    
                    movementDirection = SnakeDirection.Down
                };
            }

            if (HeadPosX == 27 && HeadPosY == 27)
            {

            }

            if (this.HeadPosY+1 == this.GridHeight)
            {
               
                if (this.HeadPosX+1 == GridWidth)
                {
                    richtung = SnakeDirection.Up;

                    return new EnvSnake.Action()
                    {
                        movementDirection = richtung
                    };

                }
                 else richtung = SnakeDirection.Right;
            }

            if (this.HeadPosX -1 == -1)
            {   
                if(this.HeadPosY+1 == this.GridHeight)
                {
                    if (this.HeadPosX+1 == this.GridWidth)
                    {
                        richtung = SnakeDirection.Up;
                    }
                    else richtung = SnakeDirection.Right;
                }
                else richtung = SnakeDirection.Down;
            }

           else if (this.HeadPosX + 1 == GridWidth )
            {

                if (this.HeadPosY + 1 == GridHeight)
                {
                    richtung = SnakeDirection.Right;
                }

                else if (this.HeadPosY - 1 == -1)
                {   
                    if (this.HeadPosX -1 == -1)
                    {
                        richtung = SnakeDirection.Down;
                    }
                    else richtung = SnakeDirection.Left;

                }


                
                else richtung = SnakeDirection.Up;   

            }

            else
                richtung = SnakeDirection.Right;


            return new EnvSnake.Action()
            {
                movementDirection = richtung
            };
        }

        protected override Color GetColor()
        {
            return Color.Teal;
        }
    }
}
