using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public class Ghost : Actor
    {
        private Vector2 _targetTile;

        /// <summary>The direction we'll take once we reach NextPosition.</summary>
        public Direction FutureDirection { get; protected set; }

        public Vector2 NextPosition { get; protected set; }

        public Vector2 TargetTile
        {
            get { return _targetTile; }
            set { _targetTile = value; }
        }

        public Ghost(Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(texture, position, sourceRect)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // We reached our next position, set the new position and calculate the next future direction
            var tileBounds = Level.TileBounds(NextPosition);

            switch (Direction)
            {
                case Direction.Up:
                    if (Bounds.Top < tileBounds.Top)
                        PerformNextMove();
                    break;
                case Direction.Down:
                    if (Bounds.Bottom > tileBounds.Bottom)
                        PerformNextMove();
                    break;
                case Direction.Left:
                    if (Bounds.Left < tileBounds.Left)
                        PerformNextMove();
                    break;
                case Direction.Right:
                    if (Bounds.Right > tileBounds.Right)
                        PerformNextMove();
                    break;
            }
        }

        private void PerformNextMove()
        {
            GridPosition = NextPosition;
            NextPosition = GetNextPosition(GridPosition, FutureDirection);

            Direction = FutureDirection;

            CalculateFutureDirection();
        }

        #region Pathfinding Methods

        protected void CalculateFutureDirection()
        {
            // Which paths are available?
            var nextPos = GetNextPosition(GridPosition, Direction);
            List<Direction> availableDirections = AvailableDirections(Direction, nextPos);

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
                Vector2 next = GetNextPosition(nextPos, direction);
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

        protected Vector2 GetNextPosition(Vector2 currentPosition, Direction nextDirection)
        {
            switch (nextDirection)
            {
                case Direction.Up:
                    return new Vector2(currentPosition.X, currentPosition.Y - 1);
                case Direction.Left:
                    // Go through the tunnels if we reach the end
                    return new Vector2((currentPosition.X - 1 < 0) ? 27 : currentPosition.X - 1, currentPosition.Y);
                case Direction.Down:
                    return new Vector2(currentPosition.X, currentPosition.Y + 1);
                case Direction.Right:
                    return new Vector2((currentPosition.X + 1 > Level.TilesWide - 1) ? 0 : currentPosition.X + 1,
                                       currentPosition.Y);
                default:
                    throw new Exception("Invalid direction: " + nextDirection);
            }
        }

        #endregion
    }
}
