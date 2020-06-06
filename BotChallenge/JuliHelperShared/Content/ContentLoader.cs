using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.IO;
using Microsoft.Xna.Framework.Media;
using JuliHelper.Particles;
using Microsoft.Xna.Framework.Audio;
using JuliHelper.ThreeD;

namespace JuliHelper
{
    public static class ContentLoader
    {
        public static Dictionary<string, Texture2D> textures;
        public static Dictionary<string, SpriteFont> fonts;
        public static Dictionary<string, Song> songs;
        public static Dictionary<string, SoundEffect> sounds;
        public static Dictionary<string, Effect> effects;
        public static Dictionary<string, Emitter> emitters;
        public static Dictionary<string, Model> models;

        public static ContentManager content;

        public static readonly string texturePath = Path.Combine(G.exeDir, $"Content{G.I}textures{G.I}");
        public static readonly string fontPath = Path.Combine(G.exeDir, $"Content{G.I}Fonts{G.I}");
        public static readonly string songPath = Path.Combine(G.exeDir, $"Content{G.I}Songs{G.I}");
        public static readonly string soundPath = Path.Combine(G.exeDir, $"Content{G.I}sounds{G.I}");
        public static readonly string effectPath = Path.Combine(G.exeDir, $"Content{G.I}Effects{G.I}");
        public static readonly string emitterPath = Path.Combine(G.exeDir, $"Content{G.I}Emitters{G.I}");
        public static readonly string modelPath = Path.Combine(G.exeDir, $"Content{G.I}Models{G.I}");
        
        public static void Initialize(ContentManager _content, GraphicsDevice device, Random emitterRand, bool loadOnInitialize = true, bool loadModels = true)
        {
            content = _content;

            textures = new Dictionary<string, Texture2D>();
            fonts = new Dictionary<string, SpriteFont>();
            songs = new Dictionary<string, Song>();
            sounds = new Dictionary<string, SoundEffect>();
            effects = new Dictionary<string, Effect>();
            emitters = new Dictionary<string, Emitter>();
            models = new Dictionary<string, Model>();
            
            if (loadOnInitialize)
                LoadContent(device, emitterRand, loadModels);
        }

        private static void LoadContent(GraphicsDevice device, Random emitterRand, bool loadModels)
        {
            //Textures
            string[] files = GetFilesInContent(texturePath, "xnb");
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i] != "pixel")
                    textures.Add(files[i], content.Load<Texture2D>("textures/" + files[i]));
            }
            
            //Fonts
            files = GetFilesInContent(fontPath, "xnb");
            for (int i = 0; i < files.Length; i++)
                fonts.Add(files[i], content.Load<SpriteFont>("Fonts/" + files[i]));

            //Songs
            files = GetFilesInContent(songPath, "xnb");
            for (int i = 0; i < files.Length; i++)
                songs.Add(files[i], content.Load<Song>("Songs/" + files[i]));

            //Sounds
            files = GetFilesInContent(soundPath, "xnb");
            for (int i = 0; i < files.Length; i++)
                sounds.Add(files[i], content.Load<SoundEffect>("sounds/" + files[i]));

            //Effects
            files = GetFilesInContent(effectPath, "xnb");
            for (int i = 0; i < files.Length; i++)
                effects.Add(files[i], content.Load<Effect>("Effects/" + files[i]));

            ////Emitters
            //files = GetFiles(emitterPath, "emi");
            //for (int i = 0; i < files.Length; i++)
            //{
            //    Emitter emitter = new Emitter(emitterRand);
            //    emitter.FromData(File.ReadAllLines(files[i]));
            //    string key = files[i].Substring(emitterPath.Length, files[i].LastIndexOf('.') - emitterPath.Length).ToLower().Replace('\\', '/');
            //    emitters.Add(key, emitter);
            //}
            if (loadModels)
            {
                //Models
                files = GetFilesInContent(modelPath, "xnb");
                //throw new Exception(files[0]);
                if (files.Length > 0)
                {
                    files = files.Where(f => f.Substring(0, 3) != "tex" && !f.Contains("cubetextex_0")).ToArray();
                    //throw new Exception(files.Length.ToString());
                    for (int i = 0; i < files.Length; i++)
                    {
                        try//TODO: remove try catch
                        {
                            Model newModel = content.Load<Model>("Models/" + files[i]);
                            models.Add(files[i].Substring(files[i].LastIndexOf(G.I) + 1), newModel);

                            bool isFbx = false;

                            if (newModel.Tag is string)
                                isFbx = ((string)newModel.Tag) == ".fbx";
                            else if (newModel.Tag is ModelExtra)
                            {
                                isFbx = ((ModelExtra)newModel.Tag).extension == ".fbx";

                                //if (isFbx)
                                //    ModelHelper.ApplyTransform(newModel);

                            }
                            //if (isFbx)
                            //{
                            //    foreach (var mesh in newModel.Meshes)
                            //    {
                            //        mesh.ParentBone.Transform *= Matrix.CreateScale(0.01f);
                            //    }
                            //}
                            //foreach (var mesh in newModel.Meshes)
                            //{
                            //    mesh.ParentBone.Transform = Matrix.Identity;
                            //}
                            //ModelHelper.ApplyTransform(newModel);


                        } catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }
        }

        public static string[] GetFilesInContent(string path, string extension = "xnb")
        {
            if (Directory.Exists(path))
            {
                return Directory.EnumerateFiles(path, "*." + extension, SearchOption.AllDirectories).Select(f => f.Substring(path.Length)).Select(f => f.Substring(0, f.Length - 1 - extension.Length).Replace(G.I, '/')).ToArray();
            }
            else
            {
                return new string[0];
            }
        }

        public static string[] GetFiles(string path, string extension)
        {
            if (Directory.Exists(path))
                return Directory.EnumerateFiles(path, "*." + extension, SearchOption.AllDirectories).ToArray();
            else
                return new string[0];
        }

        public static string contentDirectory
        {
            get { return content.RootDirectory; }
        }

        public static Model AddModel(string modelPath, string key = "")
        {
            modelPath = modelPath.Replace(G.I, '/');
            if (key == "")
                key = modelPath.Substring(modelPath.LastIndexOf('/') + 1);
            Model model = content.Load<Model>(modelPath);
            models.Add(key, model);
            return model;
        }
        public static Model AddModel(string modelPath, float loopBlend, string key = "")
        {
            modelPath = modelPath.Replace(G.I, '/');
            if (key == "")
                key = modelPath.Substring(modelPath.LastIndexOf('/') + 1);
            Model model = content.Load<Model>(modelPath);
            models.Add(key, model);

            AnimatedModel anim = new AnimatedModel(key);
            anim.LoadContent(RenderMethod.Diffuse);
            anim.MakeLoopBlend(loopBlend);
            return model;
        }
    }

    //[StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormalColor : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Color Color;

        public VertexDeclaration VertexDeclaration
        {
            get
            {
                VertexElement[] elements = new VertexElement[]
                {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                    new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0)
                };
                VertexDeclaration declaration = new VertexDeclaration(elements);
                return declaration;
            }
        }
    }

    public struct VertexPositionNormal : IVertexType
    {
        public Vector3 Position;
        public Vector3 Normal;

        public VertexDeclaration VertexDeclaration
        {
            get
            {
                VertexElement[] elements = new VertexElement[]
                {
                    new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                    new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
                };
                VertexDeclaration declaration = new VertexDeclaration(elements);
                return declaration;
            }
        }
    }
}
