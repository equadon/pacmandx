using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Pacman.Actors
{
    public class PacMan : Actor
    {
        public PacMan(Texture2D texture, Vector2 position)
            : base(texture, position, new DrawingRectangle(3, 3, 48, 48))
        {
            SpeedModifier = 3f;
            Direction = Direction.Left;
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
            if (IsDirectionLegal(direction, GridPosition))
            {
                var tileBounds = Level.TileBounds(GridPosition);

                if ((direction == Direction.Up ||
                    direction == Direction.Down) &&
                    (int)Position.X == tileBounds.Left + tileBounds.Width / 2)
                {
                    Direction = direction;
                }
                else if ((direction == Direction.Left ||
                         direction == Direction.Right) &&
                         (int)Position.Y == tileBounds.Top + tileBounds.Height / 2)
                {
                    Direction = direction;
                }

                return true;
            }

            return false;
        }
    }
}
