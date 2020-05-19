using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace JuliHelper.Gui
{
    public class G_ToolTip : GuiElement
    {
        public G_Plane plane;
        string text;
        SpriteFont font;

        Color textColor = Color.White;

        public G_ToolTip(Vector2 pos, string text, SpriteFont font, float depth)
        {
            this.posL = pos;
            this.text = text;
            this.font = font;
            this.depth = depth;

            size = font.MeasureString(text) + new Vector2(8,2);

            plane = new G_Plane(new M_Rectangle(Vector2.Zero, size), depth);

            UpdatePos(pos);
            
            textColor = Color.Black;
            plane.areaColor = new Color(255,255,255);
        }

        public void UpdatePos(Vector2 pos)
        {
            pos.Y += 18;


            if (pos.X < 0)
                pos.X = 0;
            else if (pos.X + size.X > resX)
                pos.X = resX - size.X;
            if (pos.Y < 0)
                pos.Y = 0;
            else if (pos.Y + size.Y > resY)
            {
                pos.Y -= 20 + size.Y;
                if (pos.Y + size.Y > resY)
                    pos.Y = resY - size.Y;
            }

            plane.posG = pos;
        }

        public override void Update()
        {
            if (visible)
            {
                UpdateBegin();

                plane.Update();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                plane.Draw(spriteBatch);
                DrawM.Sprite.DrawRectangleOutline(spriteBatch, plane.GetRectangle(), Color.Black, 1f, depth + 0.005f);
                spriteBatch.DrawString(font, text, plane.posG + new Vector2(4, 1), textColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth + 0.01f);

                DrawEnd(spriteBatch);
            }
        }
    }
}
