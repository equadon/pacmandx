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
        public const float BaseSpeedModifer = 1.0f;

        private bool _reachedCenter = false;

        protected Vector2 _velocity;

        #region Properties

        public float SpeedModifier { get; set; }

        public Vector2 LastGridPosition { get; private set; }

        public Direction Direction { get; protected set; }

        public Vector2 Velocity { get; protected set; }

        #endregion

        public Actor(Texture2D texture, Vector2 position, Rectangle sourceRect)
            : base(texture, position, sourceRect)
        {
            SpeedModifier = BaseSpeedModifer;
            LastGridPosition = Utils.AbsToGrid(position);
            Direction = Direction.Left;
            Velocity = GetVelocity();
        }

        public override void Update(GameTime gameTime)
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

            return;

            #region tmp


            switch (Direction)
            {
                case Direction.Up:
                    if (!IsDirectionLegal(Direction, GridPosition) &&
                        Position.Y < tileBounds.Top + tileBounds.Height/2)
                        Position = new Vector2(Position.X, tileBounds.Top + tileBounds.Height/2);
                    break;
                case Direction.Down:
                    if (!IsDirectionLegal(Direction, GridPosition) &&
                        Position.Y > tileBounds.Bottom - tileBounds.Height/2)
                        Position = new Vector2(Position.X, tileBounds.Bottom - tileBounds.Height/2);
                    break;
                case Direction.Left:
                    if (!IsDirectionLegal(Direction, GridPosition) &&
                        Position.X < tileBounds.Left + tileBounds.Width/2)
                        Position = new Vector2(tileBounds.Left + tileBounds.Width/2, Position.Y);
                    break;
                case Direction.Right:
                    if (!IsDirectionLegal(Direction, GridPosition) &&
                        Position.X > tileBounds.Right - tileBounds.Width/2)
                        Position = new Vector2(tileBounds.Right - tileBounds.Width/2, Position.Y);
                    break;
            }

            // Handle tunnels
            if (Bounds.Right > Level.TilesWide*PacmanGame.TileWidth)
                Position = new Vector2(Origin.X, Position.Y);

            if (Bounds.Left < 0)
                Position = new Vector2(Level.TilesWide*PacmanGame.TileWidth - Origin.X, Position.Y);

            #endregion
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

            return velocity * SpeedModifier;
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
                    // Go through the tunnels if we reach the end
                    return new Vector2((currentPosition.X + 1 > Level.TilesWide - 1) ? 0 : currentPosition.X + 1,
                                       currentPosition.Y);
                default:
                    throw new Exception("Invalid direction: " + nextDirection);
            }
        }
    }
}
