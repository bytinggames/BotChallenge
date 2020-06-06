using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuliHelper.ThreeD
{
    public enum RenderMethod
    {
        Deferred,
        Forward,
        Diffuse
    }

    public abstract class Renderable
    {
        public abstract void Render(MeshBatch meshBatch, Vector3 pos);
        public abstract void Render(MeshBatch meshBatch, Matrix transform);

        public abstract void ForEachEffect(Action<Effect> action);

        public abstract void SetWorld(Matrix world);
        public abstract void Transform(Matrix matrix);
    }
}
