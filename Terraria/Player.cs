using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using ImGuiNET;
using Terraria.Util;
using System.Diagnostics;

namespace Terraria
{
    internal class Player : Sprite
    {
        private const float JumpForce = 435f;
        private const float MoveSpeed = 200f;

        private Vector2 _velocity;
        private bool _frozen;

        private KeyboardState _prevKb;
        private MouseState _prevMouse;

        private Block _selectedBlock;
        private Block _previousSelectedBlock;

        public Player(Vector2 position) : base(ContentManager.GetTexture("player"))
        {
            Position = position;
            //Origin = new(16, 24);
            _velocity = Vector2.Zero;
            _frozen = false;
            _prevKb = Keyboard.GetState();
            _prevMouse = Mouse.GetState();
            _selectedBlock = null;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public void Update(GameTime gameTime, GameWorld world, Camera camera)
        {
            if (_frozen) return;

            KeyboardState kb = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _velocity.X = MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _velocity.X = -MoveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
                    Block block = world.GetBlock(x, y);

                    if (!block.IsSolid()) continue;

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
                    Block block = world.GetBlock(x, y);

                    if (!block.IsSolid()) continue;

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
            if (kb.IsKeyDown(Keys.Space) && !_prevKb.IsKeyDown(Keys.Space))
            {
                if (IsGrounded(world))
                {
                    _velocity.Y = -JumpForce * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            Point mousePos = Mouse.GetState().Position;
            Vector2 mouseWorldPos = Vector2.Transform(new Vector2(mousePos.X, mousePos.Y), Matrix.Invert(camera.GetViewMatrix()));
            Rectangle mouseRect = new(new((int)mouseWorldPos.X,(int) mouseWorldPos.Y), new(1, 1));

            for (int y = 0; y < world.GetWorldSize().Y; y++)
            {
                for (int x = 0; x < world.GetWorldSize().X; x++)
                {
                    Block block = world.GetBlock(x, y);
                    if (block.Type != BlockType.Air)
                    {
                        bool isMouseOver = mouseRect.Intersects(block.Rectangle);
                        if(isMouseOver)
                        {
                            block.SetSelected(isMouseOver);
                            if(mouse.LeftButton == ButtonState.Pressed && _prevMouse.LeftButton == ButtonState.Released)
                            {
                                Debug.WriteLine("Destroy Block with Type: " + block.Type);
                                if(block.IsBreakable())
                                {
                                    world.DestroyBlock(block);
                                }
                            }
                        } else
                        {
                            block.SetSelected(isMouseOver);
                        }
                    }
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
                Block block = world.GetBlock(x, footTile);

                if (block.IsSolid()) return true;
            }

            return false;
        }
    }
}
