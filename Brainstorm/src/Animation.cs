using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.src
{
    internal class Animation
    {
        private Texture2D spriteSheet;
        private int totalFrames;
        private int currentFrame;
        private float frameDuration;
        private float timer;

        public Animation(Texture2D texture, int frameCount, float frameDuration)
        {
            this.spriteSheet = texture;
            this.totalFrames = frameCount;
            this.frameDuration = frameDuration;
            this.currentFrame = 0;
            this.timer = 0f;
        }

        public void Update(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= frameDuration)
            {
                currentFrame++;
                if (currentFrame >= totalFrames)
                    currentFrame = 0;

                timer = 0f;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            int frameWidth = spriteSheet.Width / totalFrames;
            Rectangle sourceRectangle = new Rectangle(frameWidth * currentFrame, 0, frameWidth, spriteSheet.Height);
            spriteBatch.Draw(spriteSheet, position, sourceRectangle, Color.White);
        }
    }
}
