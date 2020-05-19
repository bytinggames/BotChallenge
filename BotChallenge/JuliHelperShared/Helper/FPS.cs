using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace JuliHelper
{
    public class FPS
    {
        private float fpsTarget, fps, fpsOut;
        private int frame = 0;

        //private int framesInSecond, fps;
        //private TimeSpan timeInSecond;

        public int Fps
        {
            get { return (int)Math.Round(fpsOut); }
        }

        public FPS()
        {
            fpsTarget = fps = fpsOut = 60;
            //timeInSecond = new TimeSpan();
        }

        public void NewFrame(GameTime gameTime)
        {
            frame++;
            if (frame % 30 == 0)
                fpsOut = fps;
            //framesInSecond++;
            //timeInSecond = timeInSecond.Add(gameTime.ElapsedGameTime);

            fpsTarget = (int)Math.Round(1f / gameTime.ElapsedGameTime.TotalSeconds);
            fps += (fpsTarget - fps) / 5f;

            //while (timeInSecond.TotalMilliseconds >= 1000)
            //{
            //    timeInSecond = timeInSecond.Subtract(TimeSpan.FromSeconds(1));
            //    fps = framesInSecond;
            //    framesInSecond = 0;
            //}
        }
    }
}
