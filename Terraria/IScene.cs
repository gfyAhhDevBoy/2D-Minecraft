using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terraria
{
    internal interface IScene
    {
        void Load();
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        void Unload();
    }
}
