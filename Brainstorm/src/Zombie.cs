using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Brainstorm.src
{
    internal class Zombie : NPC
    {
        public Zombie()
            : this(200f)
        {

        }
        public Zombie(float velocity)
            : base(velocity)
        {
            // Load zombie sprite sheet
            Texture2D spriteSheet = AssetManager.LoadTexture("zombiebasic");

            // Create and return the animation using the loaded sprites
            Animation = new Animation(spriteSheet, spritesPerRow: 3, totalFrames: 6, frameDuration: 0.175f);
        }
        public Zombie(Vector2 position)
            : this(position, 200f)
        {
            
        }
        public Zombie(Vector2 position, float velocity)
            : base(position, velocity)
        {
            // Load zombie sprite sheet
            Texture2D spriteSheet = AssetManager.LoadTexture("zombiebasic");

            // Create and return the animation using the loaded sprites
            Animation = new Animation(spriteSheet, spritesPerRow: 3, totalFrames: 6, frameDuration: 0.175f);
        }
        public override void Update(GameTime gameTime)
        {
            // Call the base Update method to handle movement and animation logic
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the zombie's animation at its current position
            Animation.Draw(spriteBatch, PositionList[PositionListIndex], 0.75f);
        }
    }
}
