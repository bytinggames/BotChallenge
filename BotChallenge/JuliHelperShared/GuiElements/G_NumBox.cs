using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;
using System.Globalization;

namespace JuliHelper.Gui
{
    public class G_NumBox : GuiElement
    {
        public decimal? value, min, max;
        public decimal decimalPlaces, increment;

        public bool modulo;

        public G_B_Sprite btn;
        public G_TextBox txt;

        public G_NumBox(M_Rectangle area, Action<object> ValueChanged = null, decimal increment = 1, decimal? min = null, decimal? max = null, int decimalPlaces = 0, bool modulo = true, float depth = 0f)
        {
            this.posL = area.pos;
            this.size = area.size;
            this.increment = increment;
            this.min = min;
            this.max = max;
            this.decimalPlaces = decimalPlaces;
            this.modulo = modulo;
            this.depth = depth;

            Initialize();

            this.ValueChanged += ValueChanged;
        }

        private void Initialize()
        {
            value = null;

            btn = new G_B_Sprite(new M_Rectangle(new Vector2(size.X - size.Y, 0), new Vector2(size.Y)),
                new Sprite(Vector2.Zero, ContentLoader.textures["btn_edit"])
                , BtnPressLeft, false, depth) { parent = this };
            btn.PressRight += BtnPressRight;

            ValueChanged = MyValueChanged;

            txt = new G_TextBox(new M_Rectangle(new Vector2(0, 0), new Vector2(size.X - size.Y, size.Y)), TextChanged, new KeyCollection().AddNumbers().AddPlusMinus(), -1, depth) { parent = this };

            if (decimalPlaces > 0)
                txt.keyCollection = txt.keyCollection.Add(new KeyP[] { Input.dot });
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
        public G_NumBox CloneChild(GuiElement parent)
        {
            G_NumBox clone = (G_NumBox)base.Clone(parent);
            //clone.Initialize();

            clone.btn = btn.CloneChild(clone);
            clone.btn.PressLeft = clone.BtnPressLeft;
            clone.btn.PressRight = clone.BtnPressRight;

            clone.txt = txt.CloneChild(clone);
            clone.txt.ValueChanged = clone.TextChanged;

            clone.ValueChanged = clone.MyValueChanged;
            return clone;
        }

        private void TextChanged(object text)
        {
            decimal newValue;
            if (MyConverter.TryParseCatch<decimal>((string)text, out newValue))
            {
                value = newValue;
                MinMax();
                if (ValueChanged != null)
                    ValueChanged(value);
            }
            else
                txt.text = value.ToString();
        }

        private void BtnPressLeft(G_Button btn)
        {
            if (value == null)
                value = 0;
            else
                value += increment;
            MinMax();

            if (ValueChanged != null)
                ValueChanged(value);
        }
        private void BtnPressRight(G_Button btn)
        {
            if (value == null)
                value = 0;
            else
                value -= increment;
            MinMax();

            if (ValueChanged != null)
                ValueChanged(value);
        }

        private void MyValueChanged(object value)
        {
            txt.text = value.ToString().Replace(',', '.');
        }

        private void MinMax()
        {
            if (value.HasValue)
            {
                if (modulo)
                {
                    if (min != null && value < min.Value)
                    {
                        decimal dist = min.Value - value.Value;
                        dist %= max.Value - min.Value;
                        value = max - dist;
                    }
                    else if (max != null && value >= max.Value)
                    {
                        decimal dist = value.Value - max.Value;
                        dist %= max.Value - min.Value;
                        value = min + dist;
                    }
                }
                else
                {
                    if (min != null && value.Value < min.Value)
                        value = min;
                    else if (max != null && value.Value > max.Value)
                        value = max;
                }
            }
        }

        public override void SetValue(object value, Type type)
        {
            if (value == null)
            {
                txt.text = "";
            }
            else
            {
                this.value = (decimal)Convert.ChangeType(value, typeof(decimal), CultureInfo.InvariantCulture);
                //this.value = (decimal)value;

                MyValueChanged(value);
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
