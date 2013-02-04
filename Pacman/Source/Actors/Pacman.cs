using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public class PacMan : Actor
    {
        public float FrightSpeedModifier { get; private set; }

        public PacMan(Level level, Texture2D texture, Vector2 position)
            : base(level, texture, position, new DrawingRectangle(3, 3, 48, 48))
        {
            Direction = Direction.Left;

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
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var nextTileBounds = Level.TileBounds(NextPosition);

            // Left wall collisions
            if (Direction == Direction.Left && !Level.IsLegal(NextPosition) &&
                Bounds.Left < nextTileBounds.Right - PacmanGame.TileWidth / 2f)
            {
                _position.X = nextTileBounds.Right - PacmanGame.TileWidth / 2f + Origin.X;
            }
            else if (Direction == Direction.Right && !Level.IsLegal(NextPosition) &&
                Bounds.Right > nextTileBounds.Left + PacmanGame.TileWidth * 1/3)
            {
                _position.X = nextTileBounds.Left + PacmanGame.TileWidth * 1 / 3 - Origin.X;
            }
            else if (Direction == Direction.Up && !Level.IsLegal(NextPosition) &&
                Bounds.Top < nextTileBounds.Bottom - PacmanGame.TileWidth / 2f)
            {
                _position.Y = nextTileBounds.Bottom - PacmanGame.TileWidth / 2f + Origin.Y;
            }
            else if (Direction == Direction.Down && !Level.IsLegal(NextPosition) &&
               Bounds.Bottom > nextTileBounds.Top + PacmanGame.TileWidth * 1 / 3)
            {
                _position.Y = nextTileBounds.Top + PacmanGame.TileWidth * 1 / 3 - Origin.Y;
            }
        }

        /// <summary>
        /// Change pacman's direction.
        /// </summary>
        /// <param name="direction">New direction</param>
        public void Move(Direction direction)
        {
            Direction = direction;
        }
    }
}
