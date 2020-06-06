using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public static class DrawM
    {
        public static Texture2D pixel;

        public static BasicEffect basicEffect;
        public static GraphicsDevice gDevice;

        public static Dictionary<int, Texture2D> circleTextures;
        public static Dictionary<Tuple<int, Color, Color>, Texture2D> circleTextures2;

        public static EffectPass currentPass;

        public static float scale = 1;

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            gDevice = graphicsDevice;

            pixel = new Texture2D(gDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });

            basicEffect = new BasicEffect(gDevice);
            basicEffect.VertexColorEnabled = true;
            ResizeWindow();

            circleTextures = new Dictionary<int, Texture2D>();
            circleTextures2 = new Dictionary<Tuple<int, Color, Color>, Texture2D>();

            Vertex.Initialize();
        }

        public static void ResizeWindow()
        {
            if (basicEffect != null && gDevice != null)
            {
                basicEffect.Projection = Matrix.CreateOrthographicOffCenter
                   (0, gDevice.Viewport.Width,     // left, right
                    gDevice.Viewport.Height, 0,    // bottom, top
                    0, 1);
            }                                       // near, far plane
        }

        public class Sprite
        {
            public static Color baseTextColor = Color.Black;
            public static char openCode = '<';
            public static char closeCode = '>';

            #region Rectangle

            public static void DrawRectangle(SpriteBatch spriteBatch, M_Rectangle rect, Color color, float depth = 0f)
            {
                spriteBatch.Draw(pixel, rect.pos, null, color, 0f, Vector2.Zero, rect.size, SpriteEffects.None, depth);
            }
            public static void DrawRectangleOutline(SpriteBatch spriteBatch, M_Rectangle rect, Color color, float thickness = 1f, float depth = 0f)
            {
                float t = thickness / 2f;
                if (rect.size.X <= thickness || rect.size.Y <= thickness)
                    DrawRectangle(spriteBatch, new M_Rectangle(rect.pos - new Vector2(t), rect.size + new Vector2(thickness)), color);
                else
                {
                    spriteBatch.Draw(pixel, rect.pos + new Vector2(t, -t), null, color, 0f, Vector2.Zero, new Vector2(rect.size.X, thickness), SpriteEffects.None, depth);
                    spriteBatch.Draw(pixel, rect.pos + new Vector2(rect.size.X - t, t), null, color, 0f, Vector2.Zero, new Vector2(thickness, rect.size.Y), SpriteEffects.None, depth);
                    spriteBatch.Draw(pixel, rect.pos + new Vector2(-t, rect.size.Y - t), null, color, 0f, Vector2.Zero, new Vector2(rect.size.X, thickness), SpriteEffects.None, depth);
                    spriteBatch.Draw(pixel, rect.pos + new Vector2(-t), null, color, 0f, Vector2.Zero, new Vector2(thickness, rect.size.Y), SpriteEffects.None, depth);
                }
            }
            public static void DrawRectangleOutlineInside(SpriteBatch spriteBatch, M_Rectangle rect, Color color, float thickness = 1f, float depth = 0f)
            {
                if (rect.size.X <= thickness * 2f || rect.size.Y <= thickness * 2f)
                    DrawRectangle(spriteBatch, rect, color);
                else
                {
                    spriteBatch.Draw(pixel, rect.pos + new Vector2(thickness, 0), null, color, 0f, Vector2.Zero, new Vector2(rect.size.X - thickness, thickness), SpriteEffects.None, depth);
                    spriteBatch.Draw(pixel, rect.pos + new Vector2(rect.size.X - thickness, thickness), null, color, 0f, Vector2.Zero, new Vector2(thickness, rect.size.Y - thickness), SpriteEffects.None, depth);
                    spriteBatch.Draw(pixel, rect.pos + new Vector2(0, rect.size.Y - thickness), null, color, 0f, Vector2.Zero, new Vector2(rect.size.X - thickness, thickness), SpriteEffects.None, depth);
                    spriteBatch.Draw(pixel, rect.pos, null, color, 0f, Vector2.Zero, new Vector2(thickness, rect.size.Y - thickness), SpriteEffects.None, depth);
                }
            }
            public static void DrawRectangleOutlineOutside(SpriteBatch spriteBatch, M_Rectangle rect, Color color, float thickness = 1f, float depth = 0f)
            {
                spriteBatch.Draw(pixel, rect.pos + new Vector2(0, -thickness), null, color, 0f, Vector2.Zero, new Vector2(rect.size.X + thickness, thickness), SpriteEffects.None, depth);
                spriteBatch.Draw(pixel, rect.pos + new Vector2(rect.size.X, 0), null, color, 0f, Vector2.Zero, new Vector2(thickness, rect.size.Y + thickness), SpriteEffects.None, depth);
                spriteBatch.Draw(pixel, rect.pos + new Vector2(-thickness, rect.size.Y), null, color, 0f, Vector2.Zero, new Vector2(rect.size.X + thickness, thickness), SpriteEffects.None, depth);
                spriteBatch.Draw(pixel, rect.pos + new Vector2(-thickness), null, color, 0f, Vector2.Zero, new Vector2(thickness, rect.size.Y + thickness), SpriteEffects.None, depth);
            }

            public static void DrawRectangleOutlineOutsideGradient(SpriteBatch spriteBatch, M_Rectangle rect, Color color1, Color color2, float thickness, float step = 1f, bool reachColor2 = true, float depth = 0f)
            {
                rect = (M_Rectangle)rect.Clone();
                float colorEnd = reachColor2 ? (thickness - step) : thickness;
                float i = 0;
                while (true)
                {
                    Color color = Color.Lerp(color1, color2, i / colorEnd);
                    DrawRectangleOutlineOutside(spriteBatch, rect, color, step, depth);

                    i += step;
                    if (i >= thickness)
                        break;

                    rect.pos -= new Vector2(step);
                    rect.size += new Vector2(step * 2f);
                }
            }
            public static void DrawRectangleOutlineInsideGradient(SpriteBatch spriteBatch, M_Rectangle rect, Color color1, Color color2, float thickness, float step = 1f, bool reachColor2 = true, float depth = 0f)
            {
                rect = (M_Rectangle)rect.Clone();
                float colorEnd = reachColor2 ? (thickness - step) : thickness;
                float i = 0;
                while (true)
                {
                    Color color = Color.Lerp(color1, color2, i / colorEnd);
                    DrawRectangleOutlineInside(spriteBatch, rect, color, step, depth);

                    i += step;
                    if (i >= thickness)
                        break;

                    rect.pos += new Vector2(step);
                    rect.size -= new Vector2(step * 2f);
                }
            }

            #endregion

            #region Other

            public static void DrawRectCross(SpriteBatch spriteBatch, M_Rectangle rect, Color color, float rectBorder, float a, float b, float depth = 0)
            {
                if (rect.size != Vector2.Zero)
                    DrawRectangleOutline(spriteBatch, rect, color, rectBorder, depth);
                else
                    DrawCross(spriteBatch, rect.pos, color, a, b, depth);
            }

            public static void DrawCross(SpriteBatch spriteBatch, Vector2 pos, Color color, float a, float b, float depth = 0)
            {
                DrawRectangle(spriteBatch, new M_Rectangle(pos - new Vector2(a, b) / 2f, new Vector2(a, b)), color, depth);
                DrawRectangle(spriteBatch, new M_Rectangle(pos - new Vector2(b, a) / 2f, new Vector2(b, a)), color, depth);
            }

            public static void DrawArrow(SpriteBatch spriteBatch, Vector2 pos, float length, float angle, Color color, float thickness = 1, float spaceToHead = 0, float depth = 0f)
            {
                Vector2 dir = -new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                DrawLineSize(spriteBatch, pos + dir * length * spaceToHead, dir * length * (1 - spaceToHead), color, thickness, depth);
                angle += MathHelper.PiOver4;
                length *= 0.2f;
                dir = -new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                DrawLineSize(spriteBatch, pos + dir * thickness * 0.5f, dir * (length - thickness * 0.5f), color, thickness, depth);
                angle -= MathHelper.PiOver2;
                dir = -new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                DrawLineSize(spriteBatch, pos - dir * thickness * 0.5f, dir * (length + thickness * 0.5f), color, thickness, depth);
            }

            public static void DrawBorders(SpriteBatch spriteBatch, bool[] boundaryEnabled, int[] boundary, Color color, int borderSize, Matrix matrix, int startX, int startY, int endX, int endY, float depth = 0)
            {
                Vector2 p, p1, p2;
                p1 = Vector2.Transform(new Vector2(0, boundary[2]), matrix);
                p2 = Vector2.Transform(new Vector2(0, boundary[3]), matrix);
                if (!boundaryEnabled[2])
                    p1.Y = startY;
                else
                    p1.Y -= borderSize;

                if (!boundaryEnabled[3])
                    p2.Y = endY;
                else
                    p2.Y += borderSize;

                if (boundaryEnabled[0])
                {
                    p = Vector2.Transform(new Vector2(boundary[0], 0), matrix);
                    DrawM.Sprite.DrawRectangle(spriteBatch, new M_Rectangle(p.X - borderSize, p1.Y, borderSize, p2.Y - p1.Y), color, depth);
                }
                if (boundaryEnabled[1])
                {
                    p = Vector2.Transform(new Vector2(boundary[1], 0), matrix);
                    DrawM.Sprite.DrawRectangle(spriteBatch, new M_Rectangle(p.X, p1.Y, borderSize, p2.Y - p1.Y), color, depth);
                }

                p1 = Vector2.Transform(new Vector2(boundary[0], 0), matrix);
                p2 = Vector2.Transform(new Vector2(boundary[1], 0), matrix);

                if (!boundaryEnabled[0])
                    p1.X = startX;
                if (!boundaryEnabled[1])
                    p2.X = endX;

                if (boundaryEnabled[2])
                {
                    p = Vector2.Transform(new Vector2(0, boundary[2]), matrix);
                    DrawM.Sprite.DrawRectangle(spriteBatch, new M_Rectangle(p1.X, p.Y - borderSize, p2.X - p1.X, borderSize), color, depth);
                }
                if (boundaryEnabled[3])
                {
                    p = Vector2.Transform(new Vector2(0, boundary[3]), matrix);
                    DrawM.Sprite.DrawRectangle(spriteBatch, new M_Rectangle(p1.X, p.Y, p2.X - p1.X, borderSize), color, depth);
                }
            }

            public static void DrawLine(SpriteBatch spriteBatch, Vector2 pos1, Vector2 pos2, Color color, float thickness = 1f, float depth = 0f)
            {
                Vector2 size = pos2 - pos1;
                float angle = (float)Math.Atan2(size.Y, size.X);
                spriteBatch.Draw(pixel, pos1, new Rectangle(0, 0, 1, 1), color, angle, new Vector2(0, 0.5f), new Vector2(size.Length(), thickness), SpriteEffects.None, depth);
            }
            public static void DrawLineSize(SpriteBatch spriteBatch, Vector2 pos, Vector2 size, Color color, float thickness = 1f, float depth = 0f)
            {
                float angle = (float)Math.Atan2(size.Y, size.X);
                spriteBatch.Draw(pixel, pos, new Rectangle(0, 0, 1, 1), color, angle, new Vector2(0, 0.5f), new Vector2(size.Length(), thickness), SpriteEffects.None, depth);
            }

            public static void DrawTexture(SpriteBatch spriteBatch, Texture2D tex, M_Rectangle rect, Color color, float depth = 0f)
            {
                spriteBatch.Draw(tex, rect.pos, null, color, 0f, Vector2.Zero, rect.size / new Vector2(tex.Width, tex.Height), SpriteEffects.None, depth);
            }
            public static void DrawTextureOrigin(SpriteBatch spriteBatch, Texture2D tex, Vector2 pos, Vector2 originNormalized, Color color)
            {
                pos -= originNormalized * tex.GetSize();
                spriteBatch.Draw(tex, pos, null, color, 0f, Vector2.Zero, 0f, SpriteEffects.None, 0f);
            }
            public static void DrawTextureOrigin(SpriteBatch spriteBatch, Texture2D tex, Vector2 pos, Vector2 originNormalized, Color color, float rotation = 0f, Vector2? rotationOrigin = null, Vector2? scale = null, SpriteEffects spriteEffects = SpriteEffects.None, float depth = 0f)
            {
                pos -= originNormalized * tex.GetSize();
                spriteBatch.Draw(tex, pos, null, color, rotation, rotationOrigin ?? Vector2.Zero, scale ?? Vector2.One, spriteEffects, depth);
            }

            public static void DrawTextureCroppedWithSameAspectRatio(SpriteBatch spriteBatch, Texture2D tex, M_Rectangle visible, Color color, float depth = 0f)
            {
                float relationW = visible.size.X / tex.Width;
                float relationH = visible.size.Y / tex.Height;

                if (relationH > relationW)
                {
                    int tooLong = ((int)(tex.Width * relationH)) - (int)visible.size.X;
                    spriteBatch.Draw(tex, new Rectangle((int)(visible.pos.X - tooLong / 2), (int)visible.pos.Y, (int)(tex.Width * relationH), (int)visible.size.Y), null, color, 0, Vector2.Zero, SpriteEffects.None, depth);
                }
                else
                {
                    int tooLong = ((int)(tex.Height * relationW)) - (int)visible.size.Y;
                    spriteBatch.Draw(tex, new Rectangle((int)visible.pos.X, (int)(visible.pos.Y - tooLong / 2), (int)visible.size.X, (int)(tex.Height * relationW)), null, color, 0, Vector2.Zero, SpriteEffects.None, depth);
                }
            }

            public static void DrawCircle(SpriteBatch spriteBatch, Vector2 pos, float radius, Color color, float depth = 0f, float textureScale = 1f)
            {
                radius = Math.Abs(radius);
                if (radius > 0)
                {
                    if (textureScale < 0.5f / radius)
                        textureScale = 0.5f / radius;

                    int key = (int)Math.Round(radius * textureScale);
                    if (!circleTextures.ContainsKey(key))
                        circleTextures.Add(key, CreateCircleTexture(key, Color.White));
                    spriteBatch.Draw(circleTextures[key], pos - new Vector2(radius), null, color, 0f, Vector2.Zero, 1 / textureScale, SpriteEffects.None, depth);
                }
            }
            public static void DrawCircle(SpriteBatch spriteBatch, Vector2 pos, float radius, Color color1, Color color2, float depth = 0f, float textureScale = 1f)
            {
                radius = Math.Abs(radius);
                if (radius > 0)
                {
                    if (textureScale < 0.5f / radius)
                        textureScale = 0.5f / radius;

                    Tuple<int, Color, Color> key = new Tuple<int, Color, Color>((int)Math.Round(radius * textureScale), color1, color2);
                    if (!circleTextures2.ContainsKey(key))
                        circleTextures2.Add(key, CreateCircleTexture((int)Math.Round(radius * textureScale), color1, color2));
                    spriteBatch.Draw(circleTextures2[key], pos - new Vector2(radius), null, Color.White, 0f, Vector2.Zero, 1 / textureScale, SpriteEffects.None, depth);
                }
            }

            public static Texture2D CreateCircleTexture(int radius, Color color)
            {
                Texture2D tex;
                if (radius < 1)
                {
                    tex = new Texture2D(gDevice, 1, 1);
                    tex.SetData<Color>(new Color[] { color });
                    return tex;
                }

                int diamInt = radius * 2;
                float radiusSquared = (float)Math.Pow(radius, 2);
                tex = new Texture2D(gDevice, diamInt, diamInt);
                Color[] data = new Color[diamInt * diamInt];

                int x, y;
                x = y = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    float dist = (float)(Math.Pow(radius - x - 0.5f, 2) + Math.Pow(radius - y - 0.5f, 2));
                    if (dist <= radiusSquared)
                        data[i] = color;

                    x++;
                    if (x == diamInt)
                    {
                        x = 0;
                        y++;
                    }
                }
                tex.SetData<Color>(data);
                return tex;
            }
            public static Texture2D CreateCircleTexture(int radius, Color color1, Color color2, bool powColor = false)
            {
                Texture2D tex;
                if (radius < 1)
                {
                    tex = new Texture2D(gDevice, 1, 1);
                    tex.SetData<Color>(new Color[] { color1 });
                    return tex;
                }

                int diamInt = radius * 2;
                float radiusSquared = (float)Math.Pow(radius, 2);
                tex = new Texture2D(gDevice, diamInt, diamInt);
                Color[] data = new Color[diamInt * diamInt];

                int x, y;
                x = y = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    float dist = (float)(Math.Pow(radius - x - 0.5f, 2) + Math.Pow(radius - y - 0.5f, 2));
                    if (dist <= radiusSquared)
                    {
                        dist = (float)Math.Sqrt(dist) / radius;
                        Vector3 colorV = color1.ToVector3() * (1f - dist) + color2.ToVector3() * dist;
                        if (powColor)
                            colorV = new Vector3((float)Math.Pow(colorV.X, 2), (float)Math.Pow(colorV.Y, 2), (float)Math.Pow(colorV.Z, 2));
                        data[i] = new Color(colorV);
                    }

                    x++;
                    if (x == diamInt)
                    {
                        x = 0;
                        y++;
                    }
                }
                tex.SetData<Color>(data);
                return tex;
            }

            #endregion

            #region Hexagon

            public static Texture2D GenerateHexagon(int width, int height, Color color)
            {
                Color[] colors = new Color[width * height];

                int yDoubled = 1; // start by 1, so that the next increment (2) halved is 1 -> instant break
                int y = 0;
                for (int x = width / 2 - 1; x > 0; x--)
                {
                    y = yDoubled / 2;
                    colors[y * width + x] = colors[y * width + width - x - 1] = colors[(height - y - 1) * width + x] = colors[(height - y - 1) * width + width - x - 1] = color;
                    yDoubled++;
                }

                if (width % 2 == 1)
                {
                    colors[width / 2] = colors[(height - 1) * width + width / 2] = color;
                }

                y++;
                int yEnd = height - y;
                for (; y < yEnd; y++)
                {
                    colors[y * width] = colors[y * width + width - 1] = color;
                }
                Texture2D tex = new Texture2D(gDevice, width, height);
                tex.SetData(colors);
                return tex;
            }

            public static Texture2D GenerateHexagonFilled(int width, int height, Color color)
            {
                Color[] colors = new Color[width * height];

                int yDoubled = 1; // start by 1, so that the next increment (2) halved is 1 -> instant break
                int y = 0;
                for (int x = width / 2 - 1; x > 0; x--)
                {
                    y = yDoubled / 2;
                    for (int cy = y; cy < height - y; cy++)
                    {
                        colors[cy * width + x] = colors[cy * width + width - x - 1] = color;
                    }
                    yDoubled++;
                }

                if (width % 2 == 1)
                {
                    for (int cy = 0; cy < height; cy++)
                    {
                        colors[cy * width + width / 2] = color;
                    }
                }

                y++;
                int yEnd = height - y;
                for (; y < yEnd; y++)
                {
                    colors[y * width] = colors[y * width + width - 1] = color;
                }
                Texture2D tex = new Texture2D(gDevice, width, height);
                tex.SetData(colors);
                return tex;
            }

            public static Texture2D GenerateRectangleOutline(int width, int height, Color color)
            {
                Color[] colors = new Color[width * height];
                for (int i = 0; i < width; i++)
                {
                    colors[i] = Color.Black;
                    colors[width * height - width + i] = Color.Black;
                }
                for (int i = 0; i < height; i++)
                {
                    colors[i * width] = Color.Black;
                    colors[i * width + width - 1] = Color.Black;
                }

                Texture2D tex = new Texture2D(gDevice, width, width);
                tex.SetData<Color>(colors);
                return tex;
            }

            #endregion

            #region String

            public static void DrawStringCenter(SpriteBatch spriteBatch, string text, Vector2 pos, Color color, SpriteFont font, bool roundPosition = true, float depth = 0f, bool coded = false)
            {
                Vector2 textSize = (coded ? MeasureCodeString(font, text) : font.MeasureString(text));

                Vector2 pos2 = pos - textSize / 2f;
                if (roundPosition)
                    pos2 = pos2.RoundVector();

                if (coded)
                    DrawStringCoded(spriteBatch, text, pos2, font, depth);
                else
                    spriteBatch.DrawString(font, text, pos2, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
            }
            public static void DrawStringCenter(SpriteBatch spriteBatch, string text, M_Rectangle rect, Color color, SpriteFont font, bool roundPosition = true, float depth = 0f, bool coded = false)
            {
                DrawStringCenter(spriteBatch, text, rect.GetCenter(), color, font, roundPosition, depth, coded);
            }
            public static void DrawStringCenterBreak(SpriteBatch spriteBatch, string text, M_Rectangle rect, Color color, SpriteFont font, bool roundPosition = true, float depth = 0f, bool coded = false, float scale = 1f)
            {
                float paddingX = 3;
                float paddingY = 0;
                M_Rectangle padding = new M_Rectangle(rect.pos.X + paddingX, rect.pos.Y + paddingY, rect.size.X - paddingX * 2, rect.size.Y - paddingY * 2);

                List<string> lines = new List<string>();
                float lineHeightSum = 0;

                bool tooLong = false;

                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i] == '\n')
                    {
                        lines.Add(text.Substring(0, i));
                        text = text.Substring(i + 1);
                        i = 0;
                    }
                    else if (font.MeasureString(text.Substring(0, i)).X * scale > padding.size.X)
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
                        float lineHeight = font.MeasureString(lines[lines.Count - 1]).Y * scale;

                        if (lineHeightSum + lineHeight > padding.size.Y)
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
                    float lineHeight = font.MeasureString(lines[lines.Count - 1]).Y * scale;
                    if (lineHeightSum + lineHeight > padding.size.Y)
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
                        lineHeightSum += font.MeasureString(lines[lines.Count - 1]).Y * scale;
                    }
                    else
                        lines[lines.Count - 1] = lines[lines.Count - 1].Remove(Math.Max(0, lines[lines.Count - 1].Length - 3)) + "...";
                }

                Vector2 pos = rect.GetCenter() + new Vector2(0, -lineHeightSum / 2f);

                for (int i = 0; i < lines.Count; i++)
                {
                    Vector2 lineSize = font.MeasureString(lines[i]) * scale;

                    Vector2 pos2 = pos + new Vector2(-lineSize.X / 2f, 0);
                    if (roundPosition)
                        pos2 = pos2.RoundVector();


                    if (coded)
                        DrawStringCoded(spriteBatch, lines[i], pos2, font, depth, scale);
                    else
                        spriteBatch.DrawString(font, lines[i], pos2, color, 0f, Vector2.Zero, scale, SpriteEffects.None, depth);

                    pos.Y += lineSize.Y;
                }
            }

            public static void DrawStringOutlined(SpriteBatch spriteBatch, string text, Vector2 pos, float breite, Color colorFill, Color colorOutline, SpriteFont font, float depth = 0, float rotation = 0f, float scale = 1)
            {
                float b = breite * scale;
                spriteBatch.DrawString(font, text, pos + new Vector2(b, 0), colorOutline, rotation, Vector2.Zero, scale, SpriteEffects.None, depth);
                spriteBatch.DrawString(font, text, pos + new Vector2(0, -b), colorOutline, rotation, Vector2.Zero, scale, SpriteEffects.None, depth + 0.000001f);
                spriteBatch.DrawString(font, text, pos + new Vector2(-b, 0), colorOutline, rotation, Vector2.Zero, scale, SpriteEffects.None, depth + 0.000002f);
                spriteBatch.DrawString(font, text, pos + new Vector2(0, b), colorOutline, rotation, Vector2.Zero, scale, SpriteEffects.None, depth + 0.000003f);
                spriteBatch.DrawString(font, text, pos, colorFill, rotation, Vector2.Zero, scale, SpriteEffects.None, depth + 0.000004f);
            }
            public static void DrawStringOutlinedSameDepth(SpriteBatch spriteBatch, string text, Vector2 pos, float breite, Color colorFill, Color colorOutline, SpriteFont font, float depth = 0, float rotation = 0f, float scale = 1)
            {
                float b = breite * scale;
                spriteBatch.DrawString(font, text, pos + new Vector2(b, 0), colorOutline, rotation, Vector2.Zero, scale, SpriteEffects.None, depth);
                spriteBatch.DrawString(font, text, pos + new Vector2(0, -b), colorOutline, rotation, Vector2.Zero, scale, SpriteEffects.None, depth);
                spriteBatch.DrawString(font, text, pos + new Vector2(-b, 0), colorOutline, rotation, Vector2.Zero, scale, SpriteEffects.None, depth);
                spriteBatch.DrawString(font, text, pos + new Vector2(0, b), colorOutline, rotation, Vector2.Zero, scale, SpriteEffects.None, depth);
                spriteBatch.DrawString(font, text, pos, colorFill, rotation, Vector2.Zero, scale, SpriteEffects.None, depth);
            }
            public static void DrawStringCenterOutlined(SpriteBatch spriteBatch, string text, Vector2 pos, float breite, Color colorFill, Color colorOutline, SpriteFont font, float depth = 0)
            {
                pos = new Vector2(pos.X - font.MeasureString(text).X / 2, pos.Y - font.MeasureString(text).Y / 2);
                pos = pos.RoundVector();
                spriteBatch.DrawString(font, text, pos + new Vector2(breite, 0), colorOutline, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
                spriteBatch.DrawString(font, text, pos + new Vector2(0, -breite), colorOutline, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
                spriteBatch.DrawString(font, text, pos + new Vector2(-breite, 0), colorOutline, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
                spriteBatch.DrawString(font, text, pos + new Vector2(0, breite), colorOutline, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
                spriteBatch.DrawString(font, text, pos, colorFill, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth + 0.001f);
            }
            public static void DrawStringScaledCenter(SpriteBatch spriteBatch, string text, Vector2 pos, Color color, SpriteFont font, Vector2 scale, bool roundPosition = true, float depth = 0f)
            {
                Vector2 textSize = font.MeasureString(text) * scale;
                Vector2 pos2 = pos - textSize / 2f;
                if (roundPosition)
                    pos2 = pos2.RoundVector();

                spriteBatch.DrawString(font, text, pos2, color, 0f, Vector2.Zero, scale, SpriteEffects.None, depth);
            }

            public static void DrawStringCoded(SpriteBatch spriteBatch, string text, Vector2 pos, SpriteFont font, float depth = 0f, float scale = 1f)
            {
                float left = pos.X;

                string[] split = text.Split(new char[] { openCode });

                Color color = baseTextColor;
                Color? backColor = null;
                int backThickness = 1;
                int align = 0;

                string lastLine = "";

                int i = 0;
                while (true)
                {

                    if (pos.X == left)
                        DrawCodedPart(spriteBatch, font, split[i], pos, color, depth, backColor, backThickness, align, scale);
                    else
                    {
                        //write new line to the left
                        int newLineIndex = split[i].IndexOf('\n');
                        if (newLineIndex == -1)
                        {
                            DrawCodedPart(spriteBatch, font, split[i], pos, color, depth, backColor, backThickness, align, scale);
                        }
                        else
                        {
                            string line = split[i].Substring(0, newLineIndex);
                            DrawCodedPart(spriteBatch, font, line, pos, color, depth, backColor, backThickness, align, scale);
                            line = split[i].Substring(newLineIndex + 1);
                            pos.X = left;
                            pos.Y += font.LineSpacing * scale;
                            DrawCodedPart(spriteBatch, font, line, pos, color, depth, backColor, backThickness, align, scale);
                            pos.Y -= font.LineSpacing * scale;
                        }
                    }

                    pos.Y += font.LineSpacing * scale * split[i].Count(f => f == '\n');

                    int lineBreak;
                    if ((lineBreak = split[i].LastIndexOf('\n')) == -1)
                    {
                        lastLine += split[i];
                        pos.X += font.MeasureString(split[i]).X * scale;
                    }
                    else
                    {
                        lastLine = split[i].Substring(lineBreak + 1);
                        pos.X = left + font.MeasureString(lastLine).X * scale;
                    }

                    //pos.X = left + font.MeasureString(lastLine).X;

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
                                        if (split[i][1] == 'c')
                                        {
                                            if (split[i][3] == '>')
                                                backColor = Calculate.HexToColor(split[i].Substring(2, 1));
                                            else if (split[i][5] == '>')
                                                backColor = Calculate.HexToColor(split[i].Substring(2, 3));
                                            else
                                                backColor = Calculate.HexToColor(split[i].Substring(2, 6));
                                        }
                                        else
                                            backColor = Color.Black;
                                    }
                                    else
                                        backColor = null;
                                    break;
                                case 'a': //alignment
                                    if (!closeCommand)
                                    {
                                        align = split[i][1] == '1' ? 1 : split[i][1] == '2' ? 2 : 0;
                                    }
                                    else
                                        align = 0;
                                    break;
                                case 'p':

                                    if (split[i].Substring(1, 2) == "os")
                                    {
                                        if (!closeCommand)
                                        {
                                            //position
                                            float p;
                                            if (float.TryParse(split[i].Substring(3, split[i].IndexOf(closeCode) - 3), NumberStyles.Any, CultureInfo.InvariantCulture, out p))
                                                pos.X = left + p;
                                        }
                                        else
                                            pos.X = left;
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
            }

            private static void DrawCodedPart(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 pos, Color color, float depth, Color? backColor, int backThickness, int align, float scale = 1f)
            {
                string[] split = text.Split(new char[] { '\n' });

                for (int i = 0; i < split.Length; i++)
                {
                    text = split[i];
                    Vector2 size = font.MeasureString(text) * scale;
                    if (backColor == null)
                    {
                        spriteBatch.DrawString(font, text, (pos - new Vector2(size.X * (align * 0.5f), 0)).RoundVector(), color, 0f, Vector2.Zero, scale, SpriteEffects.None, depth);
                    }
                    else
                    {
                        DrawStringOutlined(spriteBatch, text, (pos - new Vector2(size.X * (align * 0.5f), 0)).RoundVector(), backThickness, color, backColor.Value, font, depth, 0f, scale);
                    }
                    pos.Y += font.MeasureString(split[i]).Y * scale;
                }
            }

            public static Vector2 MeasureCodeString(SpriteFont font, string text)
            {
                    return font.MeasureString(Drawer.RemoveCodes(text));
            }
        
            //public static string RemoveCodes(string text)
            //{
            //    int i = 0;
            //    while (i < text.Length - 1 && (i = text.IndexOf(openCode, i)) != -1)
            //    {
            //        int j = text.IndexOf(closeCode, i + 1);

            //        if (j == -1)
            //            text = text.Remove(i);
            //        else
            //            text = text.Remove(i, j - i + 1);
            //    }

            //    return text;
            //}

            #endregion

            #region New String
            /*
            public static unsafe void MyDrawString(SpriteFont spriteFont, StringBuilder text, Vector2 position, Color color)
            {
                float sortKey = 0;

                var offset = Vector2.Zero;
                var firstGlyphOfLine = true;
                
                fixed (SpriteFont.Glyph* pGlyphs = spriteFont.GetGlyphs().Values.ToArray()) //TODO: improve performance -> spriteFont.Glyphs
                    for (var i = 0; i < text.Length; ++i)
                    {
                        var c = text[i];

                        if (c == '\r')
                            continue;

                        if (c == '\n')
                        {
                            offset.X = 0;
                            offset.Y += spriteFont.LineSpacing;
                            firstGlyphOfLine = true;
                            continue;
                        }
                        var currentGlyphIndex = spriteFont.GetGlyphIndexOrDefault(c);
                        var pCurrentGlyph = pGlyphs + currentGlyphIndex;

                        // The first character on a line might have a negative left side bearing.
                        // In this scenario, SpriteBatch/SpriteFont normally offset the text to the right,
                        //  so that text does not hang off the left side of its rectangle.
                        if (firstGlyphOfLine)
                        {
                            offset.X = Math.Max(pCurrentGlyph->LeftSideBearing, 0);
                            firstGlyphOfLine = false;
                        }
                        else
                        {
                            offset.X += spriteFont.Spacing + pCurrentGlyph->LeftSideBearing;
                        }

                        var p = offset;
                        p.X += pCurrentGlyph->Cropping.X;
                        p.Y += pCurrentGlyph->Cropping.Y;
                        p += position;

                        var item = _batcher.CreateBatchItem();
                        item.Texture = spriteFont.Texture;
                        item.SortKey = sortKey;

                        _texCoordTL.X = pCurrentGlyph->BoundsInTexture.X * spriteFont.Texture.TexelWidth;
                        _texCoordTL.Y = pCurrentGlyph->BoundsInTexture.Y * spriteFont.Texture.TexelHeight;
                        _texCoordBR.X = (pCurrentGlyph->BoundsInTexture.X + pCurrentGlyph->BoundsInTexture.Width) * spriteFont.Texture.TexelWidth;
                        _texCoordBR.Y = (pCurrentGlyph->BoundsInTexture.Y + pCurrentGlyph->BoundsInTexture.Height) * spriteFont.Texture.TexelHeight;

                        item.Set(p.X,
                                 p.Y,
                                 pCurrentGlyph->BoundsInTexture.Width,
                                 pCurrentGlyph->BoundsInTexture.Height,
                                 color,
                                 _texCoordTL,
                                 _texCoordBR,
                                 0);

                        offset.X += pCurrentGlyph->Width + pCurrentGlyph->RightSideBearing;
                    }

                // We need to flush if we're using Immediate sort mode.
                FlushIfNeeded();
            }
            */
            #endregion

            //public struct Text
            //{
            //    public string text;
            //    public Color color;
            //    public Vector2 pos;
            //    public Vector2 scale;
            //    public float depth;
            //    public float rotation;
            //    public Vector2 origin;
            //    public SpriteEffects spriteEffects;
            //    public bool coded;
            //    public SpriteFont font;
            //}

            //Text effects: 
            //  transformations (translation, rotation, scale)
            //  align
            //  color
            //  font
            //  bold/outline (+color)
            //  underline
            //  italic
            //  (crossed)
            //  (back-fill)
            //  (shaking) //animation
            //  (character delay) //character speaking
        }

        public class Vertex
        {
            private static VertexPositionColor[] quadVertices;

            /// <summary>
            /// is being called in DrawM.Initialize()
            /// </summary>
            public static void Initialize()
            {
                quadVertices = new VertexPositionColor[]
                        {
                            new VertexPositionColor(
                                new Vector3(0,0,0),
                                Color.Black),
                            new VertexPositionColor(
                                new Vector3(0,0,0),
                                Color.Black),
                            new VertexPositionColor(
                                new Vector3(0,0,0),
                                Color.Black),
                            new VertexPositionColor(
                                new Vector3(0,0,0),
                                Color.Black)
                        };
            }

            //Line
            public static void DrawLineThin(Vector2 pos1, Vector2 pos2, Color color)
            {
                VertexPositionColor[] vertices = new VertexPositionColor[2];
                vertices[0].Position = new Vector3(pos1, 0);
                vertices[0].Color = color;
                vertices[1].Position = new Vector3(pos2, 0);
                vertices[1].Color = color;

                basicEffect.CurrentTechnique.Passes[0].Apply();
                gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
            }
            public static void DrawLineThinSize(Vector2 pos, Vector2 size, Color color)
            {
                VertexPositionColor[] vertices = new VertexPositionColor[2];
                vertices[0].Position = new Vector3(pos, 0);
                vertices[0].Color = color;
                vertices[1].Position = new Vector3(pos + size, 0);
                vertices[1].Color = color;

                basicEffect.CurrentTechnique.Passes[0].Apply();
                gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 1);
            }
            public static void DrawLine(Vector2 pos1, Vector2 pos2, Color color, float thickness)
            {
                Vector2 thickDir = Vector2.Normalize(pos2 - pos1) * thickness;
                thickDir = new Vector2(-thickDir.Y, thickDir.X) / 2f;
                VertexPositionColor[] vertices = new VertexPositionColor[4];
                vertices[0].Position = new Vector3(pos1 + thickDir, 0);
                vertices[1].Position = new Vector3(pos1 - thickDir, 0);
                vertices[2].Position = new Vector3(pos2 + thickDir, 0);
                vertices[3].Position = new Vector3(pos2 - thickDir, 0);

                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].Color = color;

                basicEffect.CurrentTechnique.Passes[0].Apply();
                gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, vertices, 0, 2);
            }
            public static void DrawLineSize(Vector2 pos, Vector2 size, Color color, float thickness)
            {
                Vector2 thickDir = Vector2.Normalize(size);
                thickDir = new Vector2(-thickDir.Y, thickDir.X) / 2f;
                VertexPositionColor[] vertices = new VertexPositionColor[4];
                vertices[0].Position = new Vector3(pos + thickDir, 0);
                vertices[1].Position = new Vector3(pos - thickDir, 0);
                vertices[2].Position = new Vector3(pos + size + thickDir, 0);
                vertices[3].Position = new Vector3(pos + size - thickDir, 0);

                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].Color = color;

                basicEffect.CurrentTechnique.Passes[0].Apply();
                gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, vertices, 0, 2);
            }

            //Rectangle
            public static void DrawRectangle(M_Rectangle rect, Color color)
            {
                Vector2 p1 = rect.pos;
                Vector2 p2 = rect.pos + rect.size;

                VertexPositionColor[] vertices = new VertexPositionColor[4];
                vertices[0].Position = new Vector3(new Vector2(p1.X, p2.Y), 0);
                vertices[1].Position = new Vector3(p1, 0);
                vertices[2].Position = new Vector3(p2, 0);
                vertices[3].Position = new Vector3(new Vector2(p2.X, p1.Y), 0);

                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].Color = color;

                basicEffect.CurrentTechnique.Passes[0].Apply();
                gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, vertices, 0, 2);
            }
            public static void DrawRectangleOutlineThin(M_Rectangle rect, Color color)
            {
                VertexPositionColor[] vertices = new VertexPositionColor[5];
                vertices[0].Position = new Vector3(rect.pos, 0);
                vertices[1].Position = new Vector3(rect.pos + new Vector2(rect.size.X, 0), 0);
                vertices[2].Position = new Vector3(rect.pos + rect.size, 0);
                vertices[3].Position = new Vector3(rect.pos + new Vector2(0, rect.size.Y), 0);
                vertices[4].Position = vertices[0].Position;

                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].Color = color;

                basicEffect.CurrentTechnique.Passes[0].Apply();
                gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, 4);
            }
            public static void DrawRectangleOutline(M_Rectangle rect, Color color, float thickness)
            {
                float t = thickness / 2f;
                if (rect.size.X <= thickness || rect.size.Y <= thickness)
                    DrawRectangle(new M_Rectangle(rect.pos - new Vector2(t), rect.size + new Vector2(thickness)), color);
                else
                {
                    DrawRectangle(new M_Rectangle(rect.pos.X + t, rect.pos.Y - t, rect.size.X, thickness), color);
                    DrawRectangle(new M_Rectangle(rect.pos.X + rect.size.X - t, rect.pos.Y + t, thickness, rect.size.Y), color);
                    DrawRectangle(new M_Rectangle(rect.pos.X - t, rect.pos.Y + rect.size.Y - t, rect.size.X, thickness), color);
                    DrawRectangle(new M_Rectangle(rect.pos.X - t, rect.pos.Y - t, thickness, rect.size.Y), color);
                }
            }
            public static void DrawRectangleOutlineInside(M_Rectangle rect, Color color, float thickness)
            {
                if (rect.size.X <= thickness * 2f || rect.size.Y <= thickness * 2f)
                    DrawRectangle(rect, color);
                else
                {
                    DrawRectangle(new M_Rectangle(rect.pos.X + thickness, rect.pos.Y, rect.size.X - thickness, thickness), color);
                    DrawRectangle(new M_Rectangle(rect.pos.X + rect.size.X - thickness, rect.pos.Y + thickness, thickness, rect.size.Y - thickness), color);
                    DrawRectangle(new M_Rectangle(rect.pos.X, rect.pos.Y + rect.size.Y - thickness, rect.size.X - thickness, thickness), color);
                    DrawRectangle(new M_Rectangle(rect.pos.X, rect.pos.Y, thickness, rect.size.Y - thickness), color);
                }
            }
            public static void DrawRectangleOutlineOutside(M_Rectangle rect, Color color, float thickness)
            {
                DrawRectangle(new M_Rectangle(rect.pos.X, rect.pos.Y - thickness, rect.size.X + thickness, thickness), color);
                DrawRectangle(new M_Rectangle(rect.pos.X + rect.size.X, rect.pos.Y, thickness, rect.size.Y + thickness), color);
                DrawRectangle(new M_Rectangle(rect.pos.X - thickness, rect.pos.Y + rect.size.Y, rect.size.X + thickness, thickness), color);
                DrawRectangle(new M_Rectangle(rect.pos.X - thickness, rect.pos.Y - thickness, thickness, rect.size.Y + thickness), color);
            }

            //Texture
            public static void DrawTexture(M_Rectangle rect, Texture2D tex, Color color)
            {
                Vector2 p1 = rect.pos;
                Vector2 p2 = rect.pos + rect.size;

                VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[6];

                vertices[0].Position = new Vector3(new Vector2(p1.X, p2.Y), 0);
                vertices[1].Position = new Vector3(p1, 0);
                vertices[2].Position = new Vector3(p2, 0);
                vertices[3].Position = new Vector3(new Vector2(p2.X, p1.Y), 0);

                vertices[0].TextureCoordinate = new Vector2(0, 1);
                vertices[1].TextureCoordinate = new Vector2(0, 0);
                vertices[2].TextureCoordinate = new Vector2(1, 1);
                vertices[3].TextureCoordinate = new Vector2(1, 0);

                for (int i = 0; i < vertices.Length; i++)
                    vertices[i].Color = color;

                basicEffect.TextureEnabled = true;
                basicEffect.Texture = tex;

                basicEffect.CurrentTechnique.Passes[0].Apply();


                gDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, vertices, 0, 2);
                basicEffect.TextureEnabled = false;
            }

            //Polygon
            public static void DrawPolygon(Vector2 relativePos, List<Vector2> vertices, Color color)
            {
                if (vertices.Count > 2)
                {
                    VertexPositionColor[] v = new VertexPositionColor[(vertices.Count - 2) * 3];
                    int faces = v.Length / 3;
                    //v[0].Position = new Vector3(relativePos + vertices[0], 0);
                    for (int i = 0; i < faces; i++)
                    {
                        v[i * 3].Position = new Vector3(relativePos + vertices[0], 0);
                        v[i * 3 + 1].Position = new Vector3(relativePos + vertices[i + 1], 0);
                        v[i * 3 + 2].Position = new Vector3(relativePos + vertices[i + 2], 0);
                    }

                    for (int i = 0; i < v.Length; i++)
                        v[i].Color = color;

                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, v, 0, faces);
                }
            }
            public static void DrawPolygon(Vector2 relativePos, List<Vector2> vertices, Color color1, Color color2, Vector2 center)
            {
                if (vertices.Count > 2)
                {
                    VertexPositionColor[] v = new VertexPositionColor[vertices.Count * 3];
                    int faces = v.Length / 3;
                    //v[0].Position = new Vector3(relativePos + vertices[0], 0);
                    int i;
                    for (i = 0; true; i++)
                    {
                        v[i * 3].Color = color1;
                        v[i * 3 + 1].Color = color2;
                        v[i * 3 + 2].Color = color2;

                        if (i >= faces - 1)
                            break;

                        v[i * 3].Position = new Vector3(relativePos + center, 0);
                        v[i * 3 + 1].Position = new Vector3(relativePos + vertices[i], 0);
                        v[i * 3 + 2].Position = new Vector3(relativePos + vertices[i + 1], 0);
                    }

                    v[i * 3].Position = new Vector3(relativePos + center, 0);
                    v[i * 3 + 1].Position = new Vector3(relativePos + vertices[i], 0);
                    v[i * 3 + 2].Position = new Vector3(relativePos + vertices[0], 0);

                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, v, 0, faces);
                }
            }
            public static void DrawPolygonOutlineClosed(Vector2 relativePos, List<Vector2> vertices, Color color)
            {
                if (vertices.Count > 2)
                {
                    VertexPositionColor[] v = new VertexPositionColor[vertices.Count + 1];
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        v[i].Position = new Vector3(relativePos + vertices[i], 0);
                        v[i].Color = color;
                    }

                    v[vertices.Count].Position = new Vector3(relativePos + vertices[0], 0);
                    v[vertices.Count].Color = color;

                    //basicEffect.Alpha = color.A;

                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, v, 0, v.Length - 1);
                }
                else if (vertices.Count == 2)
                    DrawPolygonOutlineOpen(relativePos, vertices, color);
            }
            public static void DrawPolygonOutlineClosed(Vector2 relativePos, List<Vector2> vertices, Color color, float border)
            {
                if (vertices.Count > 2)
                {
                    border *= 0.5f;
                    VertexPositionColor[] v = new VertexPositionColor[vertices.Count * 2 + 2];
                    Vector2 n1 = Vector2.Normalize(vertices[1] - vertices[0]);
                    n1 = new Vector2(-n1.Y, n1.X) * border;
                    Vector2 n2 = Vector2.Normalize(vertices[0] - vertices[vertices.Count - 1]);
                    n2 = new Vector2(-n2.Y, n2.X) * border;
                    Vector2 n;
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        n = (n1 + n2) / 2f;

                        int j = i * 2;
                        v[j].Position = new Vector3(relativePos + vertices[i] + n, 0);
                        v[j + 1].Position = new Vector3(relativePos + vertices[i] - n, 0);
                        v[j].Color = color;
                        v[j + 1].Color = color;

                        n1 = n2;
                        if (i < vertices.Count - 2)
                        {
                            n2 = Vector2.Normalize(vertices[i + 2] - vertices[i + 1]);
                            n2 = new Vector2(-n2.Y, n2.X) * border;
                        }
                        else
                        {
                            n2 = Vector2.Normalize(vertices[0] - vertices[vertices.Count - 1]);
                            n2 = new Vector2(-n2.Y, n2.X) * border;
                        }
                    }

                    v[v.Length - 2] = v[0];
                    v[v.Length - 1] = v[1];

                    //basicEffect.Alpha = color.A;

                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, v, 0, v.Length - 2);
                }
                else if (vertices.Count == 2)
                    DrawPolygonOutlineOpen(relativePos, vertices, color, border);
            }
            public static void DrawPolygonOutlineOpen(Vector2 relativePos, List<Vector2> vertices, Color color)
            {
                if (vertices.Count > 1)
                {
                    VertexPositionColor[] v = new VertexPositionColor[vertices.Count];
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        v[i].Position = new Vector3(relativePos + vertices[i], 0);
                        v[i].Color = color;
                    }

                    //basicEffect.Alpha = color.A;

                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, v, 0, v.Length - 1);
                }
            }
            public static void DrawPolygonOutlineOpen(Vector2 relativePos, List<Vector2> vertices, Color color, float border)
            {
                if (vertices.Count > 1)
                {
                    border *= 0.5f;
                    VertexPositionColor[] v = new VertexPositionColor[vertices.Count * 2];
                    Vector2 n1 = Vector2.Normalize(vertices[1] - vertices[0]);
                    n1 = new Vector2(-n1.Y, n1.X) * border;
                    Vector2 n2 = n1;
                    Vector2 n;
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        n = (n1 + n2) / 2f;

                        int j = i * 2;
                        v[j].Position = new Vector3(relativePos + vertices[i] + n, 0);
                        v[j + 1].Position = new Vector3(relativePos + vertices[i] - n, 0);
                        v[j].Color = color;
                        v[j + 1].Color = color;

                        n1 = n2;
                        if (i < vertices.Count - 2)
                        {
                            n2 = Vector2.Normalize(vertices[i + 2] - vertices[i + 1]);
                            n2 = new Vector2(-n2.Y, n2.X) * border;
                        }
                    }

                    //basicEffect.Alpha = color.A;

                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, v, 0, v.Length - 2);
                }
            }

            public static void DrawTriangleList(Vector2 relativePos, List<Vector2> vertices, Color color)
            {
                if (vertices.Count > 2)
                {
                    VertexPositionColor[] v = new VertexPositionColor[vertices.Count];
                    int faces = v.Length / 3;
                    for (int i = 0; i < v.Length; i++)
                    {
                        v[i].Position = new Vector3(relativePos + vertices[i], 0);
                        v[i].Color = color;
                    }

                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, v, 0, faces);
                }
            }
            public static void DrawTriangleStrip(Vector2 relativePos, List<Vector2> vertices, Color color)
            {
                if (vertices.Count > 2)
                {
                    VertexPositionColor[] v = new VertexPositionColor[vertices.Count];
                    int faces = v.Length - 2;
                    for (int i = 0; i < v.Length; i++)
                    {
                        v[i].Position = new Vector3(relativePos + vertices[i], 0);
                        v[i].Color = color;
                    }

                    basicEffect.CurrentTechnique.Passes[0].Apply();
                    gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, v, 0, faces);
                }
            }

            //Circle
            public static void DrawCircle(Vector2 pos, float radius, Color color, int vertices)
            {
                radius = Math.Abs(radius);
                M_Polygon circle = M_Polygon.GetCircleOpen(pos, radius, vertices);
                DrawPolygon(pos, circle.vertices, color);
            }

            /// <param name="quality">0-16</param>
            public static void DrawCircle(Vector2 pos, float radius, Color color, float quality)
            {
                radius = Math.Abs(radius);
                int vertices = (int)(Math.Max(4, (int)Math.Pow(radius * DrawM.scale, 1 / 4f) * quality));
                DrawCircle(pos, radius, color, vertices);
            }

            public static void DrawCircle(Vector2 pos, float radius, Color color1, Color color2, int vertices)
            {
                radius = Math.Abs(radius);
                M_Polygon circle = M_Polygon.GetCircleOpen(pos, radius, vertices);
                DrawPolygon(pos, circle.vertices, color1, color2, Vector2.Zero);
            }
            /// <param name="quality">0-16</param>
            public static void DrawCircle(Vector2 pos, float radius, Color color1, Color color2, float quality)
            {
                radius = Math.Abs(radius);
                int vertices = (int)(Math.Max(4, (int)Math.Pow(radius * DrawM.scale, 1 / 4f) * quality));
                DrawCircle(pos, radius, color1, color2, vertices);
            }

            public static void DrawCircleOutline(Vector2 pos, float radius, Color color, int vertices)
            {
                radius = Math.Abs(radius);
                M_Polygon circle = M_Polygon.GetCircleOpen(pos, radius, vertices);
                DrawPolygonOutlineClosed(pos, circle.vertices, color);
            }
            public static void DrawCircleOutline(Vector2 pos, float radius, Color color, float quality)
            {
                radius = Math.Abs(radius);
                int vertices = (int)(Math.Max(4, (int)Math.Pow(radius * DrawM.scale, 1 / 4f) * quality));
                M_Polygon circle = M_Polygon.GetCircleOpen(pos, radius, vertices);
                DrawPolygonOutlineClosed(pos, circle.vertices, color);
            }
            public static void DrawCircleOutline(Vector2 pos, float radius, Color color, int vertices, float border)
            {
                radius = Math.Abs(radius);
                M_Polygon circle = M_Polygon.GetCircleOpen(pos, radius, vertices);
                DrawPolygonOutlineClosed(pos, circle.vertices, color, border);
            }
            public static void DrawCircleOutline(Vector2 pos, float radius, Color color, float quality, float border)
            {
                radius = Math.Abs(radius);
                int vertices = (int)(Math.Max(4, (int)Math.Pow(radius * DrawM.scale, 1 / 4f) * quality));
                M_Polygon circle = M_Polygon.GetCircleOpen(pos, radius, vertices);
                DrawPolygonOutlineClosed(pos, circle.vertices, color, border);
            }


            public static void DrawDonut(Vector2 pos, float innerRadius, float thickness, Color colorInner, Color colorOuter, float quality)
            {
                innerRadius = Math.Abs(innerRadius);
                float outerRadius = innerRadius + thickness;
                int steps = (int)(Math.Max(4, (int)Math.Pow(outerRadius * DrawM.scale, 1 / 4f) * quality));

                VertexPositionColor[] v = new VertexPositionColor[steps * 2 + 2];

                int index = 0;


                float step = MathHelper.TwoPi / steps;

                float angle = 0f;
                while (steps > 0)
                {
                    v[index].Color = colorInner;
                    v[index++].Position = new Vector3(pos.X + (float)Math.Cos(angle) * innerRadius, pos.Y + (float)Math.Sin(angle) * innerRadius, 0f);
                    v[index].Color = colorOuter;
                    v[index++].Position = new Vector3(pos.X + (float)Math.Cos(angle) * outerRadius, pos.Y + (float)Math.Sin(angle) * outerRadius, 0f);

                    angle += step;
                    steps--;
                }

                v[index++] = v[0];
                v[index++] = v[1];

                basicEffect.CurrentTechnique.Passes[0].Apply();
                gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, v, 0, v.Length - 2);
            }

            public static void DrawRectangleSimple(M_Rectangle rect, Color color)
            {
                Vector2 p1 = rect.pos;
                Vector2 p2 = rect.pos + rect.size;

                quadVertices[0].Position.X = p1.X;
                quadVertices[0].Position.Y = p2.Y;
                quadVertices[1].Position.X = p1.X;
                quadVertices[1].Position.Y = p1.Y;
                quadVertices[2].Position.X = p2.X;
                quadVertices[2].Position.Y = p2.Y;
                quadVertices[3].Position.X = p2.X;
                quadVertices[3].Position.Y = p1.Y;

                for (int i = 0; i < quadVertices.Length; i++)
                    quadVertices[i].Color = color;

                gDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, quadVertices, 0, 2);
            }

            public static void DrawCone(Vector2 _origin, float _radius, float _angle, float _fov, Color _colorCenter, Color _colorOutside, float quality)
            {
                _radius = Math.Abs(_radius);
                int vertices = Math.Max(2, (int)(Math.Max(4, (int)Math.Pow(_radius * DrawM.scale, 1 / 4f) * quality) * (_fov / MathHelper.TwoPi)));
                M_Polygon poly = M_Polygon.GetCirclePart(_origin, _radius, _angle, _fov, vertices);
                DrawPolygon(_origin, poly.vertices, _colorCenter, _colorOutside, Vector2.Zero);
            }
        }

        public enum FillMode
        {
            Fill = 0,
            Outline = 1
        }

    }
}
