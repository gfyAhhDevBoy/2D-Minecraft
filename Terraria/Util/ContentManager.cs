using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terraria.Util
{
    static class ContentManager
    {
        private static Dictionary<string, Texture2D> _textureMap = new();
        private static Microsoft.Xna.Framework.Content.ContentManager _contentManager;

        public static void Init(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            _contentManager = content;
        }

        public static Texture2D GetTexture(string path)
        {
            if (_textureMap.ContainsKey(path))
            {
                Texture2D foundTexture;
                _textureMap.TryGetValue(path, out foundTexture);
                return foundTexture;
            }

            Texture2D texture;

            try
            {
                texture = _contentManager.Load<Texture2D>(path);
            }
            catch (ContentLoadException e)
            {
                Debug.WriteLine("Couldn't load Texture " + path);
                return null;
            }

            _textureMap.Add(path, texture);

            return texture;
        }
    }
}
