using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TiledSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Brainstorm.src
{
    internal class NPC
    {
        protected int PositionListIndex { get; set; }
        protected float Velocity { get; set; } = 0f;
        protected Animation Animation { get; set; }

        
        // Static position list field
        protected static List<Vector2> PositionList { get; set; } = new();
        private static Decision Decision { set; get; }

        // Static TmxMap field
        protected static TmxMap Map { get; set; }
        protected static float MapScale { get; set; }
        
        public NPC(float velocity)
        {
            this.Velocity = velocity;

            // Get the valid tile positions on the map
            List<Vector2> validTilePositions = GetValidTilePositions();

            // Select a random valid tile position
            Random random = new();
            int randomIndex = random.Next(validTilePositions.Count);
            Vector2 randomPosition = validTilePositions[randomIndex];

            // Add the NPC to the position list
            this.PositionListIndex = PositionList.Count;
            PositionList.Add(randomPosition);
        }
        public NPC(Vector2 position, float velocity)
            : this(velocity)
        {
            
        }
        public NPC(Vector2 position, float velocity, Animation animation)
            : this(position, velocity)
        {
            this.Animation = animation;
        }
        public virtual void Update(GameTime gameTime)
        {
            // Update animation
            Animation.Update(gameTime);

            // Move to the next best position
            Move(Decision.BestPositionList[PositionListIndex]);
        }
        public static void Update(List<NPC> NPCList)
        {
            int depth = 10;
            Decision = CalculateBestNextPosition(NPCList, depth);
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
        private static List<Vector2> GetValidTilePositions() // TODO: Make it a static field to avoid recalculating
        {
            List<Vector2> validPositions = new();

            // Iterate over the map's tiles
            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    Vector2 tilePosition = new(x * Map.TileWidth * MapScale, y * Map.TileHeight * MapScale);

                    // Check if the tile position is valid and not obstructed
                    if (IsTileValid(tilePosition) && !CheckCollisionWithObstacles(tilePosition, Direction.Down))
                    {
                        validPositions.Add(tilePosition);
                    }
                }
            }
            return validPositions;
        }

        // CalculateBestNextPosition function and its helper functions

        private static Decision CalculateBestNextPosition(List<NPC> npcs, int depth)
        {
            List<Zombie> zombies = npcs.OfType<Zombie>().ToList();
            List<Human> humans = npcs.OfType<Human>().ToList();

            if (depth == 0 || humans.Count == 0)
            {
                return new Decision { MaxCaptures = 0, BestPositionList = Enumerable.Repeat(Direction.Up, npcs.Count).ToList() };
            }

            List<List<Direction>> allPossibleNextMoveLists = GetAllPossibleNextMoves(npcs);
            int maxCaptures = 0;
            List<Direction> bestNextMoves = null;
            List<NPC> simulatedNpcs;

            foreach (List<Direction> moveList in allPossibleNextMoveLists)
            {
                simulatedNpcs = SimulateNPCsMove(npcs, moveList);
                Decision next = CalculateBestNextPosition(simulatedNpcs, depth - 1);
                int captures = next.MaxCaptures + CurrentPositionCaptures(npcs);

                if (captures > maxCaptures)
                {
                    maxCaptures = captures;
                    bestNextMoves = moveList;
                }
            }

            return new Decision { MaxCaptures = maxCaptures, BestPositionList = bestNextMoves };
        }

        private static List<List<Direction>> GetAllPossibleNextMoves(List<NPC> npcs)
        {
            List<List<Direction>> allPossibleNextMoveLists = new List<List<Direction>>();

            // Generate all possible move combinations using recursion
            GenerateMoveCombinations(npcs, new List<Direction>(), allPossibleNextMoveLists);

            return allPossibleNextMoveLists;
        }

        private static void GenerateMoveCombinations(List<NPC> npcs, List<Direction> currentMoveList, List<List<Direction>> allPossibleNextMoveLists)
        {
            if (currentMoveList.Count == npcs.Count)
            {
                allPossibleNextMoveLists.Add(currentMoveList.ToList());
                return;
            }

            NPC currentNPC = npcs[currentMoveList.Count];
            List<Direction> possibleMoves = GetPossibleMoves(currentNPC);

            foreach (Direction move in possibleMoves)
            {
                currentMoveList.Add(move);
                GenerateMoveCombinations(npcs, currentMoveList, allPossibleNextMoveLists);
                currentMoveList.RemoveAt(currentMoveList.Count - 1);
            }
        }

        private static List<Direction> GetPossibleMoves(NPC npc)
        {
            // Return the possible moves for the NPC based on its current position and any other conditions
            // You need to implement this method according to your game logic
            // It should return a list of Direction objects representing the possible moves for the NPC
            // For example:
            // - If the NPC can move in all directions, you can return a list containing Direction.Up, Direction.Down, Direction.Left, and Direction.Right
            // - If the NPC can only move in a specific direction, you can return a list containing that direction only
            // - If the NPC's moves depend on other factors, you need to incorporate those factors into the logic of this method
            // Make sure to handle any edge cases or constraints specific to your game
            return new List<Direction>();
        }


        private static List<NPC> SimulateNPCsMove(List<NPC> positionList, List<Direction> moveList)
        {
            List<NPC> simulatedNpcs = new();

            // Implement the logic to simulate the NPCs' moves based on the provided move list
            // Add each simulated NPC position to the simulatedNpcs list

            return simulatedNpcs;
        }

        private static int CurrentPositionCaptures(List<NPC> positionList)
        {
            int captures = 0;

            // Implement the logic to calculate the captures at the current positions of the NPCs
            // Update the captures variable accordingly

            return captures;
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
