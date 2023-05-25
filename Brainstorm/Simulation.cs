using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Formats.Asn1.AsnWriter;

namespace Brainstorm
{
    public class Simulation : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D mapTexture;

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
            mapTexture = Content.Load<Texture2D>("map");
            mapTexture = ScaleTexture(mapTexture, 1.5f);
            // Set the window size to match the map dimensions
            _graphics.PreferredBackBufferWidth = mapTexture.Width;
            _graphics.PreferredBackBufferHeight = mapTexture.Height;
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
            _spriteBatch.Draw(mapTexture, Vector2.Zero, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private Texture2D ScaleTexture(Texture2D texture, float scale)
        {
            int newWidth = (int)(texture.Width * scale);
            int newHeight = (int)(texture.Height * scale);

            RenderTarget2D result = new(GraphicsDevice, newWidth, newHeight);
            GraphicsDevice.SetRenderTarget(result);
            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap, transformMatrix: Matrix.CreateScale(scale));
            _spriteBatch.Draw(texture, Vector2.Zero, Color.White);
            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            return result;
        }
    }
}