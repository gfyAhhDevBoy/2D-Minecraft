using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

namespace Terraria
{
    internal class GameWorld
    {
        public const int InitialWorldWidth = 50, InitialWorldHeight = 30;

        private Block[,] map;

        public GameWorld() {
            map = new Block[InitialWorldWidth, InitialWorldHeight];
        }

        public Vector2 GetWorldSize() => new(map.GetLength(0), map.GetLength(1));

        public void GenerateWorld()
        {
            for(int y = 0; y < InitialWorldHeight; y++)
            {
                for(int x = 0; x < InitialWorldWidth; x++)
                {
                    if (y < 10)
                        map[x, y] = new Block(0, 0, new(x, y));
                    else
                        map[x, y] = new Block(BlockType.Stone, BlockFlags.Solid | BlockFlags.Breakable, new(x, y));
                }
            }

            map[20, 9] = new Block(BlockType.Stone, BlockFlags.Solid | BlockFlags.Breakable, new(20, 9));
        }

        public void UpdateBlocks(GameTime gameTime)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    map[x, y].Update(gameTime);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    map[x,y].Draw(gameTime, spriteBatch);
                }
            }
        }

        public Block GetBlock(int x, int y)
        {
            return map[x,y];
        }

        public void DestroyBlock(Block block)
        {
            
        }

        public void DestroyBlock(int x, int y)
        {
            DestroyBlock(map[x,y]);
        }

    }
}
