using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Terraria.Util;
using ImGuiNET;
using MonoGame.ImGuiNet;
using System.Collections.Generic;

namespace Terraria
{
    public class Game1 : Game
    {
        public static Game1 Instance;

        public Dictionary<BlockType, BlockDefinition> BlockDefinitions;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static ImGuiRenderer GuiRenderer;

        public const float Gravity = 23f;

        public const int ScreenWidth = 960;
        public const int ScreenHeight = 540;

        private Texture2D _blockMap;

        private GameWorld _world;

        Player _player;

        private Camera _camera;

        public Game1()
        {
            Instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.ApplyChanges();

            ContentManager.Init(Content);
            _camera = new();

            BlockDefinitions = new();


            GuiRenderer = new(this);

            _world = new();
            

            _player = new(new(50,50));

            base.Initialize();
        }

        private void LoadBlockDefinitions()
        {
            BlockDefinitions[BlockType.Air] = new BlockDefinition
            {
                Texture = GetTextureFromBlockType(BlockType.Air),
                IsSolid = false,
                IsBreakable = false,
            };
            BlockDefinitions[BlockType.Stone] = new BlockDefinition
            {
                Texture = GetTextureFromBlockType(BlockType.Stone),
                IsSolid = true,
                IsBreakable = true
            };
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _blockMap = Content.Load<Texture2D>("blockmap");
            Debug.WriteLine(_blockMap.Width + "x" + _blockMap.Height);

            LoadBlockDefinitions();


            _world.GenerateWorld();

            GuiRenderer.RebuildFontAtlas();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _player.Update(gameTime, _world, _camera);

            _camera.SetCenter(_player.Position, ScreenWidth, ScreenHeight);
            
            
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GuiRenderer.BeginLayout(gameTime);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());

            _world.Draw(gameTime, _spriteBatch, _player.SelectedTile);

            _player.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            

            GuiRenderer.EndLayout();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public Texture2D GetTextureFromBlockType(BlockType blockType)
        {
            

            Rectangle sourceRect = new((int)blockType * 16,0,16,16);
            //Debug.WriteLine(sourceRect);

            Color[] data = new Color[16*16];
            _blockMap.GetData(0, sourceRect, data, 0, data.Length);
            

            Texture2D blockTexture = new Texture2D(_graphics.GraphicsDevice, 16,16);
            blockTexture.SetData(data);

            return blockTexture;
        }
    }
}
