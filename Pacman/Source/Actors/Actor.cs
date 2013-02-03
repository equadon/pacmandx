using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public class Actor : Sprite
    {
        /// <summary>Pacman's base speed. TODO: Still not sure on this. Speed is supposedly 11 tiles/s. We run at 60fps.</summary>
        public readonly float PacmanBaseSpeed = 11 * PacmanGame.TileWidth / 60f;

        private bool _reachedCenter = false;

        protected Vector2 _velocity;

        private Direction _direction;

        #region Properties

        public float BaseSpeedModifier { get; protected set; }
        public float SpeedModifier { get; protected set; }

        public Vector2 LastGridPosition { get; private set; }

        public Vector2 Velocity { get; protected set; }

        public Vector2 NextPosition
        {
            get { return GetNextPosition(GridPosition, Direction); }
        }

        public Direction Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                Velocity = GetVelocity();
            }
        }

        public bool IsInsideTunnels
        {
            get
            {
                if ((int)GridPosition.Y == 17 &&
                    (GridPosition.X >= 0 && GridPosition.X <= 5))
                    return true;

                if ((int)GridPosition.Y == 17 &&
                    (GridPosition.X >= 22 && GridPosition.X <= 27))
                    return true;

                return false;
            }
        }

        #endregion

        public Actor(Level level, Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(level, texture, position, sourceRect)
        {
            SpeedModifier = PacmanBaseSpeed;
            LastGridPosition = Utils.AbsToGrid(position);
        }

        public override void Update(GameTime gameTime)
        {
            LastGridPosition = GridPosition;

            Position += Velocity;

            // Handle collision with illegal tiles
            var tileBounds = Level.TileBounds(GridPosition);
            var nextTileBounds = Level.TileBounds(GetNextPosition(GridPosition, Direction));

            // Reached new tile
            if (GridPosition != LastGridPosition)
            {
                LastGridPosition = GridPosition;

                OnNewTile();

                return;
            }

            if (_reachedCenter)
                return;

            // Reached center of tile
            switch (Direction)
            {
                case Direction.Up:
                    if (Position.Y < tileBounds.Top + PacmanGame.TileWidth / 2f)
                        OnTileCenter();
                    break;
                case Direction.Down:
                    if (Position.Y > tileBounds.Top + PacmanGame.TileWidth / 2f)
                        OnTileCenter();
                    break;
                case Direction.Left:
                    if (Position.X < tileBounds.Left + PacmanGame.TileWidth / 2f)
                        OnTileCenter();
                    break;
                case Direction.Right:
                    if (Position.X > tileBounds.Left + PacmanGame.TileWidth / 2f)
                        OnTileCenter();
                    break;
            }

            // Handle tunnels
            if (GridPosition.X == 27 && GridPosition.Y == 17 &&
                Bounds.Right > tileBounds.Right)
            {
                Position = new Vector2(Origin.X, 17 * PacmanGame.TileWidth + PacmanGame.TileWidth / 2f);
            }
            else if (GridPosition.X == 0 && GridPosition.Y == 17 &&
                     Bounds.Left < 0)
            {
                Position = new Vector2(Level.TilesWide * PacmanGame.TileWidth - Origin.X , 17 * PacmanGame.TileWidth + PacmanGame.TileWidth / 2f);
            }
        }

        public void Update2(GameTime gameTime)
        {
            LastGridPosition = GridPosition;

            Position += Velocity;

            // Handle collision with illegal tiles
            var tileBounds = Level.TileBounds(GridPosition);
            var nextTileBounds = Level.TileBounds(GetNextPosition(GridPosition, Direction));

            if (GridPosition != LastGridPosition)
            {
                LastGridPosition = GridPosition;

                OnNewTile();

                return;
            }

            if (_reachedCenter)
                return;

            switch (Direction)
            {
                case Direction.Up:
                    if (Position.Y < tileBounds.Top + PacmanGame.TileWidth / 2f)
                        OnTileCenter();
                    break;
                case Direction.Down:
                    if (Position.Y > tileBounds.Top + PacmanGame.TileWidth / 2f)
                        OnTileCenter();
                    break;
                case Direction.Left:
                    if (Position.X < tileBounds.Left + PacmanGame.TileWidth / 2f)
                        OnTileCenter();
                    break;
                case Direction.Right:
                    if (Position.X > tileBounds.Left + PacmanGame.TileWidth / 2f)
                        OnTileCenter();
                    break;
            }
        }

        public virtual void OnTileCenter()
        {
            // we just reached tile's center, do something
            _reachedCenter = true;
        }

        public virtual void OnNewTile()
        {
            // we just reached tile's center, do something
            _reachedCenter = false;
        }

        public bool IsDirectionLegal(Direction direction, Vector2 currentPosition)
        {
            Vector2 nextPos = GetNextPosition(currentPosition, direction);

            return Level.IsLegal(nextPos);
        }

        public Vector2 GetVelocity()
        {
            Vector2 velocity = Vector2.Zero;

            switch (Direction)
            {
                case Direction.Up:
                    velocity = new Vector2(0, -1);
                    break;
                case Direction.Down:
                    velocity = new Vector2(0, 1);
                    break;
                case Direction.Left:
                    velocity = new Vector2(-1, 0);
                    break;
                case Direction.Right:
                    velocity = new Vector2(1, 0);
                    break;
            }

            return velocity * PacmanBaseSpeed * SpeedModifier;
        }

        protected Vector2 GetNextPosition(Vector2 currentPosition, Direction nextDirection, int steps = 1)
        {
            switch (nextDirection)
            {
                case Direction.Up:
                    return new Vector2(currentPosition.X, currentPosition.Y - steps);
                case Direction.Left:
                    // Go through the tunnels if we reach the end
                    return new Vector2((currentPosition.X - steps < 0) ? 27 : currentPosition.X - steps, currentPosition.Y);
                case Direction.Down:
                    return new Vector2(currentPosition.X, currentPosition.Y + steps);
                case Direction.Right:
                    // Go through the tunnels if we reach the end
                    return new Vector2((currentPosition.X + steps > Level.TilesWide - steps) ? 0 : currentPosition.X + steps,
                                       currentPosition.Y);
                default:
                    throw new Exception("Invalid direction: " + nextDirection);
            }
        }
    }
}
