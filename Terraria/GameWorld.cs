using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Terraria
{
    internal class GameWorld
    {
        public const int InitialWorldWidth = 50, InitialWorldHeight = 30;

        private List<List<BlockType>> map;

        private Texture2D _highlightTexture;

        public GameWorld()
        {
            map = new List<List<BlockType>>(InitialWorldWidth);
            for (int x = 0; x < InitialWorldWidth; x++)
            {
                map.Add(new List<BlockType>(InitialWorldHeight));
                for (int y = 0; y < InitialWorldHeight; y++)
                {
                    map[x].Add(BlockType.Air);
                }
            }

            _highlightTexture = new(Game1.Instance.GraphicsDevice, 1, 1);
            _highlightTexture.SetData([Color.White]);
        }

        public Vector2 GetWorldSize() => new(map.Count, map[0].Count);

        public void GenerateWorld()
        {
            for (int y = 0; y < InitialWorldHeight; y++)
            {
                for (int x = 0; x < InitialWorldWidth; x++)
                {
                    if (y < 10)
                        map[x][y] = BlockType.Air;
                    else
                        map[x][y] = BlockType.Stone;
                }
            }

            map[20][9] = BlockType.Stone;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Point? selectedTile)
        {
            for (int y = 0; y < map[0].Count; y++)
            {
                for (int x = 0; x < map.Count; x++)
                {
                    BlockType type = map[x][y];
                    BlockDefinition def = Game1.Instance.BlockDefinitions[type];

                    spriteBatch.Draw(def.Texture, new Rectangle(x* Block.BlockSize, y * Block.BlockSize, 32, 32), Color.White);
                }
            }

            if(selectedTile.HasValue)
            {
                Point pos = selectedTile.Value;

                Rectangle blockRect = new Rectangle(pos.X * Block.BlockSize, pos.Y * Block.BlockSize, 32, 32);

                spriteBatch.Draw(_highlightTexture, blockRect, Color.White * .3f);
            }
        }

        public BlockDefinition GetBlockDef(int x, int y)
        {
            return Game1.Instance.BlockDefinitions[map[x][y]];
        }

        public void TryPlaceBlock(BlockType type, int x, int y)
        {
            if (map[x][y] == BlockType.Air)
            {
                map[x][y] = type;
            }
        }

        public BlockType GetBlockType(int x, int y) => map[x][y];

        public void DestroyBlock(int posX, int posY)
        {
            for (int y = 0; y < map.Count; y++)
            {
                for (int x = 0; x < map[y].Count; x++)
                {
                    if(posX == x && posY == y)
                    {
                        map[x][y] = BlockType.Air;
                    }
                }
            }
        }
    }
}
