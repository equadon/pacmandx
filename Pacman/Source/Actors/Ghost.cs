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

        public Direction NextDirection { get; private set; }
        public Vector2 NextPosition { get; private set; }

        public Vector2 TargetTile
        {
            get { return _targetTile; }
            set
            {
                _targetTile = value;
                CalculateNextPosition();
            }
        }

        public Ghost(Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(texture, position, sourceRect)
        {
            TargetTile = new Vector2(-1, -1);
            NextDirection = Direction;
            SpeedModifier = 0f;
        }

        /// <summary>
        /// Move ghost to its next position. Direction will be ignored
        /// for ghosts.
        /// </summary>
        /// <param name="direction"></param>
        public override void Move(Direction? direction = null)
        {
            GridPosition = NextPosition;

            CalculateNextPosition();
        }

        private void CalculateNextPosition()
        {
            if (TargetTile == new Vector2(-1, -1))
                return;

            // Which paths are available?
            List<Direction> availableDirections = AvailableDirections();

            // If only one path is possible then we take it
            if (availableDirections.Count == 1)
            {
                NextPosition = GetNextPosition(availableDirections[0]);
                NextDirection = availableDirections[0];
                return;
            }

            // Measure distance between each available position and the target tile.
            var shortestDistance = (float)(Math.Pow(Level.TilesWide, 2) + Math.Pow(Level.TilesHigh, 2));

            foreach (var direction in availableDirections)
            {
                Vector2 nextPos = GetNextPosition(direction);
                float distance = Vector2.Distance(TargetTile, nextPos);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;

                    NextPosition = nextPos;
                    NextDirection = direction;
                }
            }
        }

        /// <summary>
        /// Returns all available directions for the next move.
        /// </summary>
        /// <returns>List of available directions. Directions are ordered
        /// by preferred direction. (up, left, down, right)</returns>
        private List<Direction> AvailableDirections()
        {
            var list = new List<Direction>();

            // up
            if (NextDirection != Direction.Down && Level.IsLegal(GetNextPosition(Direction.Up)))
                list.Add(Direction.Up);

            // left
            if (NextDirection != Direction.Right && Level.IsLegal(GetNextPosition(Direction.Left)))
                list.Add(Direction.Left);

            // down
            if (NextDirection != Direction.Up && Level.IsLegal(GetNextPosition(Direction.Down)))
                list.Add(Direction.Down);

            // right
            if (NextDirection != Direction.Left && Level.IsLegal(GetNextPosition(Direction.Right)))
                list.Add(Direction.Right);

            return list;
        }

        private Vector2 GetNextPosition(Direction nextDirection)
        {
            switch (nextDirection)
            {
                case Direction.Up:
                    return new Vector2(GridPosition.X, GridPosition.Y - 1);
                case Direction.Left:
                    // Go through the tunnels if we reach the end
                    return new Vector2((GridPosition.X - 1 < 0) ? 27 : GridPosition.X - 1, GridPosition.Y);
                case Direction.Down:
                    return new Vector2(GridPosition.X, GridPosition.Y + 1);
                case Direction.Right:
                    return new Vector2((GridPosition.X + 1 > Level.TilesWide - 1) ? 0 : GridPosition.X + 1, GridPosition.Y);
                default:
                    throw new Exception("Invalid direction: " + nextDirection);
            }
        }
    }
}
