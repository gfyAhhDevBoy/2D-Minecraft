using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Items;
using Terraria.Util;

namespace Terraria
{
    internal class Inventory
    {
        public List<Slot> Slots { get; private set; }
        public int SelectedIndex { get; set; } = 0;

        public int Capacity => Slots.Count;

        private Player _player;

        private Texture2D _slotTexture;
        private SpriteFont _font;


        private readonly int _uiScale;

        public Inventory(int capacity, Player player)
        {
            _player = player;
            Slots = new List<Slot>();
            for (int i = 0; i < capacity; i++)
            {
                Slots.Add(new Slot(new Air(player), 0, i));
            }

            _font = ContentManager.GetFont("inventory");
            _slotTexture = new(Game1.Instance.GraphicsDevice, 1, 1);
            _slotTexture.SetData([Color.White]);
            _uiScale = 2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Slots.Count; i++)
            {
                var slot = Slots[i];
                Rectangle rect = new Rectangle(10 + i * 56, 10, 48,48);
                
                spriteBatch.Draw(_slotTexture, slot.Rectangle, Color.White * .5f);

                
                if (!slot.IsEmpty)
                {
                    Texture2D itemTex = slot.Item.GetTexture();
                    spriteBatch.Draw(itemTex, slot.Rectangle, Color.White);


                    //string countText = slot.Count > 1 ? slot.Count.ToString() : "";
                    string countText = slot.Count.ToString();
                    spriteBatch.DrawString(_font, countText, new Vector2(rect.X + 38, rect.Y + 32), Color.Black);
                }

                if(i == SelectedIndex)
                {
                    spriteBatch.Draw(_slotTexture, slot.Rectangle, Color.White * .3f);
                }
            }
        }

        public void AddItem(Item item)
        {
            AddItem(item, 1);
        }

        public void AddItem(Item item, int amount)
        {
            foreach(var slot in Slots)
            {
                if(item is BlockItem && slot.Item is BlockItem)
                {
                    BlockItem blockItem = item as BlockItem;
                    BlockItem slotItem = slot.Item as BlockItem;
                    
                    if(blockItem.BlockType == slotItem.BlockType && slot.Count < 999)
                    {
                        int space = 999 - slot.Count;
                        //if (amount < space) return;
                        //slot.Count += amount;

                        int toAdd = Math.Min(space, amount);
                        slot.Count += toAdd;
                        amount -= toAdd;
                        if (amount <= 0) return;
                    }
                } else
                if (slot.Item.GetType() == item.GetType() && slot.Count < 999)
                {
                    int space = 999 - slot.Count;
                    //if (amount < space) return;
                    //slot.Count += amount;

                    int toAdd = Math.Min(space, amount);
                    slot.Count += toAdd;
                    amount -= toAdd;
                    if (amount <= 0) return;
                }
            }

            foreach(var slot in Slots)
            {
                if(slot.IsEmpty)
                {
                    slot.Item = item;
                    slot.Count = amount;
                    return;
                }
            }
        }

        public void RemoveItem(int index, int amount)
        {
            if(index >= 0 && index < Capacity)
            {
                var slot = Slots[index];

                if (slot.Count == amount)
                {
                    slot.Item = new Air(_player);
                    slot.Count = 0;
                }
                else if (slot.Count > amount)
                {
                    slot.Count -= amount;
                }
            } 
        }

        public void NextSlot()
        {
            if(SelectedIndex >= Capacity - 1)
            {
                SelectedIndex = 0;
            } else
            {
                SelectedIndex++;
            }
        }

        public void PreviousSlot()
        {
            if(SelectedIndex <= 0)
            {
                SelectedIndex = Capacity - 1;
            } else
            {
                SelectedIndex--;
            }
        }

        public Slot GetCurrentSlot() => Slots[SelectedIndex];
    }

    class Slot
    {
        public Item Item { get; set; }
        public int Count { get; set; }
        public int Index { get; private set; } 

        public Rectangle Rectangle
        {
            get
            {
                return new(10 + Index * 56, 10, 48, 48);
            }
        }

        public bool IsEmpty => Count <= 0;

        public Slot(Item item, int count, int index)
        {
            Item = item;
            Count = count;
            Index = index;
        }

    }
}
