﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Brainstorm.src
{
    internal class Zombie : NPC
    {
        public Zombie(Vector2 position, float velocity)
            : base( position, velocity)
        {
            // Load zombie sprite sheet
            Texture2D spriteSheet = AssetManager.LoadTexture("zombiebasic");

            // Create and return the animation using the loaded sprites
            Animation = new Animation(spriteSheet, spritesPerRow: 3, totalFrames: 6, frameDuration: 0.35f);
        }
        public override void Update(GameTime gameTime)
        {
            // Update the animation
            Animation.Update(gameTime);

            // Call the base Update method to handle movement logic
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the zombie's animation at its current position
            Animation.Draw(spriteBatch, PositionList[PositionListIndex], 0.75f);
        }
    }
}
