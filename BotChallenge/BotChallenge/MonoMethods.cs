using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BotChallenge
{
    public interface MonoMethods
    {
        void LoadContent(SpriteBatch spriteBatch, ContentManager content, GraphicsDeviceManager graphics);
        void Update();
        void Draw(SpriteBatch spriteBatch);
    }
}
