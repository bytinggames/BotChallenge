using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace JuliHelper.Particles
{
    public class Emitter
    {
        public float frame;
        public int framesTotal;

        public Action<Emitter> OnUpdate;

        public Emitter()
        {
            framesTotal = 60;
        }

        public void Update(GameTime gameTime, float speed = 1f)
        {
            if (OnUpdate != null)
                OnUpdate(this);

            frame += speed;
        }

        public Emitter Clone()
        {
            Emitter clone = (Emitter)this.MemberwiseClone();
            return clone;
        }
    }
}
