using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotChallenge;
using BotChallenge.Bumper;
using JuliHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Bumper
{
    class Human : EnvBumper.Bot
    {
        protected override Color GetColor()
        {
            return Color.Gray;
        }

        public Human()
        {

        }

        protected override EnvBumper.Action GetAction()
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            Vector2 dist = Vector2.Normalize(Vector2.Transform(mouse.Position.ToVector2(), Matrix.Invert(EnvBumper.matrix)) - Pos);

            Vector2 dir = Vector2.Zero;

            if (Input.right.down)
                dir.X++;
            if (Input.up.down)
                dir.Y--;
            if (Input.left.down)
                dir.X--;
            if (Input.down.down)
                dir.Y++;



            return new EnvBumper.Action()
            {
                angle = dir != Vector2.Zero ? (float)Math.Atan2(dir.Y, dir.X) : 0f,
                accelerate = dir != Vector2.Zero
            };
        }

    }
}
