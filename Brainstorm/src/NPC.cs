using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TiledSharp;
using System;
using System.Collections.Generic;

namespace Brainstorm.src
{
    internal class NPC
    {
        protected int PositionListIndex { get; set; }
        protected float Velocity { get; set; } = 0f;
        protected Animation Animation { get; set; }

        // Static TmxMap field
        protected static TmxMap Map { get; set; }
        // Static position list field
        protected static List<Vector2> PositionList { get; set; } = new();
        protected static float MapScale { get; set; }

        public NPC(Vector2 position, float velocity)
        {
            this.PositionListIndex = PositionList.Count;
            PositionList.Add(position);
            this.Velocity = velocity;
        }
        public NPC(Vector2 position, float velocity, Animation animation)
            : this(position, velocity)
        {
            this.Animation = animation;
        }
        public virtual void Update(GameTime gameTime)
        {
            Animation.Update(gameTime);
            Move(DecideNextStep());
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Animation.Draw(spriteBatch, PositionList[PositionListIndex]);
        }

        private bool Move(Direction direction)
        {
            Animation.SetDirection(direction);

            // Calculate the target position based on the current position and direction
            Vector2 targetPosition = GetTargetPosition(direction);

            // Set movement increment
            float movementIncrement = 0.05f;

            Vector2 nextPosition = Vector2.Lerp(PositionList[PositionListIndex], targetPosition, movementIncrement);

            // Check if the target tile position is valid and not obstructed
            if (IsTileValid(nextPosition) && !CheckCollisionWithObstacles(nextPosition, direction))
            {
                // Update the NPC's position gradually towards the target position
                PositionList[PositionListIndex] = nextPosition;

                return true; // Movement successful
            }
            int finalTileX = (int)Math.Ceiling(PositionList[PositionListIndex].X / Map.TileWidth / MapScale);
            int finalTileY = (int)Math.Ceiling(PositionList[PositionListIndex].Y / Map.TileHeight / MapScale);
            switch (direction)
            {
                case Direction.Up:
                    finalTileY--;
                    break;
                case Direction.Left:
                    finalTileX--;
                    break;
            }
            PositionList[PositionListIndex] = new Vector2(finalTileX * Map.TileWidth * MapScale, finalTileY * Map.TileHeight * MapScale);
            return false; // Movement failed
        }

        private Vector2 GetTargetPosition(Direction direction)
        {
            // Calculate the target tile position based on the current position and direction
            return direction switch
            {
                Direction.Up => new Vector2(PositionList[PositionListIndex].X, PositionList[PositionListIndex].Y - Map.TileHeight * MapScale),
                Direction.Down => new Vector2(PositionList[PositionListIndex].X, PositionList[PositionListIndex].Y + Map.TileHeight * MapScale),
                Direction.Left => new Vector2(PositionList[PositionListIndex].X - Map.TileWidth * MapScale, PositionList[PositionListIndex].Y),
                Direction.Right => new Vector2(PositionList[PositionListIndex].X + Map.TileWidth * MapScale, PositionList[PositionListIndex].Y),
                _ => PositionList[PositionListIndex],
            };
        }
        private static bool IsTileValid(Vector2 tilePosition)
        {
            // Check if the tile position is within the map's bounds
            if (tilePosition.X >= 0 && tilePosition.X < Map.TileWidth * MapScale * (Map.Width - 1)  && tilePosition.Y >= 0 && tilePosition.Y < Map.TileHeight * MapScale * (Map.Height - 1))
            {
                return true; // Tile position is valid
            }
            else
            {
                return false; // Tile position is invalid
            }
        }

        protected static bool CheckCollisionWithObstacles(Vector2 position, Direction direction)
        {
            // Convert position to tile coordinates
            int tileX = (int)Math.Ceiling((position.X / Map.TileWidth / MapScale));
            int tileY = (int)Math.Ceiling((position.Y / Map.TileHeight / MapScale));
            switch (direction)
            {
                case Direction.Up: tileY--;
                    break;
                case Direction.Left: tileX--;
                    break;
            }

            // Check if the tile position corresponds to a collision tile
            if (tileX >= 0 && tileX < Map.Width && tileY >= 0 && tileY < Map.Height)
            {
                // Iterate over the map's layers
                foreach (TmxLayer layer in Map.Layers)
                {
                    // Check if the layer has the "Collision" property set to true
                    if (layer.Properties.ContainsKey("Collision") && layer.Properties["Collision"] == "true")
                    {
                        // Retrieve the index of the tile in the one-dimensional array
                        int tileIndex = tileX + tileY * Map.Width;

                        // Retrieve the tile GID (Global Identifier) at the specified index
                        int tileGid = layer.Tiles[tileIndex].Gid;

                        // Check if the tile GID is greater than 0
                        if (tileGid > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // Static method to set the TmxMap
        public static void SetMap(TmxMap map, float scale)
        {
            Map = map;
            MapScale = scale;
        }
        public static void SetMap(TmxMap map)
        {
            SetMap(map, 1f);
        }
    }
}
