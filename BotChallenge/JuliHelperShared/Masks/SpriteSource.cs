using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class SpriteSource
    {
        public Texture2D texture;

        public SpriteSource(Texture2D texture)
        {
            this.texture = texture;
        }

        public virtual void Update(float gameSpeed = 1f) { }

        public virtual Rectangle GetSourceRectangle()
        {
            return new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public virtual Vector2 GetOrigin() { return Vector2.Zero; }
    }
}
