using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JuliHelper
{
    public static class ArrayQuery
    {
        #region InX

        public static bool AnyInXIncludeNull<T>(this T[,] arr, int x) => AnyInXIncludeNull<T>(arr, x, 0, arr.GetLength(1), f => true);
        public static bool AnyInXIncludeNull<T>(this T[,] arr, int x, Predicate<T> predicate) => AnyInXIncludeNull<T>(arr, x, 0, arr.GetLength(1), predicate);
        public static bool AnyInXIncludeNull<T>(this T[,] arr, int x, int yStart, int yCount, Predicate<T> predicate)
        {
            int yEnd = yStart + yCount;
            for (int y = yStart; y < yEnd; y++)
            {
                if (predicate(arr[x, y]))
                    return true;
            }
            return false;
        }

        public static bool AllInXIncludeNull<T>(this T[,] arr, int x, Predicate<T> predicate) => AllInXIncludeNull<T>(arr, x, 0, arr.GetLength(1), predicate);
        public static bool AllInXIncludeNull<T>(this T[,] arr, int x, int yStart, int yCount, Predicate<T> predicate)
        {
            int yEnd = yStart + yCount;
            for (int y = yStart; y < yEnd; y++)
            {
                if (!predicate(arr[x, y]))
                    return false;
            }
            return true;
        }

        public static int IndexInXIncludeNull<T>(this T[,] arr, int x) => IndexInXIncludeNull<T>(arr, x, 0, arr.GetLength(1), f => true);
        public static int IndexInXIncludeNull<T>(this T[,] arr, int x, Predicate<T> predicate) => IndexInXIncludeNull<T>(arr, x, 0, arr.GetLength(1), predicate);
        public static int IndexInXIncludeNull<T>(this T[,] arr, int x, int yStart, int yCount, Predicate<T> predicate)
        {
            int yEnd = yStart + yCount;
            for (int y = yStart; y < yEnd; y++)
            {
                if (predicate(arr[x, y]))
                    return y;
            }
            return -1;
        }

        public static T FindInXIncludeNull<T>(this T[,] arr, int x, T or = default(T)) => FindInXIncludeNull<T>(arr, x, 0, arr.GetLength(1), f => true, or);
        public static T FindInXIncludeNull<T>(this T[,] arr, int x, Predicate<T> predicate, T or = default(T)) => FindInXIncludeNull<T>(arr, x, 0, arr.GetLength(1), predicate, or);
        public static T FindInXIncludeNull<T>(this T[,] arr, int x, int yStart, int yCount, Predicate<T> predicate, T or = default(T))
        {
            int yEnd = yStart + yCount;
            for (int y = yStart; y < yEnd; y++)
            {
                if (predicate(arr[x, y]))
                    return arr[x, y];
            }
            return or;
        }

        public static List<T> FindAllInXIncludeNull<T>(this T[,] arr, int x) => FindAllInXIncludeNull<T>(arr, x, 0, arr.GetLength(1), f => true);
        public static List<T> FindAllInXIncludeNull<T>(this T[,] arr, int x, Predicate<T> predicate) => FindAllInXIncludeNull<T>(arr, x, 0, arr.GetLength(1), predicate);
        public static List<T> FindAllInXIncludeNull<T>(this T[,] arr, int x, int yStart, int yCount, Predicate<T> predicate)
        {
            List<T> found = new List<T>();
            int yEnd = yStart + yCount;
            for (int y = yStart; y < yEnd; y++)
            {
                if (predicate(arr[x, y]))
                    found.Add(arr[x, y]);
            }
            return found;
        }
        public static List<T> FindAllInXIndexIncludeNull<T>(this T[,] arr, int x, Predicate<int> predicate) => FindAllInXIndexIncludeNull<T>(arr, x, 0, arr.GetLength(1), predicate);
        public static List<T> FindAllInXIndexIncludeNull<T>(this T[,] arr, int x, int yStart, int yCount, Predicate<int> predicate)
        {
            List<T> found = new List<T>();
            int yEnd = yStart + yCount;
            for (int y = yStart; y < yEnd; y++)
            {
                if (predicate(y))
                    found.Add(arr[x, y]);
            }
            return found;
        }

        public static bool AnyInX<T>(this T[,] arr, int x) => AnyInX<T>(arr, x, 0, arr.GetLength(1), f => true);
        public static bool AnyInX<T>(this T[,] arr, int x, Predicate<T> predicate) => AnyInX<T>(arr, x, 0, arr.GetLength(1), predicate);
        public static bool AnyInX<T>(this T[,] arr, int x, int yStart, int yCount) => AnyInX<T>(arr, x, yStart, yCount, f => true);
        public static bool AnyInX<T>(this T[,] arr, int x, int yStart, int yCount, Predicate<T> predicate)
        {
            int yEnd = yStart + yCount;
            for (int y = yStart; y < yEnd; y++)
            {
                if (arr[x, y] != null && predicate(arr[x, y]))
                    return true;
            }
            return false;
        }

        public static bool AllInX<T>(this T[,] arr, int x, Predicate<T> predicate) => AllInX<T>(arr, x, 0, arr.GetLength(1), predicate);
        public static bool AllInX<T>(this T[,] arr, int x, int yStart, int yCount, Predicate<T> predicate)
        {
            int yEnd = yStart + yCount;
            for (int y = yStart; y < yEnd; y++)
            {
                if (arr[x, y] != null && !predicate(arr[x, y]))
                    return false;
            }
            return true;
        }

        public static int IndexInX<T>(this T[,] arr, int x) => IndexInX<T>(arr, x, 0, arr.GetLength(1), f => true);
        public static int IndexInX<T>(this T[,] arr, int x, Predicate<T> predicate) => IndexInX<T>(arr, x, 0, arr.GetLength(1), predicate);
        public static int IndexInX<T>(this T[,] arr, int x, int yStart, int yCount, Predicate<T> predicate)
        {
            int yEnd = yStart + yCount;
            for (int y = yStart; y < yEnd; y++)
            {
                if (arr[x, y] != null && predicate(arr[x, y]))
                    return y;
            }
            return -1;
        }

        public static T FindInX<T>(this T[,] arr, int x, T or = default(T)) => FindInX<T>(arr, x, 0, arr.GetLength(1), f => true, or);
        public static T FindInX<T>(this T[,] arr, int x, Predicate<T> predicate, T or = default(T)) => FindInX<T>(arr, x, 0, arr.GetLength(1), predicate, or);
        public static T FindInX<T>(this T[,] arr, int x, int yStart, int yCount, Predicate<T> predicate, T or = default(T))
        {
            int yEnd = yStart + yCount;
            for (int y = yStart; y < yEnd; y++)
            {
                if (arr[x, y] != null && predicate(arr[x, y]))
                    return arr[x, y];
            }
            return or;
        }

        public static List<T> FindAllInX<T>(this T[,] arr, int x) => FindAllInX<T>(arr, x, 0, arr.GetLength(1), f => true);
        public static List<T> FindAllInX<T>(this T[,] arr, int x, Predicate<T> predicate) => FindAllInX<T>(arr, x, 0, arr.GetLength(1), predicate);
        public static List<T> FindAllInX<T>(this T[,] arr, int x, int yStart, int yCount, Predicate<T> predicate)
        {
            List<T> found = new List<T>();
            int yEnd = yStart + yCount;
            for (int y = yStart; y < yEnd; y++)
            {
                if (arr[x, y] != null && predicate(arr[x, y]))
                    found.Add(arr[x, y]);
            }
            return found;
        }
        public static List<T> FindAllInXIndex<T>(this T[,] arr, int x, Predicate<int> predicate) => FindAllInXIndex<T>(arr, x, 0, arr.GetLength(1), predicate);
        public static List<T> FindAllInXIndex<T>(this T[,] arr, int x, int yStart, int yCount, Predicate<int> predicate)
        {
            List<T> found = new List<T>();
            int yEnd = yStart + yCount;
            for (int y = yStart; y < yEnd; y++)
            {
                if (arr[x, y] != null && predicate(y))
                    found.Add(arr[x, y]);
            }
            return found;
        }

        #endregion

        #region InY

        public static bool AnyInYIncludeNull<T>(this T[,] arr, int y) => AnyInYIncludeNull<T>(arr, y, 0, arr.GetLength(0), f => true);
        public static bool AnyInYIncludeNull<T>(this T[,] arr, int y, Predicate<T> predicate) => AnyInYIncludeNull<T>(arr, y, 0, arr.GetLength(0), predicate);
        public static bool AnyInYIncludeNull<T>(this T[,] arr, int y, int xStart, int xCount, Predicate<T> predicate)
        {
            int xEnd = xStart + xCount;
            for (int x = xStart; x < xEnd; x++)
            {
                if (predicate(arr[x, y]))
                    return true;
            }
            return false;
        }

        public static bool AllInYIncludeNull<T>(this T[,] arr, int y, Predicate<T> predicate) => AllInYIncludeNull<T>(arr, y, 0, arr.GetLength(0), predicate);
        public static bool AllInYIncludeNull<T>(this T[,] arr, int y, int xStart, int xCount, Predicate<T> predicate)
        {
            int xEnd = xStart + xCount;
            for (int x = xStart; x < xEnd; x++)
            {
                if (!predicate(arr[x, y]))
                    return false;
            }
            return true;
        }

        public static int IndexInYIncludeNull<T>(this T[,] arr, int y) => IndexInYIncludeNull<T>(arr, y, 0, arr.GetLength(0), f => true);
        public static int IndexInYIncludeNull<T>(this T[,] arr, int y, Predicate<T> predicate) => IndexInYIncludeNull<T>(arr, y, 0, arr.GetLength(0), predicate);
        public static int IndexInYIncludeNull<T>(this T[,] arr, int y, int xStart, int xCount, Predicate<T> predicate)
        {
            int xEnd = xStart + xCount;
            for (int x = xStart; x < xEnd; x++)
            {
                if (predicate(arr[x, y]))
                    return x;
            }
            return -1;
        }
        
        public static T FindInYIncludeNull<T>(this T[,] arr, int y, T or = default(T)) => FindInYIncludeNull<T>(arr, y, 0, arr.GetLength(0), f => true, or);
        public static T FindInYIncludeNull<T>(this T[,] arr, int y, Predicate<T> predicate, T or = default(T)) => FindInYIncludeNull<T>(arr, y, 0, arr.GetLength(0), predicate, or);
        public static T FindInYIncludeNull<T>(this T[,] arr, int y, int xStart, int xCount, Predicate<T> predicate, T or = default(T))
        {
            int xEnd = xStart + xCount;
            for (int x = xStart; x < xEnd; x++)
            {
                if (predicate(arr[x, y]))
                    return arr[x, y];
            }
            return or;
        }

        public static List<T> FindAllInYIncludeNull<T>(this T[,] arr, int y) => FindAllInYIncludeNull<T>(arr, y, 0, arr.GetLength(0), f => true);
        public static List<T> FindAllInYIncludeNull<T>(this T[,] arr, int y, Predicate<T> predicate) => FindAllInYIncludeNull<T>(arr, y, 0, arr.GetLength(0), predicate);
        public static List<T> FindAllInYIncludeNull<T>(this T[,] arr, int y, int xStart, int xCount, Predicate<T> predicate)
        {
            List<T> found = new List<T>();
            int xEnd = xStart + xCount;
            for (int x = xStart; x < xEnd; x++)
            {
                if (predicate(arr[x, y]))
                    found.Add(arr[x, y]);
            }
            return found;
        }
        public static List<T> FindAllInYIndexIncludeNull<T>(this T[,] arr, int y, Predicate<int> predicate) => FindAllInYIndexIncludeNull<T>(arr, y, 0, arr.GetLength(0), predicate);
        public static List<T> FindAllInYIndexIncludeNull<T>(this T[,] arr, int y, int xStart, int xCount, Predicate<int> predicate)
        {
            List<T> found = new List<T>();
            int xEnd = xStart + xCount;
            for (int x = xStart; x < xEnd; x++)
            {
                if (predicate(x))
                    found.Add(arr[x, y]);
            }
            return found;
        }

        public static bool AnyInY<T>(this T[,] arr, int y) => AnyInY<T>(arr, y, 0, arr.GetLength(0), f => true);
        public static bool AnyInY<T>(this T[,] arr, int y, Predicate<T> predicate) => AnyInY<T>(arr, y, 0, arr.GetLength(0), predicate);
        public static bool AnyInY<T>(this T[,] arr, int y, int xStart, int xCount) => AnyInY<T>(arr, y, xStart, xCount, f => true);
        public static bool AnyInY<T>(this T[,] arr, int y, int xStart, int xCount, Predicate<T> predicate)
        {
            int xEnd = xStart + xCount;
            for (int x = xStart; x < xEnd; x++)
            {
                if (arr[x, y] != null && predicate(arr[x, y]))
                    return true;
            }
            return false;
        }

        public static bool AllInY<T>(this T[,] arr, int y, Predicate<T> predicate) => AllInY<T>(arr, y, 0, arr.GetLength(0), predicate);
        public static bool AllInY<T>(this T[,] arr, int y, int xStart, int xCount, Predicate<T> predicate)
        {
            int xEnd = xStart + xCount;
            for (int x = xStart; x < xEnd; x++)
            {
                if (arr[x, y] != null && !predicate(arr[x, y]))
                    return false;
            }
            return true;
        }

        public static int IndexInY<T>(this T[,] arr, int y) => IndexInY<T>(arr, y, 0, arr.GetLength(0), f => true);
        public static int IndexInY<T>(this T[,] arr, int y, Predicate<T> predicate) => IndexInY<T>(arr, y, 0, arr.GetLength(0), predicate);
        public static int IndexInY<T>(this T[,] arr, int y, int xStart, int xCount, Predicate<T> predicate)
        {
            int xEnd = xStart + xCount;
            for (int x = xStart; x < xEnd; x++)
            {
                if (arr[x, y] != null && predicate(arr[x, y]))
                    return x;
            }
            return -1;
        }

        public static T FindInY<T>(this T[,] arr, int y, T or = default(T)) => FindInY<T>(arr, y, 0, arr.GetLength(0), f => true, or);
        public static T FindInY<T>(this T[,] arr, int y, Predicate<T> predicate, T or = default(T)) => FindInY<T>(arr, y, 0, arr.GetLength(0), predicate, or);
        public static T FindInY<T>(this T[,] arr, int y, int xStart, int xCount, Predicate<T> predicate, T or = default(T))
        {
            int xEnd = xStart + xCount;
            for (int x = xStart; x < xEnd; x++)
            {
                if (arr[x, y] != null && predicate(arr[x, y]))
                    return arr[x, y];
            }
            return or;
        }

        public static List<T> FindAllInY<T>(this T[,] arr, int y) => FindAllInY<T>(arr, y, 0, arr.GetLength(0), f => true);
        public static List<T> FindAllInY<T>(this T[,] arr, int y, Predicate<T> predicate) => FindAllInY<T>(arr, y, 0, arr.GetLength(0), predicate);
        public static List<T> FindAllInY<T>(this T[,] arr, int y, int xStart, int xCount, Predicate<T> predicate)
        {
            List<T> found = new List<T>();
            int xEnd = xStart + xCount;
            for (int x = xStart; x < xEnd; x++)
            {
                if (arr[x, y] != null && predicate(arr[x, y]))
                    found.Add(arr[x, y]);
            }
            return found;
        }
        public static List<T> FindAllInYIndex<T>(this T[,] arr, int y, Predicate<int> predicate) => FindAllInYIndex<T>(arr, y, 0, arr.GetLength(0), predicate);
        public static List<T> FindAllInYIndex<T>(this T[,] arr, int y, int xStart, int xCount, Predicate<int> predicate)
        {
            List<T> found = new List<T>();
            int xEnd = xStart + xCount;
            for (int x = xStart; x < xEnd; x++)
            {
                if (arr[x, y] != null && predicate(x))
                    found.Add(arr[x, y]);
            }
            return found;
        }

        #endregion

        #region 2D

        public static (int, int) IndexOfIncludeNull<T>(this T[,] arr, T of) => IndexOf(arr, f => f.Equals(of));
        public static (int, int) IndexOfIncludeNull<T>(this T[,] arr, Predicate<T> predicate)
        {
            int w = arr.GetLength(0);
            int h = arr.GetLength(1);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (predicate(arr[x, y]))
                        return (x, y);
                }
            }
            return (-1, -1);
        }

        public static bool Any<T>(this T[,] arr, Predicate<T> predicate) => Any<T>(arr, 0, 0, arr.GetLength(0), arr.GetLength(1), predicate);
        public static bool Any<T>(this T[,] arr, int xStart, int yStart, int xCount, int yCount, Predicate<T> predicate)
        {
            List<T> found = new List<T>();
            int xEnd = xStart + xCount;
            int yEnd = yStart + yCount;
            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    if (arr[x, y] != null && predicate(arr[x, y]))
                        return true;
                }
            }
            return false;
        }

        public static bool All<T>(this T[,] arr, Predicate<T> predicate) => All<T>(arr, 0, 0, arr.GetLength(0), arr.GetLength(1), predicate);
        public static bool All<T>(this T[,] arr, int xStart, int yStart, int xCount, int yCount, Predicate<T> predicate)
        {
            List<T> found = new List<T>();
            int xEnd = xStart + xCount;
            int yEnd = yStart + yCount;
            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    if (arr[x, y] != null && !predicate(arr[x, y]))
                        return false;
                }
            }
            return true;
        }

        public static int Sum<T>(this T[,] arr, Func<T, int> func) => Sum<T>(arr, 0, 0, arr.GetLength(0), arr.GetLength(1), func);
        public static int Sum<T>(this T[,] arr, int xStart, int yStart, int xCount, int yCount, Func<T, int> func)
        {
            int sum = 0;
            List<T> found = new List<T>();
            int xEnd = xStart + xCount;
            int yEnd = yStart + yCount;
            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    if (arr[x, y] != null)
                        sum += func(arr[x, y]);
                }
            }
            return sum;
        }

        public static T Find<T>(this T[,] arr, Predicate<T> predicate, T orDefault = default(T)) => Find<T>(arr, 0, 0, arr.GetLength(0), arr.GetLength(1), predicate);
        public static T Find<T>(this T[,] arr, int xStart, int yStart, int xCount, int yCount, Predicate<T> predicate, T orDefault = default(T))
        {
            List<T> found = new List<T>();
            int xEnd = xStart + xCount;
            int yEnd = yStart + yCount;
            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    if (arr[x, y] != null && predicate(arr[x, y]))
                        return arr[x, y];
                }
            }
            return orDefault;
        }

        public static List<T> FindAll<T>(this T[,] arr) => FindAll<T>(arr, 0, 0, arr.GetLength(0), arr.GetLength(1), f => true);
        public static List<T> FindAll<T>(this T[,] arr, Predicate<T> predicate) => FindAll<T>(arr, 0, 0, arr.GetLength(0), arr.GetLength(1), predicate);
        public static List<T> FindAll<T>(this T[,] arr, int xStart, int yStart, int xCount, int yCount, Predicate<T> predicate)
        {
            List<T> found = new List<T>();
            int xEnd = xStart + xCount;
            int yEnd = yStart + yCount;
            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    if (arr[x, y] != null && predicate(arr[x, y]))
                        found.Add(arr[x, y]);
                }
            }
            return found;
        }

        public static (int, int) IndexOf<T>(this T[,] arr, T of) => IndexOf(arr, f => f.Equals(of));
        public static (int, int) IndexOf<T>(this T[,] arr, Predicate<T> predicate)
        {
            int w = arr.GetLength(0);
            int h = arr.GetLength(1);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (arr[x, y] != null && predicate(arr[x, y]))
                        return (x, y);
                }
            }
            return (-1, -1);
        }

        public static T FirstOr<T>(this IEnumerable<T> enumerable, T or, Func<T, bool> predicate)
        {
            foreach (var current in enumerable)
            {
                if (current != null && predicate(current))
                {
                    return current;
                }
            }
            return or;
        }

        public static int CountIncludeNull<T>(this T[,] arr, Predicate<T> countIf)
        {
            int w = arr.GetLength(0);
            int h = arr.GetLength(1);
            int count = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (countIf(arr[x, y]))
                        count++;
                }
            }
            return count;
        }


        public static int Count<T>(this T[,] arr, Predicate<T> countIf) => Count<T>(arr, 0, 0, arr.GetLength(0), arr.GetLength(1), countIf);
        public static int Count<T>(this T[,] arr, int xStart, int yStart, int xCount, int yCount, Predicate<T> countIf)
        {
            int xEnd = xStart + xCount;
            int yEnd = yStart + yCount;
            int count = 0;
            for (int y = yStart; y < yEnd; y++)
            {
                for (int x = xStart; x < xEnd; x++)
                {
                    if (arr[x,y] != null && countIf(arr[x, y]))
                        count++;
                }
            }
            return count;
        }

        public static T[] CreateInstances<T>(this T[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = Activator.CreateInstance<T>();
            }
            return arr;
        }

        public static List<T> Remove<T>(this List<T> list, List<T> remove)
        {
            for (int i = 0; i < remove.Count; i++)
            {
                list.Remove(remove[i]);
            }
            return list;
        }

        public static void ForEach<T>(this T[,] arr, Action<T> action)
        {
            int w = arr.GetLength(0);
            int h = arr.GetLength(1);
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                    if (arr[x,y] != null)
                        action(arr[x, y]);
        }
        public static void ForEachIncludeNull<T>(this T[,] arr, Action<T> action)
        {
            int w = arr.GetLength(0);
            int h = arr.GetLength(1);
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                        action(arr[x, y]);
        }

        #endregion
    }
}
