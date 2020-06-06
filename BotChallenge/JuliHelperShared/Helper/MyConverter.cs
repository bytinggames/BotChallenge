using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using System.Globalization;

namespace JuliHelper
{
    public static class MyConverter
    {
        public static bool TryParse<T>(string input, out T output)
        {
            return TryParse<T>(input, out output, typeof(T));
        }
        public static bool TryParseRef<T>(string input, ref T output)
        {
            return TryParseRef<T>(input, ref output, typeof(T));
        }
        public static bool TryParseCatch<T>(string input, out T output)
        {
            return TryParseCatch<T>(input, out output, typeof(T));
        }
        public static bool TryParseCatchRef<T>(string input, ref T output)
        {
            return TryParseCatchRef<T>(input, ref output, typeof(T));
        }

        public static bool TryParse<T>(string input, out T output, Type typeOut)
        {
            Type typeIn = typeof(string);

            if (typeOut == typeof(Color))
            {
                if (input.Length == 3)
                {
                    input = new string(new char[] { input[0], input[0], input[1], input[1], input[2], input[2] });
                }
                else if (input.Length == 1)
                {
                    input = new string(input[0], 6);
                }

                if (input.Length == 6)
                {
                    byte r = byte.Parse(input.Substring(0, 2), NumberStyles.HexNumber);
                    byte g = byte.Parse(input.Substring(2, 2), NumberStyles.HexNumber);
                    byte b = byte.Parse(input.Substring(4, 2), NumberStyles.HexNumber);

                    output = (T)Convert.ChangeType(new Color(r, g, b), typeOut);
                    return true;
                }
            }
            else if (typeOut == typeof(M_Rectangle))
            {
                string[] split = input.Split(new char[] { ',' });
                if (split.Length > 0)
                {
                    float[] values = new float[split.Length];
                    for (int i = 0; i < split.Length; i++)
                    {
                        if (!float.TryParse(split[i], NumberStyles.Any, CultureInfo.InvariantCulture, out values[i]))
                        {
                            output = default(T);
                            return false;
                        }
                    }

                    output = (T)Convert.ChangeType(new M_Rectangle(values[0], split.Length <= 1 ? 0 : values[1], split.Length <= 2 ? 0 : values[2], split.Length <= 3 ? 0 : values[3]), typeOut);
                    return true;
                }
            }
            else
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeOut);

                if (converter.CanConvertFrom(typeIn))
                {
                    output = (T)converter.ConvertFromInvariantString(input);
                    return true;
                }
            }
            output = default(T);
            return false;
        }
        public static bool TryParseRef<T>(string input, ref T output, Type typeOut)
        {
            Type typeIn = typeof(string);

            if (typeOut == typeof(Color))
            {
                if (input.Length == 6)
                {
                    byte r = byte.Parse(input.Substring(0, 2), NumberStyles.HexNumber);
                    byte g = byte.Parse(input.Substring(2, 2), NumberStyles.HexNumber);
                    byte b = byte.Parse(input.Substring(4, 2), NumberStyles.HexNumber);

                    output = (T)Convert.ChangeType(new Color(r, g, b), typeOut);
                    return true;
                }
            }
            else
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeOut);

                if (converter.CanConvertFrom(typeIn))
                {
                    output = (T)converter.ConvertFromInvariantString(input);
                    return true;
                }
            }
            return false;
        }
        public static bool TryParseCatch<T>(string input, out T output, Type typeOut)
        {
            Type typeIn = typeof(string);

            try
            {
                if (typeOut == typeof(Color))
                {
                    if (input.Length == 6)
                    {
                        byte r = byte.Parse(input.Substring(0, 2), NumberStyles.HexNumber);
                        byte g = byte.Parse(input.Substring(2, 2), NumberStyles.HexNumber);
                        byte b = byte.Parse(input.Substring(4, 2), NumberStyles.HexNumber);

                        output = (T)Convert.ChangeType(new Color(r, g, b), typeOut);
                        return true;
                    }
                }
                else
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeOut);

                    if (converter.CanConvertFrom(typeIn))
                    {
                        output = (T)converter.ConvertFromInvariantString(input);
                        return true;
                    }
                }
            }
            catch { }
            output = default(T);
            return false;
        }
        public static bool TryParseCatchRef<T>(string input, ref T output, Type typeOut)
        {
            Type typeIn = typeof(string);

            try
            {
                if (typeOut == typeof(Color))
                {
                    if (input.Length == 6)
                    {
                        byte r = byte.Parse(input.Substring(0, 2), NumberStyles.HexNumber);
                        byte g = byte.Parse(input.Substring(2, 2), NumberStyles.HexNumber);
                        byte b = byte.Parse(input.Substring(4, 2), NumberStyles.HexNumber);

                        output = (T)Convert.ChangeType(new Color(r, g, b), typeOut);
                        return true;
                    }
                }
                else
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeOut);

                    if (converter.CanConvertFrom(typeIn))
                    {
                        output = (T)converter.ConvertFromInvariantString(input);
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public static string ToString(object input)
        {
            Type typeIn = input.GetType();
            Type typeOut = typeof(string);

            if (typeIn == typeof(Color))
            {
                Color color = (Color)input;
                return color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
            }
            else if (typeIn == typeof(M_Rectangle))
            {
                M_Rectangle rect = input as M_Rectangle;
                return rect.pos.X.ToString(CultureInfo.InvariantCulture) + "," + rect.pos.Y.ToString(CultureInfo.InvariantCulture) + "," + rect.size.X.ToString(CultureInfo.InvariantCulture) + "," + rect.size.Y.ToString(CultureInfo.InvariantCulture);
            }
            TypeConverter converter = TypeDescriptor.GetConverter(typeIn);

            if (converter.CanConvertTo(typeOut))
            {
                return converter.ConvertToInvariantString(input);
            }
            return "";
        }
    }
}
