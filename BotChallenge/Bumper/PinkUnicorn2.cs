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
    class PinkUnicorn2 : EnvBumper.Bot
    {
        protected override Color GetColor()
        {
            return Color.HotPink;
        }

        public PinkUnicorn2()
        {

        }
        int framecount = 0;
        int dest_x = -3;
        int dest_y =  3;
        protected override EnvBumper.Action GetAction()
        {
            Vector2 dir = new Vector2(0, 0);

            dir.X = dest_x - Pos.X;
            dir.Y = dest_y - Pos.Y;

            if (framecount % 20 == 0) dest_x++;
            if (framecount % 35 == 0) dest_y++;
            if (dest_x == 5) dest_x = -5;
            if (dest_y == 5) dest_y = -5;




            bool speed = framecount % 2 == 0;
            framecount++;


            return new EnvBumper.Action()
            {
                angle = dir != Vector2.Zero ? (float)Math.Atan2(dir.Y, dir.X) : 0f,
                accelerate = speed
            };
        }

        public override void Draw()
        {
            //DrawM.Vertex.DrawCircle(new Vector2(dest_x,dest_y), 1f, Color.Black, 8f);
        }

    }
}
