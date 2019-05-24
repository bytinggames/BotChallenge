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
    class Human : EnvSnake.Bot
    {

        protected override EnvSnake.Action GetAction()
        {
            /*
            KeyboardState keyboard = Keyboard.GetState();
            SnakeDirection dir = Direction;
            
            if (keyboard.IsKeyDown(Keys.D))
                dir = SnakeDirection.Right;
            else if (keyboard.IsKeyDown(Keys.S))
                dir = SnakeDirection.Down;
            else if (keyboard.IsKeyDown(Keys.A))
                dir = SnakeDirection.Left;
            else if (keyboard.IsKeyDown(Keys.W))
                dir = SnakeDirection.Up;
            */

            return new EnvSnake.Action()
            {
                movementDirection = EnvSnake.lastPressedDirection,
            };
        }

        protected override Color GetColor()
        {
            return Color.Gray;
        }
    }
}
