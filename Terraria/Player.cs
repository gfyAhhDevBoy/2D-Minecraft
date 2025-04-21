using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using ImGuiNET;
using Terraria.Util;
using System.Diagnostics;
using System.Reflection.Metadata;
using Terraria.Items;

namespace Terraria
{

    internal class Player : Sprite
    {
        private const float JumpForce = 435f;
        private float _moveSpeed = 200f;

        private Vector2 _velocity;
        private bool _frozen;

        private Point? _selectedTile = null;
        public Point? SelectedTile { get => _selectedTile; }

        public readonly Vector2 ItemOrigin;

        private KeyboardState _prevKb;
        private MouseState _prevMouse;

        private Block _selectedBlock;
        private Block _previousSelectedBlock;

        Inventory _inventory;
        public Inventory Inventory { get => _inventory; }

        public Player(Vector2 position) : base(ContentManager.GetTexture("player"))
        {
            Position = position;
            //Origin = new(16, 24);
            _velocity = Vector2.Zero;
            _frozen = false;
            _prevKb = Keyboard.GetState();
            _prevMouse = Mouse.GetState();
            _selectedBlock = null;
            ItemOrigin = new(30, 24);

            _inventory = new(9, this);

            _inventory.AddItem(new TestSword(this), 1);
            _inventory.AddItem(new BlockItem(this, BlockType.Stone));
            _inventory.AddItem(new BlockItem(this, BlockType.Dirt));
            _inventory.AddItem(new TestSword(this), 1);

            InputManager.MouseScrollEvent += InputManager_MouseScrollEvent;
            InputManager.KeyPressEvent += InputManager_KeyPressEvent;
        }

        private void InputManager_KeyPressEvent(object sender, KeboardEventArgs e)
        {

            switch (e.Key)
            {
                case Keys.Right:
                    _inventory.NextSlot(); break;
                case Keys.Left:
                    _inventory.PreviousSlot(); break;

                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    Debug.WriteLine(Int32.Parse(e.Key.ToString().Trim('D')) - 1);
                    //break;
                    _inventory.SelectedIndex = Int32.Parse(e.Key.ToString().Trim('D')) - 1; break;

            }
        }

        private void InputManager_MouseScrollEvent(object sender, ScrollWheelEventArgs e)
        {
            if (e.Dir == ScrollWheelDirection.Up)
            {
                _inventory.NextSlot();
            }
            else if (e.Dir == ScrollWheelDirection.Down) { _inventory.PreviousSlot(); }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Update(GameTime gameTime, GameWorld world, Camera camera)
        {
            if (_frozen || !Game1.Instance.IsActive) return;

            KeyboardState kb = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _velocity.X = _moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _velocity.X = -_moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (!InputManager.IsAnyKeyPressed())
            {
                _velocity.X = 0;
            }

            _velocity.Y += Game1.Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += new Vector2(_velocity.X, 0);

            int left = (int)(Position.X / Block.BlockSize);
            int right = (int)(Position.X + Rectangle.Width) / Block.BlockSize;
            int top = (int)(Position.Y / Block.BlockSize);
            int bottom = (int)(Position.Y + Rectangle.Height) / Block.BlockSize;
            left = Math.Max(0, left);
            right = Math.Min(GameWorld.InitialWorldWidth - 1, right);
            top = Math.Max(0, top);
            bottom = Math.Min(GameWorld.InitialWorldHeight - 1, bottom);

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    BlockDefinition block = world.GetBlockDef(x, y);

                    if (!block.IsSolid) continue;

                    float blockX = x * Block.BlockSize;
                    float blockY = y * Block.BlockSize;

                    if (Position.X < blockX + Block.BlockSize &&
                       Position.X + Rectangle.Width > blockX &&
                       Position.Y < blockY + Block.BlockSize &&
                       Position.Y + Rectangle.Height > blockY)
                    {
                        if (_velocity.X > 0)
                        {
                            Position = new(blockX - Rectangle.Width, Position.Y);
                        }
                        else if (_velocity.X < 0)
                        {
                            Position = new(blockX + Block.BlockSize, Position.Y);
                        }

                        _velocity.X = 0;
                    }
                }
            }
            Position += new Vector2(0, _velocity.Y);

            left = Math.Max(0, (int)(Position.X / Block.BlockSize));
            right = Math.Min(GameWorld.InitialWorldWidth - 1, (int)((Position.X + Rectangle.Width) / Block.BlockSize));
            top = Math.Max(0, (int)Position.Y / Block.BlockSize);
            bottom = Math.Min(GameWorld.InitialWorldHeight - 1, (int)((Position.Y + Rectangle.Height) / Block.BlockSize));

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    BlockDefinition block = world.GetBlockDef(x, y);

                    if (!block.IsSolid) continue;

                    float blockX = x * Block.BlockSize;
                    float blockY = y * Block.BlockSize;

                    if (Position.X < blockX + Block.BlockSize &&
                       Position.X + Rectangle.Width > blockX &&
                       Position.Y < blockY + Block.BlockSize &&
                       Position.Y + Rectangle.Height > blockY)
                    {
                        if (_velocity.Y > 0)
                        {
                            Position = new(Position.X, blockY - Rectangle.Height);
                        }
                        else if (_velocity.Y < 0)
                        {
                            Position = new(Position.X, blockY + Block.BlockSize);
                        }

                        _velocity.Y = 0;
                    }

                }
            }
            if (kb.IsKeyDown(Keys.Space))
            {
                if (IsGrounded(world))
                {
                    _velocity.Y = -JumpForce * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            Point mousePos = Mouse.GetState().Position;
            Vector2 mouseWorldPos = Vector2.Transform(mousePos.ToVector2(), Matrix.Invert(camera.GetViewMatrix()));
            Rectangle mouseRect = new Rectangle((int)mouseWorldPos.X, (int)mouseWorldPos.Y, 1, 1);

            // Get the tile coordinate the mouse is over
            int tileX = (int)(mouseWorldPos.X / Block.BlockSize);
            int tileY = (int)(mouseWorldPos.Y / Block.BlockSize);

            // Clamp
            if (tileX < 0 || tileY < 0 || tileX >= world.GetWorldSize().X || tileY >= world.GetWorldSize().Y)
                return;

            // Get tile data
            BlockType type = world.GetBlockType(tileX, tileY);
            BlockDefinition def = Game1.Instance.BlockDefinitions[type];

            // Only interact if it's not Air
            if (type != BlockType.Air)
            {
                Rectangle tileRect = new Rectangle(tileX * Block.BlockSize, tileY * Block.BlockSize, Block.BlockSize, Block.BlockSize);

                if (mouseRect.Intersects(tileRect))
                {
                    // If needed, store this tile as "selected"
                    _selectedTile = new Point(tileX, tileY); // optional, if you want to draw a highlight

                    if (mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released)
                    {
                        Debug.WriteLine($"Destroy Block with Type: {type}");
                        if (def.IsBreakable)
                        {
                            BlockType block = world.DestroyBlock(tileX, tileY, this);
                            if(block != BlockType.Air) 
                            _inventory.AddItem(new BlockItem(this, block));
                        }
                    }
                }
                else
                {
                    _selectedTile = null;
                }
            }
            if (mouse.RightButton == ButtonState.Pressed && _prevMouse.RightButton == ButtonState.Released)
            {
                if (_inventory.GetCurrentSlot().Item is BlockItem)
                {
                    BlockItem blockitem = _inventory.GetCurrentSlot().Item as BlockItem;
                    if(world.TryPlaceBlock(blockitem.BlockType, tileX, tileY))
                        _inventory.RemoveItem(_inventory.GetCurrentSlot().Index, 1);
                } else if(_inventory.GetCurrentSlot().Item is IInteractable)
                {
                    _inventory.GetCurrentSlot().Item.Interact(this);
                }

            }

            foreach(var slot in _inventory.Slots)
            {
                if(mouseRect.Intersects(slot.Rectangle))
                {
                    Debug.WriteLine("Slot");
                }
            }

            _prevKb = kb;
            _prevMouse = mouse;
            _previousSelectedBlock = _selectedBlock;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);

            ImGui.Begin("Player");
            ImGui.InputFloat("posX", ref _position.X);
            ImGui.InputFloat("posY", ref _position.Y);
            ImGui.Checkbox("Freeze", ref _frozen);
            ImGui.InputFloat("speed", ref _moveSpeed);

            ImGui.End();
        }

        private bool IsGrounded(GameWorld world)
        {
            float epsilon = .1f;

            float feetY = Position.Y + Rectangle.Height + epsilon;

            int left = (int)(Position.X / Block.BlockSize);
            int right = (int)((Position.X + Rectangle.Width) / Block.BlockSize);
            int footTile = (int)(feetY / Block.BlockSize);

            left = Math.Max(0, left);
            right = Math.Min(GameWorld.InitialWorldWidth - 1, right);
            footTile = Math.Min(GameWorld.InitialWorldHeight - 1, footTile);

            for (int x = left; x <= right; ++x)
            {
                BlockDefinition block = world.GetBlockDef(x, footTile);

                if (block.IsSolid) return true;
            }

            return false;
        }
    }
}
