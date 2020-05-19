using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper
{
    public static class ContentFenja
    {
        public static void LoadRaw(Type type, string localPath, GraphicsDevice gDevice, string contentPath)
        {
            var fields = type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            var switchType = new Dictionary<Type, Action<FieldInfo>>
            {
                {
                    typeof(Texture2D), f =>
                    {
                        Texture2D tex = f.GetValue(null) as Texture2D;
                        if (tex != null)
                            tex.Dispose();

                        string path = Path.Combine(contentPath, localPath, f.Name + ".png");
                        FileInfo info = new FileInfo(path);
                        using (FileStream stream = new FileStream(path, FileMode.Open))
                        {
                            tex = Texture2D.FromStream(gDevice, stream);
                            tex.Name = Path.Combine(localPath, f.Name);
                            f.SetValue(null, tex);
                        }
                    }
                }
            };

            // RELEASE: replace this with hard code
            for (int i = 0; i < fields.Length; i++)
            {
                if (switchType.ContainsKey(fields[i].FieldType))
                    switchType[fields[i].FieldType](fields[i]);
            }
        }

        public static void LoadProcessed(Type type, string localPath, ContentManager content)
        {
            var fields = type.GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

            var switchType = new Dictionary<Type, Action<FieldInfo>>
            {
                {
                    typeof(Texture2D), f =>
                    {
                        f.SetValue(null, content.Load<Texture2D>(Path.Combine(localPath, f.Name)));
                    }
                },
                {
                    typeof(SpriteFont), f =>
                    {
                        f.SetValue(null, content.Load<SpriteFont>(Path.Combine(localPath, f.Name)));
                    }
                },
            };

            if (fields.Length > 0)
            {
                if (fields[0].GetValue(null) == null)
                {
                    // RELEASE: replace this with hard code
                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (switchType.ContainsKey(fields[i].FieldType))
                            switchType[fields[i].FieldType](fields[i]);
                    }
                }
            }
        }
    }
}
