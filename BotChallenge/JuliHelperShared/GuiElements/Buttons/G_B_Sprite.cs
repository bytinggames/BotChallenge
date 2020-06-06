using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace JuliHelper.Gui
{
    public class G_B_Sprite : G_Button
    {
        public Sprite sprite;
        //int textureBorder;

        public G_B_Sprite(M_Rectangle area, Sprite sprite, Action<G_Button> PressLeft = null, bool stayPressed = false, float depth = 0f)
        {
            this.posL = area.pos;
            this.size = area.size;
            this.sprite = sprite;
            this.PressLeft = PressLeft;
            this.stayPressed = stayPressed;
            this.depth = depth;
        }

        private void Initialize()
        {
            if (stayPressed)
            {
                if (PressLeft != null)
                    PressLeft += g =>
                    {
                        if (ValueChanged != null)
                            ValueChanged(g.pressed);
                    };

                if (PressRight != null)
                    PressRight += g =>
                    {
                        if (ValueChanged != null)
                            ValueChanged(g.pressed);
                    };
            }
            else
            {
                if (PressLeft != null)
                    PressLeft += g =>
                    {
                        if (ValueChanged != null)
                            ValueChanged((int)((G_B_Sprite)g).sprite.indexX);
                    };
                if (PressRight != null)
                    PressRight += g =>
                    {
                        if (ValueChanged != null)
                            ValueChanged((int)((G_B_Sprite)g).sprite.indexX);
                    };
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                base.Draw(spriteBatch);

                Color color = Color.White;

                //if (hover)
                //{
                //    if (pressed)
                //        color = colorHoverPressed;
                //    else
                //        color = colorHover;
                //}
                //else
                //{
                //    if (pressed)
                //        color = colorPressed;
                //    else
                //        color = colorNormal;
                //}

                sprite.Draw(spriteBatch, posG + sprite.pos, color, depth + 0.01f);

                if (!enabled)
                    base.DrawNotEnabled(spriteBatch);

                DrawEnd(spriteBatch);
            }
        }

        public override GuiElement Clone(GuiElement parent)
        {
            return CloneChild(parent);
        }
        public new G_B_Sprite CloneChild(GuiElement parent)
        {
            G_B_Sprite clone = (G_B_Sprite)base.Clone(parent);
            clone.Initialize();
            clone.sprite = (Sprite)sprite.Clone();
            return clone;
        }

        public override void SetValue(object value, Type type)
        {
            if (stayPressed)
            {
                if (value == null)
                    sprite.visible = false;
                else
                {
                    sprite.visible = true;
                    pressed = (bool)value;
                    sprite.indexX = pressed ? 1 : 0;
                }
            }
            else
            {
                if (value == null)
                    sprite.visible = false;
                else
                {
                    sprite.visible = true;
                    sprite.indexX = (int)value;
                }
            }
        }

        public override void Hotkey(bool invert)
        {
            if (stayPressed)
                pressed = !pressed;
            if (invert && PressRight != null)
            {
                PressRight(this);
            }
            else
            {
                if (PressLeft != null)
                    PressLeft(this);
            }
        }
    }
}
