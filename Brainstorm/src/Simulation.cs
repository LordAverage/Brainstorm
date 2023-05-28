using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TiledSharp;

namespace Brainstorm.src
{
    public class Simulation : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TmxMap _map;
        private MapManager _mapManager;
        private List<NPC> NPCList = new();

        public Simulation()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            AssetManager.Initialize(Content);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // Load the background image
            _map = new TmxMap("Content/map.tmx");
            var tileset = Content.Load<Texture2D>("tileset");
            var tileWidth = _map.Tilesets[0].TileWidth;
            var tileHeight = _map.Tilesets[0].TileHeight;
            var tilesetTilesPerRow = tileset.Width / tileWidth;
            var scale = 1.5f;
            _mapManager = new MapManager(_spriteBatch, _map, tileset, tilesetTilesPerRow,
            tileWidth, tileHeight, scale);
            NPC.SetMap(_map, scale);

            // Create the NPCs
            //NPCList.Add(new Zombie(new Vector2(15 * tileWidth * scale, 10 * tileWidth * scale)));
            NPCList.Add(new Zombie());
            NPCList.Add(new Zombie());
            NPCList.Add(new Zombie());
            NPCList.Add(new Human());
            NPCList.Add(new Human());

            // Set the window size to match the map dimensions
            _graphics.PreferredBackBufferWidth = (int)(_map.Width * tileWidth * scale);
            _graphics.PreferredBackBufferHeight = (int)(_map.Height * tileHeight * scale);
            _graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            NPC.Update(NPCList);
            foreach (var npc in NPCList)
            {
                npc.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _mapManager.Draw();
            foreach (var npc in NPCList)
            {
                npc.Draw(_spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}