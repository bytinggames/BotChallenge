using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public static class KeyString
    {
        public static string keyString = "";
        private static bool record = false;
        public static bool Record { get { return record; } }
        public static Action<string> KeyStringEntered, KeyStringEscaped;
        public static bool enterOnEnter = true;
        public static string clipboard = "";
        public static string startString;
        public static int cursor;
        public static int marked;
        public static int maxLength;

        public static Color cursorColor = Color.Black, markColor = Color.Black * 0.5f;

        public static int frame = 0;

        public static int markedStart
        {
            get
            {
                if (marked < 0)
                    return cursor + marked;
                else
                    return cursor;
            }
        }

        internal static void Initialize()
        {
            keyString = "";
            record = false;
            KeyStringEntered = null;
            enterOnEnter = true;
            clipboard = "";
            cursor = 0;
            marked = 0;
        }

        public static void StartRecording(Action<string> TextEntered, string text = "", bool markAll = false, int _cursor = -1, KeyCollection keyCollection = null, int _maxLength = -1)
        {
            keyString = startString = text;
            record = true;
            KeyStringEntered = TextEntered;

            maxLength = _maxLength;
            if (maxLength != -1 && keyString.Length > maxLength)
                keyString = keyString.Remove(maxLength);

            if (markAll)
            {
                cursor = 0;
                marked = keyString.Length;
            }
            else
            {
                if (_cursor == -1)
                    cursor = keyString.Length;
                else
                    cursor = _cursor;
                marked = 0;
            }

            if (keyCollection != null)
            {
                Input.deCatchOnLoop = false;
                for (int i = 0; i < Input.keys.Length - 3; i++)
                {
                    Input.keys[i].catched = true;
                }

                keyCollection.Decatch();
            }
            frame = 0;
        }

        public static void StopRecording(bool deCatchOnLoop = true)
        {
            keyString = "";
            record = false;
            KeyStringEntered = null;

            Input.deCatchOnLoop = deCatchOnLoop;
        }

        public static void Write(string text)
        {
            if (marked != 0)
            {
                keyString = keyString.Remove(markedStart, Math.Abs(marked));
                cursor = markedStart;
                marked = 0;
            }

            if (maxLength != -1 && keyString.Length + text.Length > maxLength)
                text = text.Substring(0, maxLength - keyString.Length);

            keyString = keyString.Insert(cursor, text);
            cursor += text.Length;
        }

        public static void Delete(bool left)
        {
            if (marked != 0)
            {
                keyString = keyString.Remove(markedStart, Math.Abs(marked));
                cursor = markedStart;
                marked = 0;
            }
            else
            {
                if (left)
                {
                    if (cursor > 0)
                    {
                        keyString = keyString.Remove(cursor - 1, 1);
                        cursor--;
                    }
                }
                else
                {
                    if (cursor < keyString.Length)
                        keyString = keyString.Remove(cursor, 1);
                }
            }
        }

        public static void CursorMove(int move)
        {
            if (Input.leftShift._down || marked == 0)
            {
                int oldCursor = cursor;

                cursor += move;
                if (cursor < 0)
                    cursor = 0;
                if (cursor > keyString.Length)
                    cursor = keyString.Length;
                
                if (Input.leftShift._down)
                    marked -= cursor - oldCursor;
            }
            else
            {
                if (move < 0)
                    cursor = markedStart;
                else
                    cursor = markedStart + Math.Abs(marked);

                marked = 0;
            }
        }

        public static void Paste()
        {
            Write(clipboard);
        }

        public static void Copy()
        {
            if (marked != 0)
                clipboard = keyString.Substring(markedStart, Math.Abs(marked));
        }

        public static void Cut()
        {
            if (marked != 0)
            {
                clipboard = keyString.Substring(markedStart, Math.Abs(marked));

                keyString = keyString.Remove(markedStart, Math.Abs(marked));
                cursor = markedStart;
                marked = 0;
            }
        }

        public static void SelectAll()
        {
            cursor = 0;
            marked = keyString.Length;
        }

        public static void Escape()
        {
            keyString = startString;
            marked = 0;

            if (cursor > keyString.Length)
                cursor = keyString.Length;

            if (KeyStringEscaped != null)
                KeyStringEscaped(keyString);
            else if (KeyStringEntered != null)
                KeyStringEntered(KeyString.keyString);

            Input.esc.catched = true;
        }

        public static void ClearString()
        {
            keyString = "";
            marked = 0;

            if (cursor > keyString.Length)
                cursor = keyString.Length;
        }

        public static void UpdateMouse(Vector2 mbPosLocal, SpriteFont font)
        {
            if (Input.mbLeft._pressed || Input.mbLeft._down)
            {
                float x = mbPosLocal.X - font.MeasureString("@").X / 2f;
                float width = 0;
                int index = 0;
                float charWidth = 0;
                while (width < x && index < keyString.Length)
                {
                    index++;
                    charWidth = font.MeasureString(keyString[index - 1].ToString()).X;
                    width += charWidth;
                }

                if (index > 0 && width - charWidth / 2f >= x)
                    index--;

                if (Input.mbLeft._pressed)
                {
                    cursor = index;
                    marked = 0;
                }
                else if (Input.mbLeft._down)
                {
                    marked = index - cursor;
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 pos, Color color, float depth = 0, bool centerY = false, char? replaceChar = null, float fontScale = 1f)
        {
            string drawString = (replaceChar == null ? keyString : new string(replaceChar.Value, keyString.Length));

            Vector2 stringSize = font.MeasureString(drawString) * fontScale;

            if (stringSize.Y == 0)
                stringSize.Y = font.MeasureString("|").Y * fontScale;

            if (centerY)
                pos.Y -= stringSize.Y / 2f;

            pos = Calculate.RoundVector(pos);

            float xToCursor = font.MeasureString(drawString.Substring(0, cursor)).X * fontScale;
            float markWidth = font.MeasureString(drawString.Substring(markedStart, Math.Abs(marked))).X * fontScale;
            float xToMark = font.MeasureString(drawString.Substring(0, markedStart)).X * fontScale;

            DrawM.Sprite.DrawRectangle(spriteBatch, new M_Rectangle(pos.X + xToMark, pos.Y, markWidth, stringSize.Y), markColor, depth);
            spriteBatch.DrawString(font, drawString, pos, color, 0f, Vector2.Zero, fontScale, SpriteEffects.None, depth);

            if (frame % 60 < 30)
                DrawM.Sprite.DrawRectangle(spriteBatch, new M_Rectangle(pos.X + xToCursor - 1, pos.Y, 1, stringSize.Y), cursorColor, depth);

            frame++;
        }
    }
}
