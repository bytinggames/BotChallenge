using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.Shooter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shooter
{
    class Human : EnvShooter.Bot
    {
        protected override Color GetColor()
        {
            return Color.Gray;
        }

        public Human()
        {

        }

        protected override EnvShooter.Action GetAction()
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            Vector2 dist = Vector2.Normalize(Vector2.Transform(mouse.Position.ToVector2(), Matrix.Invert(EnvShooter.matrix)) - Pos);
            
            return new EnvShooter.Action()
            {
                right = keyboard.IsKeyDown(Keys.D),
                up = keyboard.IsKeyDown(Keys.W),
                left = keyboard.IsKeyDown(Keys.A),
                down = keyboard.IsKeyDown(Keys.S),
                charge = mouse.LeftButton == ButtonState.Pressed,
                aim = (float)Math.Atan2(dist.Y, dist.X)
            };
        }

    }
}
