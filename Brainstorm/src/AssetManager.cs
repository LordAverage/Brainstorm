using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Brainstorm.src
{
    internal class AssetManager
    {
        private static ContentManager _content;

        public static void Initialize(ContentManager content)
        {
            _content = content;
        }

        public static Texture2D LoadTexture(string assetName)
        {
            return _content.Load<Texture2D>(assetName);
        }
    }

}
