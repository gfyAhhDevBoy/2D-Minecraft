using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terraria.Items
{
    internal class BlockItem : Item
    {
        public BlockType BlockType { get; private set; }
        public BlockItem(Player player, BlockType type) : base(Game1.Instance.BlockDefinitions[type].InventoryTexture, player, Game1.Instance.BlockDefinitions[type].Name)
        {
            BlockType = type;
        }
    }
}
