using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper.Particles
{
    public class Particle
    {
        public Sprite sprite;

        public Action<Particle> OnUpdate;
        public float frame;
        public int framesTotal;
        public Vector2 velocity;
        public Vector2 acceleration;

        public float fraction
        {
            get { return (float)frame / (float)framesTotal; }
        }

        public Vector2 pos
        {
            get { return sprite.pos; }
            set { sprite.pos = value; }
        }

        public Particle(Sprite sprite)
        {
            this.sprite = sprite;
            framesTotal = 60;
            velocity = acceleration = Vector2.Zero;
        }

        public virtual void Update(GameTime gameTime, float speed = 1f)
        {
            pos += velocity * speed;
            velocity += acceleration * speed;

            if (OnUpdate != null)
                OnUpdate(this);

            frame += speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Draw(spriteBatch);
        }

        public Particle Clone()
        {
            Particle clone = (Particle)this.MemberwiseClone();
            clone.sprite = (Sprite)sprite.Clone();
            return clone;
        }
    }
}
