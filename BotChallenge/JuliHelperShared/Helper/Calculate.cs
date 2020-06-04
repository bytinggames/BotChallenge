using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace JuliHelper
{
    public static partial class Calculate
    {
        #region Math

        public static float MinAbs(float val1, float val2)
        {
            return Math.Abs(val1) < Math.Abs(val2) ? val1 : val2;
        }
        public static float MaxAbs(float val1, float val2)
        {
            return Math.Abs(val1) > Math.Abs(val2) ? val1 : val2;
        }

#if WINDOWS1
        public static T InRange<T>(this T val, T min, T max) where T : IComparable
        {
            dynamic dVal = val;
            dynamic dMin = min;
            dynamic dMax = max;
            
            if (dMin > dMax)
            {
                throw new Exception("min is greater than max!");
                //return val;
            }
            else
            {
                T range = dMax - dMin;

                if (dVal >= dMax)
                {
                    dVal -= dMin;
                    dVal %= range;
                    dVal += dMin;
                }
                else if (val < dMin)
                {
                    dVal -= dMin;
                    dVal %= range;
                    dVal += range;
                    dVal += dMin;
                }

                return dVal;
            }
        }
#endif

        /// <summary>
        /// only for structs
        /// </summary>
        public static void Swap<T>(ref T val1, ref T val2)
        {
            T val3 = val1;
            val1 = val2;
            val2 = val3;
        }

        public static float ToGoodPixelScaleFloorPow(float val)
        {
            if (val > 1)
                val = (float)Math.Floor(val);
            else
                val = (float)Math.Pow(2, Math.Floor(Math.Log(val, 2)));
            return val;
        }
        public static float ToGoodPixelScaleCeilingPow(float val)
        {
            if (val > 1)
                val = (float)Math.Ceiling(val);
            else
                val = (float)Math.Pow(2, Math.Ceiling(Math.Log(val, 2)));
            return val;
        }
        public static float ToGoodPixelScaleRoundPow(float val)
        {
            if (val > 1)
                val = (float)Math.Round(val);
            else
                val = (float)Math.Pow(2, Math.Round(Math.Log(val, 2)));
            return val;
        }

        public static float ToGoodPixelScaleFloorDivide(float val, float divide = 16)
        {
            if (val > 1)
                val = (float)Math.Floor(val);
            else
                val = (float)Math.Max(1, Math.Floor(val * divide)) / divide;
            return val;
        }
        public static float ToGoodPixelScaleCeilingDivide(float val, float divide = 16)
        {
            if (val > 1)
                val = (float)Math.Ceiling(val);
            else
                val = (float)Math.Max(1, Math.Ceiling(val * divide)) / divide;
            return val;
        }
        public static float ToGoodPixelScaleRoundDivide(float val, float divide = 16)
        {
            if (val > 1)
                val = (float)Math.Round(val);
            else
                val = (float)Math.Max(1, Math.Round(val * divide)) / divide;
            return val;
        }

#endregion

        #region Data

                //Bits to Bytes
                public static byte BitsToByte(bool[] data)
                {
                    byte result = 0;
                    for (int i = 0; i < data.Length && i < 8; i++)
                    {
                        if (data[i])
                            result += (byte)Math.Pow(2, i);
                    }
                    return result;
                }
                public static byte[] BitsToBytes(bool[] data)
                {
                    byte[] result = new byte[(int)Math.Ceiling(data.Length / 8f)];
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i])
                            result[(int)Math.Floor(i / 8f)] += (byte)Math.Pow(2, i % 8);
                    }
                    return result;
                }
                public static long BitsToLong(bool[] data)
                {
                    long result = 0;
                    for (int i = 0; i < data.Length && i < 64; i++)
                    {
                        if (data[i])
                            result += (long)Math.Pow(2, i);
                    }
                    return result;
                }

                //Bytes to Bits
                public static bool[] ByteToBits(byte data)
                {
                    bool[] result = new bool[8];
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = (data & (1 << i)) != 0;
                    }
                    return result;
                }
                public static bool[] BytesToBits(byte[] data)
                {
                    bool[] result = new bool[8 * data.Length];
                    for (int i = 0; i < result.Length; i++)
                    {
                        result[i] = (data[(int)Math.Floor(i / 8f)] & (1 << (i % 8))) != 0;
                    }
                    return result;
                }

                //Bytes to ints
                public static int[] BytesToInts(byte[] data)
                {
                    int[] output = new int[data.Length / 4];
                    for (int i = 0; i < data.Length; i += 4)
                    {
                        output[i / 4] = BitConverter.ToInt32(data, i);
                    }
                    return output;
                }
                public static byte[] IntsToBytes(int[] data)
                {
                    List<byte> output = new List<byte>();
                    for (int i = 0; i < data.Length; i++)
                    {
                        output.AddRange(BitConverter.GetBytes(data[i]));
                    }
                    return output.ToArray();
                }

                #endregion

        #region Old

                //public static Vector2 LengthDir(Vector2 pos1, Vector2 pos2, float speed)
                //{
                //    speed = Math.Min(speed, (int)Vector2.Distance(pos1, pos2));
                //    double radians = Math.Atan2(pos2.Y - pos1.Y, pos2.X - pos1.X);
                //    return new Vector2((float)(pos1.X + speed * Convert.ToDouble(Math.Cos(radians))), (float)(pos1.Y + speed * Convert.ToDouble(Math.Sin(radians))));
                //}

                //public static int RoundTo(float num, float to)
                //{
                //    int back = (int)(Math.Round(num / to) * to);
                //    return back;
                //}

                //public static Vector2 To2D(int index, int width)
                //{
                //    return new Vector2(index % width, (float)Math.Floor((float)index / width));
                //}
                //public static int To1D(Vector2 pos, int width)
                //{
                //    return (int)Math.Floor(pos.X + (int)pos.Y * width);
                //}
                //public static bool[] StringToBoolArray(string bits)
                //{
                //    bool[] result = new bool[bits.Length];
                //    for (int i = 0; i < bits.Length; i++)
                //    {
                //        if (bits[i] == '1')
                //            result[i] = true;
                //    }
                //    return result;
                //}

        #endregion

        #region Vectors

                public static Vector2 FloorVector(this Vector2 vec)
                {
                    return new Vector2((float)Math.Floor(vec.X), (float)Math.Floor(vec.Y));
                }
                public static Vector2 CeilVector(this Vector2 vec)
                {
                    return new Vector2((float)Math.Ceiling(vec.X), (float)Math.Ceiling(vec.Y));
                }
                public static Vector2 RoundVector(this Vector2 vec)
                {
                    return new Vector2((float)Math.Round(vec.X), (float)Math.Round(vec.Y));
                }
                /// <summary>
                /// use this method, if you have many values either being x.5 or x.0, cause this will prevent unwanted floating point errors
                /// </summary>
                public static Vector2 RoundVectorCustom(this Vector2 vec)
                {
                    return new Vector2((float)Math.Round(vec.X - 0.1f), (float)Math.Round(vec.Y - 0.1f));
                }
                public static Vector2 RoundInDirection(this Vector2 vec, Vector2 dir)
                {
                    return new Vector2((dir.X > 0) ? (float)Math.Ceiling(vec.X) : (float)Math.Floor(vec.X), (dir.Y > 0) ? (float)Math.Ceiling(vec.Y) : (float)Math.Floor(vec.Y));
                }

                public static Vector2 AbsVector(this Vector2 pos)
                {
                    return new Vector2(Math.Abs(pos.X), Math.Abs(pos.Y));
                }
                public static Vector2 SignVector(this Vector2 pos)
                {
                    return new Vector2(Math.Sign(pos.X), Math.Sign(pos.Y));
                }

                public static T EnumIncrement<T>(T src) where T : struct
                {
                    if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argumnent {0} is not an Enum", typeof(T).FullName));

                    T[] Arr = (T[])Enum.GetValues(src.GetType());
                    int j = Array.IndexOf<T>(Arr, src) + 1;
                    return (Arr.Length == j) ? Arr[0] : Arr[j];
                }

                public static Vector2 XPositive(Vector2 vec)
                {
                    if (vec.X < 0 || (vec.X == 0 && vec.Y < 0))
                        return -vec;
                    return vec;
                }
                public static Vector2 YPositive(Vector2 vec)
                {
                    if (vec.Y < 0 || (vec.Y == 0 && vec.X < 0))
                        return -vec;
                    return vec;
                }

                public static void VectorRectangle(ref Vector2 pos1, ref Vector2 pos2)
                {
                    Vector2 _pos1 = pos1;
                    Vector2 _pos2 = pos2;
                    pos1 = MinVector(_pos1, _pos2);
                    pos2 = MaxVector(_pos1, _pos2);
                }

                public static Vector2 MinVector(this Vector2 vec1, Vector2 vec2)
                {
                    return new Vector2(Math.Min(vec1.X, vec2.X), Math.Min(vec1.Y, vec2.Y));
                }
                public static Vector2 MaxVector(this Vector2 vec1, Vector2 vec2)
                {
                    return new Vector2(Math.Max(vec1.X, vec2.X), Math.Max(vec1.Y, vec2.Y));
                }
                public static Vector2 MinAbsVector(this Vector2 vec1, Vector2 vec2)
                {
                    return new Vector2(MinAbs(vec1.X, vec2.X), MinAbs(vec1.Y, vec2.Y));
                }
                public static Vector2 MaxAbsVector(this Vector2 vec1, Vector2 vec2)
                {
                    return new Vector2(MaxAbs(vec1.X, vec2.X), MaxAbs(vec1.Y, vec2.Y));
                }
                public static Vector2 ClampVector(this Vector2 vec, Vector2 min, Vector2 max)
                {
                    return new Vector2(Math.Max(Math.Min(vec.X, max.X), min.X), Math.Max(Math.Min(vec.Y, max.Y), min.Y));
                }

                public static Rectangle NormalizeRectangle(Rectangle rect)
                {
                    if (rect.Width < 0)
                    {
                        rect.X += rect.Width;
                        rect.Width = -rect.Width;
                    }
                    if (rect.Height < 0)
                    {
                        rect.Y += rect.Height;
                        rect.Height = -rect.Height;
                    }
                    return rect;
                }

                public static Rectangle CreateRectangle(Vector2 pos1, Vector2 pos2)
                {
                    if (pos2.X < pos1.X)
                    {
                        float save = pos1.X;
                        pos1.X = pos2.X;
                        pos2.X = save;
                    }
                    if (pos2.Y < pos1.Y)
                    {
                        float save = pos1.Y;
                        pos1.Y = pos2.Y;
                        pos2.Y = save;
                    }
                    return new Rectangle((int)pos1.X, (int)pos1.Y, (int)(pos2.X - pos1.X), (int)(pos2.Y - pos1.Y));
                }

                public static Rectangle Cut(this Rectangle rect1, Rectangle rect2)
                {
                    int left = Math.Max(rect1.Left, rect2.Left);
                    int w = Math.Min(rect1.Right, rect2.Right) - left;
                    int top = Math.Max(rect1.Top, rect2.Top);
                    int h = Math.Min(rect1.Bottom, rect2.Bottom) - top;
                    if (w > 0 && h > 0)
                        return new Rectangle(left, top, w, h);
                    else
                        return default(Rectangle);
                }
                public static Rectangle Expand(this Rectangle rect1, int x, int y)
                {
                    if (x > 0)
                        rect1.Width += x;
                    else if (x < 0)
                    {
                        rect1.X += x;
                        rect1.Width -= x;
                    }

                    if (y > 0)
                        rect1.Height += y;
                    else if (y < 0)
                    {
                        rect1.Y += y;
                        rect1.Height -= y;
                    }

                    return rect1;
                }

                public static Vector2 ModulateVector(this Vector2 vec, Vector2 mod)
                {
                    return new Vector2(vec.X % mod.X, vec.Y % mod.Y);
                }
                public static Vector2 ModulateVector(this Vector2 vec, float mod)
                {
                    return new Vector2(vec.X % mod, vec.Y % mod);
                }

                //Rotation
                public static Vector2 AngleToVector(float angle)
                {
                    return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                }
                public static float VectorToAngle(Vector2 vec)
                {
                    return (float)Math.Atan2(vec.Y, vec.X);
                }

                public static float AngleDistance(float angle1, float angle2)
                {
                    angle1 = angle1 % MathHelper.TwoPi;
                    if (angle1 < 0)
                        angle1 += MathHelper.TwoPi;

                    angle2 = angle2 % MathHelper.TwoPi;
                    if (angle2 < 0)
                        angle2 += MathHelper.TwoPi;

                    float dist = angle2 - angle1;
                    if (Math.Abs(dist) <= Math.PI)
                        return dist;
                    else
                        return -Math.Sign(dist) * ((float)MathHelper.TwoPi - Math.Abs(dist));
                }

                public static float AngleDistance(Vector2 v1, Vector2 v2)
                {
                    return AngleDistance((float)Math.Atan2(v1.Y, v1.X), (float)Math.Atan2(v2.Y, v2.X));
                    //return (float)Math.Acos(Vector2.Dot(v1, v2) / v1.Length() / v2.Length());
                    //return (float)Math.Acos((v1 * v2).Length() / v1.Length() / v2.Length());
                }

                public static Vector2 InRangeVector(this Vector2 val, Vector2 min, Vector2 max)
                {
                    if (min.X > max.X || min.Y > max.Y)
                    {
                        throw new Exception("min is greater than max!");
                        //return val;
                    }
                    else
                    {
                        Vector2 range = max - min;

                        if (val.X >= max.X)
                        {
                            val.X -= min.X;
                            val.X %= range.X;
                            val.X += min.X;
                        }
                        else if (val.X < min.X)
                        {
                            val.X -= min.X;
                            val.X %= range.X;
                            val.X += range.X;
                            val.X += min.X;
                        }

                        if (val.Y >= max.Y)
                        {
                            val.Y -= min.Y;
                            val.Y %= range.Y;
                            val.Y += min.Y;
                        }
                        else if (val.Y < min.Y)
                        {
                            val.Y -= min.Y;
                            val.Y %= range.Y;
                            val.Y += range.Y;
                            val.Y += min.Y;
                        }

                        return val;
                    }
                }

                public static Vector3 AbsVector(this Vector3 pos)
                {
                    return new Vector3(Math.Abs(pos.X), Math.Abs(pos.Y), Math.Abs(pos.Z));
                }

                //public static Vector3 GetCoordinateScale(Matrix matrix)
                //{
                //    return new Vector3(matrix.Right.Length(), matrix.Up.Length(), matrix.Backward.Length());
                //}

                public static Vector2 GetNormalizedOrZero(this Vector2 v)
                {
                    if (v == Vector2.Zero)
                        return Vector2.Zero;
                    else
                        return Vector2.Normalize(v);
                }
                public static Vector3 GetNormalizedOrZero(this Vector3 v)
                {
                    if (v == Vector3.Zero)
                        return Vector3.Zero;
                    else
                        return Vector3.Normalize(v);
                }

                public static Vector2 GetSize(this Rectangle rectangle)
                {
                    return new Vector2(rectangle.Width, rectangle.Height);
                }

        #endregion

        #region Rectangles
        
        public static M_Rectangle Encapsulate(this M_Rectangle rect1, M_Rectangle rect2)
        {
            float x1, y1, x2, y2;
            x1 = Math.Min(rect1.Left, rect2.Left);
            y1 = Math.Min(rect1.Top, rect2.Top);
            x2 = Math.Max(rect1.Right, rect2.Right);
            y2 = Math.Max(rect1.Bottom, rect2.Bottom);
            return new M_Rectangle(x1, y1, x2 - x1, y2 - y1);
        }
        public static M_Rectangle EncapsulateSelf(this M_Rectangle rect1, M_Rectangle rect2)
        {
            float x1, y1, x2, y2;
            x1 = Math.Min(rect1.Left, rect2.Left);
            y1 = Math.Min(rect1.Top, rect2.Top);
            x2 = Math.Max(rect1.Right, rect2.Right);
            y2 = Math.Max(rect1.Bottom, rect2.Bottom);
            rect1.pos = new Vector2(x1, y1);
            rect1.size = new Vector2(x2 - x1, y2 - y1);
            return rect1;
        }

        #endregion

        #region Matrices

        public static string ToMyString(this Matrix matrix)
                {
                    return "{ " + matrix.M11 + " " + matrix.M12 + " " + matrix.M13 + " " + matrix.M14 + " | "
                         + matrix.M21 + " " + matrix.M22 + " " + matrix.M23 + " " + matrix.M24 + " | "
                         + matrix.M31 + " " + matrix.M32 + " " + matrix.M33 + " " + matrix.M34 + " | "
                         + matrix.M41 + " " + matrix.M42 + " " + matrix.M43 + " " + matrix.M44 + " }";
                }

                public static string ToMyStringDecompose(this Matrix matrix)
                {
                    Vector3 s, t;
                    Quaternion r;
                    matrix.Decompose(out s, out r, out t);
                    return "s:" + s + " r:" + r + " t:" + t;
                }

        #endregion

        #region Colors

                public static Color AddColors(Color c1, Color c2)
                {
                    c1 *= (1f - (c2.A / 255f));
                    c2 *= (c2.A / 255f);
                    return new Color(c1.R + c2.R, c1.G + c2.G, c1.B + c2.B, c1.A + c2.A);
                }

                public static Color AverageColor(Color c1, Color c2)
                {
                    return new Color((c1.R + c2.R) / 2, (c1.G + c2.G) / 2, (c1.B + c2.B) / 2, (c1.A + c2.A) / 2);
                }
                public static Color AverageColor(params Color[] colors)
                {
                    int r, g, b, a;
                    r = g = b = a = 0;
                    for (int i = 0; i < colors.Length; i++)
                    {
                        r += colors[i].R;
                        g += colors[i].G;
                        b += colors[i].B;
                        a += colors[i].A;
                    }
                    int l = colors.Length;
                    return new Color(r / l, g / l, b / l, a / l);
                }

                public static HSVColor ToHSV(this Color color)
                {
                    float r = color.R / 255f;
                    float g = color.G / 255f;
                    float b = color.B / 255f;

                    float cmax = Math.Max(r, Math.Max(g, b));
                    float cmin = Math.Min(r, Math.Min(g, b));
                    float dist = cmax - cmin;

                    HSVColor hsv = new HSVColor();
                    hsv.alpha = color.A;

                    if (dist == 0)
                        hsv.hue = 0;
                    else if (cmax == r)
                        hsv.hue = 60f * ((g - b) / dist % 6);
                    else if (cmax == g)
                        hsv.hue = 60 * ((b - r) / dist + 2);
                    else if (cmax == b)
                        hsv.hue = 60 * ((r - g) / dist + 4);
                    else
                    { }

                    if (cmax == 0)
                        hsv.saturation = 0;
                    else
                        hsv.saturation = dist / cmax;

                    hsv.value = cmax;

                    return hsv;
                }
                public static Color ToRGB(this HSVColor hsv)
                {
                    Color color;

                    float c = hsv.value * hsv.saturation;
                    float x = c * (1 - Math.Abs((hsv.hue / 60) % 2 - 1));
                    float m = hsv.value - c;

                    float r, g, b;
                    r = g = b = 0;

                    if (hsv.hue < 60)
                    {
                        r = c;
                        g = x;
                    }
                    else if (hsv.hue < 120)
                    {
                        g = c;
                        r = x;
                    }
                    else if (hsv.hue < 180)
                    {
                        g = c;
                        b = x;
                    }
                    else if (hsv.hue < 240)
                    {
                        b = c;
                        g = x;
                    }
                    else if (hsv.hue < 300)
                    {
                        b = c;
                        r = x;
                    }
                    else if (hsv.hue < 360)
                    {
                        r = c;
                        b = x;
                    }
                    else
                    { }

                    color = new Color((byte)(255 * (r + m)), (byte)(255 * (g + m)), (byte)(255 * (b + m)), hsv.alpha);
                    return color;
                }
                /// <summary>
                /// warning: could crash on some graphics cards
                /// </summary>
                public static void ChangeTextureColors(Texture2D tex, int[] findHues, Color[] newColors)
                {
                    Color[] colors = new Color[tex.Width * tex.Height];
                    tex.GetData<Color>(colors);

                    for (int i = 0; i < colors.Length; i++)
                    {
                        if (colors[i].A > 0)
                        {
                            Color c = colors[i];
                            HSVColor hsv = ToHSV(colors[i]);

                            for (int j = 0; j < findHues.Length; j++)
                            {
                                if (Math.Round(hsv.hue) == findHues[j])
                                {
                                    HSVColor hsvNew = ToHSV(newColors[j]);

                                    hsv.saturation = 0;
                                    hsv.hue = hsvNew.hue;
                                    hsv.saturation = hsvNew.saturation;
                                    hsv.value *= hsvNew.value;
                                    colors[i] = ToRGB(hsv);
                                    break;
                                }
                            }
                        }
                    }

                    tex.SetData<Color>(colors);
                }

                /// <summary>
                /// warning: could crash on some graphics cards
                /// </summary>
                public static void FillTextureWithColor(Texture2D tex, Color color)
                {
                    Color[] colors = new Color[tex.Width * tex.Height];
                    tex.GetData<Color>(colors);

                    for (int i = 0; i < colors.Length; i++)
                        if (colors[i].A > 0)
                            colors[i] = color;
                    tex.SetData<Color>(colors);
                }

                /// <summary>
                /// warning: could crash on some graphics cards
                /// </summary>
                public static Texture2D FillTextureWithColorClone(Texture2D tex, Color color)
                {
                    Color[] colors = new Color[tex.Width * tex.Height];
                    tex.GetData<Color>(colors);

                    for (int i = 0; i < colors.Length; i++)
                        if (colors[i].A > 0)
                            colors[i] = color;

                    tex = new Texture2D(DrawM.gDevice, tex.Width, tex.Height);

                    tex.SetData<Color>(colors);

                    return tex;
                }

                public static Color HexToColor(string hex)
                {
                    if (hex.Length == 1)
                        hex = new string(hex[0], 6);
                    if (hex.Length == 3)
                        hex = hex.Insert(0, hex[0].ToString()).Insert(2, hex[1].ToString()).Insert(4, hex[2].ToString());

                    byte r, g, b;
                    byte.TryParse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, new System.Globalization.CultureInfo("en-GB"), out r);
                    byte.TryParse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, new System.Globalization.CultureInfo("en-GB"), out g);
                    byte.TryParse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, new System.Globalization.CultureInfo("en-GB"), out b);

                    return new Color(r, g, b);
                }
                public static Color HexToColor(int hex)
                {
                    int r = (hex & 0xff0000) >> 16;
                    int g = (hex & 0x00ff00) >> 8;
                    int b = hex & 0x0000ff;

                    return new Color(r, g, b);
                }

                public static string ColorToHex(this Color color)
                {
                    string hex = "";
                    hex += color.R.ToString("X2");
                    hex += color.G.ToString("X2");
                    hex += color.B.ToString("X2");
                    return hex;
                }


                public static Color GetNextColor(Color color)
                {
                    if (color == Color.Blue)
                        return new Color(255, 255, 0);
                    else if (color == new Color(0, 255, 255))
                        return Color.White;
                    else if (color == Color.White)
                        return Color.Black;
                    else if (color == Color.Black)
                        return Color.Red;
                    else if ((color.R == 255 || color.R == 0) && (color.G == 255 || color.G == 0) && (color.B == 255 || color.B == 0))
                        return new Color(color.B, color.R, color.G);
                    else
                        return Color.Red;
                }
                public static Color GetPrevColor(Color color)
                {
                    if (color == Color.Red)
                        return Color.Black;
                    else if (color == Color.Black)
                        return Color.White;
                    else if (color == Color.White)
                        return new Color(0, 255, 255);
                    else if (color == new Color(255, 255, 0))
                        return Color.Blue;
                    else if ((color.R == 255 || color.R == 0) && (color.G == 255 || color.G == 0) && (color.B == 255 || color.B == 0))
                        return new Color(color.G, color.B, color.R);
                    else
                        return Color.Red;
                }

                public static Color RandomRGB(Random rand)
                {
                    return new Color(rand.Next(256), rand.Next(256), rand.Next(256));
                }

                public static float GetColorBrightness(this Color color)
                {
                    Vector3 v = color.ToVector3();
                    return 0.2126f * v.X + 0.7152f * v.Y + 0.0722f * v.Z;
                }
                public static float GetColorBrightness(Vector3 color)
                {
                    return 0.2126f * color.X + 0.7152f * color.Y + 0.0722f * color.Z;
                }

                public static Color GetChangeA(this Color color, byte a)
                {
                    color.A = a;
                    return color;
                }
                public static Color GetChangeR(ref Color color, byte r)
                {
                    color.R = r;
                    return color;
                }
                public static Color GetChangeG(ref Color color, byte g)
                {
                    color.G = g;
                    return color;
                }
                public static Color GetChangeB(ref Color color, byte b)
                {
                    color.B = b;
                    return color;
                }

        #endregion

        #region Time

                public static DateTime ConvertFromUnixTimestamp(TimeSpan timestamp)
                {
                    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    return origin.Add(timestamp);
                }

                public static TimeSpan ConvertToUnixTimestamp(DateTime date)
                {
                    DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    TimeSpan diff = date.ToUniversalTime() - origin;
                    return diff;
                }

        #endregion

        #region Grid

                public static List<Int2> GetSaveCoords(M_Rectangle rect, int gridSize, int levelWidth, int levelHeight)
                {
                    int w = (int)Math.Max(1, Math.Ceiling(rect.size.X / gridSize));
                    int h = (int)Math.Max(1, Math.Ceiling(rect.size.Y / gridSize));

                    int sx = (int)Math.Floor(rect.X / gridSize);
                    int sy = (int)Math.Floor(rect.Y / gridSize);

                    return GetSaveCoords(sx, sy, w, h, gridSize, levelWidth, levelHeight); ;
                }
                public static List<Int2> GetSaveCoords(int sx, int sy, int w, int h, int gridSize, int levelWidth, int levelHeight)
                {
                    List<Int2> coords = new List<Int2>();

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            Int2 coord = new Int2(sx + x, sy + y);
                            coord.SetInRect(0, 0, levelWidth - 1, levelHeight - 1);
                            if (!coords.Contains(coord))
                                coords.Add(coord);
                        }
                    }

                    return coords;
                }

                public static List<Int2> GetPossibleColCoords(M_Rectangle rect, int gridSize, int levelWidth, int levelHeight)
                {
                    List<Int2> coords = new List<Int2>();

                    int x2 = (int)Math.Max(0, Math.Min(levelWidth - 1, Math.Floor((rect.X + rect.size.X) / gridSize)));
                    int y2 = (int)Math.Max(0, Math.Min(levelHeight - 1, Math.Floor((rect.Y + rect.size.Y) / gridSize)));

                    for (int y1 = (int)Math.Max(0, Math.Floor(rect.Y / gridSize) - 1); y1 <= y2; y1++)
                    {
                        for (int x1 = (int)Math.Max(0, Math.Floor(rect.X / gridSize) - 1); x1 <= x2; x1++)
                        {
                            coords.Add(new Int2(x1, y1));
                        }
                    }

                    return coords;
                }

        #endregion

        #region Path

                public static string GetFreePathWish(string wishPath, string ext)
                {
                    string newPath = wishPath + ext;
                    int j = 0;
                    while (File.Exists(newPath))
                    {
                        newPath = wishPath + j + ext;
                        j++;
                    }
                    return newPath;
                }
                public static string GetFreePathOrig(string origPath, string input)
                {
                    string outputExt = Path.GetExtension(origPath);
                    string outputPath = origPath.Remove(origPath.Length - outputExt.Length);

                    return GetFreePathWish(outputPath + input, outputExt);
                }

                public static string GetPathLocalIfPossible(string path)
                {
                    int cDirLength = G.exeDir.Length;
                    if (path.Length >= cDirLength && path.Substring(0, cDirLength) == G.exeDir)
                    {
                        return path.Substring(cDirLength);
                    }
                    else
                        return path;
                }

                public static void CopyDirectory(string dirIn, string dirOut)
                {
                    if (!Directory.Exists(dirOut))
                        Directory.CreateDirectory(dirOut);

                    //Now Create all of the directories
                    foreach (string dirPath in Directory.GetDirectories(dirIn, "*",
                        SearchOption.AllDirectories))
                        Directory.CreateDirectory(dirPath.Replace(dirIn, dirOut));
                    //Copy all the files & Replaces any files with the same name
                    foreach (string newPath in Directory.GetFiles(dirIn, "*.*",
                        SearchOption.AllDirectories))
                        File.Copy(newPath, newPath.Replace(dirIn, dirOut), true);
                }

                public static string[] GetFilesFromPaths(string[] paths, string regex = "*", bool recursively = true)
                {
                    Regex mask = new Regex(regex.Replace(".", "[.]").Replace("*", ".*"));//.Replace("?", "."));

                    SearchOption so = recursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    List<string> files = new List<string>();
                    for (int i = 0; i < paths.Length; i++)
                    {
                        FileAttributes attr = File.GetAttributes(paths[i]);
                        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            //dir
                            string[] dirFiles = Directory.GetFiles(paths[i], "*", so).Where(f => mask.IsMatch(f)).ToArray();
                            for (int j = 0; j < dirFiles.Length; j++)
                            {
                                if (!files.Contains(dirFiles[j]))
                                    files.Add(dirFiles[j]);
                            }
                        }
                        else if (!files.Contains(paths[i]) && FitsMask(paths[i], regex)) //file
                            files.Add(paths[i]);
                    }
                    return files.ToArray();
                }
                private static bool FitsMask(string name, string regex)
                {
                    Regex mask = new Regex(regex.Replace(".", "[.]").Replace("*", ".*"));//.Replace("?", "."));
                    return mask.IsMatch(name);
                }
                /*
                //public static string[] GetFilesFromPathsAllowDuplicates(string[] args, string searchPattern = "*", bool recursively = true)
                //{
                //    SearchOption so = recursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                //    List<string> files = new List<string>();
                //    for (int i = 0; i < args.Length; i++)
                //    {
                //        FileAttributes attr = File.GetAttributes(args[i]);
                //        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                //            files.AddRange(Directory.GetFiles(args[i], searchPattern, so));
                //        else
                //            files.Add(args[i]);
                //    }
                //    return files.ToArray();
                //}

                //public static void ForEachFileInDirRecursive(string dir, string filePattern, string folderPattern, Action<string> action)
                //{
                //    foreach (string file in Directory.GetFiles(dir, filePattern, SearchOption.TopDirectoryOnly))
                //    {

                //        File.Delete(file);
                //    }

                //    foreach (string folder in Directory.GetDirectories(dir, folderPattern, SearchOption.TopDirectoryOnly))
                //    {

                //    }
                //}
                */

                //used in Find_You and somewhere else...?
                public static string GetPathFromIndexRelativeToPath(string path, int indexRelative, string regex, bool returnFirstIfPathNotFound, bool recursively = false)
                {
                    string directory = Path.GetDirectoryName(path);
                    string[] files = GetFilesFromPaths(new string[] { directory }, regex, recursively).OrderBy(f => f).ToArray();

                    if (files.Length == 0)
                        return "";

                    for (int i = 0; i < files.Length; i++)
                    {
                        if (files[i] == path)
                        {
                            int index = (i + indexRelative) % files.Length;
                            if (index < 0)
                                index += files.Length;

                            return files[index];
                        }
                    }
                    if (returnFirstIfPathNotFound)
                        return files[0];
                    else
                        return "";
                }
                public static string GetPathFromIndex(string directory, int index, string regex, bool recursively = false)
                {
                    if (directory == "")
                        return "";
                    string[] files = GetFilesFromPaths(new string[] { directory }, regex, recursively).OrderBy(f => f).ToArray();

                    if (files.Length > index)
                        return files[index];
                    else
                        return "";
                }

                public static string GetParentPath(string path, int recursively = 1)
                {
                    DirectoryInfo info = Directory.GetParent(path);
                    for (int i = 1; i < recursively; i++)
                    {
                        info = info.Parent;

                        if (info == null)
                            return null;
                    }
                    return info.FullName;
                }

        #endregion

        #region Effect

                public static void SetWorldAndInvTransp(this Effect effect, Matrix world)
                {
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Transpose(world)));
                }

        #endregion

        #region Cursor

        #if WINDOWS1

                public static Cursor GenerateCursor(byte[] data)
                {
                    return LoadCursorFromResource(data);
                }
                [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
                private static extern IntPtr LoadCursorFromFile(string path);

                public static Cursor LoadCursorFromResource(byte[] data)  // Assuming that the resource is an Icon, but also could be a Image or a Bitmap
                {
                    string fileName = Path.GetTempPath() + Guid.NewGuid().ToString() + ".cur";
                    File.WriteAllBytes(fileName, data);
                    Cursor result = new Cursor(LoadCursorFromFile(fileName));
                    File.Delete(fileName);
                    return result;
                }

        #endif

        #endregion

        #region String

                        public static string StringReplaceUnresolvableCharacters(this SpriteFont font, string text, string replaceWith)
                        {
                            for (int i = text.Length - 1; i >= 0; i--)
                            {
                                if (!font.Characters.Contains(text[i]))
                                    text = text.Remove(i, 1).Insert(i, replaceWith);
                            }
                            return text;
                        }

                        public static bool CompareSubstring(this string _source, int _indexInSource, string _substring)
                        {
                            if (_source == null || _substring == null)
                                return false;
            
                            if (_source.Length < _indexInSource + _substring.Length)
                                return false;

                            for (int i = 0; i < _substring.Length; i++)
                            {
                                if (_source[_indexInSource + i] != _substring[i])
                                    return false;
                            }
                            return true;
                        }

        #endregion

        #region Binary Reader/Writer

                        public static void Write(this BinaryWriter writer, bool[] bools)
                        {
                            for (int i = 0; i < bools.Length; i++)
                                writer.Write(bools[i]);
                        }
                        public static bool[] ReadBooleans(this BinaryReader reader, int count)
                        {
                            bool[] bools = new bool[count];
                            for (int i = 0; i < bools.Length; i++)
                            {
                                bools[i] = reader.ReadBoolean();
                            }
                            return bools;
                        }

                        public static void Write(this BinaryWriter writer, Vector2 vec)
                        {
                            writer.Write(vec.X);
                            writer.Write(vec.Y);
                        }
                        public static Vector2 ReadVector2(this BinaryReader reader)
                        {
                            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
                        }

                        public static void Write(this BinaryWriter writer, Color color)
                        {
                            writer.Write(color.PackedValue);
                        }
                        public static Color ReadColor(this BinaryReader reader)
                        {
                            Color color = new Color(reader.ReadUInt32());
                            return color;
                        }

                        public static void Write(this BinaryWriter writer, M_Rectangle rect)
                        {
                            writer.Write(rect.pos.X);
                            writer.Write(rect.pos.Y);
                            writer.Write(rect.size.X);
                            writer.Write(rect.size.Y);
                        }
                        public static M_Rectangle ReadRectangle(this BinaryReader reader)
                        {
                            return new M_Rectangle(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                        }


        #endregion

        #region Map

                        // source: https://stackoverflow.com/questions/10966331/two-way-bidirectional-dictionary-in-c
                        public class Map<T1, T2>
                        {
                            private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
                            private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

                            public Map()
                            {
                                this.Forward = new Indexer<T1, T2>(_forward);
                                this.Backward = new Indexer<T2, T1>(_reverse);
                            }

                            public class Indexer<T3, T4>
                            {
                                private Dictionary<T3, T4> _dictionary;
                                public Indexer(Dictionary<T3, T4> dictionary)
                                {
                                    _dictionary = dictionary;
                                }
                                public T4 this[T3 index]
                                {
                                    get { return _dictionary[index]; }
                                    set { _dictionary[index] = value; }
                                }
                            }

                            public void Add(T1 t1, T2 t2)
                            {
                                _forward.Add(t1, t2);
                                _reverse.Add(t2, t1);
                            }

                            public Indexer<T1, T2> Forward { get; private set; }
                            public Indexer<T2, T1> Backward { get; private set; }
                        }

        #endregion

        #region Texture

                        public static Texture2D GenerateOutline4Directions(this Texture2D source, GraphicsDevice gDevice, Color? color = null, bool mergeWithSource = false)
                        {
                            Color[] colorOut = GenerateOutline4Directions(source, color, mergeWithSource);

                            Texture2D outline = new Texture2D(gDevice, source.Width, source.Height);
                            outline.SetData(colorOut);
                            return outline;
                        }
                        public static void GenerateOutline4DirectionsRef(this Texture2D source, GraphicsDevice gDevice, Color? color = null, bool mergeWithSource = true)
                        {
                            Color[] colorOut = GenerateOutline4Directions(source, color, mergeWithSource);

                            source.SetData(colorOut);
                        }

                        public static Color[] GenerateOutline4Directions(this Texture2D source, Color? color = null, bool mergeWithSource = false)
                        {
                            Color[] colorOut = new Color[source.Width * source.Height];
                            Color[] colorIn = new Color[source.Width * source.Height];
                            source.GetData(colorIn);

                            Color c = color ?? Color.White;

                            Shift(colorIn, colorOut, source.Width, source.Height, 1, 0, c);
                            Shift(colorIn, colorOut, source.Width, source.Height, -1, 0, c);
                            Shift(colorIn, colorOut, source.Width, source.Height, 0, 1, c);
                            Shift(colorIn, colorOut, source.Width, source.Height, 0, -1, c);

                            if (mergeWithSource)
                            {
                                for (int i = 0; i < colorOut.Length; i++)
                                {
                                    if (colorIn[i].A != 0)
                                        colorOut[i] = colorIn[i];
                                }
                            }

                            return colorOut;
                        }

                        public static void GenerateOutline8DirectionsRef(this Texture2D source, GraphicsDevice gDevice, Color? color = null, bool mergeWithSource = true)
                        {
                            Color[] colorOut = GenerateOutline8Directions(source, color, mergeWithSource);
            
                            source.SetData(colorOut);
                        }

                        public static Texture2D GenerateOutline8Directions(this Texture2D source, GraphicsDevice gDevice, Color? color = null, bool mergeWithSource = false)
		                {
                            Color[] colorOut = GenerateOutline8Directions(source, color, mergeWithSource);

                            Texture2D outline = new Texture2D(gDevice, source.Width, source.Height);
			                outline.SetData(colorOut);
			                return outline;
		                }

                        private static Color[] GenerateOutline8Directions(Texture2D source, Color? color = null, bool mergeWithSource = false)
                        {
                            Color[] colorOut = new Color[source.Width * source.Height];
                            Color[] colorIn = new Color[source.Width * source.Height];
                            source.GetData(colorIn);

                            Color c = color ?? Color.White;

                            Shift(colorIn, colorOut, source.Width, source.Height, 1, 0, c);
                            Shift(colorIn, colorOut, source.Width, source.Height, -1, 0, c);
                            Shift(colorIn, colorOut, source.Width, source.Height, 0, 1, c);
                            Shift(colorIn, colorOut, source.Width, source.Height, 0, -1, c);

                            Shift(colorIn, colorOut, source.Width, source.Height, 1, 1, c);
                            Shift(colorIn, colorOut, source.Width, source.Height, -1, 1, c);
                            Shift(colorIn, colorOut, source.Width, source.Height, -1, -1, c);
                            Shift(colorIn, colorOut, source.Width, source.Height, 1, -1, c);

                            if (mergeWithSource)
                            {
                                for (int i = 0; i < colorOut.Length; i++)
                                {
                                    if (colorIn[i].A != 0)
                                        colorOut[i] = colorIn[i];
                                }
                            }

                            return colorOut;
                        }

                        public static Texture2D GenerateColorize(this Texture2D source, GraphicsDevice gDevice, Color? color = null)
                        {
                            Color myColor;
                            if (color == null)
                                myColor = Color.White;
                            else
                                myColor = color.Value;
                            Color[] colors = new Color[source.Width * source.Height];
                            source.GetData(colors);
                            for (int i = 0; i < colors.Length; i++)
                            {
                                colors[i].R = (byte)((myColor.R * colors[i].A) / 255);
                                colors[i].G = (byte)((myColor.G * colors[i].A) / 255);
                                colors[i].B = (byte)((myColor.B * colors[i].A) / 255);
                            }
                            return colors.ToTexture(source.Width, gDevice);
                        }

                        private static void Shift(Color[] colorIn, Color[] colorOut, int w, int h, int shiftX, int shiftY, Color color)
		                {
			                int w2 = Math.Min(w, w - shiftX);
			                int h2 = Math.Min(h, h - shiftY);
			                for (int y = Math.Max(-shiftY, 0); y < h2; y++)
			                {
				                for (int x = Math.Max(-shiftX, 0); x < w2; x++)
				                {
					                if (colorIn[y * w + x].A != 0 && colorIn[(y + shiftY) * w + (x + shiftX)].A == 0)
						                colorOut[(y + shiftY) * w + (x + shiftX)] = color;
				                }
			                }
		                }

		                public static Texture2D GenerateGlow(this Texture2D source, int glowThickness, GraphicsDevice gDevice, Color? glowColor = null, bool mergeWithSource = false)
		                {
			                Color[] colorIn = new Color[source.Width * source.Height];
			                source.GetData(colorIn);
			                return GenerateGlow(colorIn, source.Width, source.Height, glowThickness, gDevice, glowColor, mergeWithSource);
		                }

                        public static void GenerateGlowRef(this Texture2D source, int glowThickness, GraphicsDevice gDevice, Color? glowColor = null, bool mergeWithSource = true)
                        {
                            Color[] colorIn = new Color[source.Width * source.Height];
                            source.GetData(colorIn);
                            source.SetData(GenerateGlow(colorIn, source.Width, source.Height, glowThickness, glowColor, mergeWithSource));
                        }

		                public static Texture2D GenerateGlow(Color[] colorIn, int w, int h, int glowThickness, GraphicsDevice gDevice, Color? glowColor = null, bool mergeWithSource = false)
		                {
                            return GenerateGlow(colorIn, w, h, glowThickness, glowColor, mergeWithSource).ToTexture(w, gDevice);
                        }
                        public static Color[] GenerateGlow(Color[] colorIn, int w, int h, int glowThickness, Color? glowColor = null, bool mergeWithSource = false)
                        {
                            Color[] colorOut = new Color[w * h];

                            Color color = glowColor ?? Color.White;

                            for (int i = 0; i < glowThickness; i++)
                            {
                                Color cColor = color * ((float)(glowThickness - i) / glowThickness);
                                // shift
                                Shift(colorIn, colorOut, w, h, 1, 0, cColor);
                                Shift(colorIn, colorOut, w, h, -1, 0, cColor);
                                Shift(colorIn, colorOut, w, h, 0, 1, cColor);
                                Shift(colorIn, colorOut, w, h, 0, -1, cColor);

                                // apply glow to source color
                                for (int j = 0; j < colorIn.Length; j++)
                                {
                                    if (colorOut[j].A != 0)
                                        colorIn[j] = colorOut[j];
                                }
                            }

                            if (mergeWithSource)
                            {
                                for (int i = 0; i < colorOut.Length; i++)
                                {
                                    if (colorIn[i].A != 0)
                                        colorOut[i] = colorIn[i];
                                }
                            }

                            return colorOut;
                        }

                        public static Color[] OverrideColorsBasic(params Texture2D[] texs)
		                {
			                Color[] output = new Color[texs[0].Width * texs[0].Height];

			                for (int j = 0; j < texs.Length; j++)
			                {
				                Color[] input = texs[j].ToColor();
				                for (int i = 0; i < output.Length; i++)
				                {
					                if (input[i].A != 0)
						                output[i] = input[i];
				                }
			                }

			                return output;
		                }

		                public static Color[] ToColor(this Texture2D tex)
		                {
			                Color[] colors = new Color[tex.Width * tex.Height];
                            tex.GetData(colors);
			                return colors;
		                }

		                public static Texture2D ToTexture(this Color[] colors, int w, GraphicsDevice gDevice)
		                {
			                Texture2D tex = new Texture2D(gDevice, w, colors.Length / w);
			                tex.SetData(colors);
			                return tex;
		                }

		                public static Vector2 GetSize(this Texture2D tex)
		                {
			                return new Vector2(tex.Width, tex.Height);
		                }
		                public static Int2 GetSizeInt(this Texture2D tex)
		                {
			                return new Int2(tex.Width, tex.Height);
		                }

                        public static Texture2D BlendOver(this Texture2D tex1, Texture2D tex2, GraphicsDevice gDevice)
                        {
                            if (tex1.Width != tex2.Width || tex1.Height != tex2.Height)
                                return null;

                            Color[] colors = tex1.ToColor();
                            Color[] colors2 = tex2.ToColor();
                            for (int i = 0; i < colors.Length; i++)
                            {
                                colors[i] = colors[i].BlendOver(colors2[i]);
                            }
                            Texture2D tex = new Texture2D(gDevice, tex1.Width, tex1.Height);
                            tex.SetData(colors);
                            return tex;
                        }

                        public static void InvertRef(this Texture2D tex)
                        {
                            Color[] colors = tex.ToColor();
                            for (int i = 0; i < colors.Length; i++)
                            {
                                if (colors[i].A != 0)
                                {
                                    colors[i].R = (byte)(255 - colors[i].R);
                                    colors[i].G = (byte)(255 - colors[i].G);
                                    colors[i].B = (byte)(255 - colors[i].B);
                                }
                            }
                            tex.SetData(colors);
                        }
                        public static Texture2D InvertCopy(this Texture2D tex, GraphicsDevice gDevice)
                        {
                            Color[] colors = tex.ToColor();
                            for (int i = 0; i < colors.Length; i++)
                            {
                                if (colors[i].A != 0)
                                {
                                    colors[i].R = (byte)(255 - colors[i].R);
                                    colors[i].G = (byte)(255 - colors[i].G);
                                    colors[i].B = (byte)(255 - colors[i].B);
                                }
                            }
                            Texture2D texOut = new Texture2D(gDevice, tex.Width, tex.Height);
                            tex.SetData(colors);
                            return tex;
                        }

                        public static Color BlendOver(this Color c1, Color c2)
                        {
                            float a = c1.A / 255f;
                            float aInv = 1f - a;
                            c2.R = (byte)(c1.R * a + c2.R * aInv);
                            c2.G = (byte)(c1.G * a + c2.G * aInv);
                            c2.B = (byte)(c1.B * a + c2.B * aInv);
                            c2.A = (byte)(c2.A + ((255f - c2.A) * (c1.A / 255f)));
                            return c2;
                        }

        #endregion

        #region GraphicsDevice

                        public static Vector2 PreferredBackBufferSize(this GraphicsDeviceManager gdm)
                        {
                            return new Vector2(gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight);
                        }

        #endregion

        #region List

                        public static void AddShuffled<T>(this List<T> listReceive, List<T> listGive, Random rand)
                        {
                            for (int i = 0; i < listGive.Count; i++)
                            {
                                listReceive.Insert(rand.Next(listReceive.Count + 1), listGive[i]);
                            }
                        }

                        public static void Shuffle<T>(this List<T> list, Random rand)
                        {
                            int c = list.Count;
                            // duplicate elements randomly to the end of the list (first half is old, last half is randomly new)
                            for (int i = 0; i < c; i++)
                            {
                                list.Insert(c + rand.Next(i + 1), list[i]);
                            }
                            // remove old half
                            while (c > 0)
                            {
                                list.RemoveAt(0);
                                c--;
                            }
                        }

                        public static T RemoveAtGet<T>(this List<T> list, int index)
                        {
                            T get = list[index];
                            list.RemoveAt(index);
                            return get;
                        }

        #endregion

        #region Random

                        public static float NextFloat(this Random rand) => (float)rand.NextDouble();
                        public static float NextFloat(this Random rand, float min, float max) => (float)rand.NextDouble() * (max - min) + min;

        #endregion

        #region Float


                        [StructLayout(LayoutKind.Explicit)]
                        struct FloatIntUnion
                        {
                            [FieldOffset(0)]
                            public int i;
                            [FieldOffset(0)]
                            public float f;
                        }

                        //  Returns the next float after x in the direction of y.
                        public static float NextAfter(float x, float y)
                        {
                            if (float.IsNaN(x) || float.IsNaN(y)) return x + y;
                            if (x == y) return y;  // nextafter(0, -0) = -0

                            FloatIntUnion u;
                            u.i = 0; u.f = x;  // shut up the compiler

                            if (x == 0)
                            {
                                u.i = 1;
                                return y > 0 ? u.f : -u.f;
                            }

                            if ((x > 0) == (y > x))
                                u.i++;
                            else
                                u.i--;
                            return u.f;
                        }

        #endregion
    }


    public struct HSVColor
    {
        private float _hue, _saturation, _value;
        public byte alpha;

		public HSVColor SetHSVA(float _h, float _s, float _v, byte _a)
		{
			_hue = _h;
			_saturation = _s;
			_value = _v;
			alpha = _a;
			return this;
		}

        public float hue
        {
            get { return _hue; }
            set
            {
                if (value < 0f)
                    _hue = (360f + value) % 360f;
                else
                    _hue = value % 360f;
            }
        }

        public float saturation
        {
            get { return _saturation; }
            set
            {
                if (value > 1f)
                    _saturation = 1f;
                else if (value < 0f)
                    _saturation = 0f;
                else
                    _saturation = value;
            }
        }

        public float value
        {
            get { return _value; }
            set
            {
                if (value > 1f)
                    _value = 1f;
                else if (value < 0f)
                    _value = 0f;
                else
                    _value = value;
            }
        }

        public HSVColor SetHue(float _hue)
        {
            hue = _hue;
            return this;
        }
        public HSVColor SetValue(float _value)
        {
            value = _value;
            return this;
        }
        public HSVColor SetSaturation(float _saturation)
        {
            saturation = _saturation;
            return this;
        }
        public HSVColor AddHue(float _hue)
        {
            hue += _hue;
            return this;
        }
        public HSVColor AddValue(float _value)
        {
            value += _value;
            return this;
        }
        public HSVColor AddSaturation(float _saturation)
        {
            saturation += _saturation;
            return this;
        }
        public HSVColor TimesHue(float _hue)
        {
            hue *= _hue;
            return this;
        }
        public HSVColor TimesValue(float _value)
        {
            value *= _value;
            return this;
        }
        public HSVColor TimesSaturation(float _saturation)
        {
            saturation *= _saturation;
            return this;
        }
    }
}
