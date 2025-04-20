using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Terraria
{
    internal class GameWorld
    {
        public const int InitialWorldWidth = 50, InitialWorldHeight = 30;

        private List<List<Block>> map;

        public GameWorld()
        {
            map = new List<List<Block>>(InitialWorldWidth);
            for (int x = 0; x < InitialWorldWidth; x++)
            {
                map.Add(new List<Block>(InitialWorldHeight));
                for (int y = 0; y < InitialWorldHeight; y++)
                {
                    map[x].Add(null);
                }
            }
        }

        public Vector2 GetWorldSize() => new(map.Count, map[0].Count);

        public void GenerateWorld()
        {
            for (int y = 0; y < InitialWorldHeight; y++)
            {
                for (int x = 0; x < InitialWorldWidth; x++)
                {
                    if (y < 10)
                        map[x][y] = new Block(0, 0, new(x, y));
                    else
                        map[x][y] = new Block(BlockType.Stone, BlockFlags.Solid | BlockFlags.Breakable, new(x, y));
                }
            }

            map[20][9] = new Block(BlockType.Stone, BlockFlags.Solid | BlockFlags.Breakable, new(20, 9));
        }

        public void UpdateBlocks(GameTime gameTime)
        {
            for (int y = 0; y < map[0].Count; y++)
            {
                for (int x = 0; x < map.Count; x++)
                {
                    map[x][y].Update(gameTime);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < map[0].Count; y++)
            {
                for (int x = 0; x < map.Count; x++)
                {
                    map[x][y].Draw(gameTime, spriteBatch);
                }
            }
        }

        public Block GetBlock(int x, int y)
        {
            return map[x][y];
        }

        public void DestroyBlock(Block block)
        {
        
        }

        public void DestroyBlock(int x, int y)
        {
            DestroyBlock(map[x][y]);
        }
    }
}
