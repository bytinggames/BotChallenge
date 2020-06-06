#if WINDOWS1

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace JuliHelper
{
    public static class BmpHelper
    {
        public static int CreateMipMap(string pathIn, int levels = 100)
        {
            Bitmap mipmap;
            using (Bitmap tex = new Bitmap(pathIn))
            {
                //levels = (int)Math.Log(Math.Min(tex.Width, tex.Height), 2);
                mipmap = GenerateAtlas(tex, ref levels);
            }
            mipmap.Save(Calculate.GetFreePathOrig(pathIn, "_mip"));
            mipmap.Dispose();

            return levels;
        }
        public static Bitmap GenerateAtlas(Bitmap tex, ref int levels)
        {
            if (tex.Width <= 1 || tex.Height <= 1)
                return (Bitmap)tex.Clone();

            int maxLevel = (int)Math.Floor(Math.Log(tex.Width, 2));
            maxLevel = Math.Min(maxLevel, (int)Math.Floor(Math.Log(tex.Width, 2)));

            if (levels > maxLevel)
                levels = maxLevel;

            int w = tex.Width;
            int h = tex.Height;
            int atlasW = 0;
            int atlasH = 0;
            for (int i = 0; i < levels; i++)
            {
                atlasW += w;
                atlasH += h;
                w /= 2;
                h /= 2;
            }
            w = tex.Width;
            h = tex.Height;
            Bitmap atlas = new Bitmap(atlasW, atlasH);

            //fill original tex (0,0)
            for (int y = 0; y < w; y++)
                for (int x = 0; x < h; x++)
                    atlas.SetPixel(x, y, tex.GetPixel(x, y));


            //unwrap to right
            int cy = 0;
            int ch = h;
            int old = 0; //last tex index (x or y)
            int cx = w;
            int cw = w / 2;
            for (int texX = 1; texX < levels; texX++)
            {
                for (int y = 0; y < ch; y++)
                {
                    for (int x = 0; x < cw; x++)
                    {
                        Color color1 = atlas.GetPixel(old + x * 2, y);
                        Color color2 = atlas.GetPixel(old + x * 2 + 1, y);
                        atlas.SetPixel(cx + x, cy + y, AverageColors(color1, color2));
                    }
                }
                old = cx;
                cx += cw;
                cw /= 2;
            }

            //unwrap top downwards
            cy = ch;
            ch /= 2;
            old = 0;
            for (int texY = 1; texY < levels; texY++)
            {
                cx = 0;
                cw = w;

                for (int texX = 0; texX < levels; texX++)
                {
                    for (int y = 0; y < ch; y++)
                    {
                        for (int x = 0; x < cw; x++)
                        {
                            Color color1 = atlas.GetPixel(cx + x, y * 2 + old);
                            Color color2 = atlas.GetPixel(cx + x, y * 2 + old + 1);
                            atlas.SetPixel(cx + x, cy + y, AverageColors(color1, color2));
                        }
                    }
                    cx += cw;
                    cw /= 2;
                }
                old = cy;
                cy += ch;
                ch /= 2;
            }
            return atlas;
        }

        public static Color AverageColors(Color c1, Color c2)
        {
            return Color.FromArgb((c1.A + c2.A) / 2, (c1.R + c2.R) / 2, (c1.G + c2.G) / 2, (c1.B + c2.B) / 2);
        }
        
    }
}

#endif