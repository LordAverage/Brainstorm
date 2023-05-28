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
        protected Animation Animation { get ; set; }
        protected bool Hidden { get; set; } = false;
        protected bool Busy { get; set; } = false;

        // Static position list field
        protected static List<Vector2> PositionList { get; set; } = new();
        private static Decision Decision { set; get; }

        // Static TmxMap field
        protected static TmxMap Map { get; set; }
        protected static float MapScale { get; set; }
        
        public NPC(float velocity)
        {
            Velocity = velocity;

            // Get the valid tile positions on the map
            List<Vector2> validTilePositions = GetValidTilePositions();

            // Select a random valid tile position
            Random random = new();
            int randomIndex = random.Next(validTilePositions.Count);
            Vector2 randomPosition = validTilePositions[randomIndex];

            // Add the NPC to the position list
            PositionListIndex = PositionList.Count;
            PositionList.Add(randomPosition);
        }
        public NPC(Vector2 position, float velocity)
        {
            this.Velocity = velocity;
            PositionListIndex = PositionList.Count;
            PositionList.Add(position);
        }
        public NPC(Vector2 position, float velocity, Animation animation)
            : this(position, velocity)
        {
            Animation = animation;
        }
        public NPC(Vector2 position, float velocity, Animation animation, bool hidden)
            : this(position, velocity, animation)
        {
            Hidden = hidden;
        }
        public virtual void Update(GameTime gameTime)
        {
            // Update animation
            Animation.Update(gameTime);
            if (!Hidden)
            {
                if (GetType() == typeof(Zombie))
                {
                    // Move to the next best position
                    // Move(Decision.BestPositionList[PositionListIndex]);
                    if (!Busy)
                    {
                        Move(Direction.Right);
                    }
                    else
                    {
                        Step(Animation.GetDirection());
                        if (!Busy)
                        {
                            int finalTileX = (int)Math.Ceiling(PositionList[PositionListIndex].X / Map.TileWidth / MapScale);
                            int finalTileY = (int)Math.Ceiling(PositionList[PositionListIndex].Y / Map.TileHeight / MapScale);
                            switch (Animation.GetDirection())
                            {
                                case Direction.Up:
                                    finalTileY--;
                                    break;
                                case Direction.Left:
                                    finalTileX--;
                                    break;
                            }
                            PositionList[PositionListIndex] = new Vector2(finalTileX * Map.TileWidth * MapScale, finalTileY * Map.TileHeight * MapScale);
                        }
                    }
                }
                else if (GetType() == typeof(Human))
                {
                    if (!Busy)
                    {
                        Array directions = Enum.GetValues(typeof(Direction));
                        int randomIndex = new Random().Next(directions.Length);
                        Direction randomDirection = (Direction)directions.GetValue(randomIndex);
                        Move(randomDirection);
                    }
                    else
                    {
                        Step(Animation.GetDirection());
                        if (!Busy)
                        {
                            int finalTileX = (int)Math.Ceiling(PositionList[PositionListIndex].X / Map.TileWidth / MapScale);
                            int finalTileY = (int)Math.Ceiling(PositionList[PositionListIndex].Y / Map.TileHeight / MapScale);
                            switch (Animation.GetDirection())
                            {
                                case Direction.Up:
                                    finalTileY--;
                                    break;
                                case Direction.Left:
                                    finalTileX--;
                                    break;
                            }
                            PositionList[PositionListIndex] = new Vector2(finalTileX * Map.TileWidth * MapScale, finalTileY * Map.TileHeight * MapScale);
                        }
                    }
                }
            }
        }
        public static void Update(List<NPC> NPCList)
        {
            int depth = 50;
            Decision = CalculateBestNextPosition(NPCList, depth);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Animation.Draw(spriteBatch, PositionList[PositionListIndex]);
        }
        private void Move(Direction direction)
        {
            Animation.SetDirection(direction);
            Busy = true;
            Step(direction);
        }
        private void Step(Direction direction)
        {
            // Calculate the target position based on the current position and direction
            Vector2 targetPosition = GetTargetPosition(direction);
            Vector2 targetTilePosition;
            targetTilePosition.X = PositionList[PositionListIndex].X / Map.TileWidth / MapScale;
            targetTilePosition.Y = PositionList[PositionListIndex].X / Map.TileWidth / MapScale;
            switch (direction)
            {
                case Direction.Up:
                    targetTilePosition.Y = (int)Math.Floor(targetTilePosition.Y);
                    break;
                case Direction.Left:
                    targetTilePosition.X = (int)Math.Floor(targetTilePosition.X);
                    break;
                case Direction.Right:
                    targetTilePosition.X = (int)Math.Ceiling(targetTilePosition.X);
                    break;
                case Direction.Down:
                    targetTilePosition.Y = (int)Math.Ceiling(targetTilePosition.Y);
                    break;
            }

            // How much left for tile completion
            Vector2 targetTileLeft = targetTilePosition * new Vector2(Map.TileWidth * MapScale, Map.TileHeight * MapScale);
            float distanceLeft = Vector2.Distance(PositionList[PositionListIndex], targetTileLeft);

            // Set movement increment
            float movementIncrement = 0.05f;

            Vector2 nextPosition = Vector2.Lerp(PositionList[PositionListIndex], targetPosition, movementIncrement);

            if (distanceLeft < Vector2.Distance(PositionList[PositionListIndex], nextPosition))
            {
                PositionList[PositionListIndex] = targetTileLeft;
                Busy = false;
                return;
            }
            // Check if the target tile position is valid and not obstructed
            if (IsTileValid(nextPosition) && !CheckCollisionWithObstacles(nextPosition, direction))
            {
                // Update the NPC's position gradually towards the target position
                PositionList[PositionListIndex] = nextPosition;
                return;
            }
            int finalTileX = (int)Math.Ceiling(PositionList[PositionListIndex].X / Map.TileWidth / MapScale);
            int finalTileY = (int)Math.Ceiling(PositionList[PositionListIndex].Y / Map.TileHeight / MapScale);
            PositionList[PositionListIndex] = new Vector2(finalTileX * Map.TileWidth * MapScale, finalTileY * Map.TileHeight * MapScale);
            Busy = false;
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
        private static List<Vector2> GetValidTilePositions() // TODO: Make this a static field to avoid extra calculation
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
            //List<Zombie> zombies = npcs.OfType<Zombie>().ToList();
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

                if (captures >= maxCaptures)
                {
                    maxCaptures = captures;
                    bestNextMoves = moveList;
                }
            }

            return new Decision { MaxCaptures = maxCaptures, BestPositionList = bestNextMoves };
        }

        private static List<List<Direction>> GetAllPossibleNextMoves(List<NPC> npcs)
        {
            List<List<Direction>> allPossibleNextMoveLists = new();

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
            List<Direction> possibleMoves = currentNPC.GetPossibleMoves();

            foreach (Direction move in possibleMoves)
            {
                currentMoveList.Add(move);
                GenerateMoveCombinations(npcs, currentMoveList, allPossibleNextMoveLists);
                currentMoveList.RemoveAt(currentMoveList.Count - 1);
            }
        }

        private List<Direction> GetPossibleMoves()
        {
            List<Direction> possibleMoves = new();

            // Get the valid tile positions on the map
            List<Vector2> validPositions = GetValidTilePositions();

            // Calculate the NPC's current position
            Vector2 currentPosition = PositionList[PositionListIndex];

            // Get the current direction from the animation
            Direction animationDirection = Animation.GetDirection();

            // Check each direction for possible moves
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                Vector2 nextPosition = currentPosition;
                switch (direction)
                {
                    case Direction.Up:
                        nextPosition += new Vector2(0, -Map.TileHeight * MapScale);
                        if (animationDirection == Direction.Up)
                            nextPosition.Y = (float)Math.Floor(nextPosition.Y / (Map.TileHeight * MapScale)) * (Map.TileHeight * MapScale);
                        break;
                    case Direction.Down:
                        nextPosition += new Vector2(0, Map.TileHeight * MapScale);
                        if (animationDirection == Direction.Down)
                            nextPosition.Y = (float)Math.Ceiling(nextPosition.Y / (Map.TileHeight * MapScale)) * (Map.TileHeight * MapScale);
                        break;
                    case Direction.Left:
                        nextPosition += new Vector2(-Map.TileWidth * MapScale, 0);
                        if (animationDirection == Direction.Left)
                            nextPosition.X = (float)Math.Floor(nextPosition.X / (Map.TileWidth * MapScale)) * (Map.TileWidth * MapScale);
                        break;
                    case Direction.Right:
                        nextPosition += new Vector2(Map.TileWidth * MapScale, 0);
                        if (animationDirection == Direction.Right)
                            nextPosition.X = (float)Math.Ceiling(nextPosition.X / (Map.TileWidth * MapScale)) * (Map.TileWidth * MapScale);
                        break;
                }
                if (validPositions.Contains(nextPosition))
                    possibleMoves.Add(direction);
            }
            return possibleMoves;
        }




        private static List<NPC> SimulateNPCsMove(List<NPC> npcs, List<Direction> moveList)
        {
            List<NPC> simulatedNPCs = new();

            // Create a deep copy of the NPCs list to avoid modifying the original list
            foreach (NPC npc in npcs)
            {
                NPC simulatedNPC = new(PositionList[npc.PositionListIndex], npc.Velocity, npc.Animation, true);
                simulatedNPCs.Add(simulatedNPC);
            }

            // Simulate the movement of NPCs based on the move list
            for (int i = 0; i < simulatedNPCs.Count; i++)
            {
                NPC npc = simulatedNPCs[i];
                Direction move = moveList[i];

                // Update the position based on the move direction
                switch (move)
                {
                    case Direction.Up:
                        PositionList[npc.PositionListIndex] += new Vector2(0, -Map.TileHeight * MapScale);
                        break;
                    case Direction.Down:
                        PositionList[npc.PositionListIndex] += new Vector2(0, Map.TileHeight * MapScale);
                        break;
                    case Direction.Left:
                        PositionList[npc.PositionListIndex] += new Vector2(-Map.TileWidth * MapScale, 0);
                        break;
                    case Direction.Right:
                        PositionList[npc.PositionListIndex] += new Vector2(Map.TileWidth * MapScale, 0);
                        break;
                    default:
                        break;
                }
            }

            return simulatedNPCs;
        }


        private static int CurrentPositionCaptures(List<NPC> npcs)
        {
            List<Human> humans = new List<Human>();
            List<Zombie> zombies = new List<Zombie>();

            foreach (NPC npc in npcs)
            {
                Type npcType = npc.GetType();

                if (npcType == typeof(Human))
                {
                    humans.Add((Human)npc);
                }
                else if (npcType == typeof(Zombie))
                {
                    zombies.Add((Zombie)npc);
                }
            }

            int captures = 0;

            // Check if humans are captured by zombies
            foreach (Human human in humans)
            {
                foreach (Zombie zombie in zombies)
                {
                    if (PositionList[human.PositionListIndex] == PositionList[zombie.PositionListIndex])
                    {
                        captures++;
                        break;
                    }
                }
            }

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
