using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public class PacMan : Actor
    {
        private int _framesLeftToFreeze = 0;

        public float FrightSpeedModifier { get; private set; }

        public PacMan(Level level, Texture2D texture, Vector2 position)
            : base(level, texture, position, new DrawingRectangle(3, 3, 48, 48))
        {
            Direction = Direction.Left;

            FlashSourceRect = new DrawingRectangle(56, 3, 48, 48);

            // Set speeds
            int currentLevel = Level.ScreenManager.CurrentLevel;

            if (currentLevel == 1)
            {
                SpeedModifier = 0.8f;
                FrightSpeedModifier = 0.9f;
            }
            else if (currentLevel >= 2 && currentLevel <= 4)
            {
                SpeedModifier = 0.9f;
                FrightSpeedModifier = 0.95f;
            }
            else if (currentLevel >= 5 && currentLevel <= 20)
            {
                SpeedModifier = 1f;
                FrightSpeedModifier = 1f;
            }
            else
            {
                SpeedModifier = 0.9f;
                FrightSpeedModifier = SpeedModifier;
            }

            Velocity = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            if (_framesLeftToFreeze > 0)
            {
                _framesLeftToFreeze--;
                return;
            }

            base.Update(gameTime);

            var tileBounds = Level.TileBounds(GridPosition);
            var nextTileBounds = Level.TileBounds(NextPosition);

            // Make sure we stay centered
            // TODO: Don't center while we're moving diagonally.
            if (Direction == Direction.Left || Direction == Direction.Right)
                _position.Y = tileBounds.Bottom - tileBounds.Height/2f;
            else if (Direction == Direction.Up || Direction == Direction.Down)
                _position.X = tileBounds.Right - tileBounds.Width / 2f;

            // Wall collisions
            if (Direction == Direction.Left && !Level.IsLegal(NextPosition) &&
                     Position.X < tileBounds.Right - tileBounds.Width/2f)
                _position.X = tileBounds.Right - tileBounds.Width/2f;
            else if (Direction == Direction.Right && !Level.IsLegal(NextPosition) &&
                     Position.X > tileBounds.Right - tileBounds.Width / 2f)
                _position.X = tileBounds.Right - tileBounds.Width / 2f;
            else if (Direction == Direction.Up && !Level.IsLegal(NextPosition) &&
                     Position.Y < tileBounds.Bottom - tileBounds.Height / 2f)
                _position.Y = tileBounds.Bottom - tileBounds.Height / 2f;
            else if (Direction == Direction.Down && !Level.IsLegal(NextPosition) &&
                     Position.Y > tileBounds.Bottom - tileBounds.Height / 2f)
                _position.Y = tileBounds.Bottom - tileBounds.Height / 2f;

            // Collision with fruit
            if (Level.Fruit != null)
            {
                if (Position.Y >= Level.Fruit.Bounds.Top &&
                    Position.Y <= Level.Fruit.Bounds.Bottom &&
                    Vector2.Distance(Position, Level.Fruit.Position) <= Origin.X + Level.Fruit.Origin.X)
                {
                    Level.EatFruit();
                }
            }
        }

        public override void OnTileCenter()
        {
            base.OnTileCenter();

            // Eat item at current position
            TileItem type = Level.EatItem(GridPosition);

            // Freeze position if we ate something
            // TODO: Instead of freezing position just slow his movement enough to avoid choppy movement
            if (type == TileItem.Dot)
                _framesLeftToFreeze = 1;
            else if (type == TileItem.Energizer)
                _framesLeftToFreeze = 3;
        }

        /// <summary>
        /// Change pacman's direction.
        /// </summary>
        /// <param name="direction">New direction</param>
        public void Move(Direction direction)
        {
            // No need to check any further if the next tile is illegal
            if (!IsDirectionLegal(GridPosition, direction))
                return;

            var tileBounds = Level.TileBounds(GridPosition);
            var tileCenterX = tileBounds.Left + tileBounds.Width/2f;
            var tileCenterY = tileBounds.Top + tileBounds.Height/2f;

            const int xRange = 10;
            const int yRange = 10;

            switch (direction)
            {
                case Direction.Down:
                case Direction.Up:
                    if (Position.X >= tileCenterX - xRange && Position.X < tileCenterX)
                    {
                        Direction = direction;
                        _position.X += Math.Abs(Velocity.Y);
                    }
                    //else if (Position.X == tileCenterX)
                    else if (Utils.NearlyEqual(Position.X, tileCenterX, float.Epsilon))
                    {
                        Direction = direction;
                    }
                    else if (Position.X > tileCenterX && Position.X <= tileCenterX + xRange)
                    {
                        Direction = direction;
                        _position.X -= Math.Abs(Velocity.Y);
                    }
                    break;

                case Direction.Left:
                case Direction.Right:
                    if (Position.Y > tileCenterY && Position.Y <= tileCenterY + yRange)
                    {
                        Direction = direction;
                        _position.Y -= Math.Abs(Velocity.X);
                    }
                    //else if (Position.Y == tileCenterY)
                    else if (Utils.NearlyEqual(Position.Y, tileCenterY, float.Epsilon))
                    {
                        Direction = direction;
                    }
                    else if (Position.Y >= tileCenterY - yRange && Position.Y < tileCenterY)
                    {
                        Direction = direction;
                        _position.Y += Math.Abs(Velocity.X);
                    }
                    break;

                default:
                    Direction = direction;
                    break;
            }
        }
    }
}
