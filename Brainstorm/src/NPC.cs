using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainstorm.src
{
    internal class NPC
    {
        protected Texture2D texture;
        protected Vector2 position;
        protected Animation animation;

        public Vector2 Position { get; set; }

        public NPC(Texture2D texture, Vector2 position, Animation animation)
        {
            this.texture = texture;
            this.position = position;
            this.animation = animation;
        }

        public virtual void Update(GameTime gameTime)
        {
            animation.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(spriteBatch, position);
        }
    }
}
