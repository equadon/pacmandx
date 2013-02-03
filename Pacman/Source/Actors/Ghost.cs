using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
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
        private Vector2 _targetTile;

        #region Properties

        /// <summary>The direction we'll take once we reach NextPosition.</summary>
        public Direction FutureDirection { get; protected set; }

        public Vector2 NextPosition { get; protected set; }

        public Vector2 TargetTile
        {
            get { return _targetTile; }
            set { _targetTile = value; }
        }

        #endregion

        public Ghost(Level level, Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(level, texture, position, sourceRect)
        {
            // TODO: Better handling of movement speeds
            //SpeedModifier = PacmanBaseSpeed * 0.75f;
            SpeedModifier = PacmanBaseSpeed * 0.2f;
        }

        /// <summary>
        /// We reached the tile's center point, calculate the next move.
        /// </summary>
        public override void OnTileCenter()
        {
            base.OnTileCenter();

            // Update target
            UpdateTarget();

            NextPosition = GetNextPosition(GridPosition, FutureDirection);

            Direction = FutureDirection;

            CalculateFutureDirection();
        }

        /// <summary>
        /// Called when ghost mode was changed.
        /// </summary>
        public void GhostModeChanged()
        {
            // Reverse direction
            switch (Direction)
            {
                case Direction.Up:
                    Direction = Direction.Down;
                    break;
                case Direction.Down:
                    Direction = Direction.Up;
                    break;
                case Direction.Left:
                    Direction = Direction.Right;
                    break;
                case Direction.Right:
                    Direction = Direction.Left;
                    break;
            }

            NextPosition = GetNextPosition(GridPosition, Direction);
            FutureDirection = Direction;

            // Update target
            UpdateTarget();
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
