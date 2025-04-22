using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Items;
using SimplexNoise;
using System.Diagnostics;
using System;

namespace Terraria
{
    struct Walker
    {
        public int X;
        public int Y;
        public int Life;
    }
   
    internal class GameWorld
    {
        public const int InitialWorldWidth = 700, InitialWorldHeight = 128;
        float _terrainScale = 0.01f;
        float _caveThreshold = .3f;

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

        public void GenerateWorld(int seed)
        {
            List<Walker> walkers = new();
            Noise.Seed = seed;
            FastNoiseLite caveNoise = new FastNoiseLite();
            caveNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            Random random = new(seed);
            double walkerModifier;
            do
            {
                walkerModifier = random.NextDouble();
            } while(walkerModifier == 0);

            for(int i = 0; i < 100; i++)
            {
                walkers.Add(new Walker
                {
                    X = random.Next(InitialWorldWidth * Block.BlockSize),
                    Y = random.Next(InitialWorldHeight * Block.BlockSize),
                    Life = 200 + random.Next(50)
                });
            }

            for(int x = 0; x < InitialWorldWidth;x++)
            {
                float noiseValue = Noise.CalcPixel1D(x, _terrainScale);
                int height = (int)(noiseValue / 255f * 100f);

                for(int y = 0; y < InitialWorldHeight;y++)
                {
                    if (y < height)
                    {
                        map[x][y] = BlockType.Air;
                    }
                    else if (y == height)
                        map[x][y] = BlockType.Grass;
                    else if (y < height + 5)
                        map[x][y] = BlockType.Dirt;
                    else
                    {
                        //map[x][y] = BlockType.Stone;
                        float caveValue = caveNoise.GetNoise(x, y);
                        //Debug.WriteLine(caveValue);
                        if(caveValue < -.7 || caveValue > .9)
                        {
                            map[x][y] = BlockType.Air;
                        } else
                        {
                            map[x][y] = BlockType.Stone;
                        }
                    }
                }
            }

            while(walkers.Count > 0)
            {
                for(int i = walkers.Count - 1; i >= 0; i--)
                {
                    var walker = walkers[i];

                    for(int dx = -1; dx <= 1; dx++)
                    {
                        for(int dy = -1; dy <= 1; dy++)
                        {
                            int tx = walker.X + dx;
                            int ty = walker.Y + dy; 
                            if(tx >= 0 && tx < InitialWorldWidth && ty >= 0 && ty < InitialWorldHeight)
                            {
                                map[tx][ty] = BlockType.Air;
                            }
                        }
                    }

                    walker.X += random.Next(-1, 2);
                    walker.Y += random.Next(-1, 2);

                    walker.X = Math.Clamp(walker.X, 1, InitialWorldWidth - 2);
                    walker.Y = Math.Clamp(walker.Y, 50, InitialWorldHeight - 2);

                    walker.Life--;
                    if (walker.Life <= 0)
                        walkers.RemoveAt(i);
                    else
                        walkers[i] = walker;

                    //if(random.NextDouble() < 0.01)
                    //{
                    //    walkers.Add(new Walker
                    //    {
                    //        X = walker.X,
                    //        Y = walker.Y,
                    //        Life = 100 + random.Next(50)
                    //    });
                    //}
                }
            }
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

        public bool TryPlaceBlock(BlockType type, int x, int y)
        {
            if (map[x][y] == BlockType.Air)
            {
                map[x][y] = type;
                return true;
            }
            return false;
        }

        public BlockType GetBlockType(int x, int y) => map[x][y];

        public BlockType DestroyBlock(int posX, int posY, Player player)
        {
            for (int y = 0; y < map.Count; y++)
            {
                for (int x = 0; x < map[y].Count; x++)
                {
                    if(posX == x && posY == y)
                    {
                        BlockType originalBlock = map[x][y];
                        map[x][y] = BlockType.Air;
                        return originalBlock;
                        break;
                    }
                }
            }
            return BlockType.Air;
        }
    }
}
