using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terraria
{
    internal class Chunk
    {
        public const int ChunkWidth = 16, ChunkHeight = 16;

        List<List<BlockType>> blocks;

        public Chunk()
        {
            blocks = new(ChunkWidth);
            for(int x = 0; x < ChunkWidth; x++)
            {
                blocks.Add(new List<BlockType>(ChunkHeight));
                for(int y = 0; y < ChunkHeight; y++)
                {
                    blocks.Add
                }
            }
        }
    }
}
