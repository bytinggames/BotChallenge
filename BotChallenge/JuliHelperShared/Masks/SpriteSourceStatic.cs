using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public class SpriteSourceStatic : SpriteSource
    {
        public Rectangle rectangle;

        public SpriteSourceStatic(Texture2D texture, int x, int y, int w, int h)
            :base (texture)
        {
            rectangle = new Rectangle(x, y, w, h);
        }

        public SpriteSourceStatic(Texture2D texture, Rectangle rectangle)
            :base(texture)
        {
            this.rectangle = rectangle;
        }

        public override Rectangle GetSourceRectangle()
        {
            return rectangle;
        }

        public override void Update(float gameSpeed = 1f) { }
    }
}
