﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Brainstorm.src
{
    internal class Human : NPC
    {
        public Human(Vector2 position, float velocity)
            : base(position, velocity)
        {
            // Load zombie sprite sheet
            Texture2D spriteSheet = AssetManager.LoadTexture("player_run_strip6");

            // Create and return the animation using the loaded sprites
            Animation = new Animation(spriteSheet, spritesPerRow: 1, totalFrames: 6, frameDuration: 0.3f);
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