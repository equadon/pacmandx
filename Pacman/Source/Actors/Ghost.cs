using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public enum GhostMode
    {
        Chase,
        Scatter,
        Frightened
    }

    public abstract class Ghost : Actor
    {
        private Random _random;

        #region Properties

        public bool ForceNewDirection { get; set; }

        /// <summary>The direction we'll take once we reach NextPosition.</summary>
        public Direction FutureDirection { get; protected set; }

        public Vector2 TargetTile { get; protected set; }

        // Speeds
        public float FrightSpeedModifier { get; private set; }
        public float TunnelSpeedModifier { get; private set; }

        #endregion

        public Ghost(Level level, Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(level, texture, position, sourceRect)
        {
            _random = new Random();

            ForceNewDirection = false;

            UpdateTarget();

            // Set speeds
            int currentLevel = Level.ScreenManager.CurrentLevel;

            if (currentLevel == 1)
            {
                BaseSpeedModifier = 0.75f;
                FrightSpeedModifier = 0.5f;
                TunnelSpeedModifier = 0.4f;
            }
            else if (currentLevel >= 2 && currentLevel <= 4)
            {
                BaseSpeedModifier = 0.85f;
                FrightSpeedModifier = 0.55f;
                TunnelSpeedModifier = 0.45f;
            }
            else if (currentLevel >= 5 && currentLevel <= 20)
            {
                BaseSpeedModifier = 0.95f;
                FrightSpeedModifier = 0.60f;
                TunnelSpeedModifier = 0.50f;
            }
            else
            {
                BaseSpeedModifier = 0.95f;
                FrightSpeedModifier = SpeedModifier;
                TunnelSpeedModifier = 0.5f;
            }

            SpeedModifier = BaseSpeedModifier;
        }

        public override void OnNewTile()
        {
            base.OnNewTile();

            if (ForceNewDirection)
                ReverseDirection();

            // If this is Blinky and he's Elroy, don't change speed.
            var blinky = this as Ghosts.Blinky;
            if (blinky != null && blinky.IsElroy)
                return;

            // Are we in the tunnels?
            if (IsInsideTunnels)
                SpeedModifier = TunnelSpeedModifier;
            else if (Level.GhostMode == GhostMode.Frightened)
                SpeedModifier = FrightSpeedModifier;
            else
                SpeedModifier = BaseSpeedModifier;
        }

        /// <summary>
        /// We reached the tile's center point, calculate the next move.
        /// </summary>
        public override void OnTileCenter()
        {
            base.OnTileCenter();

            // Update target
            UpdateTarget();

            // Random direction when frightened
            if (!ForceNewDirection && Level.GhostMode == GhostMode.Frightened)
                Direction = RandomDirection();
            else
                Direction = FutureDirection;

            GridPosition = GridPosition;

            if (Bounds.Left < 0)
                _position.X += Math.Abs(Bounds.Left);
            else if (Bounds.Right > Level.TilesWide * PacmanGame.TileWidth)
                _position.X -= (Bounds.Right - Level.TilesWide*PacmanGame.TileWidth);

            CalculateFutureDirection();
        }

        private Direction RandomDirection()
        {
            var randomDirection = (Direction) _random.Next(4);

            var isOpposite = randomDirection == OppositeDirection(Direction);
            var isIllegal = !IsDirectionLegal(GetNextPosition(GridPosition, randomDirection), randomDirection);

            while (isOpposite || isIllegal)
            {
                switch (randomDirection)
                {
                    case Direction.Up:
                        randomDirection = Direction.Right;
                        break;
                    case Direction.Right:
                        randomDirection = Direction.Down;
                        break;
                    case Direction.Down:
                        randomDirection = Direction.Left;
                        break;
                    default:
                        randomDirection = Direction.Up;
                        break;
                }
                isOpposite = randomDirection == OppositeDirection(Direction);
                isIllegal = !IsDirectionLegal(GridPosition, randomDirection);
            }

            return randomDirection;
        }

        private Direction OppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Down:
                    return Direction.Up;
                default:
                    return Direction.Left;
            }
        }

        /// <summary>
        /// Called when ghost mode was changed.
        /// </summary>
        public void ReverseDirection()
        {
            ForceNewDirection = false;

            // Change speed modifier
            if (IsInsideTunnels)
                SpeedModifier = TunnelSpeedModifier;
            else if (Level.GhostMode == GhostMode.Frightened)
                SpeedModifier = FrightSpeedModifier;
            else
                SpeedModifier = BaseSpeedModifier;

            // Reverse direction
            var newDirection = Direction.Left;

            switch (Direction)
            {
                case Direction.Up:
                    newDirection = Direction.Down;
                    break;
                case Direction.Down:
                    newDirection = Direction.Up;
                    break;
                case Direction.Left:
                    newDirection = Direction.Right;
                    break;
                case Direction.Right:
                    newDirection = Direction.Left;
                    break;
            }

            Direction = newDirection;

            UpdateTarget();

            CalculateFutureDirection();
        }

        public abstract void UpdateTarget();

        #region Pathfinding Methods

        protected void CalculateFutureDirection()
        {
            // Which paths are available?
            List<Direction> availableDirections = AvailableDirections(Direction, NextPosition);

            if (availableDirections.Count == 0)
                return;

            // If only one path is possible then we take it
            if (availableDirections.Count == 1)
            {
                FutureDirection = availableDirections[0];
                return;
            }

            // Measure distance between each available position and the target tile.
            var shortestDistance = (float)(Math.Pow(Level.TilesWide, 2) + Math.Pow(Level.TilesHigh, 2));

            foreach (var direction in availableDirections)
            {
                Vector2 next = GetNextPosition(NextPosition, direction);
                float distance = Vector2.Distance(TargetTile, next);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;

                    FutureDirection = direction;
                }
            }
        }

        /// <summary>
        /// Returns all available directions for the specified position.
        /// </summary>
        /// <returns>List of available directions. Directions are ordered
        /// by preferred direction. (up, left, down, right)</returns>
        private List<Direction> AvailableDirections(Direction direction, Vector2 position)
        {
            var list = new List<Direction>();

            // up
            Vector2 nextPos = GetNextPosition(position, Direction.Up);
            if (direction != Direction.Down && Level.IsLegal(nextPos))
                list.Add(Direction.Up);

            // left
            nextPos = GetNextPosition(position, Direction.Left);
            if (direction != Direction.Right && Level.IsLegal(nextPos))
                list.Add(Direction.Left);

            // down
            nextPos = GetNextPosition(position, Direction.Down);
            if (direction != Direction.Up && Level.IsLegal(nextPos))
                list.Add(Direction.Down);

            // right
            nextPos = GetNextPosition(position, Direction.Right);
            if (direction != Direction.Left && Level.IsLegal(nextPos))
                list.Add(Direction.Right);

            return list;
        }

        #endregion
    }
}
