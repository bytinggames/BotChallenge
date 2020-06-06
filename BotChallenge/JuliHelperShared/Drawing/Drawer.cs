using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public static class Drawer
    {
        public static Color baseTextColor;
        public static char openCode = '<';
        public static char closeCode = '>';
        
        static readonly Color white = Color.White;

        public static DepthLayer depth;
        public static float roundPositionTo = 1f;

        public static SpriteBatch batch;
        public static GraphicsDevice gDevice => batch.GraphicsDevice;

        private static Texture2D pixel;
        private static Texture2D Pixel => pixel;

        public static Line outline;
        public static Line underline;

        private static int fat;

        private static float textAlpha = 1f;
        private static Color textLerpColor;
        private static float textLerp = 0f;

        public class Line
        {
            public Color color = Color.Black;
            public float thickness = 1f;
        }

        public static void TextOutline(Color _outlineColor, Action _action)
        {
            Line previousOutline = outline;
            outline = new Line() { color = _outlineColor };
            _action();
            outline = previousOutline;
        }

        public static void TextOutline(Color _outlineColor, float _thickness, Action _action)
        {
            Line previousOutline = outline;
            outline = new Line() { color = _outlineColor, thickness = _thickness };
            _action();
            outline = previousOutline;
        }

        public static void TextUnderline(Color _underlineColor, Action _action)
        {
            Line previousUnderline = underline;
            underline = new Line() { color = _underlineColor };
            _action();
            underline = previousUnderline;
        }
        public static void TextUnderline(Color _underlineColor, float _thickness, Action _action)
        {
            Line previousUnderline = underline;
            underline = new Line() { color = _underlineColor, thickness = _thickness };
            _action();
            underline = previousUnderline;
        }

        public static void TextFat(this SpriteFont _font, Action _action) => _font.TextFat(1, _action);
        public static void TextFat(this SpriteFont _font, int _fat, Action _action)
        {
            fat = _fat;
            _font.Spacing += _fat;
            
            _action();

            _font.Spacing -= _fat;
        }
        public static void TextSetFat(this SpriteFont _font, int _fat)
        {
            float relativeFat = _fat - fat;
            _font.Spacing += relativeFat;
            fat = _fat;
        }

        public static void TextAlphaRelative(float _alpha, Action action)
        {
            float a = textAlpha;
            textAlpha *= _alpha;
            action();
            textAlpha = a;
        }
        public static void TextLerpSetRelative(Color _color, float _lerp, Action action)
        {
            float l = textLerp;
            Color c = textLerpColor;

            if (textLerp == 0)
            {
                textLerpColor = _color;
                textLerp = _lerp;
            }
            else
            {
                textLerpColor = Color.Lerp(textLerpColor, _color, _lerp);
                textLerp += (1f - textLerp) * _lerp;
            }
            action();


            textLerpColor = c;
            textLerp = l;
        }

        public static float GetTextAlpha() => textAlpha;
        
        public static Color GetRelativeColor(Color _color)
        {
            return (textLerp == 0f ? _color : Color.Lerp(_color, textLerpColor, textLerp)) * textAlpha;
        }


        public static void Initialize(SpriteBatch _spriteBatch)
        {
            batch = _spriteBatch;
            depth = new DepthLayer(0);
            
            pixel = new Texture2D(_spriteBatch.GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
        }

        #region Texture

        public static void Draw(this Texture2D _texture, Vector2 _position, Color? _color = null, Rectangle? _sourceRectangle = null, Vector2? _scale = null, float _rotation = 0f, SpriteEffects _effects = SpriteEffects.None)
        {
            batch.Draw(_texture, _position, _sourceRectangle, _color ?? Color.White, _rotation, Vector2.Zero, _scale ?? Vector2.One, _effects, depth);
        }

        public static void Draw(this Texture2D _texture, Anchor _anchor, Color? _color = null, Rectangle? _sourceRectangle = null, Vector2? _scale = null, float _rotation = 0f, SpriteEffects _effects = SpriteEffects.None)
        {
            Vector2 size = _sourceRectangle != null ? _sourceRectangle.Value.GetSize() : _texture.GetSize();

            Vector2 pos = _anchor.pos;

            if (roundPositionTo != 0) // TODO: improve this correction, by calculating the anchor by yourself
            {
                if (_rotation == 0)
                {
                    Vector2 shift = _anchor.origin * size * (_scale ?? Vector2.One);

                    Vector2 drawPos = _anchor.pos - shift;

                    if (roundPositionTo == 1f)
                    {
                        drawPos = drawPos.RoundVectorCustom();
                    }
                    else
                    {
                        drawPos = (drawPos / roundPositionTo).RoundVectorCustom() * roundPositionTo;
                    }

                    pos = drawPos + shift;
                }
            }


            batch.Draw(_texture, pos, _sourceRectangle, _color ?? Color.White, _rotation, _anchor.origin * size, _scale ?? Vector2.One, _effects, depth);
        }
        public static void Draw(this Texture2D _texture, Rectangle _rectangle, Color? _color = null, Rectangle? _sourceRectangle = null, float _rotation = 0f, SpriteEffects _effects = SpriteEffects.None)
        {
            batch.Draw(_texture, _rectangle, _sourceRectangle, _color ?? Color.White, _rotation, Vector2.Zero, _effects, depth);
        }

        public static void SetBaseTextColor(Color _color, Action _action)
        {
            Color remember = baseTextColor;
            baseTextColor = _color;
            _action();
            baseTextColor = remember;
        }

        public static void Draw(this Texture2D _texture, M_Rectangle _rectangle, Color? _color = null, Rectangle? _sourceRectangle = null, float _rotation = 0f, SpriteEffects _effects = SpriteEffects.None)
        {
            Vector2 scale = _rectangle.size / _texture.GetSize();
            _texture.Draw(_rectangle.pos, _color, _sourceRectangle, scale, _rotation, _effects);
        }

        #endregion

        #region String
        
        public static void Draw(this SpriteFont _font, string _text, Vector2 _position, Color? _color = null, Vector2? _scale = null, float _rotation = 0f, SpriteEffects _effects = SpriteEffects.None)
        {
            Draw(_font, _text, new Anchor(_position, Vector2.Zero), _color, _scale, _rotation, _effects);
        }
        public static void Draw(this SpriteFont _font, string _text, Anchor _anchor, Color? _color = null, Vector2? _scale = null, float _rotation = 0f, SpriteEffects _effects = SpriteEffects.None)
        {
            _font.DrawFinal(_text, _anchor, _color, _scale, _rotation, _effects);
        }
        private static void DrawFinal(this SpriteFont _font, string _text, Anchor _anchor, Color? _color = null, Vector2? _scale = null, float _rotation = 0f, SpriteEffects _effects = SpriteEffects.None)
        {
            Vector2 scale = _scale ?? Vector2.One;

            Vector2 textSize = Vector2.Zero;
            if (_anchor.origin != Vector2.Zero || underline != null)
                textSize = _font.MeasureString(_text);

            Vector2 origin = _anchor.origin * textSize;
            textSize *= scale;

            Vector2 pos = _anchor.pos;

            if (roundPositionTo != 0)
            {
                if (_rotation == 0)
                {
                    Vector2 drawPos = pos - origin * scale;

                    if (roundPositionTo == 1f)
                    {
                        drawPos = drawPos.RoundVector();
                    }
                    else
                    {
                        drawPos = (drawPos / roundPositionTo).RoundVector() * roundPositionTo;
                    }

                    pos = drawPos + origin * scale;
                }
            }

            if (outline != null)
            {
                Color c = GetRelativeColor(outline.color);
                batch.DrawString(_font, _text, pos + new Vector2(outline.thickness, 0), c, _rotation, origin, scale, _effects, depth);
                batch.DrawString(_font, _text, pos + new Vector2(0, -outline.thickness), c, _rotation, origin, scale, _effects, depth);
                batch.DrawString(_font, _text, pos + new Vector2(-outline.thickness, 0), c, _rotation, origin, scale, _effects, depth);
                batch.DrawString(_font, _text, pos + new Vector2(0, outline.thickness), c, _rotation, origin, scale, _effects, depth);
            }

            Color color = _color ?? Color.White;

            color = GetRelativeColor(color);
            
            if (fat > 0)
            {
                for (int i = 0; i < fat; i++)
                {
                    batch.DrawString(_font, _text, pos + new Vector2(i + 1, 0), color, _rotation, origin, scale, _effects, depth);
                }
            }

            if (underline != null)
            {
                pixel.Draw(new M_Rectangle(pos.X, pos.Y + textSize.Y - 3f, textSize.X, underline.thickness), GetRelativeColor(underline.color));
            }

            batch.DrawString(_font, _text, pos, color, _rotation, origin, scale, _effects, depth);
        }

        public static void DrawCoded(this SpriteFont _font, string _text, Anchor _anchor, float align = 0f, Color? _color = null, Vector2? _scale = null, float _rotation = 0f, SpriteEffects _effects = SpriteEffects.None)
        {
            Vector2 size = _font.MeasureStringCoded(_text);
            Vector2 pos = _anchor.pos - size * (_anchor.origin - new Vector2(align, 0));
            _font.DrawCoded(_text, pos, align, _color, _scale, _rotation, _effects);
        }
        public static void DrawCoded(this SpriteFont _font, string _text, Vector2 _position, float align = 0f, Color? _color = null, Vector2? _scale = null, float _rotation = 0f, SpriteEffects _effects = SpriteEffects.None)
        {
            Line outlineSave = outline;
            Line underlineSave = underline;

            if (_color == null)
                RealDraw();
            else
                SetBaseTextColor(_color.Value, RealDraw);

            outline = outlineSave;
            underline = underlineSave;

            void RealDraw()
            {
                _scale = _scale ?? Vector2.One;

                float left = _position.X;

                string[] lines = _text.Split(new char[] { '\n' });

                Color color = baseTextColor;

                for (int l = 0; l < lines.Length; l++)
                {
                    Vector2 size = _font.MeasureStringCoded(lines[l]) * _scale.Value;
                    _position.X = left - size.X * align;


                    string[] split = lines[l].Split(new char[] { openCode });

                    int i = 0;
                    while (true)
                    {
                        string codeless = RemoveCodes(split[i]);
                        if (codeless != "")
                        {
                            Vector2 cSize = _font.MeasureString(codeless) * _scale.Value;

                            DrawCodedPart(batch, _font, split[i], _position, color, depth, _scale.Value);

                            _position.X += cSize.X;
                        }

                        i++;

                        if (i < split.Length)
                        {
                            if (split[i].Length >= 2)
                            {
                                char command;
                                bool closeCommand;
                                if (split[i][0] == '/')
                                {
                                    command = split[i][1];
                                    closeCommand = true;
                                }
                                else
                                {
                                    command = split[i][0];
                                    closeCommand = false;
                                }

                                switch (command)
                                {
                                    case 'c': //color
                                        if (!closeCommand)
                                        {
                                            if (split[i][2] == '>')
                                                color = Calculate.HexToColor(split[i].Substring(1, 1));
                                            else if (split[i][4] == '>')
                                                color = Calculate.HexToColor(split[i].Substring(1, 3));
                                            else
                                                color = Calculate.HexToColor(split[i].Substring(1, 6));
                                        }
                                        else
                                            color = baseTextColor;
                                        break;
                                    case 'b'://background
                                        if (!closeCommand)
                                        {
                                            outline = new Line();
                                            if (split[i][1] != '>')
                                            {
                                                if (split[i][2] == '>')
                                                    outline.color = Calculate.HexToColor(split[i].Substring(1, 1));
                                                else if (split[i][4] == '>')
                                                    outline.color = Calculate.HexToColor(split[i].Substring(1, 3));
                                                else
                                                    outline.color = Calculate.HexToColor(split[i].Substring(1, 6));
                                            }
                                            else
                                                outline.color = baseTextColor;
                                        }
                                        else
                                            outline = null;
                                        break;
                                    case 'u'://background
                                        if (!closeCommand)
                                        {
                                            underline = new Line();
                                            if (split[i][1] != '>')
                                            {
                                                if (split[i][2] == '>')
                                                    underline.color = Calculate.HexToColor(split[i].Substring(1, 1));
                                                else if (split[i][4] == '>')
                                                    underline.color = Calculate.HexToColor(split[i].Substring(1, 3));
                                                else
                                                    underline.color = Calculate.HexToColor(split[i].Substring(1, 6));
                                            }
                                            else
                                                underline.color = baseTextColor;
                                        }
                                        else
                                            underline = null;
                                        break;
                                    //case 'a': //alignment
                                    //    if (!closeCommand)
                                    //    {
                                    //        align = split[i][1] == '0' ?
                                    //            (split[i][2] == '>' ? 0 : (split[i][2] == '.' && split[i][3] == '5' ? 1 : 0))
                                    //            : split[i][1] == '1' ? 1 : 0;
                                    //    }
                                    //    else
                                    //        align = 0;
                                    //    break;
                                    case 'p':

                                        if (split[i].Substring(1, 2) == "os")
                                        {
                                            if (!closeCommand)
                                            {
                                                //position
                                                float p;
                                                if (float.TryParse(split[i].Substring(3, split[i].IndexOf(closeCode) - 3), NumberStyles.Any, CultureInfo.InvariantCulture, out p))
                                                    _position.X = left + p;
                                            }
                                            else
                                                _position.X = left;
                                        }

                                        break;
                                    case 't':
                                        int strEnd = split[i].IndexOf('>', 1);


                                        float[] offsets = new float[3];// xOffset, yOffset, xOffsetAfter;
                                        int[] cuts = new int[4];// rectangle;

                                        int comma = 0;
                                        int commaCount = 0;
                                        bool paramsCut = false; // false = offset, true = cut rectangle out of texture

                                        Color c = Color.White;

                                        if (i + 1 < split[i].Length)
                                            if (split[i][1] == 'c')
                                            {
                                                comma++;
                                                c = color;
                                            }

                                        for (int k = comma + 1; k < strEnd; k++)
                                        {
                                            switch (split[i][k])
                                            {
                                                case ',':
                                                case '=':
                                                    if (k > comma+ 1)
                                                    {
                                                        if (!paramsCut)
                                                            offsets[commaCount] = float.Parse(split[i].Substring(comma + 1, k - comma - 1), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
                                                        else
                                                            cuts[commaCount] = int.Parse(split[i].Substring(comma + 1, k - comma - 1));
                                                        comma = k;
                                                        commaCount++;
                                                    }

                                                    if (split[i][k] == '=')
                                                    {
                                                        string name = split[i].Substring(k + 1, strEnd - k - 1);
                                                        Texture2D tex = ContentLoader.textures[name];

                                                        c = GetRelativeColor(c);

                                                        tex.Draw((_position + new Vector2(offsets[0], (_font.LineSpacing - tex.Height) / 2f + offsets[1])).RoundVectorCustom()
                                                            , c, (!paramsCut ? (Rectangle?)null : new Rectangle(cuts[0], cuts[1], cuts[2], cuts[3])));
                                                        _position.X += (paramsCut ? cuts[2] : tex.Width) + offsets[0] + offsets[2];
                                                    }
                                                    break;
                                                case '|':
                                                    paramsCut = true;
                                                    commaCount = 0;
                                                    comma++;
                                                    break;
                                            }
                                        }

                                        break;
                                    case 'f':
                                        if (!closeCommand)
                                        {
                                            _font.TextSetFat(1);
                                        }
                                        else
                                        {
                                            _font.TextSetFat(0);
                                        }
                                        break;
                                }

                                int j = split[i].IndexOf(closeCode);
                                if (j != -1)
                                    split[i] = split[i].Remove(0, j + 1);
                                else
                                    split[i] = "";
                            }
                        }
                        else
                            break;

                    }

                    _position.Y += _font.LineSpacing * _scale.Value.Y;
                }

                _font.TextSetFat(0);
            }
        }

        private static void DrawCodedPart(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 pos, Color color, float depth, Vector2 scale)
        {
            Vector2 size = font.MeasureString(text) * scale;

            font.DrawFinal(text, Anchor.TopLeft(pos), color, scale, 0f, SpriteEffects.None);
        }

        //private static void DrawCodedPart(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 pos, Color color, float depth, Color? backColor, int backThickness, int align, Vector2 scale)
        //{
        //    string[] split = text.Split(new char[] { '\n' });

        //    for (int i = 0; i < split.Length; i++)
        //    {
        //        text = split[i];
        //        Vector2 size = font.MeasureString(text) * scale;

        //        if (backColor == null)
        //            Draw();
        //        else
        //            OutlineText(backColor.Value, backThickness, Draw);

        //        void Draw() => font.DrawFinal(text, new Anchor((pos - new Vector2(size.X * (align * 0.5f), 0)).RoundVector(), Vector2.Zero), color, scale, 0f, SpriteEffects.None); 

        //        pos.Y += font.LineSpacing * scale.Y;
        //    }
        //}

        //public static void DrawAligned(this SpriteFont _font, string _text, Anchor _anchor, float alignX, Color? _color = null, Vector2? _scale = null, float _rotation = 0f, SpriteEffects _effects = SpriteEffects.None)
        //{
        //    _scale = _scale ?? Vector2.One;

        //    Vector2 fullSize = _font.MeasureString(_text) * _scale.Value;
        //    M_Rectangle fullRect = _anchor.Rectangle(fullSize);
        //    Anchor perLine = new Anchor(fullRect.X + fullSize.X * alignX, fullRect.Y, alignX, 0);

        //    string[] split = _text.Split(new char[] { '\n' });
        //    for (int i = 0; i < split.Length; i++)
        //    {
        //        _font.DrawFinal(split[i], perLine, _color, _scale.Value, _rotation, _effects);
        //        perLine.Y += _font.LineSpacing;
        //    }
        //}

        public static Vector2 MeasureStringCoded(this SpriteFont _font, string _text)
        {
            return _font.MeasureString(RemoveCodes(_text));
        }

        public static string RemoveCodes(string text)
        {
            int i = 0;
            while (i < text.Length - 1 && (i = text.IndexOf(openCode, i)) != -1)
            {
                int j = text.IndexOf(closeCode, i + 1);

                if (j == -1)
                    return text.Remove(i);

                bool texture = text.Length > i + 1 && text[i + 1] == 't';

                if (texture)
                {
                    int index = text.IndexOf('|', i, j - i);
                    int texWidth;
                    if (index == -1)
                    {
                        texWidth = 12; //just assume this
                    }
                    else
                    {
                        int commaCount = 0;
                        while (++index < j && commaCount < 2)
                        {
                            if (text[index] == ',')
                                commaCount++;
                        }

                        int nextComma = text.IndexOf(',', index, j - index);
                        if (nextComma == -1)
                            throw new Exception("comma missing");

                        texWidth = int.Parse(text.Substring(index, nextComma - index));
                    }
                    string add = new string('_', (int)Math.Ceiling(texWidth / 5f));
                    text = text.Insert(i, add);
                    i += add.Length;
                    j += add.Length;
                }
                else
                {
                    if (text.Length > i + 1 && text[i + 1] == 'f' && text[i + 2] == '>')
                    {
                        // TODO: support line breaks... or just make a better coded string draw function in general (with glyphs or so)
                        int closeIndex = text.IndexOf("</f>", i + 3);
                        int letters = closeIndex - (i + 3);

                        string add = new string('.', (int)Math.Ceiling(letters / 3f));
                        text = text.Insert(i, add);
                        i += add.Length;
                        j += add.Length;
                    }
                }

                if (j == -1)
                    text = text.Remove(i);
                else
                    text = text.Remove(i, j - i + 1);
            }

            return text;
        }

        //public static string SplitStringByWidth(this SpriteFont _font, string _text, float _width)
        //{
        //    string finalString = "";
        //    string insideWidth = "";

        //    for (int i = 0; i < _text.Length; i++)
        //    {
        //        insideWidth += _text[i];
        //        if (_font.MeasureString(insideWidth).X > _width)
        //        {
        //            finalString += insideWidth.Remove(insideWidth.Length - 1) + (i < _text.Length - 1 ? "\n" : "");
        //            insideWidth = insideWidth.Substring(insideWidth.Length - 1);
        //        }
        //    }
        //    if (insideWidth != "")
        //        finalString += insideWidth;

        //    return finalString;
        //}


        public static string SplitStringBySize(this SpriteFont font, string text, Vector2 size, bool coded = false, Vector2? scale = null)
        {
            float paddingX = 3;
            float paddingY = 0;
            Vector2 space = new Vector2(size.X - paddingX * 2, size.Y - paddingY * 2);
            if (space.X == -1)
                space.X = float.PositiveInfinity;
            if (space.Y == -1)
                space.Y = float.PositiveInfinity;

            List<string> lines = new List<string>();
            float lineHeightSum = 0;

            bool tooLong = false;

            if (!scale.HasValue)
                scale = Vector2.One;

            Func<string, Vector2> measureString;
            if (coded)
                measureString = font.MeasureStringCoded;
            else
                measureString = font.MeasureString;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    lines.Add(text.Substring(0, i));
                    text = text.Substring(i + 1);
                    i = 0;
                }
                else if (measureString(text.Substring(0, i)).X * scale.Value.X > space.X)
                {
                    int spaceIndex = text.LastIndexOf(' ', i, i);
                    if (spaceIndex != -1)
                    {
                        string sub = text.Substring(0, spaceIndex);
                        lines.Add(sub);
                        text = text.Substring(spaceIndex + 1);
                    }
                    else
                    {
                        if (i <= 1)
                        {
                            tooLong = true;
                            break;
                        }
                        string sub = text.Substring(0, i - 2);
                        lines.Add(sub + "-");
                        text = text.Substring(sub.Length);
                    }
                    i = 0;
                }

                if (i == 0 && lines.Count > 0)
                {
                    float lineHeight = measureString(lines[lines.Count - 1]).Y * scale.Value.X;

                    if (lineHeightSum + lineHeight > space.Y)
                    {
                        lines.RemoveAt(lines.Count - 1);
                        tooLong = true;
                        break;
                    }
                    else
                        lineHeightSum += lineHeight;
                }
            }
            if (!tooLong)
            {
                lines.Add(text);
                float lineHeight = measureString(lines[lines.Count - 1]).Y * scale.Value.X;
                if (lineHeightSum + lineHeight > space.Y)
                {
                    lines.RemoveAt(lines.Count - 1);
                    tooLong = true;
                }
                else
                    lineHeightSum += lineHeight;
            }


            if (tooLong)
            {
                if (lines.Count == 0)
                {
                    lines.Add("...");
                    lineHeightSum += measureString(lines[lines.Count - 1]).Y * scale.Value.X;
                }
                else
                    lines[lines.Count - 1] = lines[lines.Count - 1].Remove(Math.Max(0, lines[lines.Count - 1].Length - 3)) + "...";
            }

            //Vector2 pos = rect.GetCenter() + new Vector2(0, -lineHeightSum / 2f);

            string output = "";
            for (int i = 0; i < lines.Count; i++)
            {
                output += lines[i] + (i < lines.Count - 1 ? "\n" : "");
            }

            return output;
        }

        #endregion

        #region Rectangle

        public static void Draw(this M_Rectangle _rect, Color _color)
        {
            batch.Draw(pixel, _rect.pos, new Rectangle(0, 0, 1, 1), _color, 0f, Vector2.Zero, _rect.size, SpriteEffects.None, depth);
        }
        public static void DrawOutline(this M_Rectangle _rect, Color _color, float _thickness = 1f)
        {
            float t = _thickness / 2f;
            if (_rect.size.X <= _thickness || _rect.size.Y <= _thickness)
                new M_Rectangle(_rect.pos - new Vector2(t), _rect.size + new Vector2(_thickness)).Draw(_color);
            else
            {
                batch.Draw(pixel, _rect.pos + new Vector2(t, -t), null, _color, 0f, Vector2.Zero, new Vector2(_rect.size.X, _thickness), SpriteEffects.None, depth);
                batch.Draw(pixel, _rect.pos + new Vector2(_rect.size.X - t, t), null, _color, 0f, Vector2.Zero, new Vector2(_thickness, _rect.size.Y), SpriteEffects.None, depth);
                batch.Draw(pixel, _rect.pos + new Vector2(-t, _rect.size.Y - t), null, _color, 0f, Vector2.Zero, new Vector2(_rect.size.X, _thickness), SpriteEffects.None, depth);
                batch.Draw(pixel, _rect.pos + new Vector2(-t), null, _color, 0f, Vector2.Zero, new Vector2(_thickness, _rect.size.Y), SpriteEffects.None, depth);
            }
        }
        public static void DrawOutlineInside(this M_Rectangle _rect, Color _color, float _thickness = 1f)
        {
            if (_rect.size.X <= _thickness * 2f || _rect.size.Y <= _thickness * 2f)
                _rect.Draw(_color);
            else
            {
                batch.Draw(pixel, _rect.pos + new Vector2(_thickness, 0), null, _color, 0f, Vector2.Zero, new Vector2(_rect.size.X - _thickness, _thickness), SpriteEffects.None, depth);
                batch.Draw(pixel, _rect.pos + new Vector2(_rect.size.X - _thickness, _thickness), null, _color, 0f, Vector2.Zero, new Vector2(_thickness, _rect.size.Y - _thickness), SpriteEffects.None, depth);
                batch.Draw(pixel, _rect.pos + new Vector2(0, _rect.size.Y - _thickness), null, _color, 0f, Vector2.Zero, new Vector2(_rect.size.X - _thickness, _thickness), SpriteEffects.None, depth);
                batch.Draw(pixel, _rect.pos, null, _color, 0f, Vector2.Zero, new Vector2(_thickness, _rect.size.Y - _thickness), SpriteEffects.None, depth);
            }
        }
        public static void DrawOutlineOutside(this M_Rectangle _rect, Color _color, float _thickness = 1f)
        {
            batch.Draw(pixel, _rect.pos + new Vector2(0, -_thickness), null, _color, 0f, Vector2.Zero, new Vector2(_rect.size.X + _thickness, _thickness), SpriteEffects.None, depth);
            batch.Draw(pixel, _rect.pos + new Vector2(_rect.size.X, 0), null, _color, 0f, Vector2.Zero, new Vector2(_thickness, _rect.size.Y + _thickness), SpriteEffects.None, depth);
            batch.Draw(pixel, _rect.pos + new Vector2(-_thickness, _rect.size.Y), null, _color, 0f, Vector2.Zero, new Vector2(_rect.size.X + _thickness, _thickness), SpriteEffects.None, depth);
            batch.Draw(pixel, _rect.pos + new Vector2(-_thickness), null, _color, 0f, Vector2.Zero, new Vector2(_thickness, _rect.size.Y + _thickness), SpriteEffects.None, depth);
        }

        #endregion

        #region Line

        public static void DrawLine(Vector2 _pos1, Vector2 _pos2, Color _color, float _thickness = 1f) => DrawLineRelative(_pos1, _pos2 - _pos1, _color, _thickness);
        public static void DrawLineRelative(Vector2 _pos, Vector2 _size, Color _color, float _thickness = 1f)
        {
            float angle = (float)Math.Atan2(_size.Y, _size.X);
            batch.Draw(pixel, _pos, new Rectangle(0, 0, 1, 1), _color, angle, new Vector2(0, 0.5f), new Vector2(_size.Length(), _thickness), SpriteEffects.None, depth);
        }

        #endregion

        #region Generate

        public static Texture2D GenerateMaskTexture(Mask mask, Color? color = null)
        {
            if (color == null)
                color = Color.White;

            Texture2D tex = new Texture2D(gDevice, (int)Math.Ceiling(mask.GetSize().X), (int)Math.Ceiling(mask.GetSize().Y));
            Vector2 topLeft = mask.GetRectangle().pos - mask.pos;

            Color[] data = new Color[tex.Width * tex.Height];

            for (int x = 0; x < tex.Width; x++)
            {
                for (int y = 0; y < tex.Height; y++)
                {
                    if (mask.ColVector(new Vector2(x, y) + new Vector2(0.5f) + topLeft))
                        data[y * tex.Width + x] = color.Value;
                }
            }

            tex.SetData(data);
            return tex;
        }

        #endregion

    }
}
