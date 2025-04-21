using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terraria
{
    abstract class Item : Sprite
    {
        private string _name;

        public string Name { get => _name; private set => _name = value; }

        private Player _player;

        public Item(Texture2D texture, Player player, string name) : base(texture)
        {
            _player = player;
            _name = name;
            Origin = new(8, 8);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Rectangle dest = new((int)_player.ItemOrigin.X, (int)_player.ItemOrigin.Y, Rectangle.Width, Rectangle.Height);
            spriteBatch.Draw(_texture, dest, null, Color.White, Rotation, Origin, SpriteEffects.None, 0f);
        }

        public void DrawPreview(SpriteBatch spriteBatch, Vector2 pos)
        {
            Rectangle dest = new((int)pos.X, (int)pos.Y, 75,75);
            spriteBatch.Draw(_texture, dest, null, Color.White, Rotation, Origin, SpriteEffects.None, 0f);
        }

        public virtual void Interact(Player player)
        {
            Debug.WriteLine(Name + " clicked!");
        }

        public Texture2D GetTexture() => _texture;
    }
}
