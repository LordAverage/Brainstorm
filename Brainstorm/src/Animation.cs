using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Brainstorm.src
{
    internal class Animation
    {
        private Texture2D SpriteSheet { get; }
        private int SpritesPerRow { get; }
        private int TotalFrames { get; }
        private int CurrentFrame { get; set; }
        private float FrameDuration { get; }
        private float Timer { get; set; }
        private Direction Direction { get; set; } = Direction.Up;
        public Direction GetDirection()
        {
            return Direction;
        }

        public void SetDirection(Direction direction)
        {
            Direction = direction;
        }

        public Animation(Texture2D texture, int spritesPerRow, int totalFrames, float frameDuration)
        {
            this.SpriteSheet = texture;
            this.SpritesPerRow = spritesPerRow;
            this.TotalFrames = totalFrames;
            this.FrameDuration = frameDuration;
            this.CurrentFrame = 0;
            this.Timer = 0f;
        }

        public void Update(GameTime gameTime)
        {
            Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (Timer >= FrameDuration)
            {
                CurrentFrame++;
                if (CurrentFrame >= TotalFrames)
                    CurrentFrame = 0;

                Timer = 0f;
            }
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Draw(spriteBatch, position, 1f);
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale)
        {
            int frameWidth = SpriteSheet.Width / SpritesPerRow;
            int frameHeight = frameWidth;
            int row = CurrentFrame / SpritesPerRow;
            int column = CurrentFrame % SpritesPerRow;
            Rectangle sourceRectangle = new(frameWidth * column, frameHeight * row, frameWidth, frameHeight);
            float rotation = GetSpriteRotationFromDirection();
            Vector2 origin = GetOriginFromDirection(); // Use the center of the frame as the origin
            spriteBatch.Draw(SpriteSheet, position, sourceRectangle, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
        }
        private float GetSpriteRotationFromDirection()
        {
            return Direction switch
            {
                Direction.Up => 0f,
                Direction.Down => MathHelper.Pi,
                Direction.Left => -MathHelper.PiOver2,
                Direction.Right => MathHelper.PiOver2,
                _ => 0f,
            };
        }
        private Vector2 GetOriginFromDirection()
        {
            int frameWidth = SpriteSheet.Width / SpritesPerRow;
            int frameHeight = frameWidth;
            return Direction switch
            {
                Direction.Up => Vector2.Zero,
                Direction.Down => new Vector2(frameWidth, frameHeight),
                Direction.Left => new Vector2(frameWidth, 0),
                Direction.Right => new Vector2(0, frameHeight),
                _ => Vector2.Zero,
            };
        }
    }
}
