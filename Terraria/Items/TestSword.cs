using System.Diagnostics;
using Terraria.Util;

namespace Terraria.Items
{
    internal class TestSword : Item, IInteractable
    {
        public TestSword(Player player) : base(ContentManager.GetTexture("sword"), player, "Sigma Sword")
        {

        }

        public override void Interact(Player player)
        {
            Debug.WriteLine(Name + " clicked!");
        }
    }
}
