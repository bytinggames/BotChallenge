using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace JuliHelper.Gui
{
    public class G_Color : GuiElement
    {
        G_B_Text btn;
        G_TextBox txt;

        public Color? color;

        public G_Color(M_Rectangle area, Action<object> ColorChanged = null, Color? color = null, float depth = 0f)
        {
            this.posL = area.pos;
            this.size = area.size;
            this.color = color;
            this.depth = depth;

            Initialize();

            this.ValueChanged += ColorChanged;
        }

        private void Initialize()
        {
            btn = new G_B_Text(new M_Rectangle(new Vector2(size.X - size.Y, 0), new Vector2(size.Y)), "", BtnPressLeft, false, null, null, depth) { parent = this };
            btn.PressRight += BtnPressRight;
            if (color.HasValue)
                btn.colorHover = btn.colorHoverPressed = btn.colorNormal = btn.colorPressed = color.Value;

            txt = new G_TextBox(new M_Rectangle(new Vector2(0, 0), new Vector2(size.X - size.Y, size.Y)), TextChanged, new KeyCollection().AddHex(), 6, depth) { parent = this };

            if (color.HasValue)
                txt.text = MyConverter.ToString(color.Value);

            ValueChanged = MyColorChanged;
        }

        public override void Update()
        {
            if (visible)
            {
                UpdateBegin();

                btn.Update();
                txt.Update();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                btn.Draw(spriteBatch);
                txt.Draw(spriteBatch);

                DrawEnd(spriteBatch);
            }
        }

        public override GuiElement Clone(GuiElement parent)
        {
            return CloneChild(parent);
        }
        public G_Color CloneChild(GuiElement parent)
        {
            G_Color clone = (G_Color)base.Clone(parent);
            //clone.Initialize();

            clone.btn = btn.CloneChild(clone);
            clone.btn.PressLeft = clone.BtnPressLeft;
            clone.btn.PressRight = clone.BtnPressRight;

            clone.txt = txt.CloneChild(clone);
            clone.txt.ValueChanged = clone.TextChanged;
            
            clone.ValueChanged = clone.MyColorChanged;
            return clone;
        }

        private void TextChanged(object text)
        {
            Color newColor;
            if (MyConverter.TryParse<Color>((string)text, out newColor))
            {
                color = newColor;
                if (ValueChanged != null)
                    ValueChanged(color.Value);
            }
            else
            {
                if (color == Color.Transparent)
                    txt.text = "";
                else
                    txt.text = MyConverter.ToString(color);
            }
        }

        private void BtnPressLeft(G_Button btn)
        {
            if (!color.HasValue)
                color = Color.Red;
            else
                color = Calculate.GetNextColor(color.Value);
            if (ValueChanged != null)
                ValueChanged(color.Value);
        }
        private void BtnPressRight(G_Button btn)
        {
            if (!color.HasValue)
                color = Color.Red;
            else
                color = Calculate.GetPrevColor(color.Value);
            if (ValueChanged != null)
                ValueChanged(color.Value);
        }

        private void MyColorChanged(object color)
        {
            btn.colorHover = btn.colorHoverPressed = btn.colorNormal = btn.colorPressed = (Color)color;
            txt.text = MyConverter.ToString(color);
        }

        public override void SetValue(object value, Type type)
        {
            if (value == null)
            {
                color = Color.Transparent;
                txt.text = "";
            }
            else
            {
                color = (Color)value;
                MyColorChanged(color);
            }
        }

        public override void Hotkey(bool invert)
        {
            if (invert)
                btn.Press(MouseButton.Right);
            else
                btn.Press(MouseButton.Left);
        }
    }
}
