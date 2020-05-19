using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace JuliHelper
{
    public struct Int2
    {
        private int x, y;

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public Int2(int xy)
        {
            this.x = this.y = xy;
        }
        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Int2(Vector2 vec)
        {
            this.x = (int)vec.X;
            this.y = (int)vec.Y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        public bool SetInRect(int minX, int minY, int maxX, int maxY)
        {
            int x2 = x;
            int y2 = y;
            x = Math.Min(maxX, Math.Max(minX, x));
            y = Math.Min(maxY, Math.Max(minY, y));

            return x2 != x || y2 != y;
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj is Int2)
        //    return x == vint.x && Y == vint.y;
        //}

        public static bool operator ==(Int2 v1, Int2 v2)
        {
            return v1.x == v2.x && v1.y == v2.y;
        }
        public static bool operator !=(Int2 v1, Int2 v2)
        {
            return v1.x != v2.x || v1.y != v2.y;
        }

        public static Int2 operator +(Int2 v1, Int2 v2)
        {
            return new Int2(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static Int2 operator -(Int2 v1, Int2 v2)
        {
            return new Int2(v1.X - v2.X, v1.Y - v2.Y);
        }
        public static Int2 operator -(Int2 v1)
        {
            return new Int2(-v1.X, -v1.Y);
        }
        public static Int2 operator *(Int2 v1, Int2 v2)
        {
            return new Int2(v1.X * v2.X, v1.Y * v2.Y);
        }
        public static Int2 operator *(Int2 v1, float f2)
        {
            return new Int2((int)(v1.X * f2), (int)(v1.Y * f2));
        }
        public static Int2 operator /(Int2 v1, Int2 v2)
        {
            return new Int2(v1.X / v2.X, v1.Y / v2.Y);
        }

        public static Int2 operator +(Int2 v1, Vector2 v2)
        {
            return new Int2(v1.X + (int)v2.X, v1.Y + (int)v2.Y);
        }

        public static Int2 operator *(Int2 v1, int i2)
        {
            return new Int2(v1.x * i2, v1.y * i2);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
            //return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return x + " " + y;
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y);
        }

        public void SwapXY()
        {
            int save = x;
            x = y;
            y = save;
        }
    }
}
