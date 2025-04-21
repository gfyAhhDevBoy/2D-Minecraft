using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Terraria
{
    public class BlockDefinition
    {
        public string Name;
        public Texture2D Texture;
        public Texture2D InventoryTexture;
        public bool IsSolid;
        public bool IsBreakable;
        public Color MinimapColor;
    }

}
