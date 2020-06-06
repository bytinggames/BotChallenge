using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JuliHelper;
using Microsoft.Xna.Framework;

namespace BotChallenge
{
    public abstract class Env
    {
        private bool visible;
        public static Random constRand;

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        internal abstract float[] Loop();
        internal virtual void Dispose() { }

        protected T[] GetBots<T>(Type[] types)
        {
            T[] bots = new T[types.Length];
            for (int i = 0; i < bots.Length; i++)
                bots[i] = (T)(Activator.CreateInstance(types[i]));
            return bots;
        }

        internal static void InitRand(int? seed)
        {
            if (seed.HasValue)
                constRand = new Random(seed.Value);
            else
                constRand = new Random();
        }
        /*
public static Color GetBotColor(int index)
{
   float v = 1;
   float hueOffset = 0;
   float hueJump = 120;

   if (index >= 24)
   {
       return Calculate.RandomRGB(new Random(index));
   }

   if (index >= 12)
   {
       index -= 12;
       v = 0.5f;
   }

   if (index < 3)
   {
       hueOffset = 0;
   }
   else if (index < 6)
   {
       hueOffset = 60;
       index -= 3;
   }
   else if (index < 12)
   {
       hueOffset = 30;
       index -= 6;
       hueJump = 60;
   }

   return new HSVColor() { hue = hueOffset + index * hueJump, saturation = 1, value = v, alpha = 255 }.HSVToRGB();
}*/
    }
}
