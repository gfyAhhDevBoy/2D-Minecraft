using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Terraria.Util;

namespace Terraria
{
    public enum BlockType
    {
        Air = 0,
        Stone,
        Dirt,
        Grass,
    }

    public enum BlockFlags
    {
        Solid = 1 << 0,
        Breakable = 1 << 1,
    }

    internal class Block : Sprite
    {
        private readonly BlockType type;
        private readonly int flags;

        private Texture2D _highlightTexture;

        public BlockType Type { get { return type; } }
        public int Flags { get { return flags; } }

        public const int BlockSize = 32;

        private bool _selected;


        public Block() { }

        public Block(int type, int flags, Vector2 position) : this((BlockType)type, (BlockFlags)flags, position)
        {

        }

        public Block(BlockType type, BlockFlags flags, Vector2 position) : base(Game1.Instance.GetTextureFromBlockType(type))
        {
            this.type = type;
            this.flags = (int)flags;
            Position = new(position.X * BlockSize, position.Y * BlockSize);
            Origin = new();

            

            Scale = 2f;
            //Debug.WriteLine(Position.ToString());



            //if(type != BlockType.Air)
            //{
            //    Color outline = Color.Red;
            //    Color[] outlineData = new Color[_texture.Width * _texture.Height];
            //    _texture.GetData(outlineData);

            //    for (int i = 0; i < outlineData.Length; i++)
            //    {
            //        bool isBorder = (i % 16 == 0) || (i % 16 == 15) || (i / 16 == 0) || (i / 16 == 15);
            //        if(isBorder)
            //        {
            //            outlineData[i] = Color.Red;
            //        }
            //    }

            //    _texture.SetData(outlineData);
            //}

            //Color[] data = new Color[_texture.Width * _texture.Height];
            //_texture.GetData(data);
            //for (int i = 0; i < data.Length; i++)
            //{
            //    bool isBorder = (i % 16 == 0) || (i % 16 == 15) || (i / 16 == 0) || (i / 16 == 15);
            //    if (isBorder)
            //    {
            //        data[i] = Color.Black;
            //    }
            //}
            //_selectedTexture = new(Game1.Instance.GraphicsDevice, _texture.Width, _texture.Height);
            //_selectedTexture.SetData(data);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            if(_selected)
            {
                spriteBatch.Draw(_highlightTexture, Rectangle, Color.White * .3f);
            }
        }
        public bool IsSolid() => (flags & (int)BlockFlags.Solid) != 0;
        public bool IsBreakable() => (flags & (int)BlockFlags.Breakable) != 0;

        public void SetSelected(bool selected) => _selected = selected;

    }
}
