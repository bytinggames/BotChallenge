using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper.Particles
{
    public class ParticleSystem
    {
        public List<Emitter> emitters;
        public List<Particle> particles;
        public Random rand;

        public ParticleSystem(Random rand)
        {
            this.rand = rand;

            emitters = new List<Emitter>();
            particles = new List<Particle>();
        }

        public void Update(GameTime gameTime, float speed = 1f)
        {
            for (int i = 0; i < emitters.Count;)
            {
                emitters[i].Update(gameTime, speed);

                if (emitters[i].frame >= emitters[i].framesTotal)
                    emitters.RemoveAt(i);
                else
                    i++;
            }

            for (int i = 0; i < particles.Count; )
            {
                particles[i].Update(gameTime, speed);

                if (particles[i].frame >= particles[i].framesTotal)
                    particles.RemoveAt(i);
                else
                    i++;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                particles[i].Draw(spriteBatch);
            }
        }

        public void Clear()
        {
            particles.Clear();
            emitters.Clear();
        }
    }
}
