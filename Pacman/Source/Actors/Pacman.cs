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

            Velocity = Vector2.Zero;

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
