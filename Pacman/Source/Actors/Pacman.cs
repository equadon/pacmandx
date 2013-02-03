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

            Velocity = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// We reached the center of the tile, make sure we don't run into a wall.
        /// </summary>
        public override void OnTileCenter()
        {
            base.OnTileCenter();
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
            Direction = direction;
            return true;

            if (direction == Direction)
                return false;

            if (Level.IsLegal(GetNextPosition(GridPosition, direction)))
            {
                Direction = direction;

                return true;
            }

            return false;
        }
    }
}
