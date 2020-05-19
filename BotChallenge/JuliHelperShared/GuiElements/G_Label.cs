using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;


namespace JuliHelper.Gui
{
    public class G_Label : GuiElement
    {
        public Color color;
        public Color? backColor;
        private string text;
        private SpriteFont font;
        public Center center;
        public bool roundPos;

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                UpdateText();
            }
        }
        public SpriteFont Font
        {
            get { return font; }
            set
            {
                font = value;
                UpdateText();
            }
        }


        public G_Label(string text, Vector2 pos, SpriteFont font, Color color, Center center = Center.None, float depth = 0f)
        {
            this.center = center;
            this.text = text;
            this.posL = pos;
            this.font = font;
            this.color = color;
            this.depth = depth;

            this.roundPos = true;

            UpdateText();
        }

        //public void CenterText(Vector2 centerL)
        //{
        //    switch (center)
        //    {
        //        case Center.X:
        //            posL.X = centerL.X - size.X / 2f;
        //            posL.Y = centerL.Y;
        //            break;
        //        case Center.Y:
        //            posL.X = centerL.X;
        //            posL.Y = centerL.Y - size.Y / 2f;
        //            break;
        //        case Center.XY:
        //            posL = centerL - size / 2f;
        //            break;
        //    }

        //}

        private void UpdateText()
        {
            Vector2 oldSize = size;

            if (font != null)
                size = font.MeasureString(text);
            else
                size = Vector2.Zero;

            float dist;
            switch (center)
            {
                case Center.X:
                    dist = size.X - oldSize.X;
                    posL.X -= dist / 2f;
                    break;
                case Center.Y:
                    dist = size.Y - oldSize.Y;
                    posL.Y -= dist / 2f;
                    break;
                case Center.XY:
                    Vector2 distV = size - oldSize;
                    posL -= distV / 2f;
                    break;
            }
        }

        public override void Update() { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                if (backColor.HasValue)
                    DrawM.Sprite.DrawRectangle(spriteBatch, GetRectangle(), backColor.Value, depth);

                spriteBatch.DrawString(font, text, Calculate.RoundVector(posG), color, 0, Vector2.Zero, 1f, SpriteEffects.None, depth + 0.0001f);
            }
        }

        public enum Center
        {
            None,
            X,
            Y,
            XY
        }
    }
}
