using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public class PacMan : Actor
    {
        public PacMan(Level level, Texture2D texture, Vector2 position)
            : base(level, texture, position, new DrawingRectangle(3, 3, 48, 48))
        {
            SpeedModifier = PacmanBaseSpeed * 0.8f;
            Direction = Direction.Left;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Handle tunnels
            if (Bounds.Right > Level.TilesWide * PacmanGame.TileWidth)
                Position = new Vector2(Origin.X, Position.Y);

            if (Bounds.Left < 0)
                Position = new Vector2(Level.TilesWide * PacmanGame.TileWidth - Origin.X, Position.Y);
        }

        /// <summary>
        /// We reached the center of the tile, make sure we don't run into a wall.
        /// </summary>
        public override void OnTileCenter()
        {
            base.OnTileCenter();

            // Stop if the next position is an illegal tile.
            if (!Level.IsLegal(GetNextPosition(GridPosition, Direction)))
            {
                Position -= Velocity;
                Velocity = Vector2.Zero;
            }
        }

        /// <summary>
        /// Request direction change.
        /// 
        /// TODO: Note that pacman can take shortcuts if he requests direction a few pixels before ghosts can.
        /// </summary>
        /// <param name="direction">New direction</param>
        /// <returns>True if the direction change was performed.</returns>
        public bool ChangeDirection(Direction direction)
        {
            if (direction == Direction)
                return false;

            if (Level.IsLegal(GetNextPosition(GridPosition, direction)))
            {
                Direction = direction;
                Velocity = GetVelocity();

                return true;
            }

            return false;
        }
    }
}
