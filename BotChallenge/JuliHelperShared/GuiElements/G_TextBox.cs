using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuliHelper;

namespace JuliHelper.Gui
{
    public class G_TextBox : GuiElement
    {
        const int SPACE = 2;

        public Color colorNormal, colorHover, colorPressed, colorHoverPressed, colorText;

        private string _text;
        private string drawText;

        public string text
        {
            get { return _text; }
            set
            {
                _text = value;

                UpdateDrawText();
            }
        }
        private SpriteFont _font;
        public SpriteFont font
        {
            get { return _font; }
            set
            {
                _font = value;

                UpdateDrawText();
            }
        }

        public override Vector2 size
        {
            get
            {
                return base.size;
            }

            set
            {
                base.size = value;

                UpdateDrawText();
            }
        }

        public bool pressed;
        private bool setCursor;

        public KeyCollection keyCollection;
        public int maxLength;

        public char? replaceChar = null;

        public Action PressRight;

        public G_TextBox(M_Rectangle area, Action<object> ValueChanged = null, KeyCollection keyCollection = null, int maxLength = -1, float depth = 0f)
        {
            Initialize();

            this.posL = area.pos;
            this.size = area.size;
            this.ValueChanged = ValueChanged;
            this.keyCollection = keyCollection;
            this.maxLength = maxLength;
            this.depth = depth;
        }

        private void Initialize()
        {
            font = GuiElement.fontStd;

            colorText = Color.Black;
            colorNormal = G_Button.colors[0];
            colorHover = G_Button.colors[1];
            colorPressed = G_Button.colors[2];
            colorHoverPressed = G_Button.colors[3];

            text = "";
        }

        public override void Update()
        {
            if (visible)
            {
                UpdateBegin();

                if (pressed)
                {
                    if (!hover && Input.mbLeft._pressed)
                        TextEntered(KeyString.keyString);
                    else
                    {
                        if (hover && Input.mbLeft._pressed)
                            setCursor = true;

                        if (Input.mbLeft._released)
                            setCursor = false;

                        if (setCursor)
                            KeyString.UpdateMouse(mbPos - posG, font);
                        text = KeyString.keyString;
                    }
                }
                else if (hover)
                {
                    if (Input.mbLeft.pressed)
                        Enter();
                    else if (Input.mbRight.pressed && PressRight != null)
                        PressRight();
                }
            }
        }

        public void Enter()
        {
            //enter type mode
            pressed = true;

            KeyString.StartRecording(TextEntered, text, true, -1, keyCollection, maxLength);
            KeyString.KeyStringEscaped = TextEscaped;

            SetCatchUpdate(Update);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (visible)
            {
                Color color;

                if (hover)
                {
                    if (pressed)
                        color = colorHoverPressed;
                    else
                        color = colorHover;
                }
                else
                {
                    if (pressed)
                        color = colorPressed;
                    else
                        color = colorNormal;
                }

                M_Rectangle area = GetRectangle();
                DrawM.Sprite.DrawRectangle(spriteBatch, area, color, depth);

                if (pressed && KeyString.Record)
                    KeyString.Draw(spriteBatch, font, posG + new Vector2(SPACE, size.Y / 2f), colorText, depth + 0.01f, true, replaceChar);
                else
                {
                    spriteBatch.DrawString(font, drawText, posG + new Vector2(SPACE, (int)Math.Round((size.Y - font.MeasureString(drawText).Y) / 2f)), colorText, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth + 0.01f);
                }

                DrawEnd(spriteBatch);
            }
        }

        public void TextEntered(string text)
        {
            this.text = text;
            pressed = false;

            if (ValueChanged != null)
                ValueChanged(text);

            KeyString.StopRecording(true);

            SetCatchUpdate(null);
        }

        public void TextEscaped(string text)
        {
            this.text = text;
            pressed = false;

            KeyString.StopRecording(true);

            SetCatchUpdate(null);
        }

        public override GuiElement Clone(GuiElement parent)
        {
            return (GuiElement)CloneChild(parent);
        }

        public G_TextBox CloneChild(GuiElement parent)
        {
            G_TextBox clone = (G_TextBox)base.Clone(parent);
            //clone.TextChanged = null;

            if (ValueChanged != null)
                clone.ValueChanged = (Action<object>)ValueChanged.Clone();
            if (PressRight != null)
                clone.PressRight = (Action)PressRight.Clone();
            return clone;
        }

        public override void SetValue(object value, Type type)
        {
            if (value == null)
                text = "";
            else
                text = MyConverter.ToString(value);
                //text = (string)value.ToString();
        }

        private void UpdateDrawText()
        {
            if (_text != null)
            {
                if (size.X == 0 || _text == "")
                    drawText = "";
                else
                {
                if (replaceChar == null)
                    drawText = _text;
                else
                    drawText = new string(replaceChar.Value, _text.Length);

                    if (font != null)// && drawText.Length > 0)
                    {
                        float width = font.MeasureString(drawText).X + SPACE * 2;
                        if (width > size.X)
                        {
                            float stdWidth = font.MeasureString("...").X + SPACE * 2;

                            float width2;
                            int pos = (int)Math.Min(((size.X / width) * text.Length), drawText.Length);
                            bool larger = font.MeasureString(drawText.Substring(0, pos)).X + stdWidth > size.X;
                            do
                            {
                                pos += larger ? -1 : 1;
                            } while (pos > 0 && pos < drawText.Length && larger == ((width2 = font.MeasureString(drawText.Substring(0, pos)).X + stdWidth) > size.X));

                            if (!larger && pos < drawText.Length)
                                pos--;

                            if (pos < 0)
                                pos = 0;

                            drawText = drawText.Substring(0, pos) + "...";
                        }
                    }
                }
            }
        }
    }
}
