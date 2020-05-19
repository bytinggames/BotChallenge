using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace JuliHelper.Gui
{
    public class G_B_Text : G_Button
    {
        public Color colorText;

        public string text;
        SpriteFont font;


        public G_B_Text(M_Rectangle area, string text, Action<G_Button> Press = null, bool stayPressed = false, SpriteFont font = null, Color? colorText = null, float depth = 0f)
        {
            this.posL = area.pos;
            this.size = area.size;
            this.text = text;
            this.PressLeft = Press;
            this.stayPressed = stayPressed;
            this.font = font ?? GuiElement.fontStd;
            this.colorText = colorText ?? Color.Black;
            this.depth = depth;
        }
        public G_B_Text(Vector2 posL, Vector2 size, string text, Action<G_Button> PressLeft = null, bool stayPressed = false, SpriteFont font = null, Color? colorText = null, float depth = 0f)
        {
            this.posL = posL;
            this.size = size;
            this.text = text;
            this.PressLeft = PressLeft;
            this.stayPressed = stayPressed;
            this.font = font ?? GuiElement.fontStd;
            this.colorText = colorText ?? Color.Black;
            this.depth = depth;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                base.Draw(spriteBatch);


                DrawM.Sprite.DrawStringCenter(spriteBatch, text, GetBounds(), colorText, font, true, depth + 0.0001f);

                if (!enabled)
                    base.DrawNotEnabled(spriteBatch);

                DrawEnd(spriteBatch);
            }
        }

        public override GuiElement Clone(GuiElement parent)
        {
            return CloneChild(parent);
        }
        public new G_B_Text CloneChild(GuiElement parent)
        {
            G_B_Text clone = (G_B_Text)base.Clone(parent);
            return clone;
        }
    }
}
