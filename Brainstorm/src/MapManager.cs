using Microsoft.Xna.Framework.Graphics;
using System;
using TiledSharp;
using Microsoft.Xna.Framework;

namespace Brainstorm.src
{
    public class MapManager
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly TmxMap _map;
        private readonly Texture2D _tileset;
        private readonly int _tilesetTilesPerRow;
        private readonly int _tileWidth;
        private readonly int _tileHeight;
        private float _scale;
        public float Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }
        public MapManager(SpriteBatch _spriteBatch, TmxMap _map, Texture2D _tileset,
            int _tilesetTilesWide, int _tileWidth, int _tileHeight)
        : this(_spriteBatch, _map, _tileset, _tilesetTilesWide, _tileWidth, _tileHeight, 1f)
        {
        }
        public MapManager(SpriteBatch _spriteBatch, TmxMap _map, Texture2D _tileset,
            int _tilesetTilesWide, int _tileWidth, int _tileHeight, float _scale)
        {
            this._spriteBatch = _spriteBatch;
            this._map = _map;
            this._tileset = _tileset;
            this._tilesetTilesPerRow = _tilesetTilesWide;
            this._tileWidth = _tileWidth;
            this._tileHeight = _tileHeight;
            this._scale = _scale;
        }
        public void Draw()
        {
            for (var i = 0; i < _map.Layers.Count; i++)
            {
                for (var j = 0; j < _map.Layers[i].Tiles.Count; j++)
                {
                    int gid = _map.Layers[i].Tiles[j].Gid;
                    if (gid == 0)
                    {
                        // If empty, do nothing
                    }
                    else
                    {
                        int tileFrame = gid - 1;
                        int column = tileFrame % _tilesetTilesPerRow;
                        int row = (int)Math.Floor((double)tileFrame / (double)_tilesetTilesPerRow);
                        float x = (j % _map.Width) * _map.TileWidth * _scale;
                        float y = (float)Math.Floor(j / (double)_map.Width) * _map.TileHeight * _scale;
                        Rectangle tilesetRec = new((_tileWidth) * column, (_tileHeight) * row,
                            _tileWidth, _tileHeight);
                        _spriteBatch.Draw(_tileset, new Rectangle((int)x, (int)y, (int)(_tileWidth * _scale), (int)(_tileHeight * _scale)),
                            tilesetRec, Color.White);
                    }
                }
            }
        }

    }
}
