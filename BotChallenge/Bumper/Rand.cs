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
    class Rand : EnvBumper.Bot
    {
        float angle = (float)EnvBumper.constRand.NextDouble() * MathHelper.TwoPi;

        protected override Color GetColor()
        {
            return Color.Gray;
        }

        public Rand()
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

            angle += ((float)EnvBumper.constRand.NextDouble() - 0.5f) * 1f;

            return new EnvBumper.Action()
            {
                angle = angle,
                accelerate = false
            };
        }

    }
}
