using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TiledSharp;

namespace Brainstorm.src
{
    public class Simulation : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private TmxMap _map;
        private MapManager _mapManager;

        public Simulation()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _mapManager.Draw();
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}